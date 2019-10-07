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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused())
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
        if (IsGamePaused() == false)
        {
            return;
        }

        pauseMenuItems2.SetActive(false);
        pauseMenuItems3.SetActive(false);

        GameManager.Instance.isGameActive = true;
        Time.timeScale = 1.0F;
        Metronome.ToggleMetronomePause();
    }

    private void PauseGame()
    {
        if (IsGamePaused() == true)
        {
            return;
        }

        if (GameManager.Instance.isGameActive)
        {
            pauseMenuItems3.SetActive(true);
        }
        else
        {
            pauseMenuItems2.SetActive(true);
        }

        GameManager.Instance.isGameActive = false;
        Time.timeScale = 0.0F;
        Metronome.ToggleMetronomePause();
    }

    private bool IsGamePaused()
    {
        return pauseMenuItems2.activeInHierarchy || pauseMenuItems3.activeInHierarchy;
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
