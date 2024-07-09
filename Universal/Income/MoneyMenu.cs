using System;
using System.Collections;
using TMPro;
using UnityEngine;
using YG;
using Random = UnityEngine.Random;

public class MoneyMenu : DataStructure
{
    [SerializeField] private float _memeCoins = 50;
    [SerializeField] private float[] _moneyCapital = new float[Game.ScenesCount];
    [SerializeField] private float[] _clickIncome = new float[Game.ScenesCount];
    [SerializeField] private float[] _tickIncome = new float[Game.ScenesCount];

    private float _secondIncome;
    private float _minuteIncome;
    private float _hourIncome;

    [NonSerialized] public float[] AccumulatedMoney = new float [Game.ScenesCount];
    [NonSerialized] public float AccumulatedMemeCoins;
    [NonSerialized] public const float StartClickIncome = 0.25f;

    [Header("¬˚‚Ó‰ ÒÂ‰ÒÚ‚")]
    [SerializeField] private GameObject _absenceWindow;
    [SerializeField] private TextMeshProUGUI _textMemeCoins;
    [SerializeField] private TextMeshProUGUI _textMemeCoinsForMemeShop;
    [SerializeField] private TextMeshProUGUI _textMemeCoinsForCaseMenu;
    [Tooltip("CombinedUniverse only")] [SerializeField] private TextMeshProUGUI _textMemeCoinsForCalendarMenu;

    [SerializeField] private TextMeshProUGUI _moneyCapitalText;
    [SerializeField] private TextMeshProUGUI _clickIncomeText;
    [SerializeField] private TextMeshProUGUI _idleIncomePerTickText;
    [SerializeField] private TextMeshProUGUI _idleIncomePerSecondText;
    [SerializeField] private TextMeshProUGUI _idleIncomePerMinuteText;
    [SerializeField] private TextMeshProUGUI _idleIncomePerHourText;

    [NonSerialized] internal bool IdleIncomeIsActive = false;

    private IEnumerator _idleIncomeCoroutine;
    private IEnumerator _coroutineSaveMemeCoins;
    private float _memeCoinsSaveTime;

    public void Init() //Start
    {
        DisplayMemeCoins();

        if (Game.CurrentScene != 1)
        {
            CheckIdleIncome();
            DisplayMoneyCapital();
            DisplayClickIncome();
            DisplayAllIdleIncome();

            if (IdleIncomeIsActive)
            {
                Instantiate(_absenceWindow, Game.WorldCanvasTranform);
            }
        }
    }

    #region StandardOperations
    public void IncreaseMoneyCapital(float income)
    {
        _moneyCapital[Game.CurrentScene] += income;
        if (_moneyCapitalText != null)
            DisplayMoneyCapital();
        Upgrades.CheckPurchasingPower();
        AccumulatedMoney[Game.CurrentScene] += income;
        if (Upgrades._maxBuyAmountFactor == true) Upgrades.CalculateSomeUpgrades(0);
        Instantiate(FloatPrefabs.Outlier, _moneyCapitalText.transform).GetComponent<Outlier>().DesignateOutlier("Down", "+", "$", income, Color.green);
    }

    public void DecreaseMoneyCapital(float cost)
    {
        _moneyCapital[Game.CurrentScene] -= cost;
        if (_moneyCapitalText != null)
            DisplayMoneyCapital();
        Upgrades.CheckPurchasingPower();
        Instantiate(FloatPrefabs.Outlier, _moneyCapitalText.transform).GetComponent<Outlier>().DesignateOutlier("Down", "-", "$", cost, Color.red);
    }

    public void Increase—lickIncome(float value)
    {
        _clickIncome[Game.CurrentScene] += value;
        if (_clickIncomeText != null)
            DisplayClickIncome();
        Instantiate(FloatPrefabs.Outlier, _clickIncomeText.transform).GetComponent<Outlier>().DesignateOutlier("Down", "+", "$",value, Color.green);
    }

    public void IncreaseTickIncome(float value)
    {
        _tickIncome[Game.CurrentScene] += value;
        DisplayAllIdleIncome();

        if (!IdleIncomeIsActive)
            CheckIdleIncome();

        float secondIncome = value / GlobalUpgrades.TickTime;
        Instantiate(FloatPrefabs.Outlier, _idleIncomePerTickText.transform).GetComponent<Outlier>().DesignateOutlier("Down", "+", "$", value, Color.green);
        Instantiate(FloatPrefabs.Outlier, _idleIncomePerSecondText.transform).GetComponent<Outlier>().DesignateOutlier("Down", "+", "$", secondIncome, Color.green);
        Instantiate(FloatPrefabs.Outlier, _idleIncomePerMinuteText.transform).GetComponent<Outlier>().DesignateOutlier("Down", "+", "$", secondIncome * 60, Color.green);
        Instantiate(FloatPrefabs.Outlier, _idleIncomePerHourText.transform).GetComponent<Outlier>().DesignateOutlier("Down", "+", "$", secondIncome * 3600, Color.green);
    }
    #endregion

    #region DisplayMethods
    public void DisplayAllIdleIncome()
    {
        CalcIdleIncome();
        if (_idleIncomePerTickText != null)
            DisplayIdleIncomePerTick();
        if (_idleIncomePerSecondText != null)
            DisplayIdleIncomePerSecond();
        if (_idleIncomePerMinuteText != null)
            DisplayIdleIncomePerMinute();
        if (_idleIncomePerHourText != null)
            DisplayIdleIncomePerHour();
    }

    private void CalcIdleIncome()
    {
        PerSecond();
        PerMinute();
        PerHour();

        void PerSecond() { _secondIncome = (1 / GlobalUpgrades.TickTime) * _tickIncome[Game.CurrentScene]; }
        void PerMinute() { _minuteIncome = _secondIncome * 60; }
        void PerHour() { _hourIncome = _minuteIncome * 60; }
    }

    private void DisplayIdleIncomePerTick()
    { _idleIncomePerTickText.text = ValuesRounding.FormattingValue("", "$/ÚËÍ", _tickIncome[Game.CurrentScene]); }

    private void DisplayIdleIncomePerSecond()
    {  _idleIncomePerSecondText.text = ValuesRounding.FormattingValue("", "$/ÒÂÍ", _secondIncome); }

    private void DisplayIdleIncomePerMinute()
    { _idleIncomePerMinuteText.text = ValuesRounding.FormattingValue("", "$/ÏËÌ", _minuteIncome); }

    private void DisplayIdleIncomePerHour()
    { _idleIncomePerHourText.text = ValuesRounding.FormattingValue("", "$/˜‡Ò", _hourIncome);}

    private void DisplayMemeCoins()
    {
        _textMemeCoins.text = $"{ValuesRounding.FormattingValue("", "", _memeCoins)}";

        if(_textMemeCoinsForCalendarMenu != null)
            _textMemeCoinsForCalendarMenu.text = $"{ValuesRounding.FormattingValue("", "", _memeCoins)}";
        if(_textMemeCoinsForMemeShop != null)
            _textMemeCoinsForMemeShop.text = $"{ValuesRounding.FormattingValue("", "", _memeCoins)}";
        if (_textMemeCoinsForCaseMenu != null)
            _textMemeCoinsForCaseMenu.text = $"{ValuesRounding.FormattingValue("", "", _memeCoins)}";
    }

    private void DisplayMoneyCapital()
    { _moneyCapitalText.text = ValuesRounding.FormattingValue("", "$", _moneyCapital[Game.CurrentScene]); }

    private void DisplayClickIncome()
    { _clickIncomeText.text = ValuesRounding.FormattingValue("", "$", _clickIncome[Game.CurrentScene]); }
    #endregion

    #region Getters/Setters
    public float GetMemeCoins()
    { return _memeCoins; }

    public void SetMemeCoins(float value)
    {
        _memeCoins = value;
        DisplayMemeCoins();
    }

    public void SpendMemeCoins(float value)
    {
        _memeCoins -= value;
        DisplayMemeCoins();

        if (_coroutineSaveMemeCoins != null)
            StopCoroutine(_coroutineSaveMemeCoins);
        _coroutineSaveMemeCoins = SaveMemeCoinsCoroutine();
        StartCoroutine(_coroutineSaveMemeCoins);
    }

    public void AddMemeCoins(float value)
    {
        _memeCoins += value;
        AccumulatedMemeCoins += value;
        DisplayMemeCoins();

        if (Game.CurrentScene == (int)Game.BuildIndex.CombinedUniverse)
        {
            GlobalUpgrades.CheckPurchaseStrengthAllUpgrades();
            GlobalUpgrades.DisplayMemeCoins();
        }

        if (_coroutineSaveMemeCoins != null)
            StopCoroutine(_coroutineSaveMemeCoins);
        _coroutineSaveMemeCoins = SaveMemeCoinsCoroutine();
        StartCoroutine(_coroutineSaveMemeCoins);
    }

    public float[] GetMoneyCapital()
    { return _moneyCapital; }

    public float[] GetIdleIncomePerTick()
    { return _tickIncome; }

    public float GetIdleIncomePerSecond()
    { return _secondIncome; }

    public float[] GetClickIncome()
    { return _clickIncome; }

    public bool —heckTheAmountOfMoney(float cost)
    {
        if (cost > _moneyCapital[Game.CurrentScene])
            return false;
        else return true;
    }

    public void SetMoneyCapital(float[] value)
    { 
        _moneyCapital = value;
        if (_moneyCapitalText != null)
            DisplayMoneyCapital();
    }

    public void SetTickIncome(float[] value)
    { 
        _tickIncome = value;
        DisplayAllIdleIncome();
    }

    public void SetClickIncome(float[] value)
    { 
        _clickIncome = value;
        if (_clickIncomeText != null)
            DisplayClickIncome();
    }

    public void SetTargetClickIncome(float value)
    {
        _clickIncome[Game.CurrentScene] = value;
        if (_clickIncomeText != null)
            DisplayClickIncome();
    }

    public void SetTargetTickIncome(float value)
    {
        _tickIncome[Game.CurrentScene] = value;
        DisplayAllIdleIncome();
    }
    #endregion

    private void CheckIdleIncome()
    {
        if (GetIdleIncomePerTick()[Game.CurrentScene] > 0)
        {
            IdleIncomeIsActive = true;

            if (_idleIncomeCoroutine != null)
            {
                StopCoroutine(_idleIncomeCoroutine);
            }

            _idleIncomeCoroutine = IdleIncome();
            StartCoroutine(_idleIncomeCoroutine);
        }
            
        IEnumerator IdleIncome()
        {
            while (IdleIncomeIsActive)
            {
                float income = CheckHitType(_tickIncome[Game.CurrentScene]);

                if (AudioEffects.IdleAudioEffectsIsOn)
                {
                    AudioEffects.PlayMemeClip();
                }

                IncreaseMoneyCapital(income);
                yield return new WaitForSeconds(GlobalUpgrades.TickTime);
            }
        }
    }

    public void OnClicked()
    {
        float income = CheckHitType(_clickIncome[Game.CurrentScene]);

        AudioEffects.PlayMemeClip();
        Game.AccumulateClicks();
        IncreaseMoneyCapital(income);
    }

    private float CheckHitType(float income)
    {
        float randomNumber = (Random.Range(0, 100) / 100.0f);

        if (randomNumber <= GlobalUpgrades.CritChance)
        {
            income *= GlobalUpgrades.CritStrength;
            DisplayCritFloatPrefab(income);
        }
        else DisplayStandardFloatPrefab(income);

        return income;
    }

    private void DisplayStandardFloatPrefab(float income)
    {
        FloatPrefabs.clickTextPool[FloatPrefabs.StandardTextNumber].StartMotion(income);
        FloatPrefabs.StandardTextNumber = FloatPrefabs.StandardTextNumber == FloatPrefabs.clickTextPool.Length - 1 ? 0 : FloatPrefabs.StandardTextNumber + 1;
    }

    private void DisplayCritFloatPrefab(float income)
    {
        FloatPrefabs.CritClickTextPool[FloatPrefabs.CritTextNumber].StartMotionCrit(income);
        FloatPrefabs.CritTextNumber = FloatPrefabs.CritTextNumber == FloatPrefabs.CritClickTextPool.Length - 1 ? 0 : FloatPrefabs.CritTextNumber + 1;
    }

    IEnumerator SaveMemeCoinsCoroutine()
    {
        _memeCoinsSaveTime = 1;

        while (_memeCoinsSaveTime > 0)
        {
            _memeCoinsSaveTime -= Time.unscaledDeltaTime;

            if (_memeCoinsSaveTime <= 0)
            {
                YandexGame.savesData.MemeCoins = _memeCoins;
                YandexGame.SaveProgress();
            }

            yield return null;
        }
    }
}