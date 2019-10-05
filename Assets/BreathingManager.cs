using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingManager : MonoBehaviour
{
    public delegate void BreathEvent();
    public static event BreathEvent OnHit;
    public static event BreathEvent OnMiss;

    public const float INPUT_GRACE_BUFFER = 0.1f;

    public AudioSource clickSound;
    public AudioSource breathingSound;

    public Image feedbackImage;

    public double calibrationValue;

    private List<double> calibrationKeys;

    private double _rawInputTimestamp = 0;

    private double adjustedInputTimestamp
    {
        get { return _rawInputTimestamp - this.calibrationValue; }
        set { _rawInputTimestamp = value; }
    }

    public void Awake()
    {
        this.calibrationKeys = new List<double>();
        Metronome.OnBeat += this.PlayClickSound;
        StartCoroutine(Metronome.StartMetronome());
    }

    private void PlayClickSound()
    {
        this.clickSound.PlayScheduled(Metronome.nextBeatTime);

        //this.breathingSound.PlayScheduled(Metronome.nextBeatTime);

        this.StartCoroutine(this.DetectNoActionMiss());
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.adjustedInputTimestamp = AudioSettings.dspTime;

            bool inTimeForBeat = ((this.adjustedInputTimestamp - Metronome.previousBeatTime) <= INPUT_GRACE_BUFFER) ? true : false;

            if (inTimeForBeat)
            {
                this.BreathSuccess();
            }
            else
            {
                this.BreathFail();
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            double calibrationTimestamp = AudioSettings.dspTime;

            this.calibrationKeys.Add(calibrationTimestamp - Metronome.previousBeatTime);

            this.SetCalibrationAverage();
        }
    }

    private void BreathSuccess()
    {
        feedbackImage.sprite = Resources.Load<Sprite>("good");

        if (BreathingManager.OnHit != null)
        {
            BreathingManager.OnHit();
        }
    }

    private void BreathFail()
    {
        feedbackImage.sprite = Resources.Load<Sprite>("bad");
        Debug.LogError("BAD INPUT MISS");
        if (BreathingManager.OnMiss != null)
        {
            BreathingManager.OnMiss();
        }
    }

    private void SetCalibrationAverage()
    {
        double runningTotal = 0;

        for (int i = 0; i < this.calibrationKeys.Count; i++)
        {
            runningTotal += this.calibrationKeys[i];
        }

        this.calibrationValue = (runningTotal / this.calibrationKeys.Count);
    }

    //Mark a miss if the player did not input anything and completely missed a beat
    private IEnumerator DetectNoActionMiss()
    {
        double beatTime = Metronome.nextBeatTime;
        double curTime = Metronome.nextBeatTime;

        while (curTime <= (beatTime + INPUT_GRACE_BUFFER))
        {
            curTime = AudioSettings.dspTime;
            yield return null;
        }

        //Debug.LogError("Diff: " + Mathf.Abs((float)this.adjustedInputTimestamp - (float)beatTime));

        if (Mathf.Abs((float)this.adjustedInputTimestamp - (float)beatTime) > Metronome.secondsBetweenBeats) 
        {
            Debug.LogError("NO ACTION MISS");
            if (BreathingManager.OnMiss != null)
            {
                BreathingManager.OnMiss();
            }
        }

            
    }
}
