using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Game Level")]
public class GameLevel : ScriptableObject
{
    public float levelDurationInSeconds = 60.0F;
    public string levelName = "Nirvana";
    public GameLevel nextLevel = null;
}
