using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static Action OnLevelStarted;
    public static Action OnLevelContinued;
    public static Action OnGameStarted;
    public static Action OnGameOver;
    public static Action PlayerDamaged;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameManager[] instances = FindObjectsOfType<GameManager>();
                if (instances.Length == 0)
                {
                    throw new System.Exception("ERROR: Could not find an instance of the GameManager in the current scene. Please attach one to a gameobject.");
                }
                else if (instances.Length > 1)
                {
                    throw new System.Exception("ERROR: Found multiple instances of the gamemanager in the current scene. Count=" + instances.Length);
                }

                instance = instances[0];
            }

            return instance;
        }
    }

    public Difficulty Difficulty { get; private set; }

    [SerializeField] public Sentry sentry = null;

    public Rank currentRank;
    public Vector3 finalPlayerPosition;
    public bool isGameActive;
    public uint maximumAmmoCount = 100;

    private static GameManager instance = null;

    [SerializeField] private RankTimer rankTimer = null;
    [SerializeField] private Rank initialRank = null;
    [SerializeField] private float breathPauseInSeconds = 10.0F;
    [SerializeField] private int startingPlayerHealth = 3;

    private int currentPlayerHealth;
    private bool isPausedToBreathe = true;

    static GameManager() { }
    private GameManager() { }

    private void Awake()
    {
        BreathingManager.OnHit += OnHit;
        BreathingManager.OnFail += OnFail;
        RankTimer.OnNextRankReached += OnNextRankReached;

        if (Metronome.metronomeStarted == false)
        {
            StartCoroutine(Metronome.StartMetronome());
        }
    }

    private void OnHit()
    {
        if (isGameActive == false)
        {
            return;
        }

        sentry.AddAmmo(Difficulty.ammoGrantForHit);
    }

    private void OnFail()
    {
        if (isGameActive == false)
        {
            return;
        }

        sentry.RemoveAmmo(Difficulty.ammoReductionForMiss);
    }

    private void OnNextRankReached()
    {
        if (isGameActive == false)
        {
            return;
        }

        rankTimer.PauseTimer();
        PauseGameToBreathe();
        bool isLevelComplete = currentRank.nextRank.gameMode == GameMode.HighScore;
        if (isLevelComplete)
        {
            SetFirstRankInLevel(currentRank.nextRank);
        }
        else
        {
            SetNextRankInLevel(currentRank.nextRank);
        }
    }

    private void SetFirstRankInLevel(Rank newRank)
    {
        currentRank = newRank ?? currentRank;
        if (currentRank.gameMode == GameMode.Normal)
        {
            rankTimer.ResetTimer(GetAllRanksForNormalLevel());
        }
        else
        {
            rankTimer.ResetTimer(currentRank);
        }

        Difficulty = currentRank.difficulty;
    }

    private void SetNextRankInLevel(Rank newRank)
    {
        currentRank = newRank ?? currentRank;
        Difficulty = currentRank.difficulty;
    }

    private void Start()
    {
        // TODO: Pull from player storage here.
        // TODO: Delay this after tutorial.
        //StartGameFromNormal();
    }

    public void StartGameFromNormal()
    {
        isGameActive = true;
        currentPlayerHealth = startingPlayerHealth;
        SetFirstRankInLevel(initialRank);
        OnGameStarted?.Invoke();
        PauseGameToBreathe();
    }

    public void StartGameFromHighScore()
    {
        isGameActive = true;
        currentPlayerHealth = startingPlayerHealth;
        SetFirstRankInLevel(FindFirstHighScoreLevel());
        OnGameStarted?.Invoke();
        PauseToBreathe();
    }

    public void DamagePlayer(int damageAmount)
    {
        if (damageAmount < 0 || isGameActive == false || isPausedToBreathe == true) { return; }

        if (damageAmount > currentPlayerHealth)
        {
            currentPlayerHealth = 0;
        }
        else
        {
            currentPlayerHealth -= damageAmount;
            PlayerDamaged?.Invoke();
        }
    }

    private Rank FindFirstHighScoreLevel()
    {
        Rank rank = initialRank;
        while (rank != null || rank.gameMode != GameMode.HighScore)
        {
            rank = rank.nextRank;
        }

        return rank;
    }

    private void Update()
    {
        if (isGameActive == false)
        {
            return;
        }

        if (currentPlayerHealth <= 0)
        {
            rankTimer.PauseTimer();
            isGameActive = false;
            OnGameOver?.Invoke();
        }

        if (Input.GetAxis("Fire1") > 0)
        {
            sentry.FireSentry();
        }
    }

    private void StartLevel()
    {
        rankTimer.RestartTimer();
        OnLevelStarted?.Invoke();
    }

    private void PauseGameToBreathe()
    {
        isPausedToBreathe = true;
        StartCoroutine(PauseToBreathe());
    }
    
    private IEnumerator PauseToBreathe()
    {
        yield return new WaitForSeconds(breathPauseInSeconds);
        if (currentRank.gameMode == GameMode.Normal)
        {
            ContinueLevelFromPauseForBreath();
        }
        else
        {
            StartLevel();
        }

        isPausedToBreathe = false;
    }

    private void ContinueLevelFromPauseForBreath()
    {
        rankTimer.ResumeTimer();
        OnLevelContinued?.Invoke();
    }

    private void OnDestroy()
    {
        BreathingManager.OnHit -= OnHit;
        BreathingManager.OnFail -= OnFail;
        RankTimer.OnNextRankReached -= OnNextRankReached;
    }

    private IList<Rank> GetAllRanksForNormalLevel()
    {
        if (currentRank == null || currentRank.gameMode != GameMode.Normal)
        {
            throw new System.Exception("Attempted to get ranks for non-normal game mode. High score game mode only has one rank per level.");
        }

        IList<Rank> ranks = new List<Rank>();
        Rank rank = currentRank;
        do
        {
            ranks.Add(rank);
            rank = rank.nextRank;
        } while (rank != null && rank.gameMode == GameMode.Normal);

        return ranks;
    }

    
}
