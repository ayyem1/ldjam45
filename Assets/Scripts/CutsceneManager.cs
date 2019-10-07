using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public Animator cutsceneAnimator;
    public Animator cloudAnimator;
    public bool isFirstSpacebarHit = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && this.isFirstSpacebarHit)
        {
            this.isFirstSpacebarHit = false;
            this.cutsceneAnimator.SetBool("InitiatedGame", true);
        }

        if (BreathingManager.calibrationKeys.Count >= 20)
        {
            this.cutsceneAnimator.SetBool("SufficientlyCalibrated", true);
            this.cloudAnimator.SetBool("ZoomInStarted", true);
        }
    }
}
