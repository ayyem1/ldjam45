using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public UIManager uiManager;
    public Animator cutsceneAnimator;
    public Animator cloudAnimator;
    public static bool isFirstSpacebarHit = true;
    public bool isRotationFinished = false;
    public bool isZoomFinished = false;
    public Spawner tutorialSpawner;
    public static bool isCutsceneStarted = false;

    private void Start()
    {
        isCutsceneStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCutsceneStarted == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && CutsceneManager.isFirstSpacebarHit)
        {
            CutsceneManager.isFirstSpacebarHit = false;
            this.cutsceneAnimator.SetBool("InitiatedGame", true);
            this.cutsceneAnimator.SetBool("MainMenuClicked", false);
        }

        if (BreathingManager.calibrationKeys.Count >= 20)
        {
            this.cutsceneAnimator.SetBool("SufficientlyCalibrated", true);
            this.cloudAnimator.SetBool("ZoomInStarted", true);
        }

        if (isRotationFinished == true)
        {
            GameManager.Instance.tutorialStarted = true;
            isRotationFinished = false;
            this.uiManager.DisplayAmmoBar();
        }

        if (isZoomFinished == true)
        {
            isZoomFinished = false;
            this.tutorialSpawner.gameObject.SetActive(true);
            GameManager.Instance.sentry.gameObject.SetActive(true);
            uiManager.sentryArc.SetActive(true);
            tutorialSpawner.StartTutorialSpawner();
            isCutsceneStarted = false;
        }
    }
}
