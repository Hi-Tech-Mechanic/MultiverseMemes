using System.Collections;
using TMPro;
using UnityEngine;

public class CalendarCountingDown : DataStructure
{
    [SerializeField] private TextMeshProUGUI _timeToNextRewardText;

    private void OnEnable()
    {
        StartCoroutine(CountingDownCoroutine());
        Game.NewDayEvent += Calendar.Init;
    }

    private void OnDisable()
    {
        StopCoroutine(CountingDownCoroutine());
        Game.NewDayEvent -= Calendar.Init;
    }

    private IEnumerator CountingDownCoroutine()
    {
        while (Game.GetSecondsToNextDay() >= 0)
        {
            DisplayTimeToNextReward();
            Game.CheckVisitingDays();
            yield return new WaitForSeconds(1);
        }
    }

    private void DisplayTimeToNextReward()
    {
        _timeToNextRewardText.text = $"До следующего дня:" +
            $"{ValuesRounding.GetFormattedLongTime(Game.GetSecondsToNextDay())}";
    }
}
