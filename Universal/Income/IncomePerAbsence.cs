using System;
using TMPro;
using UnityEngine;
using YG;

public class IncomePerAbsence : DataStructure
{
    [SerializeField] private TextMeshProUGUI _absenceIncomeText;
    [SerializeField] private TextMeshProUGUI _maxAbsenceTimeText;
    [SerializeField] private TextMeshProUGUI _timePassedText;
    [SerializeField] private TextMeshProUGUI _ADButtonText;
    private long _accumulatedSeconds;
    private float _accumulatedIncome;

    private void OnEnable()
    {
        _ADButtonText.text = $"{GlobalUpgrades.ADRewardMultiplier}x(AD)";
        CalcIncomePerAbsence();

        YandexGame.RewardVideoEvent += ADReward;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= ADReward;
    }

    public void GetAverageReward()
    {
        MoneyMenu.IncreaseMoneyCapital(_accumulatedIncome);
        Destroy(gameObject);
    }

    public void GetADReward()
    {
        if (_accumulatedIncome > 0)
        {
            YandexGame.RewVideoShow((int)Game.RewardIndex.OfflineReward);
        }
    }

    private void ADReward(int i)
    {
        if (i == (int)Game.RewardIndex.OfflineReward)
        {
            Game.AccumulateWatchedAD();

            MoneyMenu.IncreaseMoneyCapital(_accumulatedIncome * GlobalUpgrades.ADRewardMultiplier);
            Destroy(gameObject);

            SaveAndLoad.SavePlayerData();
        }
    }

    private void CalcIncomePerAbsence()
    {
        long residual = Game.CurrentTime - Game.LastPlayingTime[Game.CurrentScene];
        TimeSpan elapsedSpan = new(residual);

        _accumulatedSeconds = (long)elapsedSpan.TotalSeconds;

        if (_accumulatedSeconds < 0)
            _accumulatedSeconds = 0;

        if (_accumulatedSeconds > GlobalUpgrades.AbsenceTime)
            _accumulatedSeconds = GlobalUpgrades.AbsenceTime;

        _accumulatedIncome = MoneyMenu.GetIdleIncomePerSecond() * _accumulatedSeconds;

        DisplayIdlePerAbsence(elapsedSpan);
        DisplayMaxAbsenceTime();
        DisplayAbsenceIncome(_accumulatedIncome);
    }

    private void DisplayIdlePerAbsence(TimeSpan time)
    {
        int days = time.Days;
        string dayMessage;
        int hours = time.Hours;
        string hourMessage;
        int minutes = time.Minutes;
        string minuteMessage;
        int seconds = time.Seconds;
        string secondMessage;

        if ((days % 10) == 1)
            dayMessage = "день";
        else if ((days % 10) > 1 && (days % 10) <= 4)
            dayMessage = "дня";
        else dayMessage = "дней";
        if (days > 10 && days <= 20)
            dayMessage = "дней";

        if ((hours % 10) == 1)
            hourMessage = "час";
        else if ((hours % 10) > 1 && (hours % 10) <= 4)
            hourMessage = "часа";
        else hourMessage = "часов";
        if (hours > 10 && hours <= 20)
            hourMessage = "часов";

        if ((minutes % 10) == 1)
        {
            minuteMessage = "минута";
            if (minutes == 11)
                minuteMessage = "минут";
        }
        else if ((minutes % 10) > 1 && (minutes % 10) <= 4)
            minuteMessage = "минуты";
        else minuteMessage = "минут";

        if ((seconds % 10) == 1)
        {
            secondMessage = "секунда";
            if (seconds == 11)
                secondMessage = "секунд";
        }
        else if ((seconds % 10) > 1 && (seconds % 10) <= 4)
            secondMessage = "секунды";
        else secondMessage = "секунд";

        _timePassedText.text = $"{days} {dayMessage}, {hours} {hourMessage}, {minutes} {minuteMessage}, {seconds} {secondMessage}";
    }

    private void DisplayMaxAbsenceTime()
    {
        _maxAbsenceTimeText.text = $"Максимальное время оффлайн награды: {ValuesRounding.GetFormattedLongTime(GlobalUpgrades.AbsenceTime)}";
    }

    private void DisplayAbsenceIncome(float income)
    { _absenceIncomeText.text = $"Доход составил: {ValuesRounding.FormattingValue("+", "$", income)}"; }
}