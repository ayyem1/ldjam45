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

    public uint playerHealth;
    public Vector3 finalPlayerPosition;

    static GameManager() { }
    private GameManager() { }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        // TODO: Need to extend this for high score mode.
        rankTimer.ResetTimer(GetAllRanksInNormalMode());
        rankTimer.StartTimer();
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
