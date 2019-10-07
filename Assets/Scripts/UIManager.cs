using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverMenu = null;
    [SerializeField] private GameObject pauseMenuItems2 = null;
    [SerializeField] private GameObject pauseMenuItems3 = null;

    private void Awake()
    {
        GameManager.OnGameOver += DisplayGameOver;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//Input.GetAxis("Cancel") > 0)
        {
            if (pauseMenuItems2.activeInHierarchy || pauseMenuItems3.activeInHierarchy)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void ContinueGame()
    {
        pauseMenuItems2.SetActive(false);
        pauseMenuItems3.SetActive(false);
        Time.timeScale = 1.0F;
    }

    private void PauseGame()
    {
        if (GameManager.Instance.isGameActive)
        {
            pauseMenuItems3.SetActive(true);
        }
        else
        {
            pauseMenuItems2.SetActive(true);
        }

        Time.timeScale = 0.0F;
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

    public void ExitGame()
    {
        Application.Quit();
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
