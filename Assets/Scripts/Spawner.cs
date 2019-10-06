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

    private void Start()
    {
        ResetSpawner();
        StartSpawner();   
    }

    public void ResetSpawner()
    {
        Difficulty difficulty = GameManager.Instance.difficulty;
        secondsBetweenSpawnAttempt = Random.Range(difficulty.minSecondsBetweenSpawns, difficulty.maxSecondsBetweenSpawns);
        probabiltyOfSpawnSuccess = Random.Range(difficulty.minChanceToSpawnSuccessfully, difficulty.minChanceToSpawnSuccessfully);
        probabilityOfEliteSpawn = Random.Range(difficulty.minChanceToSpawnElite, difficulty.maxChanceToSpawnElite);
    }

    public void StartSpawner()
    {
        if (spawnTemptations != null)
        {
            StopSpawner();
        }
        spawnTemptations = SpawnTemptations();
        StartCoroutine(spawnTemptations);
    }

    public IEnumerator SpawnTemptations()
    {
        Difficulty difficulty = GameManager.Instance.difficulty;

        while (true)
        {
            yield return new WaitForSeconds(this.secondsBetweenSpawnAttempt);
            this.AttemptTemptationSpawn();
        }
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
        if (spawnTemptations != null)
        {
            StopCoroutine(spawnTemptations);
        }
    }
}

