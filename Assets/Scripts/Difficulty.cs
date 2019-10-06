using UnityEngine;

[CreateAssetMenu(fileName = "New Difficulty", menuName = "Difficulty")]
public class Difficulty : ScriptableObject
{
    public float minSecondsBetweenSpawns;
    public float maxSecondsBetweenSpawns;
    [Range(0.0F, 100.0F)] public float minChanceToSpawnSuccessfully;
    [Range(0.0F, 100.0F)] public float maxChanceToSpawnSuccessfully;
    public uint ammoReductionForMiss;
    public uint ammoGrantForHit;
    public int breathBPM;
}
