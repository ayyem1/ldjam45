using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static int numSpawnedEliteTemptations;
    public static int numSpawnedTemptations;

    [SerializeField] private GameObject temptationPrefab;
    [SerializeField] private GameObject eliteTemptationPrefab;
    private float secondsBetweenSpawnAttempt;
    private float probabiltyOfSpawnSuccess;
    private float probabilityOfEliteSpawn;

    private IEnumerator spawnTemptations = null;

    public void ResetSpawner()
    {
        Difficulty difficulty = GameManager.Instance.difficulty;
        secondsBetweenSpawnAttempt = Random.Range(difficulty.minSecondsBetweenSpawns, difficulty.maxChanceToSpawnElite);
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
        // TODO: Implement this.
        yield return null;
    }

    public void StopSpawner()
    {
        if (spawnTemptations != null)
        {
            StopCoroutine(spawnTemptations);
        }
    }
}

