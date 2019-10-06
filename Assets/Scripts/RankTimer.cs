using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankTimer : MonoBehaviour
{
    [SerializeField] private Slider slider = null;
    [SerializeField] private GameObject[] rankNotches = null;
    [SerializeField] private RectTransform sliderFillArea = null;

    private bool isActive = false;
    private float startTime = 0.0F;
    private float totalDurationOfCurrentLevel;

    public void ResetTimer(Rank rank)
    {
        IList<Rank> ranks = new List<Rank> { rank };
        ResetTimer(ranks, rank.rankDurationInSeconds);
    }

    public void ResetTimer(IList<Rank> ranksInLevel, float totalDurationOfLevel)
    {
        slider.SetValueWithoutNotify(0.0F);
        isActive = false;
        totalDurationOfCurrentLevel = totalDurationOfLevel;
        SetRankNotches(ranksInLevel);
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

    public void StartTimer(float startTime)
    {
        if (isActive == true)
        {
            return;
        }

        this.startTime = startTime;
        isActive = true;
    }

    public void StopTimer()
    {
        isActive = false;
    }

    private void Update()
    {
        if (isActive == false)
        {
            return;
        }

        slider.value = (Time.realtimeSinceStartup - startTime) / totalDurationOfCurrentLevel;
    }
}
