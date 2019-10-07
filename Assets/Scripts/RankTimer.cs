using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankTimer : MonoBehaviour
{
    public static Action OnTimeInRankCompleted;

    [SerializeField] private Slider slider = null;
    [SerializeField] private GameObject[] rankNotches = null;
    [SerializeField] private RectTransform sliderFillArea = null;

    private bool isActive = false;
    private float timeCompleted = 0.0F;
    private float totalDurationOfCurrentLevel = 0.0F;
    private IList<float> rankTimesForLevel = new List<float>();
    private int currentRankTimeIndex = 0;

    public void ResetTimer(Rank rank)
    {
        IList<Rank> ranks = new List<Rank> { rank };
        ResetTimer(ranks);
    }

    public void ResetTimer(IList<Rank> ranksInLevel)
    {
        if (ranksInLevel.Count == 0)
        {
            throw new System.Exception("Reset rank timer with no ranks.");
        }

        slider.SetValueWithoutNotify(0.0F);
        isActive = false;
        timeCompleted = 0.0F;
        SetRankTimesForLevel(ranksInLevel);
        DisableAllRankNotches();
        SetRankNotches(ranksInLevel);
    }

    private void SetRankTimesForLevel(IList<Rank> ranksInLevel)
    {
        float runningDuration = 0.0F;
        for (int i = 0, count = ranksInLevel.Count; i < count; i++)
        {
            runningDuration += ranksInLevel[i].rankDurationInSeconds;
            rankTimesForLevel.Add(runningDuration);
        }

        totalDurationOfCurrentLevel = runningDuration;
        currentRankTimeIndex = 0;
    }

    private void DisableAllRankNotches()
    {
        for (int i = 0, count = rankNotches.Length; i < count;  i++)
        {
            rankNotches[i].SetActive(false);
        }
    }

    private void SetRankNotches(IList<Rank> ranksInLevel)
    {
        if (rankNotches.Length < ranksInLevel.Count)
        {
            throw new System.Exception("There are more ranks to display than there are notches available. Number of Ranks: " + ranksInLevel.Count + " Number of notches: " + rankNotches.Length);
        }

        if (rankNotches.Length == 1)
        {
            // No point in setting a notch if there
            // is only one rank in the level.
            return;
        }

        Rect worldSpaceRect = GetRectInScreenCoordinates(sliderFillArea);
        float totalRange = worldSpaceRect.yMax - worldSpaceRect.yMin;
        float runningTotalOfRange = 0.0f;
        Vector3 notchPosition;
        for (int i = 0, count = ranksInLevel.Count - 1; i < count; i++)
        {
            runningTotalOfRange += ranksInLevel[i].rankDurationInSeconds;
            float percentageOfTotal = runningTotalOfRange / totalDurationOfCurrentLevel;
            notchPosition = rankNotches[i].transform.position;
            notchPosition.y = worldSpaceRect.yMin + (totalRange * percentageOfTotal);
            rankNotches[i].transform.position = notchPosition;
            rankNotches[i].SetActive(true);
        }
    }

    private Rect GetRectInScreenCoordinates(RectTransform uiElement)
    {
        Vector3[] worldCorners = new Vector3[4];
        uiElement.GetWorldCorners(worldCorners);
        return new Rect(
                      worldCorners[0].x,
                      worldCorners[0].y,
                      worldCorners[2].x - worldCorners[0].x,
                      worldCorners[2].y - worldCorners[0].y);
    }

    public void RestartTimer()
    {
        if (isActive == true)
        {
            return;
        }

        timeCompleted = 0.0F;
        isActive = true;
    }

    public void PauseTimer()
    {
        isActive = false;
    }

    public void ResumeTimer()
    {
        isActive = true;
    }

    private void Update()
    {
        if (isActive == false)
        {
            return;
        }

        timeCompleted += Time.deltaTime;
        slider.value = (timeCompleted) / totalDurationOfCurrentLevel;
        if (timeCompleted >= rankTimesForLevel[currentRankTimeIndex])
        {
            OnTimeInRankCompleted?.Invoke();
            currentRankTimeIndex = Math.Min(rankTimesForLevel.Count - 1, currentRankTimeIndex + 1);
        }
    }
}
