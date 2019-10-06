using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathingManager : MonoBehaviour
{
    public delegate void BreathEvent();
    public static event BreathEvent OnHit;
    public static event BreathEvent OnMiss;
    public static event BreathEvent OnFail;

    public const float INPUT_GRACE_BUFFER = 0.1f;

    public AudioSource clickSound;
    public AudioSource breathingSound;

    public double calibrationValue;

    private List<double> calibrationKeys;

    private double _rawInputTimestamp = 0;

    private double adjustedInputTimestamp
    {
        get { return _rawInputTimestamp - this.calibrationValue; }
        set { _rawInputTimestamp = value; }
    }

    #region UnityFunctions
    public void Awake()
    {
        this.calibrationKeys = new List<double>();
        Metronome.OnBeat += this.PlayClickSound;
        StartCoroutine(Metronome.StartMetronome());
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.adjustedInputTimestamp = AudioSettings.dspTime;

            if (this.IsMostRecentInputOnBeat() == true)
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

            this.calibrationKeys.Add(calibrationTimestamp - Metronome.currentBeatTime);

            this.SetCalibrationAverage();
        }
    }
    #endregion

    private void SetCalibrationAverage()
    {
        double runningTotal = 0;

        for (int i = 0; i < this.calibrationKeys.Count; i++)
        {
            runningTotal += this.calibrationKeys[i];
        }

        this.calibrationValue = (runningTotal / this.calibrationKeys.Count);
    }

    #region InputBeatSyncChecks
    //Checks the right grace window of the current beat for overshoots and the left grace window of the next beat for undershoots
    //Used only to check if an input is a success or failure 
    private bool IsMostRecentInputOnBeat()
    {

        bool undershootTest = ((Metronome.nextBeatTime - INPUT_GRACE_BUFFER) <= this.adjustedInputTimestamp);
        bool overshootTest = ((Metronome.currentBeatTime + INPUT_GRACE_BUFFER) >= this.adjustedInputTimestamp);

        return (undershootTest || overshootTest);
    }

    //Only checks the left and right grace window of the current beat
    //Used only for detecting missed beats
    private bool WasBeatMissed()
    {
        bool withinUndershootThreshold = (this.adjustedInputTimestamp >= (Metronome.currentBeatTime - INPUT_GRACE_BUFFER));
        bool withinOvershootThreshold = (this.adjustedInputTimestamp <= (Metronome.currentBeatTime + INPUT_GRACE_BUFFER));

        return (!withinUndershootThreshold || !withinOvershootThreshold);
    }

    #endregion

    #region BreathCallbacks
    private void BreathSuccess()
    {
        Debug.LogError("BREATH SUCCESS!");
        if (BreathingManager.OnHit != null)
        {
            BreathingManager.OnHit();
        }
    }

    private void BreathFail()
    {
        Debug.LogError("BREATH FAIL");
        if (BreathingManager.OnFail != null)
        {
            BreathingManager.OnFail();
        }
    }
    #endregion

    private void PlayClickSound()
    {
        this.clickSound.PlayScheduled(Metronome.currentBeatTime);
        StartCoroutine(this.DetectBreathMiss());
    }

    #region Coroutines
    private IEnumerator DetectBreathMiss()
    {
        double currentDspTime = Metronome.currentBeatTime;

        //0.1 seconds is too fast to process additional inputs...
        //So we wait a little longer before we check the proper grace window in WasBeatMissed()
        //Yeah, I don't like it either.
        double endOfGraceBuffer = Metronome.currentBeatTime + (2* INPUT_GRACE_BUFFER);  
        
        //First, wait grace period
        while (currentDspTime < endOfGraceBuffer)
        {
            currentDspTime = AudioSettings.dspTime;
            yield return 0;
        }

        //Then, check to see if the beat was missed.
        //It's possible the player hit the beat within the grace window before and after the beat, so checks both sides
        if (this.WasBeatMissed())
        {
            Debug.LogError("MISS");
            if (BreathingManager.OnMiss != null)
            {
                BreathingManager.OnMiss();
            }
        }
    }
    #endregion
}
