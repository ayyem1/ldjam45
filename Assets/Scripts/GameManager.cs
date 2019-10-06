using System.Collections.Generic;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    [SerializeField] private Rank firstRankOfHighScoreMode = null;
    [SerializeField] private RankTimer rankTimer = null;
    
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
    [SerializeField] public Rank currentRank = null;

    private bool isGameActive = false;

    static GameManager() { }
    private GameManager() { }

    private void Awake()
    {
        BreathingManager.OnHit += OnHit;
        BreathingManager.OnFail += OnFail;
    }

    private void Start()
    {
        ResetGame();
    }

    public void ResetGame()
    {
        rankTimer.ResetTimer(GetAllRanksInNormalMode());
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
        if (Input.GetKeyDown(KeyCode.S) && isGameActive == false)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        // TODO: Need to extend this for high score mode.
        rankTimer.StartTimer();
        isGameActive = true;
    }

    public void StopGame()
    {
        rankTimer.StopTimer();
        isGameActive = false;
    }

    private void OnDestroy()
    {
        BreathingManager.OnHit -= OnHit;
        BreathingManager.OnFail -= OnFail;
    }

    private IList<Rank> GetAllRanksInNormalMode()
    {
        IList<Rank> ranks = new List<Rank>();
        Rank rank = currentRank;
        while (rank !=null && rank.rankName != firstRankOfHighScoreMode.rankName)
        {
            ranks.Add(rank);
            rank = rank.nextRank;
        }

        return ranks;
    }
}
