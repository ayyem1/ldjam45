using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public const int MAX_SPAWNABLE_ELITE_TEMPTATIONS = 3;
    public static int numSpawnedEliteTemptations;
    public static int numSpawnedTemptations;

    [SerializeField] private GameObject temptationPrefab;
    [SerializeField] private GameObject eliteTemptationPrefab;
    private float secondsBetweenSpawnAttempt;
    private float probabiltyOfSpawnSuccess;
    private float probabilityOfEliteSpawn;

    private IEnumerator spawnTemptations = null;

    public bool isTutorial = false;

    private void Awake()
    {
        if (isTutorial == false)
        {
            GameManager.OnLevelStarted += StartSpawner;
            GameManager.OnLevelContinued += StartSpawner;
            GameManager.OnGameOver += StopSpawner;
            RankTimer.OnTimeInRankCompleted += StopSpawner;
        }
    }

    public void StartTutorialSpawner()
    {
        ResetSpawner();
        StartCoroutine(SpawnTutorialTemptations());
    }

    public void StartSpawner()
    {
        ResetSpawner();
        if (spawnTemptations != null)
        {
            StopSpawner();
        }

        StartCoroutine(SpawnTemptations());
    }

    private void ResetSpawner()
    {
        Difficulty difficulty = GameManager.Instance.Difficulty;
        secondsBetweenSpawnAttempt = Random.Range(difficulty.minSecondsBetweenSpawns, difficulty.maxSecondsBetweenSpawns);
        probabiltyOfSpawnSuccess = Random.Range(difficulty.minChanceToSpawnSuccessfully, difficulty.minChanceToSpawnSuccessfully);
        probabilityOfEliteSpawn = Random.Range(difficulty.minChanceToSpawnElite, difficulty.maxChanceToSpawnElite);
    }

    public IEnumerator SpawnTemptations()
    {
        Difficulty difficulty = GameManager.Instance.Difficulty;

        while (true)
        {
            yield return new WaitForSeconds(this.secondsBetweenSpawnAttempt);
            this.AttemptTemptationSpawn();
        }
    }

    public IEnumerator SpawnTutorialTemptations()
    {


        Difficulty difficulty = GameManager.Instance.Difficulty;
        uint spawnedTemptations = 0;

        while (spawnedTemptations < 2)
        {
            yield return new WaitForSeconds(this.secondsBetweenSpawnAttempt);
            Instantiate(temptationPrefab, this.transform.position, new Quaternion());
            ++spawnedTemptations;
        }

        GameManager.Instance.StartGameFromNormal();
        this.gameObject.SetActive(false);
    }

    private void AttemptTemptationSpawn()
    {
        float initialSpawnRoll = Random.Range(0.0f, 100.0f);
        if (initialSpawnRoll <= probabiltyOfSpawnSuccess)
        {
            float eliteSpawnRoll = Random.Range(0.0f, 100.0f);
            if ((eliteSpawnRoll <= probabilityOfEliteSpawn) && (numSpawnedEliteTemptations < MAX_SPAWNABLE_ELITE_TEMPTATIONS))
            {
                Instantiate(eliteTemptationPrefab, this.transform.position, new Quaternion());
            }
            else
            {
                Instantiate(temptationPrefab, this.transform.position, new Quaternion());
            }
        }
    }

    public void StopSpawner()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        GameManager.OnLevelStarted -= StartSpawner;
        GameManager.OnLevelContinued -= StartSpawner;
        GameManager.OnGameOver -= StopSpawner;
        RankTimer.OnTimeInRankCompleted -= StopSpawner;
    }
}

