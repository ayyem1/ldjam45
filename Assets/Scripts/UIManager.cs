using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen = null;
    private void Awake()
    {
        GameManager.OnGameOver += DisplayGameOver;
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= DisplayGameOver;
    }

    public void RestartGame()
    {
        DisableGameOver();
        Rank rank = GameManager.Instance.currentRank;
        if (rank.gameMode == GameMode.Normal)
        {
            GameManager.Instance.StartGameFromNormal();
        }
        else
        {
            GameManager.Instance.StartGameFromHighScore();
        }
    }

    public void ToMainMenu()
    {
        Debug.Log("To Main Menu.");
        DisableGameOver();
    }

    public void DisplayGameOver()
    {
        gameOverScreen.SetActive(true);
    }

    public void DisableGameOver()
    {
        gameOverScreen.SetActive(false);
    }
}
