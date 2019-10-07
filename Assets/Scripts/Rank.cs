using UnityEngine;

[CreateAssetMenu(fileName = "New Rank", menuName = "Rank")]
public class Rank : ScriptableObject
{
    public float rankDurationInSeconds = 60.0F;
    public string rankName = "Sample Rank";
    public Rank nextRank;
    public GameMode gameMode;
    public Difficulty difficulty;
    public bool isBoss = false;
}
