using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public const string RankUpAnnouncementText = "Rank Up!";
    public const string BossLevelAnnouncementText = "Cheeramid Incoming!";
    public const string BossLevelAnnouncementSubText = "Prepare Yourself!";

    [SerializeField] private GameObject gameOverMenu = null;
    [SerializeField] private GameObject victoryMenu = null;
    [SerializeField] private GameObject pauseMenuItems2 = null;
    [SerializeField] private GameObject pauseMenuItems3 = null;
    [SerializeField] private Text announcementText = null;
    [SerializeField] private Text announcementSubText = null;
    [SerializeField] private GameObject ammoBar = null;
    [SerializeField] public GameObject sentryArc;
    public Image titleImage;

    public Animator introCutsceneAnimator;

    public AudioSource clickSound;

    private void Awake()
    {
        GameManager.OnGameOver += DisplayGameOver;
        GameManager.OnRankChanged += DisplayRankUpAnnouncement;
        GameManager.OnBossSpawned += DisplayBossAnnouncement;
        Temptation.OnBossDied += DisplayVictory;
    }

    public void DisableGameOver()
    {
        gameOverMenu.SetActive(false);
    }

    public void DisableVictory()
    {
        victoryMenu.SetActive(false);
    }

    private void DisplayRankUpAnnouncement(Rank newRank)
    {
        announcementText.text = RankUpAnnouncementText;
        announcementSubText.text = newRank.rankName;

        StartCoroutine(DisplayAnnouncement());
    }

    private void DisplayBossAnnouncement()
    {
        announcementText.text = BossLevelAnnouncementText;
        announcementSubText.text = BossLevelAnnouncementSubText;
        StartCoroutine(DisplayAnnouncement());
    }

    private IEnumerator DisplayAnnouncement()
    {
        float alpha = 0.0F;
        float time = 1.0F;
        announcementText.color = new Color(announcementText.color.r, announcementText.color.g, announcementText.color.b, alpha);
        announcementSubText.color = new Color(announcementSubText.color.r, announcementSubText.color.g, announcementSubText.color.b, alpha);

        announcementText.gameObject.SetActive(true);
        announcementSubText.gameObject.SetActive(true);

        while (alpha < 1.0f)
        {
            alpha += (Time.deltaTime / time);
            announcementText.color = new Color(announcementText.color.r, announcementText.color.g, announcementText.color.b, alpha);
            announcementSubText.color = new Color(announcementSubText.color.r, announcementSubText.color.g, announcementSubText.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1.5F);

        alpha = 1.0F;
        while (alpha > 0.0f)
        {
            alpha -= (Time.deltaTime / time);
            announcementText.color = new Color(announcementText.color.r, announcementText.color.g, announcementText.color.b, alpha);
            announcementSubText.color = new Color(announcementSubText.color.r, announcementSubText.color.g, announcementSubText.color.b, alpha);
            yield return null;
        }

        announcementText.gameObject.SetActive(false);
        announcementSubText.gameObject.SetActive(false);
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
        GameManager.Instance.isGameActive = true;
        Time.timeScale = 1.0F;
        Metronome.metronomePaused = false;
    }

    private void PauseGame()
    {
        if (IsGamePaused() == true)
        {
            return;
        }

        pauseMenuItems2.SetActive(true);
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
        Rank rank = GameManager.Instance.CurrentRank;
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
        this.clickSound.Play();

        this.ammoBar.SetActive(false);
        GameManager.Instance.sentry.gameObject.SetActive(false);
        sentryArc.SetActive(false);
        GameManager.Instance.isGameActive = false;
        GameManager.Instance.tutorialStarted = false;
        GameManager.Instance.rankTimer.PauseTimer();
        GameManager.Instance.rankTimer.gameObject.SetActive(false);
        introCutsceneAnimator.SetBool("MainMenuClicked", true);
        introCutsceneAnimator.SetBool("InitiatedGame", false);
        introCutsceneAnimator.SetBool("SufficientlyCalibrated", false);
        BreathingManager.calibrationKeys.Clear();
        CutsceneManager.calibrated = false;
        if (Metronome.metronomeStarted == false)
        {
            StartCoroutine(Metronome.StartMetronome());
        }
        else if (Metronome.metronomePaused)
        {
            Metronome.ToggleMetronomePause();
        }

        CutsceneManager.StartCutscene();
        GameManager.Instance.StartGameFromTutorial();
        DisableGameOver();
        Color titleImageColor = this.titleImage.color;
        this.titleImage.color = new Color(titleImageColor.r, titleImageColor.g, titleImageColor.b, 1.0f);
    }

    public void ExitGame()
    {
        this.clickSound.Play();
        Application.Quit();
    }

    public void DisplayGameOver()
    {
        Metronome.metronomePaused = true;
        gameOverMenu.SetActive(true);
    }

    public void DisplayVictory()
    {
        Metronome.metronomePaused = true;
        victoryMenu.SetActive(true);
    }

    public void DisplayAmmoBar()
    {
        ammoBar.SetActive(true);
    }
}
