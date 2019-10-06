using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "Difficulty")]
public class Difficulty : ScriptableObject
{
    public float minSecondsBetweenSpawns;
    public float maxSecondsBetweenSpawns;
    [Range(0.0F, 100.0F)] public float minChanceToSpawnSuccessfully;
    [Range(0.0F, 100.0F)] public float maxChanceToSpawnSuccessfully;
    [Range(0.0F, 100.0F)] public float minChanceToSpawnElite;
    [Range(0.0F, 100.0F)] public float maxChanceToSpawnElite;
    public float minTemptationSpeed;
    public float maxTemptationSpeed;
    public uint ammoReductionForMiss;
    public uint ammoGrantForHit;
    public int breathBPM;
}
