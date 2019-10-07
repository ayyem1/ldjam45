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
    public static Action<Rank> OnRankChanged;
    public static Action OnBossSpawned;

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
    public Rank CurrentRank { get; private set; }

    [SerializeField] public Sentry sentry = null;

    public Vector3 finalPlayerPosition;
    public bool isGameActive;
    public bool tutorialStarted;
    public uint maximumAmmoCount = 100;

    private static GameManager instance = null;

    [SerializeField] public RankTimer rankTimer = null;
    [SerializeField] private Rank initialRank = null;
    [SerializeField] private Difficulty tutorialDifficulty = null;
    [SerializeField] private float breathPauseInSeconds = 10.0F;
    [SerializeField] private int startingPlayerHealth = 3;
    [SerializeField] public uint ammoAmount = 10;

    private int currentPlayerHealth;
    private bool isPausedToBreathe = true;

    public AudioSource playerDamagedSound;
    public AudioSource rankUpSound;

    public Spawner bossSpawner;

    static GameManager() { }
    private GameManager() { }

    private void Awake()
    {
        BreathingManager.OnHit += OnHit;
        BreathingManager.OnFail += OnFail;

        if (Metronome.metronomeStarted == false)
        {
            StartCoroutine(Metronome.StartMetronome());
        }

        RankTimer.OnTimeInRankCompleted += OnNextRankReached;
    }

    private void OnHit()
    {
        if (tutorialStarted == true || isGameActive == true)
        {
            AddAmmo(Difficulty.ammoGrantForHit);
        }
    }

    private void OnFail()
    {
        if (tutorialStarted == true || isGameActive == true)
        {
            RemoveAmmo(Difficulty.ammoReductionForMiss);
        }
    }

    private void OnNextRankReached()
    {
        if (isGameActive == false)
        {
            return;
        }

        rankTimer.PauseTimer();
        StartCoroutine(CompleteRank());
    }

    private IEnumerator CompleteRank()
    {
        yield return new WaitForSeconds(1.0F);
        bool isLevelComplete = CurrentRank.nextRank.gameMode == GameMode.HighScore;
        if (isLevelComplete)
        {
            SetFirstRankInLevel(CurrentRank.nextRank, false);
        }
        else
        {
            SetNextRankInLevel(CurrentRank.nextRank, false);
        }
        PauseGameToBreathe();
    }

    private void SetFirstRankInLevel(Rank newRank, bool isFirstLoadedRank)
    {
        CurrentRank = newRank ?? CurrentRank;
        if (isFirstLoadedRank == false && newRank != null)
        {
            OnRankChanged?.Invoke(CurrentRank);
        }

        if (CurrentRank.gameMode == GameMode.Normal)
        {
            rankTimer.ResetTimer(GetAllRanksForNormalLevel());
        }
        else
        {
            rankTimer.ResetTimer(CurrentRank);
        }

        Difficulty = CurrentRank.difficulty;
    }

    private void SetNextRankInLevel(Rank newRank, bool isFirstLoadedRank)
    {
        CurrentRank = newRank ?? CurrentRank;
        if (isFirstLoadedRank == false && newRank != null)
        {
            OnRankChanged?.Invoke(CurrentRank);

            this.rankUpSound.Play();
        }

        Difficulty = CurrentRank.difficulty;
    }

    private void Start()
    {
        // TODO: Pull from player storage here.
        StartGameFromTutorial();
    }

    public void StartGameFromTutorial()
    {
        CurrentRank = null;
        currentPlayerHealth = startingPlayerHealth;
        Difficulty = tutorialDifficulty;
    }

    public void StartGameFromNormal()
    {
        this.rankTimer.gameObject.SetActive(true);
        isGameActive = true;
        currentPlayerHealth = startingPlayerHealth;
        SetFirstRankInLevel(initialRank, true);
        OnGameStarted?.Invoke();
        PauseGameToBreathe();
    }

    public void StartGameFromHighScore()
    {
        isGameActive = true;
        currentPlayerHealth = startingPlayerHealth;
        SetFirstRankInLevel(FindFirstHighScoreLevel(), true);
        OnGameStarted?.Invoke();
        PauseGameToBreathe();
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

            float pitch = UnityEngine.Random.Range(-0.1f, 0.1f);
            this.playerDamagedSound.pitch = this.playerDamagedSound.pitch + pitch;

            this.playerDamagedSound.Play();
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
        if (isGameActive == true)
        {
            if (currentPlayerHealth <= 0)
            {
                rankTimer.PauseTimer();
                isGameActive = false;
                OnGameOver?.Invoke();
            }
        }

        if (isGameActive == true || (isGameActive == false && sentry.gameObject.activeSelf == true))
        {
            if (Input.GetAxis("Fire1") > 0)
            {
                sentry.FireSentry();
            }
        }
    }

    private void StartLevel()
    {
        if (CurrentRank.isBoss == false)
        {
            rankTimer.RestartTimer();
            OnLevelStarted?.Invoke();
        }
    }

    private void PauseGameToBreathe()
    {
        if (tutorialStarted == true)
        {
            StartLevel();
            tutorialStarted = false;
            isPausedToBreathe = false;
            return;
        }

        isPausedToBreathe = true;
        StartCoroutine(PauseToBreathe());
    }
    
    private IEnumerator PauseToBreathe()
    {
        // Wait for 2/3s of the time.
        yield return new WaitForSeconds(2 * breathPauseInSeconds / 3);

        if (this.CurrentRank.isBoss == true)
        {
            OnBossSpawned?.Invoke();
        }

        // Then wait for the remaining 1/3 of the time.
        yield return new WaitForSeconds(breathPauseInSeconds / 3);

        if (this.CurrentRank.isBoss == true)
        {
            this.bossSpawner.ForceSpawnBoss();
        }

        if (CurrentRank.gameMode == GameMode.Normal)
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
        if (CurrentRank.isBoss == false)
        {
            rankTimer.ResumeTimer();
            OnLevelContinued?.Invoke();
        }
    }

    private void OnDestroy()
    {
        BreathingManager.OnHit -= OnHit;
        BreathingManager.OnFail -= OnFail;
        RankTimer.OnTimeInRankCompleted -= OnNextRankReached;
    }

    private IList<Rank> GetAllRanksForNormalLevel()
    {
        if (CurrentRank == null || CurrentRank.gameMode != GameMode.Normal)
        {
            throw new System.Exception("Attempted to get ranks for non-normal game mode. High score game mode only has one rank per level.");
        }

        IList<Rank> ranks = new List<Rank>();
        Rank rank = CurrentRank;
        do
        {
            ranks.Add(rank);
            rank = rank.nextRank;
        } while (rank != null && rank.gameMode == GameMode.Normal);

        return ranks;
    }

    public void AddAmmo(uint amountToAdd)
    {
        ammoAmount += amountToAdd;

        if (Instance.ammoAmount > GameManager.Instance.maximumAmmoCount)
        {
            Instance.ammoAmount = GameManager.Instance.maximumAmmoCount;
        }
    }

    public void RemoveAmmo(uint amountToRemove)
    {
        if (amountToRemove > GameManager.Instance.ammoAmount)
        {
            Instance.ammoAmount = 0;
        }
        else
        {
            Instance.ammoAmount -= amountToRemove;
        }
    }
}
