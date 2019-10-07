using System.Collections;
using UnityEngine;

public static class Metronome
{
    public delegate void MetronomeBeat();
    public static event MetronomeBeat OnBeat;

    private static float beatsPerMinute = 80f;
    public static float secondsBetweenBeats = 0f;

    public static double currentBeatTime = 0;
    public static double nextBeatTime = 0;

    public static bool metronomeStarted = false;
    public static bool metronomePaused = false;

    public static IEnumerator StartMetronome()
    {
        Metronome.secondsBetweenBeats = 60.0f / Metronome.beatsPerMinute;

        Metronome.nextBeatTime = AudioSettings.dspTime;

        Metronome.metronomeStarted = true;


        while (true)
        {
            if (Metronome.metronomePaused == false)
            {
                double curTime = AudioSettings.dspTime;
                if (curTime >= nextBeatTime)
                {
                    Metronome.currentBeatTime = Metronome.nextBeatTime;
                    Metronome.nextBeatTime += Metronome.secondsBetweenBeats;

                    if (Metronome.OnBeat != null)
                    {
                        Metronome.OnBeat();
                    }
                }
            }
            else
            {
                Metronome.nextBeatTime = AudioSettings.dspTime;
            }

            yield return null;
        }
    }

    public static void ToggleMetronomePause()
    {
        Metronome.metronomePaused = !Metronome.metronomePaused;
    }
}
