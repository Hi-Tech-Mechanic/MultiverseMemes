using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GlobalUpgrades : DataStructure
{
    public static int GlobalUpgradesCount => Enum.GetNames(typeof(GlobalUpgradesIndex)).Length;
    [SerializeField] private GameObject[] _cells;
    [SerializeField] private AudioClip _openRowEffect;
    [SerializeField] private Animator _animator;
    [SerializeField] private RectTransform _backgroundRect;
    [SerializeField] private Sprite _incomeMultiplierSprite;
    [SerializeField] private Sprite _upgradeCostSprite;
    [SerializeField] private Sprite _tickTimeSprite;
    [SerializeField] private Sprite _critChanceSprite;
    [SerializeField] private Sprite _critStrengthSprite;
    [SerializeField] private Sprite _ADRewardSprite;
    [SerializeField] private Sprite AdvertisingBuffTimeSprite;
    [SerializeField] private Sprite AbsenceTimeSprite;
    [SerializeField] private Sprite MemeClipCountSprite;
    [SerializeField] private TextMeshProUGUI _memeCoinsText;
    [SerializeField] Color _buyActive;
    [SerializeField] Color _buyDisabled;

    [Header("CombinedUniverse only")]
    [SerializeField] private TextMeshProUGUI _idleADMultiplierText;
    [SerializeField] private TextMeshProUGUI _clickADMultiplierText;

    public static Action AdvertisingBuffTimeIsChange;

    private readonly Animator[] _cellAnimators = new Animator[GlobalUpgradesCount];
    private float _cellHeight;
    private bool _disclosureIsOver = true;
    private readonly GlobalUpgradeCell[] _cellsScript = new GlobalUpgradeCell[GlobalUpgradesCount];
    public static bool[,] AchievementsRewardsIsClaimed = new bool[Game.ScenesCount, Game.MaxAchievementsCountInGame];

    public enum GlobalUpgradesIndex
    {
        IncomeMultiplier,
        UpgradeCostMultiplier,
        TickTime,
        CritChance,
        CritStrength,
        ADRewardMultiplier,
        AdvertisingBuffTime,
        AbsenceTime,
        MemeClipCount
    }

    public float IncomeMultiplier
    {
        get => _incomeMultiplier;
        set
        {
            _incomeMultiplier = value;
            if (Game.CurrentScene == 0)
                DisplayValues((int)GlobalUpgradesIndex.IncomeMultiplier);
        }
    }
    private float _incomeMultiplier = 1.00f;
    private const float _additiveIncomeMultiplier = 0.04f;
    private const float _maxIncomeMultiplier = 8.00f; //80 от ежедневника
    private const int _maxUpgradeCount_IncomeMultiplier = 180;
    private const int _additiveCost_IncomeMultiplier = 5;

    public float UpgradeCostMultiplier
    {
        get => _upgradeCostMultiplier;
        set
        {
            _upgradeCostMultiplier = value;
            if (Game.CurrentScene == 0)
                DisplayValues((int)GlobalUpgradesIndex.UpgradeCostMultiplier);
        }
    }
    private float _upgradeCostMultiplier = 1.05f;
    private const float _startUpgradeCostMultiplier = 1.05f;
    private const float _deductibleValueUpgradeCostMultiplier = 0.0005f;
    private const float _minUpgradeCostMultiplier = 1.005f;
    private const int _maxUpgradeCount_UpgradeCostMultiplier = 75;
    private const int _additiveCost_CostMultiplier = 20;

    public float TickTime
    {
        get => _tickTime;
        set
        {
            _tickTime = value;
            if (Game.CurrentScene == 0)
                DisplayValues((int)GlobalUpgradesIndex.TickTime);
        }
    }
    private float _tickTime = 2;
    private const float _deductibleValueFromTickTime = 0.02f;
    private const float _minTickTime = 0.05f;
    private const int _maxUpgradeCount_TickTime = 80;
    private const int _additiveCost_TickTime = 15;

    public float CritChance
    {
        get => _critChance;
        set
        {
            _critChance = value;
            if (Game.CurrentScene == 0)
                DisplayValues((int)GlobalUpgradesIndex.CritChance);
        }
    }
    private float _critChance = 0.01f;
    private const float _additiveCritChance = 0.01f;
    private const float _maxCritChance = 0.70f;
    private const int _maxUpgradeCount_CritChance = 64;
    private const int _additiveCost_CritChance = 20;

    public float CritStrength
    {
        get => _critStrength;
        set
        {
            _critStrength = value;
            if (Game.CurrentScene == 0)
                DisplayValues((int)GlobalUpgradesIndex.CritStrength);
        }
    }
    private float _critStrength = 2.00f;
    private const float _additiveCritStrength = 0.08f;
    private const float _maxCritStrength = 8.00f; //40 от ежедневника
    private const int _maxUpgradeCount_CritStrength = 70;
    private const int _additiveCost_CritStrength = 15;

    public Action OnChangedADRewardMultiplier;
    public float ADRewardMultiplier
    {
        get => _ADRewardMultiplier;
        set
        {
            _ADRewardMultiplier = value;
            if (Game.CurrentScene == 0)
                DisplayValues((int)GlobalUpgradesIndex.ADRewardMultiplier);
        }
    } //max upgrade count = 50
    private float _ADRewardMultiplier = 1.50f;
    private const float _additiveADRewardMultiplier = 0.03f;
    private const float _maxADRewardMultiplier = 3.00f;
    private const int _additiveCost_ADRewardMultiplier = 30;

    public int AdvertisingBuffTime
    {
        get => _advertisingBuffTime;
        set
        {
            _advertisingBuffTime = value;
            if (Game.CurrentScene == 0)
                DisplayValues((int)GlobalUpgradesIndex.AdvertisingBuffTime);
        }
    } //max upgrade count = 40
    private int _advertisingBuffTime = 60;
    private const int _maxAdvertisingBuffTime = 300;
    private const int _additiveAdvertisingBuffTime = 6;
    private const int _additiveCost_AdvertisingBuffTime = 15;

    public int AbsenceTime
    {
        get => _absenceTime;
        set
        {
            _absenceTime = value;
            if (Game.CurrentScene == 0)
                DisplayValues((int)GlobalUpgradesIndex.AbsenceTime);

        }
    }
    private int _absenceTime = 7200;
    private const int _maxAbsenceTime = 86400; //5400 -> 8100 от ежедневника
    private const int _additiveAbsenceTime = 600;
    private const int _maxUpgradeCount_AbsenceTime = 123; //82 -> 79
    private const int _additiveCost_AbsenceTime = 5;

    public int MemeClipCount
    {
        get => _memeClipCount;
        set
        {
            _memeClipCount = value;
            if (Game.CurrentScene == 0)
                DisplayValues((int)GlobalUpgradesIndex.MemeClipCount);

        }
    }
    private int _memeClipCount = 5; //max upgrade count = 25
    private const int _maxMemeClipCount = 30;
    private const int _additiveCost_MemeClipCount = 20;

    public float[] Prices
    {
        get => _prices;
        set
        {
            _prices = value;
            if (Game.CurrentScene == 0)
                DisplayAllPrices();
        }
    }
    private float[] _prices = { 5, 15, 20, 25, 20, 50, 15, 10, 80 };

    public int[] PurchasedCounter
    {
        get => _purchaseCounter;
        set
        {
            _purchaseCounter = value;
            if (Game.CurrentScene == 0)
                DisplayAllPurchasedCount();
        }
    }
    private int[] _purchaseCounter = new int[GlobalUpgradesCount];

    private const int _globalUpgradesCostAdditive = 10;
    private const int _stepToIncreaseCost = 1;

    private void Awake()
    {
        if (Game.CurrentScene == (int)Game.BuildIndex.CombinedUniverse)
        {
            _cellHeight = _cells[0].GetComponent<RectTransform>().sizeDelta.y;

            int iter = 0;
            foreach (GameObject cell in _cells)
            {
                _cellsScript[iter] = cell.GetComponent<GlobalUpgradeCell>();
                _cellAnimators[iter] = cell.GetComponent<Animator>();
                _cellsScript[iter].FindComponents();
                iter++;
            }
        }
    }

    public void Init() //Start
    {
        if (Game.CurrentScene == 0)
        {
            LocalInit();
            //PurchaseAndOutputSummGlobalUpgradePrices();
        }

        void LocalInit()
        {
            foreach (int index in Enum.GetValues(typeof(GlobalUpgradesIndex)))
            {
                DisplayValues(index);
                DisplayPrice(index);
                DisplayPurchasedCount(index);

                switch (index)
                {
                    case (int)GlobalUpgradesIndex.IncomeMultiplier:
                        _cellsScript[index].DisplayName("Мультипликатор дохода");
                        _cellsScript[index].UpgradeButton.onClick.AddListener(IcreaseIncomeMultiplier);
                        _cellsScript[index].DisplayAdditiveValue(ValuesRounding.FormattingValue("Добавляет ", "%", _additiveIncomeMultiplier * 100));
                        _cellsScript[index].SetCellIcon(_incomeMultiplierSprite);
                        if (_purchaseCounter[index] >= _maxUpgradeCount_IncomeMultiplier)
                            SampleHideCellElements(index);
                        break;
                    case (int)GlobalUpgradesIndex.UpgradeCostMultiplier:
                        float percent = _deductibleValueUpgradeCostMultiplier / (_startUpgradeCostMultiplier - 1);
                        _cellsScript[index].DisplayName("Стоимость улучшений");
                        _cellsScript[index].UpgradeButton.onClick.AddListener(DecreaseUpgradeCostMultiplier);
                        _cellsScript[index].DisplayAdditiveValue(ValuesRounding.FormattingValue("Сокращает на ", "%", percent * 100));
                        _cellsScript[index].SetCellIcon(_upgradeCostSprite);
                        if (_purchaseCounter[index] >= _maxUpgradeCount_UpgradeCostMultiplier)
                            SampleHideCellElements(index);
                        break;
                    case (int)GlobalUpgradesIndex.TickTime:
                        _cellsScript[index].DisplayName("Время тика");
                        _cellsScript[index].UpgradeButton.onClick.AddListener(DecreaseTickTime);
                        _cellsScript[index].DisplayAdditiveValue(ValuesRounding.FormattingValue("Сокращает на ", "с.", _deductibleValueFromTickTime));
                        _cellsScript[index].SetCellIcon(_tickTimeSprite);
                        if (_purchaseCounter[index] >= _maxUpgradeCount_TickTime)
                            SampleHideCellElements(index);
                        break;
                    case (int)GlobalUpgradesIndex.CritChance:
                        _cellsScript[index].DisplayName("Шанс критического клика");
                        _cellsScript[index].UpgradeButton.onClick.AddListener(IncreaseMultiplierCritChance);
                        _cellsScript[index].DisplayAdditiveValue(ValuesRounding.FormattingValue("Добавляет ", "%", _additiveCritChance * 100));
                        _cellsScript[index].SetCellIcon(_critChanceSprite);
                        if (_purchaseCounter[index] >= _maxUpgradeCount_CritChance)
                            SampleHideCellElements(index);
                        break;
                    case (int)GlobalUpgradesIndex.CritStrength:
                        _cellsScript[index].DisplayName("Сила критического клика");
                        _cellsScript[index].UpgradeButton.onClick.AddListener(IncreaseMultiplierCritStrength);
                        _cellsScript[index].DisplayAdditiveValue(ValuesRounding.FormattingValue("Добавляет ", "%", _additiveCritStrength * 100));
                        _cellsScript[index].SetCellIcon(_critStrengthSprite);
                        if (_purchaseCounter[index] >= _maxUpgradeCount_CritStrength)
                            SampleHideCellElements(index);
                        break;
                    case (int)GlobalUpgradesIndex.ADRewardMultiplier:
                        _cellsScript[index].DisplayName("Рекламное вознаграждение");
                        _cellsScript[index].UpgradeButton.onClick.AddListener(IncreaseADRewardMultiplier);
                        _cellsScript[index].DisplayAdditiveValue(ValuesRounding.FormattingValue("Добавляет ", "x", _additiveADRewardMultiplier));
                        _cellsScript[index].SetCellIcon(_ADRewardSprite);
                        if (ADRewardMultiplier >= _maxADRewardMultiplier)
                            SampleHideCellElements(index);
                        break;
                    case (int)GlobalUpgradesIndex.AdvertisingBuffTime:
                        _cellsScript[index].DisplayName("Время рекламного усилителя");
                        _cellsScript[index].UpgradeButton.onClick.AddListener(IncreaseAdvertisingBuffTime);
                        _cellsScript[index].DisplayAdditiveValue(ValuesRounding.FormattingValue("Добавляет ", "c.", _additiveAdvertisingBuffTime));
                        _cellsScript[index].SetCellIcon(AdvertisingBuffTimeSprite);
                        if (AdvertisingBuffTime >= _maxAdvertisingBuffTime)
                            SampleHideCellElements(index);
                        break;
                    case (int)GlobalUpgradesIndex.AbsenceTime:
                        _cellsScript[index].DisplayName("Максимальное оффлайн время");
                        _cellsScript[index].UpgradeButton.onClick.AddListener(IncreaseAbsenceTime);
                        _cellsScript[index].DisplayAdditiveValue($"Добавляет {ValuesRounding.GetMinutes(_additiveAbsenceTime)}м.");
                        _cellsScript[index].SetCellIcon(AbsenceTimeSprite);
                        if (_purchaseCounter[index] >= _maxUpgradeCount_AbsenceTime)
                            SampleHideCellElements(index);
                        break;
                    case (int)GlobalUpgradesIndex.MemeClipCount:
                        _cellsScript[index].DisplayName("Размер буфера мемных звуков");
                        _cellsScript[index].UpgradeButton.onClick.AddListener(IncreaseMemeClipCount);
                        _cellsScript[index].DisplayAdditiveValue(ValuesRounding.FormattingValue("Даёт ", " место", 1));
                        _cellsScript[index].SetCellIcon(MemeClipCountSprite);
                        if (MemeClipCount >= _maxMemeClipCount)
                            SampleHideCellElements(index);
                        break;
                }
            }

            DisplayMemeCoins();
            DisplayAllPrices();
            DisplayAllPurchasedCount();
            CheckPurchaseStrengthAllUpgrades();
        }
    }

    #region ButtonEvent
    public void SequentialDisclosureUpgradeElements(string state)
    {
        if (_disclosureIsOver)
        {
            _disclosureIsOver = false;
            StartCoroutine(Coroutine());
        }

        IEnumerator Coroutine()
        {
            if (state == "Open")
            {
                if (_animator.gameObject.activeInHierarchy == true)
                {
                    _backgroundRect.sizeDelta = new Vector2(_backgroundRect.sizeDelta.x, 100);
                    float tmp_y = _backgroundRect.sizeDelta.y;
                    _animator.SetTrigger("Active");
                    yield return new WaitForSeconds(0.3f);

                    for (int i = 0; i < _cells.Length; i++)
                    {
                        _cells[i].SetActive(true);
                        _cellAnimators[i].SetTrigger(state);
                        _backgroundRect.sizeDelta = new Vector2(_backgroundRect.sizeDelta.x, tmp_y + ((i + 1) * _cellHeight));
                        AudioEffects.PlayOneShotEffect(_openRowEffect);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            else if (state == "Close")
            {
                if (gameObject.transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    _backgroundRect.sizeDelta = new Vector2(_backgroundRect.sizeDelta.x, _cellHeight * _cells.Length + 100);
                    float tmp_y = _backgroundRect.sizeDelta.y;

                    for (int i = (_cells.Length - 1); i > 0; i--)
                    {
                        _cellAnimators[i].SetTrigger(state);
                        _backgroundRect.sizeDelta = new Vector2(_backgroundRect.sizeDelta.x,
                            tmp_y - (((_cells.Length - 1) - i) * _cellHeight));
                        AudioEffects.PlayOneShotEffect(_openRowEffect);
                        yield return new WaitForSeconds(0.1f);
                    }

                    _animator.SetTrigger(state);
                }
            }

            _disclosureIsOver = true;
        }
    }
    #endregion

    #region Displaying

    private void DisplayPurchasedCount(int index)
    { _cellsScript[index].DisplayPurchasedCount($"{_purchaseCounter[index]}"); }

    private void DisplayPrice(int index)
    { _cellsScript[index].DisplayPrice(ValuesRounding.FormattingValue("", "", _prices[index])); }

    private void DisplayAllPrices()
    {
        if (_cellsScript[0] == null)
            Awake();
        foreach (int i in Enum.GetValues(typeof(GlobalUpgradesIndex)))
            _cellsScript[i].DisplayPrice(ValuesRounding.FormattingValue("", "", _prices[i]));
    }

    private void DisplayAllPurchasedCount()
    {
        if (_cellsScript[0] == null)
            Awake();
        foreach (int i in Enum.GetValues(typeof(GlobalUpgradesIndex)))
            _cellsScript[i].DisplayPurchasedCount($"{_purchaseCounter[i]}");
    }

    private void DisplayValues(int index)
    {
        if (_cellsScript[0] == null)
            Awake();
        DisplayPurchasedCount(index);
        DisplayPrice(index);

        switch (index)
        {
            case (int)GlobalUpgradesIndex.IncomeMultiplier:
                _cellsScript[index].DisplayCurrentValue(ValuesRounding.FormattingValue("", "%", (_incomeMultiplier - 1) * 100));
                _cellsScript[index].DisplayTargetValue(ValuesRounding.FormattingValue("", "%", (_incomeMultiplier + _additiveIncomeMultiplier - 1) * 100));
                break;
            case (int)GlobalUpgradesIndex.UpgradeCostMultiplier:
                float currentValue = (_upgradeCostMultiplier - 1) / (_startUpgradeCostMultiplier - 1);
                float targetValue = ((_upgradeCostMultiplier - 1) - _deductibleValueUpgradeCostMultiplier) / (_startUpgradeCostMultiplier - 1);
                _cellsScript[index].DisplayCurrentValue(ValuesRounding.FormattingValue("", "%", currentValue * 100));
                _cellsScript[index].DisplayTargetValue(ValuesRounding.FormattingValue("", "%", targetValue * 100));
                break;
            case (int)GlobalUpgradesIndex.TickTime:
                _cellsScript[index].DisplayCurrentValue(ValuesRounding.FormattingValue("", "с.", _tickTime));
                _cellsScript[index].DisplayTargetValue(ValuesRounding.FormattingValue("", "с.", _tickTime - _deductibleValueFromTickTime));
                break;
            case (int)GlobalUpgradesIndex.CritChance:
                _cellsScript[index].DisplayCurrentValue(ValuesRounding.FormattingValue("", "%", _critChance * 100));
                _cellsScript[index].DisplayTargetValue(ValuesRounding.FormattingValue("", "%", (_critChance + _additiveCritChance) * 100));
                break;
            case (int)GlobalUpgradesIndex.CritStrength:
                _cellsScript[index].DisplayCurrentValue(ValuesRounding.FormattingValue("", "%", _critStrength * 100));
                _cellsScript[index].DisplayTargetValue(ValuesRounding.FormattingValue("", "%", (_critStrength + _additiveCritStrength) * 100));
                break;
            case (int)GlobalUpgradesIndex.ADRewardMultiplier:
                _cellsScript[index].DisplayCurrentValue(ValuesRounding.FormattingValue("", "x", _ADRewardMultiplier));
                _cellsScript[index].DisplayTargetValue(ValuesRounding.FormattingValue("", "x", _ADRewardMultiplier + _additiveADRewardMultiplier));
                _idleADMultiplierText.text = $"{_ADRewardMultiplier}x";
                _clickADMultiplierText.text = $"{_ADRewardMultiplier}x";
                break;
            case (int)GlobalUpgradesIndex.AdvertisingBuffTime:
                _cellsScript[index].DisplayCurrentValue(ValuesRounding.FormattingValue("", "c.", _advertisingBuffTime));
                _cellsScript[index].DisplayTargetValue(ValuesRounding.FormattingValue("", "c.", _advertisingBuffTime + _additiveAdvertisingBuffTime));
                break;
            case (int)GlobalUpgradesIndex.AbsenceTime:
                _cellsScript[index].DisplayCurrentValue($"{ValuesRounding.GetMinutes(_absenceTime)}м.");
                _cellsScript[index].DisplayTargetValue($"{ValuesRounding.GetMinutes(_absenceTime + _additiveAbsenceTime)}м.");
                break;
            case (int)GlobalUpgradesIndex.MemeClipCount:
                _cellsScript[index].DisplayCurrentValue(ValuesRounding.FormattingValue("", "шт.", _memeClipCount));
                _cellsScript[index].DisplayTargetValue(ValuesRounding.FormattingValue("", "шт.", _memeClipCount + 1));
                break;
        }
    }
    #endregion

    public void IcreaseIncomeMultiplier(float additiveValue)
    {
        IncomeMultiplier += additiveValue;
        IncomeMultiplier = (float)Math.Round(IncomeMultiplier, 2);

        float[] click = new float[Game.ScenesCount];
        float[] tick = new float[Game.ScenesCount];
        float divider = _incomeMultiplier == 100 ? 1 : _incomeMultiplier - additiveValue;

        for (int j = 0; j < Game.ScenesCount; j++)
        {
            click[j] = ((MoneyMenu.GetClickIncome()[j] / divider) * _incomeMultiplier);
            tick[j] = ((MoneyMenu.GetIdleIncomePerTick()[j] / divider) * _incomeMultiplier);

            for (int k = 0; k < Upgrades.ClickCellsCount; k++)
            {
                Upgrades._Click.Income[j, k] = (Upgrades._Click.Income[j, k] / divider) * _incomeMultiplier;
                Upgrades._Click.CachedIncome[j, k] = (Upgrades._Click.CachedIncome[j, k] / divider) * _incomeMultiplier;
                Upgrades._Click.SummIncome[j, k] = (Upgrades._Click.SummIncome[j, k] / divider) * _incomeMultiplier;
            }

            for (int k = 0; k < Upgrades.IdleCellsCount; k++)
            {
                Upgrades._Idle.Income[j, k] = (Upgrades._Idle.Income[j, k] / divider) * _incomeMultiplier;
                Upgrades._Idle.CachedIncome[j, k] = (Upgrades._Idle.CachedIncome[j, k] / divider) * _incomeMultiplier;
                Upgrades._Idle.SummIncome[j, k] = (Upgrades._Idle.SummIncome[j, k] / divider) * _incomeMultiplier;
            }
        }
        MoneyMenu.SetClickIncome(click);
        MoneyMenu.SetTickIncome(tick);

        for (int j = 0; j < Upgrades.ClickCellsCount; j++)
            Upgrades._Click.DisplayCellAdditivesIncome(j);
        for (int j = 0; j < Upgrades.IdleCellsCount; j++)
            Upgrades._Idle.DisplayCellAdditivesIncome(j);
    }

    private void IcreaseIncomeMultiplier()
    {
        int i = (int)GlobalUpgradesIndex.IncomeMultiplier;

        if (CheckAmountOfMemeCoins(i))
        {
            SampleGlobalUpgrade(i);
            IcreaseIncomeMultiplier(_additiveIncomeMultiplier);

            if (_purchaseCounter[i] >= _maxUpgradeCount_IncomeMultiplier)
            {
                SampleHideCellElements(i);
            }
        }
    }

    public void DecreaseUpgradeCostMultiplier(float deductibleValue)
    {
        UpgradeCostMultiplier -= deductibleValue;
        UpgradeCostMultiplier = (float)Math.Round(UpgradeCostMultiplier, 4);

        float divider = UpgradeCostMultiplier == (_startUpgradeCostMultiplier - deductibleValue) ? 1 :
            (_upgradeCostMultiplier + deductibleValue - 1) / (_startUpgradeCostMultiplier - 1);
        float discount = (_upgradeCostMultiplier - 1) / (_startUpgradeCostMultiplier - 1);

        for (int j = 0; j < Game.ScenesCount; j++)
        {
            int clickCellCount = Upgrades._Click.Prices.GetUpperBound(0) + 1;
            int idleCellCount = Upgrades._Idle.Prices.GetUpperBound(0) + 1;

            for (int k = 0; k < clickCellCount; k++)
            {
                Upgrades._Click.Prices[j, k] = ((Upgrades._Click.Prices[j, k] / divider) * discount);
                Upgrades._Click.CachedPrice[j, k] = ((Upgrades._Click.CachedPrice[j, k] / divider) * discount);
            }
            for (int k = 0; k < idleCellCount; k++)
            {
                Upgrades._Idle.Prices[j, k] = ((Upgrades._Idle.Prices[j, k] / divider) * discount);
                Upgrades._Idle.CachedPrice[j, k] = ((Upgrades._Idle.CachedPrice[j, k] / divider) * discount);
            }
        }

        for (int j = 0; j < Upgrades.ClickCellsCount; j++)
            Upgrades._Click.DisplayUpgradePrice(j);
        for (int j = 0; j < Upgrades.IdleCellsCount; j++)
            Upgrades._Idle.DisplayUpgradePrice(j);
    }

    private void DecreaseUpgradeCostMultiplier()
    {
        int i = (int)GlobalUpgradesIndex.UpgradeCostMultiplier;

        if (CheckAmountOfMemeCoins(i))
        {
            SampleGlobalUpgrade(i);
            DecreaseUpgradeCostMultiplier(_deductibleValueUpgradeCostMultiplier);

            if (_purchaseCounter[i] >= _maxUpgradeCount_UpgradeCostMultiplier)
            {
                SampleHideCellElements(i);
            }
        }
    }

    private void IncreaseMultiplierCritChance()
    {
        int i = (int)GlobalUpgradesIndex.CritChance;

        if (CheckAmountOfMemeCoins(i))
        {
            SampleGlobalUpgrade(i);
            CritChance += _additiveCritChance;
            CritChance = (float)Math.Round(CritChance, 2);

            if (_purchaseCounter[i] >= _maxUpgradeCount_CritChance)
            {
                SampleHideCellElements(i);
            }
        }
    }

    private void IncreaseMultiplierCritStrength()
    {
        int i = (int)GlobalUpgradesIndex.CritStrength;

        if (CheckAmountOfMemeCoins(i))
        {
            SampleGlobalUpgrade(i);
            CritStrength += _additiveCritStrength;
            CritStrength = (float)Math.Round(CritStrength, 2);

            if (_purchaseCounter[i] >= _maxUpgradeCount_CritStrength)
            {
                SampleHideCellElements(i);
            }
        }
    }

    private void DecreaseTickTime()
    {
        int i = (int)GlobalUpgradesIndex.TickTime;

        if (CheckAmountOfMemeCoins(i))
        {
            SampleGlobalUpgrade(i);
            TickTime -= _deductibleValueFromTickTime;
            TickTime = (float)Math.Round(TickTime, 2);

            if (_purchaseCounter[i] >= _maxUpgradeCount_TickTime)
            {
                SampleHideCellElements(i);
            }
            MoneyMenu.DisplayAllIdleIncome();
        }
    }

    private void IncreaseADRewardMultiplier()
    {
        int i = (int)GlobalUpgradesIndex.ADRewardMultiplier;

        if (CheckAmountOfMemeCoins(i))
        {
            SampleGlobalUpgrade(i);
            ADRewardMultiplier += _additiveADRewardMultiplier;
            ADRewardMultiplier = (float)Math.Round(ADRewardMultiplier, 2);

            if (ADRewardMultiplier >= _maxADRewardMultiplier)
            {
                ADRewardMultiplier = _maxADRewardMultiplier;
                SampleHideCellElements(i);
            }

            OnChangedADRewardMultiplier?.Invoke();
        }
    }

    private void IncreaseAbsenceTime()
    {
        int i = (int)GlobalUpgradesIndex.AbsenceTime;

        if (CheckAmountOfMemeCoins(i))
        {
            SampleGlobalUpgrade(i);
            AbsenceTime += _additiveAbsenceTime;

            if (_purchaseCounter[i] >= _maxUpgradeCount_AbsenceTime)
            {
                SampleHideCellElements(i);
            }
        }
    }

    private void IncreaseAdvertisingBuffTime()
    {
        int i = (int)GlobalUpgradesIndex.AdvertisingBuffTime;

        if (CheckAmountOfMemeCoins(i))
        {
            SampleGlobalUpgrade(i);
            AdvertisingBuffTime += _additiveAdvertisingBuffTime;
            AdvertisingBuffTimeIsChange?.Invoke();

            if (AdvertisingBuffTime >= _maxAdvertisingBuffTime)
            {
                AdvertisingBuffTime = _maxAdvertisingBuffTime;
                SampleHideCellElements(i);
            }
        }
    }

    private void IncreaseMemeClipCount()
    {
        int i = (int)GlobalUpgradesIndex.MemeClipCount;

        if (CheckAmountOfMemeCoins(i))
        {
            SampleGlobalUpgrade(i);
            MemeClipCount ++;

            if (MemeClipCount >= _maxMemeClipCount)
            {
                MemeClipCount = _maxMemeClipCount;
                SampleHideCellElements(i);
            }
        }
    }

    private bool CheckAmountOfMemeCoins(int i)
    {
        if (MoneyMenu.GetMemeCoins() >= Prices[i])
            return true;
        else return false;
    }

    private void SampleGlobalUpgrade(int i)
    {
        float additivePrice = 0;

        AudioEffects.PlayUpgradeEffect_1();
        _purchaseCounter[i]++;
        MoneyMenu.SpendMemeCoins(Prices[i]);
        Instantiate(FloatPrefabs.Outlier, _memeCoinsText.transform).GetComponent<Outlier>().DesignateOutlier("Up", "-", "", Prices[i], Color.red);
        DisplayMemeCoins();

        switch(i)
        {
            case (int)GlobalUpgradesIndex.IncomeMultiplier:
                additivePrice = _additiveCost_IncomeMultiplier;
                break;
            case (int)GlobalUpgradesIndex.UpgradeCostMultiplier:
                additivePrice = _additiveCost_CostMultiplier;
                break;
            case (int)GlobalUpgradesIndex.TickTime:
                additivePrice = _additiveCost_TickTime;
                break;
            case (int)GlobalUpgradesIndex.CritChance:
                additivePrice = _additiveCost_CritChance;
                break;
            case (int)GlobalUpgradesIndex.CritStrength:
                additivePrice = _additiveCost_CritStrength;
                break;
            case (int)GlobalUpgradesIndex.ADRewardMultiplier:
                additivePrice = _additiveCost_ADRewardMultiplier;
                break;
            case (int)GlobalUpgradesIndex.AdvertisingBuffTime:
                additivePrice = _additiveCost_AdvertisingBuffTime;
                break;
            case (int)GlobalUpgradesIndex.AbsenceTime:
                additivePrice = _additiveCost_AbsenceTime;
                break;
            case (int)GlobalUpgradesIndex.MemeClipCount:
                additivePrice = _additiveCost_MemeClipCount;
                break;
        }

        if (_purchaseCounter[i] % _stepToIncreaseCost == 0)
            Prices[i] += additivePrice;
        //Debug.Log("MemeCoins = " + MoneyMenu.MemeCoins);

        CheckPurchaseStrengthAllUpgrades();
    }

    public void CheckPurchaseStrengthAllUpgrades()
    {
        foreach (int i in Enum.GetValues(typeof(GlobalUpgradesIndex)))
        {
            if (CheckAmountOfMemeCoins(i))
                _cellsScript[i].UpgradeButton.image.color = _buyActive;
            else _cellsScript[i].UpgradeButton.image.color = _buyDisabled;
        }
    }

    private void SampleHideCellElements(int index)
    {
        TextMeshProUGUI tmpText = _cellsScript[index].GetCurrentValueTMP();

        _cellsScript[index].MAXMessageText.SetActive(true);
        _cellsScript[index].ArrowIcon.SetActive(false);
        _cellsScript[index].UpgradeButton.gameObject.SetActive(false);
        _cellsScript[index].GetTargetValueObj().SetActive(false);
        _cellsScript[index].GetAdditiveValueObj().SetActive(false);

        tmpText.alignment = TextAlignmentOptions.Left;
        tmpText.rectTransform.anchorMin = new Vector2(0, 0);
        tmpText.rectTransform.sizeDelta = new Vector2(0, -60);
        tmpText.rectTransform.anchoredPosition = new Vector2(120, 10);
    }

    public void DisplayMemeCoins()
    { _memeCoinsText.text = ValuesRounding.FormattingValue("", "", MoneyMenu.GetMemeCoins()); }

    private void PurchaseAndOutputSummGlobalUpgradePrices()
    {
        float[] summ = new float[9];

        for (int i = 0; i < _maxUpgradeCount_IncomeMultiplier; i++)
        {
            summ[0] += Prices[0];
            IcreaseIncomeMultiplier();
        }
        for (int i = 0; i < _maxUpgradeCount_UpgradeCostMultiplier; i++)
        {
            summ[1] += Prices[1];
            DecreaseUpgradeCostMultiplier();
        }
        for (int i = 0; i < _maxUpgradeCount_TickTime; i++)
        {
            summ[2] += Prices[2];
            DecreaseTickTime();
        }
        for (int i = 0; i < _maxUpgradeCount_CritChance; i++)
        {
            summ[3] += Prices[3];
            IncreaseMultiplierCritChance();
        }
        for (int i = 0; i < _maxUpgradeCount_CritStrength; i++)
        {
            summ[4] += Prices[4];
            IncreaseMultiplierCritStrength();
        }
        for (int i = 0; i < 50; i++)
        {
            summ[5] += Prices[5];
            IncreaseADRewardMultiplier();
        }
        for (int i = 0; i < 40; i++)
        {
            summ[6] += Prices[6];
            IncreaseAdvertisingBuffTime();
        }
        for (int i = 0; i < _maxUpgradeCount_AbsenceTime; i++)
        {
            summ[7] += Prices[7];
            IncreaseAbsenceTime();
        }
        for (int i = 0; i < 25; i++)
        {
            summ[8] += Prices[8];
            IncreaseMemeClipCount();
        }

        Debug.Log
            ($"IncomeMultiplier = {summ[0]}\n" +
            $"CostMultiplier = {summ[1]}\n" +
            $"TickTime = {summ[2]}\n" +
            $"CritChance = {summ[3]}\n" +
            $"CritStrength = {summ[4]}\n" +
            $"ADRewardMultiplier = {summ[5]}\n" +
            $"AdvertisingBuffTime = {summ[6]}\n" +
            $"AbsenceTime = {summ[7]}\n" +
            $"MemeClipCount = {summ[8]}\n" +
            $"ALL SUMM = {summ[0] + summ[1] + summ[2] + summ[3] + summ[4] + summ[5] + summ[6] + summ[7] + summ[8]}");
    }
}