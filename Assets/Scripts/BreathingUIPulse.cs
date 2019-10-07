using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingUIPulse : MonoBehaviour
{
    const float FEEDBACK_DISPLAY_DURATION = 0.1f;

    private Color originalColor;

    public GameObject successObject;
    public GameObject failObject;

    private void Awake()
    {
        MaskableGraphic mainPanel = this.gameObject.GetComponent<MaskableGraphic>();

        Metronome.OnBeat += this.ExecutePulse;
        BreathingManager.OnHit += this.DisplaySuccess;
        BreathingManager.OnFail += this.DisplayFail;
        this.originalColor = mainPanel.color;
    }


    private void ExecutePulse()
    {
        this.StartCoroutine(Pulse());
    }

    private IEnumerator Pulse()
    {
        MaskableGraphic[] uiElements = this.gameObject.GetComponentsInChildren<MaskableGraphic>();

        double currentDspTime = Metronome.currentBeatTime;
        double nextBeatTime = Metronome.nextBeatTime;
        double shineDuration = BreathingManager.INPUT_GRACE_BUFFER;
        double shineTimestamp = currentDspTime + shineDuration;
        double fadeDuration = (nextBeatTime - currentDspTime - shineDuration);

        //Initial Shine
        while (currentDspTime < shineTimestamp)
        {
            uiElements[0].color = new Color(originalColor.r, originalColor.g, 0, originalColor.a);
            currentDspTime = AudioSettings.dspTime;
            yield return 0;
        }

        uiElements[0].color = new Color(originalColor.r, originalColor.g, originalColor.b, originalColor.a);

        //Fade Out
        while (currentDspTime < nextBeatTime)
        {
            float currentAlphaLevel = Mathf.Lerp(0.0f, 1.0f, (float)(nextBeatTime - currentDspTime) / (float)(fadeDuration));
            
            for (int i = 0; i < uiElements.Length; i++)
            {
                Color existingColor = uiElements[i].color;
                uiElements[i].color = new Color(existingColor.r, existingColor.g, existingColor.b, currentAlphaLevel);
            }

            currentDspTime = AudioSettings.dspTime;
            yield return 0;
        }

        //Fade In
        /*while (currentDspTime < nextBeatTime)
        {
            float currentAlphaLevel = Mathf.Lerp(1.0f, 0.0f, (float)(nextBeatTime - currentDspTime) / (float)(fadeDuration));

            for (int i = 0; i < uiElements.Length; i++)
            {
                Color existingColor = uiElements[i].color;
                uiElements[i].color = new Color(existingColor.r, existingColor.g, existingColor.b, currentAlphaLevel);
            }

            //Shine to signal when player can hit the beat
            if (currentDspTime >= (nextBeatTime - shineDuration))
            {
                uiElements[0].color = new Color(originalColor.r, originalColor.g, 0, currentAlphaLevel);
            }
            else
            {
                uiElements[0].color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlphaLevel);
            }

            currentDspTime = AudioSettings.dspTime;
            yield return 0;
        }*/
    }

    private void DisplaySuccess()
    {
        if (GameManager.Instance.isGameActive == true || GameManager.Instance.tutorialStarted == true)
        {
            StartCoroutine(this.DisplayFeedbackObject(this.successObject));
        }
        
    }

    private void DisplayFail()
    {
        if (GameManager.Instance.isGameActive == true || GameManager.Instance.tutorialStarted == true)
        {
            StartCoroutine(this.DisplayFeedbackObject(this.failObject));
        }
    }

    private IEnumerator DisplayFeedbackObject(GameObject objectToToggle)
    {
        objectToToggle.SetActive(true);
        yield return new WaitForSeconds(FEEDBACK_DISPLAY_DURATION);
        objectToToggle.SetActive(false);
    }
}
