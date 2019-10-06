using UnityEngine;

[CreateAssetMenu(fileName = "New Rank", menuName = "Rank")]
public class Rank : ScriptableObject
{
    public float rankDurationInSeconds = 60.0F;
    public string rankName = "Sample Rank";
    public Rank nextRank = null;
}
