using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverMenu = null;
    private void Awake()
    {
        GameManager.OnGameOver += DisplayGameOver;
    }

    //private void Update()
    //{
    //    if (Input.get)
    //}

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
        gameOverMenu.SetActive(true);
    }

    public void DisableGameOver()
    {
        gameOverMenu.SetActive(false);
    }
}
