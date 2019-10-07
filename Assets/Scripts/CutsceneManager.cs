using System.Collections;
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

    private bool calibrated = false;
    private bool hasStartedPlayingMusic = false;
    public bool playWoo = false;
    private bool playedWoo = false;

    public AudioSource partyMusic;
    public AudioSource tranquilMusic;
    public AudioSource wooSound;

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

        if (BreathingManager.calibrationKeys.Count >= 20 && calibrated == false)
        {
            this.cutsceneAnimator.SetBool("SufficientlyCalibrated", true);
            this.cloudAnimator.SetBool("ZoomInStarted", true);
            calibrated = true;
        }

        if (calibrated == true && hasStartedPlayingMusic == false)
        {
            StartCoroutine(AttenuatePartyMusic());
            hasStartedPlayingMusic = true;
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

        if (playWoo == true && playedWoo == false)
        {
            playedWoo = true;
            this.wooSound.Play();
        }
    }

    private IEnumerator AttenuatePartyMusic()
    {
        this.partyMusic.Play();

        float attenuationAmount = 0.01f;

        while (this.partyMusic.volume < 1)
        {
            this.partyMusic.volume += attenuationAmount;
            this.tranquilMusic.volume -= (2 * attenuationAmount);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        while (this.partyMusic.volume > 0)
        {
            this.partyMusic.volume -= (2* attenuationAmount);
            this.tranquilMusic.volume += (2 * attenuationAmount);

            if (this.tranquilMusic.volume > 0.3f)
            {
                this.tranquilMusic.volume = 0.3f;
            }

            yield return null;
        }
    }
}
