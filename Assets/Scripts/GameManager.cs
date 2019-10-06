using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    /// <summary>
    /// The boolean flag will represent whether
    /// the level was also completed along with
    /// ranking up.
    /// </summary>
    public static Action<bool> OnRankReached;
    public static Action OnGameStarted;

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

    private static GameManager instance = null;

    [SerializeField] private RankTimer rankTimer = null;
    [SerializeField] private Rank initialRank = null;
    [SerializeField] private float breathPauseInSeconds = 10.0F;
    private Rank currentRank;
    private bool isGamePausedForBreadth = true;
    private float levelStartTime = 0.0F;
    private float levelPauseTime = 0.0F;

    static GameManager() { }
    private GameManager() { }

    private void Awake()
    {
        BreathingManager.OnHit += OnHit;
        BreathingManager.OnFail += OnFail;
    }

    private void Start()
    {
        ChangeRank(initialRank);
    }

    public void ChangeRank(Rank newRank)
    {
        currentRank = newRank ?? currentRank;
        if (currentRank.gameMode == GameMode.Normal)
        {
            rankTimer.ResetTimer(GetAllRanksForNormalLevel(), GetTotalDurationOfNormalLevel());
        }
        else
        {
            rankTimer.ResetTimer(currentRank);
        }
    }

    private void OnHit()
    {
        sentry.AddAmmo(difficulty.ammoGrantForHit);
    }

    private void OnFail()
    {
        sentry.RemoveAmmo(difficulty.ammoReductionForMiss);
    }

    private void Update()
    {
        // TODO: Remove this debug code.
        if (Input.GetKeyDown(KeyCode.S) && isGamePausedForBreadth == true)
        {
            StartGame();
        }

        if (isGamePausedForBreadth == false)
        {
            if (Time.realtimeSinceStartup > levelStartTime + currentRank.rankDurationInSeconds)
            {
                PauseGameForBreadth();
                bool isLevelComplete = currentRank.nextRank.gameMode == GameMode.HighScore;
                OnRankReached?.Invoke(isLevelComplete);
                if (isLevelComplete)
                {
                    ChangeRank(currentRank.nextRank);
                } else
                {
                    // TODO: Clean this up.
                    currentRank = currentRank.nextRank;
                }
            }
        }
    }

    private void StartGame()
    {
        if (isGamePausedForBreadth == false)
        {
            return;
        }

        levelStartTime = Time.realtimeSinceStartup;
        rankTimer.StartTimer(levelStartTime);
        isGamePausedForBreadth = false;
        OnGameStarted?.Invoke();
    }

    public void PauseGameForBreadth()
    {
        if (isGamePausedForBreadth == true)
        {
            return;
        }

        levelPauseTime = Time.realtimeSinceStartup;
        rankTimer.StopTimer();
        isGamePausedForBreadth = true;
        StartCoroutine(PauseForBreath());
    }
    
    private IEnumerator PauseForBreath()
    {
        yield return new WaitForSeconds(breathPauseInSeconds);
        ContinueGameFromPauseForBreadth();
    }

    public void ContinueGameFromPauseForBreadth()
    {
        if (isGamePausedForBreadth == false)
        {
            return;
        }

        float diff = Time.realtimeSinceStartup - levelPauseTime;
        levelStartTime += diff;
        levelPauseTime = 0.0F;
        rankTimer.StartTimer(levelStartTime);
        isGamePausedForBreadth = false;
    }

    private void OnDestroy()
    {
        BreathingManager.OnHit -= OnHit;
        BreathingManager.OnFail -= OnFail;
    }

    private float GetTotalDurationOfNormalLevel()
    {
        IList<Rank> ranksInLevel = GetAllRanksForNormalLevel();
        float levelDuration = 0.0F;
        for (int i = 0, count = ranksInLevel.Count; i < count; i++)
        {
            levelDuration += ranksInLevel[i].rankDurationInSeconds;
        }

        return levelDuration;
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
