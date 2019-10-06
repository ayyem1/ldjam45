using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankTimer : MonoBehaviour
{
    [SerializeField] private Slider slider = null;
    [SerializeField] private GameObject[] rankNotches = null;
    [SerializeField] private RectTransform sliderFillArea;

    private bool isActive = false;
    private float levelStartTime = 0.0F;

    private IList<Rank> ranksToDisplay;

    public void ResetTimer(IList<Rank> ranksToDisplay)
    {
        this.ranksToDisplay = ranksToDisplay;
        slider.SetValueWithoutNotify(0.0F);
        isActive = false;
        SetRankNotches();
    }

    public void ResetTimer(Rank rankToDisplay)
    {
        IList<Rank> ranks = new List<Rank> { rankToDisplay };
        ResetTimer(ranks);
    }

    private void SetRankNotches()
    {
        if (rankNotches.Length < ranksToDisplay.Count)
        {
            throw new System.Exception("There are more ranks to display than there are notches available. Number of Ranks: " + ranksToDisplay.Count + " Number of notches: " + rankNotches.Length);
        }

        if (rankNotches.Length == 1)
        {
            // No point in setting a notch if there
            // is only one rank in the level.
            return;
        }

        float totalDuration = GetTotalDurationOfLevel();
        Rect worldSpaceRect = GetRectInScreenCoordinates(sliderFillArea);
        float totalRange = worldSpaceRect.yMax - worldSpaceRect.yMin;
        float runningTotalOfRange = 0.0f;
        Vector3 notchPosition;
        for (int i = 0, count = ranksToDisplay.Count - 1; i < count; i++)
        {
            runningTotalOfRange += ranksToDisplay[i].rankDurationInSeconds;
            float percentageOfTotal = runningTotalOfRange / totalDuration;
            notchPosition = rankNotches[i].transform.position;
            notchPosition.y = worldSpaceRect.yMin + (totalRange * percentageOfTotal);
            rankNotches[i].transform.position = notchPosition;
            rankNotches[i].SetActive(true);
        }
    }

    public Rect GetRectInScreenCoordinates(RectTransform uiElement)
    {
        Vector3[] worldCorners = new Vector3[4];
        uiElement.GetWorldCorners(worldCorners);
        return new Rect(
                      worldCorners[0].x,
                      worldCorners[0].y,
                      worldCorners[2].x - worldCorners[0].x,
                      worldCorners[2].y - worldCorners[0].y);
    }

    private void Update()
    {
        if (isActive == false)
        {
            return;
        }

        slider.SetValueWithoutNotify(GetPercentageOfLevelComplete());
        if (slider.value == slider.maxValue)
        {
            StopTimer();
        }
    }

    private float GetPercentageOfLevelComplete()
    {
        float levelDuration = GetTotalDurationOfLevel();
        float currentTime = Time.realtimeSinceStartup;
        return (currentTime - levelStartTime) / levelDuration;
    }

    private float GetTotalDurationOfLevel()
    {
        float levelDuration = 0.0F;
        for (int i = 0, count = ranksToDisplay.Count; i < count; i++)
        {
            levelDuration += ranksToDisplay[i].rankDurationInSeconds;
        }

        return levelDuration;
    }

    public void StopTimer()
    {
        isActive = false;
    }

    public void StartTimer()
    {
        levelStartTime = Time.realtimeSinceStartup;
        isActive = true;
    }
}
