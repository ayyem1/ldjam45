
public enum GameMode
{
    Normal,
    HighScore
}

public static class GameModeExtensions
{
    public static GameMode FromString(string gameMode)
    {
        if (gameMode == "Normal")
        {
            return GameMode.Normal;
        }

        return GameMode.HighScore;
    }
}
