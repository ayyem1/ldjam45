using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    public static Action OnLevelStarted;
    public static Action OnLevelContinued;

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

    [SerializeField] public Difficulty difficulty = null;
    [SerializeField] public Sentry sentry = null;
    public uint playerHealth;
    public Vector3 finalPlayerPosition;

    public uint maximumAmmoCount = 100;

    private static GameManager instance = null;

    [SerializeField] private RankTimer rankTimer = null;
    [SerializeField] private Rank initialRank = null;
    [SerializeField] private float breathPauseInSeconds = 10.0F;
    private Rank currentRank;
    private bool isGamePausedForBreadth = true;

    static GameManager() { }
    private GameManager() { }

    private void Awake()
    {
        BreathingManager.OnHit += OnHit;
        BreathingManager.OnFail += OnFail;
        RankTimer.OnNextRankReached += OnNextRankReached;
    }

    private void OnHit()
    {
        sentry.AddAmmo(difficulty.ammoGrantForHit);
    }

    private void OnFail()
    {
        sentry.RemoveAmmo(difficulty.ammoReductionForMiss);
    }

    private void OnNextRankReached()
    {
        rankTimer.PauseTimer();
        PauseGameForBreadth();
        bool isLevelComplete = currentRank.nextRank.gameMode == GameMode.HighScore;
        if (isLevelComplete)
        {
            SetFirstRankInLevel(currentRank.nextRank);
        }
        else
        {
            currentRank = currentRank.nextRank;
        }

        // TODO: Replace GameManager difficulty with rank's difficulty.
        //difficulty = currentRank.difficulty;
    }

    private void Start()
    {
        // TODO: Pull from player storage here.
        SetFirstRankInLevel(initialRank);
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
    }

    private void Update()
    {
        // TODO: Remove this debug code.
        if (Input.GetKeyDown(KeyCode.S) && isGamePausedForBreadth == true)
        {
            StartNewLevel();
        }
    }

    private void StartNewLevel()
    {
        if (isGamePausedForBreadth == false)
        {
            return;
        }

        isGamePausedForBreadth = false;
        rankTimer.RestartTimer();
        OnLevelStarted?.Invoke();
    }

    public void PauseGameForBreadth()
    {
        if (isGamePausedForBreadth == true)
        {
            return;
        }

        isGamePausedForBreadth = true;
        StartCoroutine(PauseForBreath());
    }
    
    private IEnumerator PauseForBreath()
    {
        yield return new WaitForSeconds(breathPauseInSeconds);
        if (currentRank.gameMode == GameMode.Normal)
        {
            ContinueLevelFromPauseForBreadth();
        }
        else
        {
            StartNewLevel();
        }
    }

    public void ContinueLevelFromPauseForBreadth()
    {
        if (isGamePausedForBreadth == false)
        {
            return;
        }

        rankTimer.ResumeTimer();
        isGamePausedForBreadth = false;
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
