using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Skills : DataStructure
{
    #region SerializeField
    public GameObject[] MarketSkills;
    public GameObject[] PlasesForSkills;
    public GameObject[] SelectedSkills;
    public GameObject[] BattleMenuSkills;
    public GameObject[] ShowCircles;
    public AudioClip[] SkillNamesEffects;
    public AudioClip[] LightningEffectsBuffer;
    public AudioClip PerunIntroduction;
    #endregion

    #region NonSerialized
    public static int SkillsCount => Enum.GetValues(typeof(SkillEnum)).Length;
    [NonSerialized] public const int PlasesForSkillsCount = 4;

    [NonSerialized] public int ReturnedPlaseIndex;
    [NonSerialized] private int _cachedSkillIndex;
    [NonSerialized] public int[] SelectedSkillIndex = { 0, -1, -1, -1 };
    [NonSerialized] public bool[] PlaseForSkillIsBought = { true, false, false, false };
    [NonSerialized] public bool[] SkillIsBought = new bool[SkillsCount];
    [NonSerialized] public bool[] SkillIsSelected = new bool[SkillsCount];
    [NonSerialized] public bool[] SkillIsChoosing = new bool[SkillsCount];
    [NonSerialized] public bool DeleteChoose = false;
    [NonSerialized] public int[] PlasesForSkillsCost = { 0, 100, 1000, 5000 };
    [NonSerialized] public float[] SavedReloadTimeForSelectedSkill = new float[PlasesForSkillsCount];
    private const int _maxUpgradeCount = 25;
    private const float _maxDecreaseReloadMultiplier = 0.5f;

    [Header("Магазин способностей")]
    [NonSerialized] public Button[] MarketSkillButton = new Button[SkillsCount];
    [NonSerialized] public TextMeshProUGUI[] MarketSkillNameText = new TextMeshProUGUI[SkillsCount];
    [NonSerialized] private readonly TextMeshProUGUI[] _marketSkillCostText = new TextMeshProUGUI[SkillsCount];
    [NonSerialized] private readonly TextMeshProUGUI[] _marketSkillWillCostText = new TextMeshProUGUI[SkillsCount];

    [Header ("Меню выбранных способностей")]
    [NonSerialized] public TextMeshProUGUI[] SelectedSkillWillCostText = new TextMeshProUGUI[PlasesForSkillsCount];
    [NonSerialized] public TextMeshProUGUI[] BattleMenuSkillWillCostText = new TextMeshProUGUI[PlasesForSkillsCount];
    [NonSerialized] public TextMeshProUGUI[] SelectedSkillNameText = new TextMeshProUGUI[PlasesForSkillsCount];
    [NonSerialized] public TextMeshProUGUI[] BattleMenuSkillNameText = new TextMeshProUGUI[PlasesForSkillsCount];
    [NonSerialized] public TextMeshProUGUI[] PlaceCostText = new TextMeshProUGUI[PlasesForSkillsCount];
    [NonSerialized] public TextMeshProUGUI[] ReloadValueText = new TextMeshProUGUI[PlasesForSkillsCount];
    [NonSerialized] public Button[] SelectedSkillButton = new Button[PlasesForSkillsCount];
    [NonSerialized] public Button[] BuyPlaceButton = new Button[PlasesForSkillsCount];
    [NonSerialized] public Image[] Lock = new Image[PlasesForSkillsCount];
    [NonSerialized] public Image[] ReloadSkillImage = new Image[PlasesForSkillsCount];
    [NonSerialized] public Text[] PlaceMessage = new Text[PlasesForSkillsCount];
    private Color _green = new(0, 1, 0, 0.65f);
    private Color _red = new(1, 0, 0, 0.65f);

    public enum SkillEnum
    {
        BallClamp = 0,
        ShootingGlance = 1,
        BlackSauna = 2,
        ArmorBreak = 3,
        ZeusAnger = 4,
        HelpPerun = 5,
        PrayerToTheGods = 6,
        GeneralFee = 7
    }

    private enum TypeOfValue
    {
        Percent,
        Seconds,
        UnitsInSeconds,
        Void
    }

    #endregion

    #region SkillsClasses

    internal class SkillTemplate : DataStructure
    {
        protected GameObject[] Stats;
        protected TextMeshProUGUI[] CurrentValue;
        protected TextMeshProUGUI[] FutureValue;
        protected TextMeshProUGUI[] UpgradeCost;
        protected TextMeshProUGUI[] MaxMessage;
        protected Button[] UpgradeButton;
        protected float _deductibleReloadTime;
        protected int _additiveWillCost;
        protected int _summMaxUpgradeCount;
        public int SummCurrentPurchaseCount;
        public float CurrentPurchasePercent = 0.2f;
        public float StartReloadTime;
        public int StartWillCost;
        public int[] PurchaseCount;
        public int[] Cost;
        public int[] CostAdditive;

        public float ReloadTime
        {
            get { return _reloadTime; }
            set { _reloadTime = value; }
        }
        private float _reloadTime;

        public float UnlockCost
        {
            get { return _unlockCost; }
            set { _unlockCost = value; }
        }
        private float _unlockCost;

        public int WillCost
        {
            get { return _willCost; }
            set { _willCost = value; }
        }
        private int _willCost;

        public virtual void Start()
        {
            DisplayAllInfo();

            for (int i = 0; i < Stats.Length; i++)
            {
                if (PurchaseCount[i] == _maxUpgradeCount)
                    MaxLevelReceived(i);
            }
        }

        public virtual void Init(GameObject[] stats)
        {
            Stats = stats;
            CurrentValue = new TextMeshProUGUI[Stats.Length];
            FutureValue = new TextMeshProUGUI[Stats.Length];
            UpgradeCost = new TextMeshProUGUI[Stats.Length];
            MaxMessage = new TextMeshProUGUI[Stats.Length];
            UpgradeButton = new Button[Stats.Length];

            FindComponentsInInfoWindow();
        }

        public virtual void ResetStats() 
        {
            SummCurrentPurchaseCount = 0;
            CurrentPurchasePercent = 0.2f;
        }

        protected void FindComponentsInInfoWindow()
        {
            for (int i = 0; i < Stats.Length; i++)
            {
                Component[] array = Stats[i].GetComponentsInChildren<Component>(includeInactive: true);

                foreach (Component targer in array)
                {
                    switch (targer.name)
                    {
                        case ("CurrentValue"):
                            CurrentValue[i] = targer.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("FutureValue"):
                            FutureValue[i] = targer.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("UpgradeCost"):
                            UpgradeCost[i] = targer.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("MaxMessage"):
                            MaxMessage[i] = targer.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("UpgradeButton"):
                            UpgradeButton[i] = targer.GetComponent<Button>();
                            break;
                    }
                }
            }
        }

        protected void DisplayStatInfo(int index, float curr_value, float future_value, float cost, int type_of_value)
        {
            int roundNumber = 2;

            switch (type_of_value)
            {
                case (int)TypeOfValue.Seconds:
                    roundNumber = 1;
                    DisplayCurrentValue(" сек.");
                    DisplayFutureValue(" сек.");
                    break;
                case (int)TypeOfValue.UnitsInSeconds:
                    roundNumber = 1;
                    DisplayCurrentValue("/сек.");
                    DisplayFutureValue("/сек.");
                    break;
                case (int)TypeOfValue.Percent:
                    roundNumber = 2;
                    curr_value *= 100;
                    future_value *= 100;
                    DisplayCurrentValue("%");
                    DisplayFutureValue("%");                    
                    break;
                case (int)TypeOfValue.Void:
                    roundNumber = 2;
                    DisplayCurrentValue("");
                    DisplayFutureValue("");
                    break;
            }
            DisplayCost();

            void DisplayCurrentValue(string postfix)
            { CurrentValue[index].text = $"{Math.Round(curr_value, roundNumber) + postfix}"; }

            void DisplayFutureValue(string postfix)
            { FutureValue[index].text = $"{Math.Round(future_value, roundNumber) + postfix}"; }

            void DisplayCost()
            { UpgradeCost[index].text = $"{Math.Round(cost, roundNumber)} веры"; }
        }

        protected virtual void DisplayAllInfo() { }

        protected bool CheckUpgradeConditions(int index)
        {
            if ((PurchaseCount[index] < _maxUpgradeCount) && Facilities.CheckFaithCurrencyAmount(Cost[index]))
            {
                AudioEffects.PlayPurchaseEffect();
                Facilities.FaithCurrencyPay(Cost[index]);
                SummCurrentPurchaseCount++;
                PurchaseCount[index]++;
                Cost[index] += CostAdditive[index];
                CheckAndAddWillCost();

                if (PurchaseCount[index] == _maxUpgradeCount)
                    MaxLevelReceived(index);

                return true;
            }
            else return false;
        }

        protected virtual void MaxLevelReceived(int index)
        {
            UpgradeButton[index].gameObject.SetActive(false);
            FutureValue[index].transform.parent.gameObject.SetActive(false);
            MaxMessage[index].gameObject.SetActive(true);
        }

        protected virtual void MaxLevelRevert(int index)
        {
            UpgradeButton[index].gameObject.SetActive(true);
            FutureValue[index].transform.parent.gameObject.SetActive(true);
            MaxMessage[index].gameObject.SetActive(false);
        }

        protected virtual void CheckAndAddWillCost()
        {
            if (((float)SummCurrentPurchaseCount / _summMaxUpgradeCount) >= CurrentPurchasePercent)
            {
                WillCost += _additiveWillCost;
                CurrentPurchasePercent += 0.2f;
            }
        }
    }

    internal class BallClamp : SkillTemplate
    {
        public new const float StartReloadTime = 12;
        public new const int StartWillCost = 25;
        private readonly int[] _startCost = { 60, 80, 80 };
        private const int _statCount = 3;

        public float Damage = _startDamage;
        private const float _startDamage = 50;
        private const float _maxDamage = 800;
        private float _additiveDamage;

        public float StunDuration = _startStunDuration;
        private const float _startStunDuration = 2;
        private const float _maxStun = 5;
        private float _additiveStun;

        public new int[] PurchaseCount = new int[_statCount];
        public new int[] Cost = new int[_statCount];
        public new int[] CostAdditive = { 15, 20, 20};
        protected new int _summMaxUpgradeCount = _statCount * _maxUpgradeCount;

        public override void Init(GameObject[] stats)
        {
            for (int i = 0; i < Cost.Length; i++)
                Cost[i] = _startCost[i];
            base.Cost = Cost;
            base.PurchaseCount = PurchaseCount;
            base.CostAdditive = CostAdditive;

            _additiveDamage = _maxDamage / _maxUpgradeCount;
            _additiveStun = (_maxStun - _startStunDuration) / _maxUpgradeCount;
            _deductibleReloadTime = (StartReloadTime * _maxDecreaseReloadMultiplier) / _maxUpgradeCount;
            _additiveWillCost = StartWillCost / 5;

            base.Init(stats);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            ReloadTime = StartReloadTime;
            Damage = _startDamage;
            StunDuration = _startStunDuration;
            WillCost = StartWillCost;
            base.PurchaseCount = PurchaseCount = new int[3];
            for (int i = 0; i < Cost.Length; i++)
            {
                base.Cost[i] = Cost[i] = _startCost[i];
                MaxLevelRevert(i);
            }

            DisplayAllInfo();
        }

        public void AddDamage()
        {
            int index = 0;

            if (CheckUpgradeConditions(index))
            {
                Damage += _additiveDamage;
                DisplayStatInfo(index, Damage, Damage + _additiveDamage, Cost[index], (int)TypeOfValue.Void);
            }
        }

        public void AddStunDuration()
        {
            int index = 1;
            if (CheckUpgradeConditions(index))
            {
                StunDuration += _additiveStun;
                DisplayStatInfo(index, StunDuration, StunDuration + _additiveStun, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        public void DecreaseReload()
        {
            int index = 2;

            if (CheckUpgradeConditions(index))
            {
                ReloadTime -= _deductibleReloadTime;
                DisplayStatInfo(index, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        protected override void DisplayAllInfo()
        {
            DisplayStatInfo(0, Damage, Damage + _additiveDamage, Cost[0], (int)TypeOfValue.Void);
            DisplayStatInfo(1, StunDuration, StunDuration + _additiveStun, Cost[1], (int)TypeOfValue.Seconds);
            DisplayStatInfo(2, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[2], (int)TypeOfValue.Seconds);
        }

        protected override void MaxLevelReceived(int index)
        {
            UpgradeButton[index].gameObject.SetActive(false);
            FutureValue[index].transform.parent.gameObject.SetActive(false);
            MaxMessage[index].gameObject.SetActive(true);
        }

        protected override void MaxLevelRevert(int index)
        {
            UpgradeButton[index].gameObject.SetActive(true);
            FutureValue[index].transform.parent.gameObject.SetActive(true);
            MaxMessage[index].gameObject.SetActive(false);
        }

        protected override void CheckAndAddWillCost()
        {
            if (((float)SummCurrentPurchaseCount / _summMaxUpgradeCount) >= CurrentPurchasePercent)
            {
                WillCost += _additiveWillCost;
                CurrentPurchasePercent += 0.2f;

                Skills.DisplayMarketSkillCostMoneyAndWill((int)SkillEnum.BallClamp);

                for (int i = 0; i < PlasesForSkillsCount; i++)
                {
                    if (Skills.SelectedSkillIndex[i] == (int)SkillEnum.BallClamp)
                    {
                        Skills.ChangeWillCostText(i, WillCost);
                    }
                }
            }
        }
    }
    internal BallClamp _BallClamp = new()
    { ReloadTime = BallClamp.StartReloadTime, UnlockCost = 2500, WillCost = BallClamp.StartWillCost };

    internal class ShootingGlance : SkillTemplate
    {
        public new const float StartReloadTime = 15;
        public new const int StartWillCost = 50;
        private readonly int[] _startCost = { 60, 80, 80, 90 };
        private const int _statCount = 4;

        public float Damage = _startDamage;
        private const float _startDamage = 50;
        private const float _maxDamage = 800;
        private float _additiveDamage = 0;

        public float PeriodicDamage = _startPeriodicDamage;
        private const float _startPeriodicDamage = 35;
        private const float _maxPeriodicDamage = 560;
        private float _additivePeriodicDamage = 0;

        public float Duration = _startDuration;
        private const float _startDuration = 2;
        private const float _maxDuration = 10;
        private float _additiveDuration = 0;

        public new int[] PurchaseCount = new int[_statCount];
        public new int[] Cost = new int[_statCount];
        public new int[] CostAdditive = { 15, 20, 20, 25 };
        protected new int _summMaxUpgradeCount = _statCount * _maxUpgradeCount;

        public override void Init(GameObject[] stats)
        {
            for (int i = 0; i < Cost.Length; i++)
                Cost[i] = _startCost[i];
            base.Cost = Cost;
            base.PurchaseCount = PurchaseCount;
            base.CostAdditive = CostAdditive;

            _additiveDamage = (_maxDamage - _startDamage) / _maxUpgradeCount;
            _additivePeriodicDamage = (_maxPeriodicDamage - _startPeriodicDamage) / _maxUpgradeCount;
            _additiveDuration = (_maxDuration - _startDuration) / _maxUpgradeCount;
            _deductibleReloadTime = (StartReloadTime * _maxDecreaseReloadMultiplier) / _maxUpgradeCount;
            _additiveWillCost = StartWillCost / 5;

            base.Init(stats);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            ReloadTime = StartReloadTime;
            Damage = _startDamage;
            PeriodicDamage = _startPeriodicDamage;
            Duration = _startDuration;
            WillCost = StartWillCost;
            base.PurchaseCount = PurchaseCount = new int[4];
            for (int i = 0; i < Cost.Length; i++)
            {
                base.Cost[i] = Cost[i] = _startCost[i];
                MaxLevelRevert(i);
            }

            DisplayAllInfo();
        }

        public void AddDamage()
        {
            int index = 0;

            if (CheckUpgradeConditions(index))
            {
                Damage += _additiveDamage;
                DisplayStatInfo(index, Damage, Damage + _additiveDamage, Cost[index], (int)TypeOfValue.Void);
            }
        }

        public void AddPeriodicDamage()
        {
            int index = 1;

            if (CheckUpgradeConditions(index))
            {
                PeriodicDamage += _additivePeriodicDamage;
                DisplayStatInfo(index, PeriodicDamage, PeriodicDamage + _additivePeriodicDamage, Cost[index], (int)TypeOfValue.UnitsInSeconds);
            }
        }

        public void AddDuration()
        {
            int index = 2;

            if (CheckUpgradeConditions(index))
            {
                Duration += _additiveDuration;
                DisplayStatInfo(index, Duration, Duration + _additiveDuration, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        public void DecreaseReload()
        {
            int index = 3;

            if (CheckUpgradeConditions(index))
            {
                ReloadTime -= _deductibleReloadTime;
                DisplayStatInfo(index, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        protected override void DisplayAllInfo()
        {
            DisplayStatInfo(0, Damage, Damage + _additiveDamage, Cost[0], (int)TypeOfValue.Void);
            DisplayStatInfo(1, PeriodicDamage, PeriodicDamage + _additivePeriodicDamage, Cost[1], (int)TypeOfValue.UnitsInSeconds);
            DisplayStatInfo(2, Duration, Duration + _additiveDuration, Cost[2], (int)TypeOfValue.Seconds);
            DisplayStatInfo(3, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[3], (int)TypeOfValue.Seconds);
        }

        protected override void MaxLevelReceived(int index)
        {
            UpgradeButton[index].gameObject.SetActive(false);
            FutureValue[index].transform.parent.gameObject.SetActive(false);
            MaxMessage[index].gameObject.SetActive(true);
        }

        protected override void CheckAndAddWillCost()
        {
            if (((float)SummCurrentPurchaseCount / _summMaxUpgradeCount) >= CurrentPurchasePercent)
            {
                WillCost += _additiveWillCost;
                CurrentPurchasePercent += 0.2f;

                Skills.DisplayMarketSkillCostMoneyAndWill((int)SkillEnum.ShootingGlance);

                for (int i = 0; i < PlasesForSkillsCount; i++)
                {
                    if (Skills.SelectedSkillIndex[i] == (int)SkillEnum.ShootingGlance)
                        Skills.ChangeWillCostText(i, WillCost);
                }
            }
        }
    }
    internal ShootingGlance _ShootingGlance = new()
    { ReloadTime = ShootingGlance.StartReloadTime, UnlockCost = 3500, WillCost = ShootingGlance.StartWillCost };

    internal class BlackSauna : SkillTemplate
    {
        public new const float StartReloadTime = 20;
        public new const int StartWillCost = 70;
        private static readonly int[] _startCost = { 70, 80, 80, 100 };
        private const int _statCount = 4;

        public float PeriodicDamage = _startPeriodicDamage;
        private const float _startPeriodicDamage = 75;
        private const float _maxPeriodicDamage = 1200;
        private float _additivePeriodicDamage;

        public float DecreasedHealthRegeneration = _startDecreasedHealthRegeneration;
        private const float _startDecreasedHealthRegeneration = 0.1f;
        private const float _maxDecreasedHealthRegeneration = 0.8f;
        private float _additiveDecreasedHealthRegeneration;

        public float Duration = _startDuration;
        private const float _startDuration = 2;
        private const float _maxDuration = 12;
        private float _additiveDuration = 0;

        public new int[] PurchaseCount = new int[_statCount];
        public new int[] Cost = new int[_statCount];
        public new int[] CostAdditive = { 20, 20, 20, 25 };
        protected new int _summMaxUpgradeCount = _statCount * _maxUpgradeCount;

        public override void Init(GameObject[] stats)
        {
            for (int i = 0; i < Cost.Length; i++)
                Cost[i] = _startCost[i];
            base.Cost = Cost;
            base.PurchaseCount = PurchaseCount;
            base.CostAdditive = CostAdditive;

            _additivePeriodicDamage = (_maxPeriodicDamage - _startPeriodicDamage) / _maxUpgradeCount;
            _additiveDecreasedHealthRegeneration = (_maxDecreasedHealthRegeneration - _startDecreasedHealthRegeneration) / _maxUpgradeCount;
            _additiveDuration = (_maxDuration - _startDuration) / _maxUpgradeCount;
            _deductibleReloadTime = (StartReloadTime * _maxDecreaseReloadMultiplier) / _maxUpgradeCount;
            _additiveWillCost = StartWillCost / 5;

            base.Init(stats);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            ReloadTime = StartReloadTime;
            PeriodicDamage = _startPeriodicDamage;
            DecreasedHealthRegeneration = _startDecreasedHealthRegeneration;
            Duration = _startDuration;
            WillCost = StartWillCost;
            base.PurchaseCount = PurchaseCount = new int[4];
            for (int i = 0; i < Cost.Length; i++)
            {
                base.Cost[i] = Cost[i] = _startCost[i];
                MaxLevelRevert(i);
            }

            DisplayAllInfo();
        }

        public void AddPeriodicDamage()
        {
            int index = 0;

            if (CheckUpgradeConditions(index))
            {
                PeriodicDamage += _additivePeriodicDamage;
                DisplayStatInfo(index, PeriodicDamage, PeriodicDamage + _additivePeriodicDamage, Cost[index], (int)TypeOfValue.UnitsInSeconds);
            }
        }

        public void DecreaseHealthRegeneration()
        {
            int index = 1;

            if (CheckUpgradeConditions(index))
            {
                DecreasedHealthRegeneration += _additiveDecreasedHealthRegeneration;
                DisplayStatInfo(index, DecreasedHealthRegeneration,
                    DecreasedHealthRegeneration + _additiveDecreasedHealthRegeneration, Cost[index], (int)TypeOfValue.Percent);
            }
        }

        public void AddDuration()
        {
            int index = 2;

            if (CheckUpgradeConditions(index))
            {
                Duration += _additiveDuration;
                DisplayStatInfo(index, Duration, Duration + _additiveDuration, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        public void DecreaseReload()
        {
            int index = 3;

            if (CheckUpgradeConditions(index))
            {
                ReloadTime -= _deductibleReloadTime;
                DisplayStatInfo(index, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        protected override void DisplayAllInfo()
        {
            DisplayStatInfo(0, PeriodicDamage, PeriodicDamage + _additivePeriodicDamage, Cost[0], (int)TypeOfValue.UnitsInSeconds);
            DisplayStatInfo(1, DecreasedHealthRegeneration, 
                DecreasedHealthRegeneration + _additiveDecreasedHealthRegeneration, Cost[1], (int)TypeOfValue.Percent);
            DisplayStatInfo(2, Duration, Duration + _additiveDuration, Cost[2], (int)TypeOfValue.Seconds);
            DisplayStatInfo(3, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[3], (int)TypeOfValue.Seconds);
        }

        protected override void MaxLevelReceived(int index)
        {
            UpgradeButton[index].gameObject.SetActive(false);
            FutureValue[index].transform.parent.gameObject.SetActive(false);
            MaxMessage[index].gameObject.SetActive(true);
        }

        protected override void CheckAndAddWillCost()
        {
            if (((float)SummCurrentPurchaseCount / _summMaxUpgradeCount) >= CurrentPurchasePercent)
            {
                WillCost += _additiveWillCost;
                CurrentPurchasePercent += 0.2f;

                Skills.DisplayMarketSkillCostMoneyAndWill((int)SkillEnum.BlackSauna);

                for (int i = 0; i < PlasesForSkillsCount; i++)
                {
                    if (Skills.SelectedSkillIndex[i] == (int)SkillEnum.BlackSauna)
                        Skills.ChangeWillCostText(i, WillCost);
                }
            }
        }
    }
    internal BlackSauna _BlackSauna = new()
    { ReloadTime = BlackSauna.StartReloadTime, UnlockCost = 7500, WillCost = BlackSauna.StartWillCost };

    internal class ArmorBreak : SkillTemplate
    {
        public new const float StartReloadTime = 30;
        public new const int StartWillCost = 70;
        private static readonly int[] _startCost = { 80, 90, 90, 110 };
        private const int _statCount = 4;

        public float Damage = _startDamage;
        private const float _startDamage = 100;
        private const float _maxDamage = 1600;
        private float _additiveDamage;

        public float DecreasedArmor = _startDecreasedArmor;
        private const float _startDecreasedArmor = 0.1f;
        private const float _maxDecreasedArmor = 0.8f;
        private float _additiveDecreasedArmor;

        public float Duration = _startDuration;
        private const float _startDuration = 6;
        private const float _maxDuration = 14;
        private float _additiveDuration;

        public new int[] PurchaseCount = new int[_statCount];
        public new int[] Cost = new int[_statCount];
        public new int[] CostAdditive = { 20, 25, 25, 35 };
        protected new int _summMaxUpgradeCount = _statCount * _maxUpgradeCount;

        public override void Init(GameObject[] stats)
        {
            for (int i = 0; i < Cost.Length; i++)
                Cost[i] = _startCost[i];
            base.Cost = Cost;
            base.PurchaseCount = PurchaseCount;
            base.CostAdditive = CostAdditive;

            _additiveDamage = (_maxDamage - _startDamage) / _maxUpgradeCount;
            _additiveDecreasedArmor = (_maxDecreasedArmor - _startDecreasedArmor) / _maxUpgradeCount;
            _additiveDuration = (_maxDuration - _startDuration) / _maxUpgradeCount;
            _deductibleReloadTime = (StartReloadTime * _maxDecreaseReloadMultiplier) / _maxUpgradeCount;
            _additiveWillCost = StartWillCost / 5;

            base.Init(stats);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            ReloadTime = StartReloadTime;
            Damage = _startDamage;
            DecreasedArmor = _startDecreasedArmor;
            Duration = _startDuration;
            WillCost = StartWillCost;
            base.PurchaseCount = PurchaseCount = new int[4];
            for (int i = 0; i < Cost.Length; i++)
            {
                base.Cost[i] = Cost[i] = _startCost[i];
                MaxLevelRevert(i);
            }

            DisplayAllInfo();
        }

        public void AddDamage()
        {
            int index = 0;

            if (CheckUpgradeConditions(index))
            {
                Damage += _additiveDamage;
                DisplayStatInfo(index, Damage, Damage + _additiveDamage, Cost[index], (int)TypeOfValue.Void);
            }
        }

        public void DecreaseArmor()
        {
            int index = 1;

            if (CheckUpgradeConditions(index))
            {
                DecreasedArmor += _additiveDecreasedArmor;
                DisplayStatInfo(index, DecreasedArmor, DecreasedArmor + _additiveDecreasedArmor, Cost[index], (int)TypeOfValue.Percent);
            }
        }

        public void AddDuration()
        {
            int index = 2;

            if (CheckUpgradeConditions(index))
            {
                Duration += _additiveDuration;
                DisplayStatInfo(index, Duration, Duration + _additiveDuration, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        public void DecreaseReload()
        {
            int index = 3;

            if (CheckUpgradeConditions(index))
            {
                ReloadTime -= _deductibleReloadTime;
                DisplayStatInfo(index, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        protected override void DisplayAllInfo()
        {
            DisplayStatInfo(0, Damage, Damage + _additiveDamage, Cost[0], (int)TypeOfValue.Void);
            DisplayStatInfo(1, DecreasedArmor, DecreasedArmor + _additiveDecreasedArmor , Cost[1], (int)TypeOfValue.Percent);
            DisplayStatInfo(2, Duration, Duration + _additiveDuration, Cost[2], (int)TypeOfValue.Seconds);
            DisplayStatInfo(3, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[3], (int)TypeOfValue.Seconds);
        }

        protected override void MaxLevelReceived(int index)
        {
            UpgradeButton[index].gameObject.SetActive(false);
            FutureValue[index].transform.parent.gameObject.SetActive(false);
            MaxMessage[index].gameObject.SetActive(true);
        }

        protected override void CheckAndAddWillCost()
        {
            if (((float)SummCurrentPurchaseCount / _summMaxUpgradeCount) >= CurrentPurchasePercent)
            {
                WillCost += _additiveWillCost;
                CurrentPurchasePercent += 0.2f;

                Skills.DisplayMarketSkillCostMoneyAndWill((int)SkillEnum.ArmorBreak);

                for (int i = 0; i < PlasesForSkillsCount; i++)
                {
                    if (Skills.SelectedSkillIndex[i] == (int)SkillEnum.ArmorBreak)
                        Skills.ChangeWillCostText(i, WillCost);
                }
            }
        }
    }
    internal ArmorBreak _ArmorBreak = new()
    { ReloadTime = ArmorBreak.StartReloadTime, UnlockCost = 12000, WillCost = ArmorBreak.StartWillCost };

    internal class ZeusAnger : SkillTemplate
    {
        public new const float StartReloadTime = 35;
        public new const int StartWillCost = 125;
        private static readonly int[] _startCost = { 110, 1600, 1000, 110 };
        private const int _statCount = 4;

        public float PercentDamage = _startPercentDamage;
        private const float _startPercentDamage = 0.05f;
        private const float _maxPercentDamage = 0.1f;
        private float _additivePercentDamage;

        public float TimeBetweenAttacks = _startTimeBetweenAttacks;
        private const float _startTimeBetweenAttacks = 1;
        private const float _minTimeBetweenAttacks = 0.1f;
        private const int _maxUpgradeTimeBetweenAttacks = 9;
        private float _deductibleAttackTime;

        public int AttackCount = _startAttackCount;
        private const int _startAttackCount = 1;
        private const int _maxAttackCount = 9;
        private const int _additiveAttackCount = 1;

        public new int[] PurchaseCount = new int[_statCount];
        public new int[] Cost = new int[_statCount];
        public new int[] CostAdditive = { 40, 800, 500, 35 };
        protected new int _summMaxUpgradeCount = (_statCount * _maxUpgradeCount);

        public override void Init(GameObject[] stats)
        {
            for (int i = 0; i < Cost.Length; i++)
                Cost[i] = _startCost[i];
            base.Cost = Cost;
            base.PurchaseCount = PurchaseCount;
            base.CostAdditive = CostAdditive;

            _additivePercentDamage = (_maxPercentDamage - _startPercentDamage) / _maxUpgradeCount;
            _deductibleAttackTime = (_startTimeBetweenAttacks - _minTimeBetweenAttacks) / _maxUpgradeTimeBetweenAttacks;
            _deductibleReloadTime = (StartReloadTime * _maxDecreaseReloadMultiplier) / _maxUpgradeCount;
            _additiveWillCost = StartWillCost / 5;

            base.Init(stats);
        }

        public override void Start()
        {
            base.Start();

            if (PurchaseCount[1] == _maxAttackCount)
            {
                MaxLevelReceived(1);
            }
            if (PurchaseCount[2] == _maxUpgradeTimeBetweenAttacks)
            {
                MaxLevelReceived(2);
            }
        }

        public override void ResetStats()
        {
            base.ResetStats();
            ReloadTime = StartReloadTime;
            PercentDamage = _startPercentDamage;
            TimeBetweenAttacks = _startTimeBetweenAttacks;
            AttackCount = _startAttackCount;
            WillCost = StartWillCost;
            base.PurchaseCount = PurchaseCount = new int[4];
            for (int i = 0; i < Cost.Length; i++)
            {
                base.Cost[i] = Cost[i] = _startCost[i];
                MaxLevelRevert(i);
            }

            DisplayAllInfo();
        }

        public void AddPercentDamage()
        {
            int index = 0;

            if (CheckUpgradeConditions(index))
            {
                PercentDamage += _additivePercentDamage;
                DisplayStatInfo(index, PercentDamage, PercentDamage + _additivePercentDamage, Cost[index], (int)TypeOfValue.Percent);
            }
        }

        public void AddLightningCount()
        {
            int index = 1;

            if (PurchaseCount[index] < _maxAttackCount && Facilities.CheckFaithCurrencyAmount(Cost[index]))
            {
                AudioEffects.PlayPurchaseEffect();
                Facilities.FaithCurrencyPay(Cost[index]);
                PurchaseCount[index]++;
                SummCurrentPurchaseCount += 3;
                AttackCount += _additiveAttackCount;
                Cost[index] += CostAdditive[index];
                CheckAndAddWillCost();
                DisplayStatInfo(index, AttackCount, AttackCount + _additiveAttackCount, Cost[index], (int)TypeOfValue.Void);

                if (PurchaseCount[index] == _maxAttackCount)
                    MaxLevelReceived(index);
            }
        }

        public void DecreaseTimeBetweenAttacks()
        {
            int index = 2;

            if (PurchaseCount[index] < _maxUpgradeTimeBetweenAttacks && Facilities.CheckFaithCurrencyAmount(Cost[index]))
            {
                AudioEffects.PlayPurchaseEffect();
                Facilities.FaithCurrencyPay(Cost[index]);
                PurchaseCount[index]++;
                SummCurrentPurchaseCount += 3;
                TimeBetweenAttacks -= _deductibleAttackTime;
                Cost[index] += CostAdditive[index];
                CheckAndAddWillCost();
                DisplayStatInfo(index, TimeBetweenAttacks, TimeBetweenAttacks - _deductibleAttackTime, Cost[index], (int)TypeOfValue.Seconds);

                if (PurchaseCount[index] == _maxUpgradeTimeBetweenAttacks)
                    MaxLevelReceived(index);
            }
        }

        public void DecreaseReload()
        {
            int index = 3;

            if (CheckUpgradeConditions(index))
            {
                ReloadTime -= _deductibleReloadTime;
                DisplayStatInfo(index, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        protected override void DisplayAllInfo()
        {
            DisplayStatInfo(0, PercentDamage, PercentDamage + _additivePercentDamage, Cost[0], (int)TypeOfValue.Percent);
            DisplayStatInfo(1, AttackCount, AttackCount + _additiveAttackCount, Cost[1], (int)TypeOfValue.Void);
            DisplayStatInfo(2, TimeBetweenAttacks, TimeBetweenAttacks - _deductibleAttackTime, Cost[2], (int)TypeOfValue.Seconds);
            DisplayStatInfo(3, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[3], (int)TypeOfValue.Seconds);
        }

        protected override void MaxLevelReceived(int index)
        {
            UpgradeButton[index].gameObject.SetActive(false);
            FutureValue[index].transform.parent.gameObject.SetActive(false);
            MaxMessage[index].gameObject.SetActive(true);
        }

        protected override void CheckAndAddWillCost()
        {
            if (((float)SummCurrentPurchaseCount / _summMaxUpgradeCount) >= CurrentPurchasePercent)
            {
                WillCost += _additiveWillCost;
                CurrentPurchasePercent += 0.2f;

                Skills.DisplayMarketSkillCostMoneyAndWill((int)SkillEnum.ZeusAnger);

                for (int i = 0; i < PlasesForSkillsCount; i++)
                {
                    if (Skills.SelectedSkillIndex[i] == (int)SkillEnum.ZeusAnger)
                        Skills.ChangeWillCostText(i, WillCost);
                }
            }
        }
    }
    internal ZeusAnger _ZeusAnger = new()
    { ReloadTime = ZeusAnger.StartReloadTime, UnlockCost = 17000, WillCost = ZeusAnger.StartWillCost };

    internal class HelpPerun : SkillTemplate
    {
        public new const float StartReloadTime = 60;
        public new const int StartWillCost = 150;
        private static readonly int[] _startCost = { 5000, 120 };
        private const int _statCount = 2;

        public int AttackCount = _startAttackCount;
        private const int _startAttackCount = 1;
        private const int _maxAttackCount = 2;
        private const int _additiveAttackCount = 1;

        public const float DamagePercentFromBossActualHealth = 0.2f;

        public new int[] PurchaseCount = new int[_statCount];
        public new int[] Cost = new int[_statCount];
        public new int[] CostAdditive = { 2500, 30 };
        protected new int _summMaxUpgradeCount = (_statCount * _maxUpgradeCount);

        public override void Init(GameObject[] stats)
        {
            for (int i = 0; i < Cost.Length; i++)
                Cost[i] = _startCost[i];
            base.Cost = Cost;
            base.PurchaseCount = PurchaseCount;
            base.CostAdditive = CostAdditive;
            _deductibleReloadTime = (StartReloadTime * _maxDecreaseReloadMultiplier) / _maxUpgradeCount;
            _additiveWillCost = StartWillCost / 5;

            base.Init(stats);
        }

        public override void Start()
        {
            base.Start();

            if (PurchaseCount[0] == _maxAttackCount)
                MaxLevelReceived(0);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            ReloadTime = StartReloadTime;
            AttackCount = _startAttackCount;
            WillCost = StartWillCost;
            base.PurchaseCount = PurchaseCount = new int[_statCount];
            for (int i = 0; i < Cost.Length; i++)
            {
                base.Cost[i] = Cost[i] = _startCost[i];
                MaxLevelRevert(i);
            }

            DisplayAllInfo();
        }

        public void AddLightningCount()
        {
            int index = 0;

            if (PurchaseCount[index] < _maxAttackCount && Facilities.CheckFaithCurrencyAmount(Cost[index])) 
            {
                AudioEffects.PlayPurchaseEffect();
                Facilities.FaithCurrencyPay(Cost[index]);
                PurchaseCount[index]++;
                SummCurrentPurchaseCount += 10;
                AttackCount += _additiveAttackCount;
                Cost[index] += CostAdditive[index];
                CheckAndAddWillCost();
                DisplayStatInfo(index, AttackCount, AttackCount + _additiveAttackCount, Cost[index], (int)TypeOfValue.Void);

                if (PurchaseCount[index] == _maxAttackCount)
                    MaxLevelReceived(index);
            }
        }

        public void DecreaseReload()
        {
            int index = 1;

            if (CheckUpgradeConditions(index))
            {
                ReloadTime -= _deductibleReloadTime;
                DisplayStatInfo(index, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        protected override void DisplayAllInfo()
        {
            DisplayStatInfo(0, AttackCount, AttackCount + _additiveAttackCount, Cost[0], (int)TypeOfValue.Void);
            DisplayStatInfo(1, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[1], (int)TypeOfValue.Seconds);
        }

        protected override void MaxLevelReceived(int index)
        {
            UpgradeButton[index].gameObject.SetActive(false);
            FutureValue[index].transform.parent.gameObject.SetActive(false);
            MaxMessage[index].gameObject.SetActive(true);
        }

        protected override void CheckAndAddWillCost()
        {
            if (((float)SummCurrentPurchaseCount / _summMaxUpgradeCount) >= CurrentPurchasePercent)
            {
                WillCost += _additiveWillCost;
                CurrentPurchasePercent += 0.2f;

                Skills.DisplayMarketSkillCostMoneyAndWill((int)SkillEnum.HelpPerun);

                for (int i = 0; i < PlasesForSkillsCount; i++)
                {
                    if (Skills.SelectedSkillIndex[i] == (int)SkillEnum.HelpPerun)
                        Skills.ChangeWillCostText(i, WillCost);
                }
            }
        }
    }
    internal HelpPerun _HelpPerun = new()
    { ReloadTime = HelpPerun.StartReloadTime, UnlockCost = 25000, WillCost = HelpPerun.StartWillCost };

    internal class PrayerToTheGods : SkillTemplate
    {
        public new const float StartReloadTime = 80;
        public new const int StartWillCost = 120;
        private static readonly int[] _startCost = { 80, 80, 100 };
        private const int _statCount = 3;

        public float PercentToStats = _startPercentToStats;
        private const float _startPercentToStats = 0.05f;
        private const float _maxPercentToStats = 0.5f;
        private float _additivePercentToStats;

        public float Duration = _startDuration;
        private const float _startDuration = 8;
        private const float _maxDuration = 25;
        private float _additiveDuration;

        public new int[] PurchaseCount = new int[_statCount];
        public new int[] Cost = new int[_statCount];
        public new int[] CostAdditive = { 20, 20, 25 };
        protected new int _summMaxUpgradeCount = _statCount * _maxUpgradeCount;

        public override void Init(GameObject[] stats)
        {
            for (int i = 0; i < Cost.Length; i++)
                Cost[i] = _startCost[i];
            base.Cost = Cost;
            base.PurchaseCount = PurchaseCount;
            base.CostAdditive = CostAdditive;

            _additivePercentToStats = _maxPercentToStats / _maxUpgradeCount;
            _additiveDuration = _maxDuration / _maxUpgradeCount;
            _deductibleReloadTime = (StartReloadTime * _maxDecreaseReloadMultiplier) / _maxUpgradeCount;
            _additiveWillCost = StartWillCost / 5;

            base.Init(stats);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            ReloadTime = StartReloadTime;
            PercentToStats = _startPercentToStats;
            Duration = _startDuration;
            WillCost = StartWillCost;
            base.PurchaseCount = PurchaseCount = new int[3];
            for (int i = 0; i < Cost.Length; i++)
            {
                base.Cost[i] = Cost[i] = _startCost[i];
                MaxLevelRevert(i);
            }

            DisplayAllInfo();
        }

        public void AddPercentToStats()
        {
            int index = 0;

            if (CheckUpgradeConditions(index))
            {
                PercentToStats += _additivePercentToStats;
                DisplayStatInfo(index, PercentToStats, PercentToStats + _additivePercentToStats, Cost[index], (int)TypeOfValue.Percent);
            }
        }

        public void AddDuration()
        {
            int index = 1;

            if (CheckUpgradeConditions(index))
            {
                Duration += _additiveDuration;
                DisplayStatInfo(index, Duration, Duration + _additiveDuration, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        public void DecreaseReload()
        {
            int index = 2;

            if (CheckUpgradeConditions(index))
            {
                ReloadTime -= _deductibleReloadTime;
                DisplayStatInfo(index, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        protected override void DisplayAllInfo()
        {
            DisplayStatInfo(0, PercentToStats, PercentToStats + _additivePercentToStats, Cost[0], (int)TypeOfValue.Percent);
            DisplayStatInfo(1, Duration, Duration + _additiveDuration, Cost[1], (int)TypeOfValue.Seconds);
            DisplayStatInfo(2, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[2], (int)TypeOfValue.Seconds);
        }

        protected override void MaxLevelReceived(int index)
        {
            UpgradeButton[index].gameObject.SetActive(false);
            FutureValue[index].transform.parent.gameObject.SetActive(false);
            MaxMessage[index].gameObject.SetActive(true);
        }

        protected override void CheckAndAddWillCost()
        {
            if (((float)SummCurrentPurchaseCount / _summMaxUpgradeCount) >= CurrentPurchasePercent)
            {
                WillCost += _additiveWillCost;
                CurrentPurchasePercent += 0.2f;

                Skills.DisplayMarketSkillCostMoneyAndWill((int)SkillEnum.PrayerToTheGods);

                for (int i = 0; i < PlasesForSkillsCount; i++)
                {
                    if (Skills.SelectedSkillIndex[i] == (int)SkillEnum.PrayerToTheGods)
                        Skills.ChangeWillCostText(i, WillCost);
                }
            }
        }
    }
    internal PrayerToTheGods _PrayerToTheGods = new()
    { ReloadTime = PrayerToTheGods.StartReloadTime, UnlockCost = 30000, WillCost = PrayerToTheGods.StartWillCost };

    internal class GeneralFee : SkillTemplate
    {
        public new const float StartReloadTime = 150;
        public new const int StartWillCost = 300;
        private static readonly int[] _startCost = { 100, 100, 125 };
        private const int _statCount = 3;

        public float PercentToStats = _startPercentToStats;
        private const float _startPercentToStats = 0.05f;
        private const float _maxPercentToStats = 0.2f;
        private float _additivePercentToStats;

        public float Duration = _startDuration;
        private const float _startDuration = 15;
        private const float _maxDuration = 45;
        private float _additiveDuration;

        public new int[] PurchaseCount = new int[_statCount];
        public new int[] Cost = new int[_statCount];
        public new int[] CostAdditive = { 25, 25, 35 };
        protected new int _summMaxUpgradeCount = _statCount * _maxUpgradeCount;

        public override void Init(GameObject[] stats)
        {
            for (int i = 0; i < Cost.Length; i++)
                Cost[i] = _startCost[i];
            base.Cost = Cost;
            base.PurchaseCount = PurchaseCount;
            base.CostAdditive = CostAdditive;

            _additivePercentToStats = _maxPercentToStats / _maxUpgradeCount;
            _additiveDuration = _maxDuration / _maxUpgradeCount;
            _deductibleReloadTime = (StartReloadTime * _maxDecreaseReloadMultiplier) / _maxUpgradeCount;
            _additiveWillCost = StartWillCost / 5;

            base.Init(stats);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            ReloadTime = StartReloadTime;
            PercentToStats = _startPercentToStats;
            Duration = _startDuration;
            WillCost = StartWillCost;
            base.PurchaseCount = PurchaseCount = new int[3];
            for (int i = 0; i < Cost.Length; i++)
            {
                base.Cost[i] = Cost[i] = _startCost[i];
                MaxLevelRevert(i);
            }

            DisplayAllInfo();
        }

        public void AddPercentToStats()
        {
            int index = 0;

            if (CheckUpgradeConditions(index))
            {
                PercentToStats += _additivePercentToStats;
                DisplayStatInfo(index, PercentToStats, PercentToStats + _additivePercentToStats, Cost[index], (int)TypeOfValue.Percent);
            }
        }

        public void AddDuration()
        {
            int index = 1;

            if (CheckUpgradeConditions(index))
            {
                Duration += _additiveDuration;
                DisplayStatInfo(index, Duration, Duration + _additiveDuration, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        public void DecreaseReload()
        {
            int index = 2;

            if (CheckUpgradeConditions(index))
            {
                ReloadTime -= _deductibleReloadTime;
                DisplayStatInfo(index, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[index], (int)TypeOfValue.Seconds);
            }
        }

        protected override void DisplayAllInfo()
        {
            DisplayStatInfo(0, PercentToStats, PercentToStats + _additivePercentToStats, Cost[0], (int)TypeOfValue.Percent);
            DisplayStatInfo(1, Duration, Duration + _additiveDuration, Cost[1], (int)TypeOfValue.Seconds);
            DisplayStatInfo(2, ReloadTime, ReloadTime - _deductibleReloadTime, Cost[2], (int)TypeOfValue.Seconds);
        }

        protected override void MaxLevelReceived(int index)
        {
            UpgradeButton[index].gameObject.SetActive(false);
            FutureValue[index].transform.parent.gameObject.SetActive(false);
            MaxMessage[index].gameObject.SetActive(true);
        }

        protected override void CheckAndAddWillCost()
        {
            if (((float)SummCurrentPurchaseCount / _summMaxUpgradeCount) >= CurrentPurchasePercent)
            {
                WillCost += _additiveWillCost;
                CurrentPurchasePercent += 0.2f;

                Skills.DisplayMarketSkillCostMoneyAndWill((int)SkillEnum.GeneralFee);

                for (int i = 0; i < PlasesForSkillsCount; i++)
                {
                    if (Skills.SelectedSkillIndex[i] == (int)SkillEnum.GeneralFee)
                        Skills.ChangeWillCostText(i, WillCost);
                }
            }
        }

    }
    internal GeneralFee _GeneralFee = new()
    { ReloadTime = GeneralFee.StartReloadTime, UnlockCost = 35000, WillCost = GeneralFee.StartWillCost };

    #endregion

    private void Awake()
    {
        MarketSkills[0].GetComponent<global::BallClamp>().Awake();
        MarketSkills[1].GetComponent<global::ShootingGlance>().Awake();
        MarketSkills[2].GetComponent<global::BlackSauna>().Awake();
        MarketSkills[3].GetComponent<global::ArmorBreak>().Awake();
        MarketSkills[4].GetComponent<global::ZeusAnger>().Awake();
        MarketSkills[5].GetComponent<global::HelpPerun>().Awake();
        MarketSkills[6].GetComponent<global::PrayerToTheGods>().Awake();
        MarketSkills[7].GetComponent<global::GeneralFee>().Awake();

        FindComponents();

        if (Game.FirstLaunchScene[Game.CurrentScene])
        {
            SkillIsSelected[0] = true;
        }
    }

    private void Start()
    {
        CheckSavedData();

        for (int iter = 0; iter < SkillsCount; iter++)
            DisplayMarketSkillCostMoneyAndWill(iter);
        for (int iter = 0; iter < PlasesForSkillsCount; iter++)
            DisplayPlaceCost(iter);

        //int summ = 400;
        //int summ_2 = 0;
        //int add = 200;
        //for (int i = 0; i < 30; i++)
        //{
        //    summ += add;
        //    summ_2 += summ;
        //}
        //Debug.Log("Summ = " + summ_2);
    }

    private void FindComponents()
    {
        for (int iter = 0; iter < SkillsCount; iter++)
        {
            RectTransform[] target_component = MarketSkills[iter].GetComponentsInChildren<RectTransform>(includeInactive: true);
            foreach (RectTransform var in target_component)
            {
                switch (var.name)
                {
                    case ("SkillCost"):
                        _marketSkillCostText[iter] = var.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("WillCost"):
                        _marketSkillWillCostText[iter] = var.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("SkillName"):
                        MarketSkillNameText[iter] = var.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("Skill_Button"):
                        MarketSkillButton[iter] = var.GetComponent<Button>();
                        break;
                }
            }
        }

        for (int iter = 0; iter < PlasesForSkillsCount; iter++)
        {
            RectTransform[] target_component = SelectedSkills[iter].GetComponentsInChildren<RectTransform>(includeInactive: true);
            foreach (RectTransform var in target_component)
            {
                switch (var.name)
                {
                    case ("SkillName"):
                        SelectedSkillNameText[iter] = var.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("WillCost"):
                        SelectedSkillWillCostText[iter] = var.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("Skill_Button"):
                        SelectedSkillButton[iter] = var.GetComponent<Button>();
                        break;
                }
            }
        }

        for (int iter = 0; iter < PlasesForSkillsCount; iter++)
        {
            RectTransform[] target_components;
            target_components = PlasesForSkills[iter].GetComponentsInChildren<RectTransform>(includeInactive: true);
            foreach (RectTransform var in target_components)
            {
                switch (var.name)
                {
                    case ("SkillLock"):
                        Lock[iter] = var.GetComponent<Image>();
                        break;
                    case ("AncestralPowerButton"):
                        BuyPlaceButton[iter] = var.GetComponent<Button>();
                        break;
                    case ("Message"):
                        PlaceMessage[iter] = var.GetComponent<Text>();
                        break;
                    case ("Cost"):
                        PlaceCostText[iter] = var.GetComponent<TextMeshProUGUI>();
                        break;
                }
            }
        }

        for (int iter = 0; iter < PlasesForSkillsCount; iter++)
        {
            RectTransform[] target_component = BattleMenuSkills[iter].GetComponentsInChildren<RectTransform>(includeInactive: true);
            foreach (RectTransform var in target_component)
            {
                switch (var.name)
                {
                    case ("SkillName"):
                        BattleMenuSkillNameText[iter] = var.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("WillCost"):
                        BattleMenuSkillWillCostText[iter] = var.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("ReloadImage"):
                        ReloadSkillImage[iter] = var.GetComponent<Image>();
                        break;
                    case ("ReloadValue"):
                        ReloadValueText[iter] = var.GetComponent<TextMeshProUGUI>();
                        break;
                }
            }
        }
    }

    private void CheckSavedData()
    {
        for (int i = 0; i < SkillsCount; i++)
        {
            if (SkillIsBought[i] == true)
                SwitchSkillExcessComponents(i, false);
        }

        for (int placeIndex = 0; placeIndex < PlasesForSkillsCount; placeIndex++)
        {
            if (PlaseForSkillIsBought[placeIndex] == true)
            {
                CloseSkillPlacesExcessComponents(placeIndex);
                if (placeIndex > 0)
                    BattleHero.CrossObject[placeIndex - 1].SetActive(false);

                if (SelectedSkillIndex[placeIndex] != -1)
                {
                    ChangeNameText(placeIndex, SelectedSkillIndex[placeIndex]);
                    ChangeImage(placeIndex, SelectedSkillIndex[placeIndex]);
                    ChangeSkillsInBattleWindow(placeIndex, "Activate");
                    AddListenerToBattleSkillButton(placeIndex);
                    ChangeCurrentSkillWillCostText(placeIndex, SelectedSkillIndex[placeIndex]);
                    SelectSkillBackgroundColor(SelectedSkillIndex[placeIndex]);
                }
            }

            if (SavedReloadTimeForSelectedSkill[placeIndex] > 0)
            {
                float reload = 0;

                switch (SelectedSkillIndex[placeIndex])
                {
                    case (int)SkillEnum.BallClamp:
                        reload = _BallClamp.ReloadTime;
                        break;
                    case (int)SkillEnum.ShootingGlance:
                        reload = _ShootingGlance.ReloadTime;
                        break;
                    case (int)SkillEnum.BlackSauna:
                        reload = _BlackSauna.ReloadTime;
                        break;
                    case (int)SkillEnum.ArmorBreak:
                        reload = _ArmorBreak.ReloadTime;
                        break;
                    case (int)SkillEnum.ZeusAnger:
                        reload = _ZeusAnger.ReloadTime;
                        break;
                    case (int)SkillEnum.HelpPerun:
                        reload = _HelpPerun.ReloadTime;
                        break;
                    case (int)SkillEnum.PrayerToTheGods:
                        reload = _PrayerToTheGods.ReloadTime;
                        break;
                    case (int)SkillEnum.GeneralFee:
                        reload = _GeneralFee.ReloadTime;
                        break;
                }

                SkillsMethods.SkillReload(reload, placeIndex);
            }
        }
    }

    #region PresentationArea
    private void ChangeCurrentSkillWillCostText(int placeIndex, int skillIndex)
    {
        switch (skillIndex)
        {
            case ((int)SkillEnum.BallClamp):
                ChangeWillCostText(placeIndex, _BallClamp.WillCost);
                break;
            case ((int)SkillEnum.ShootingGlance):
                ChangeWillCostText(placeIndex, _ShootingGlance.WillCost);
                break;
            case ((int)SkillEnum.BlackSauna):
                ChangeWillCostText(placeIndex, _BlackSauna.WillCost);
                break;
            case ((int)SkillEnum.ArmorBreak):
                ChangeWillCostText(placeIndex, _ArmorBreak.WillCost);
                break;
            case ((int)SkillEnum.ZeusAnger):
                ChangeWillCostText(placeIndex, _ZeusAnger.WillCost);
                break;
            case ((int)SkillEnum.HelpPerun):
                ChangeWillCostText(placeIndex, _HelpPerun.WillCost);
                break;
            case ((int)SkillEnum.PrayerToTheGods):
                ChangeWillCostText(placeIndex, _PrayerToTheGods.WillCost);
                break;
            case ((int)SkillEnum.GeneralFee):
                ChangeWillCostText(placeIndex, _GeneralFee.WillCost);
                break;
        }
    }

    private void CloseSkillPlacesExcessComponents(int placeIndex)
    {
        Lock[placeIndex].gameObject.SetActive(false);
        BuyPlaceButton[placeIndex].gameObject.SetActive(false);
        PlaceMessage[placeIndex].gameObject.SetActive(true);
    }

    private void SwitchSkillExcessComponents(int skillIndex, bool state)
    {
        MarketSkillButton[skillIndex].interactable = !state;

        Image[] target_components = MarketSkills[skillIndex].GetComponentsInChildren<Image>(includeInactive: true);
        foreach (Image var in target_components)
        {
            switch (var.name)
            {
                case ("Lock"):
                    var.gameObject.SetActive(state);
                    break;
                case ("BuySkill_Button"):
                    var.gameObject.SetActive(state);
                    break;
            }
        }
    }

    private void ChangeSkillsInBattleWindow(int placeIndex, string state)
    {
        if (state == "Activate")
        {
            SelectedSkills[placeIndex].SetActive(true);
            BattleMenuSkills[placeIndex].SetActive(true);
            BattleMenuSkillNameText[placeIndex].text = SelectedSkillNameText[placeIndex].text;
            BattleMenuSkills[placeIndex].GetComponent<Image>().sprite = SelectedSkillButton[placeIndex].GetComponent<Image>().sprite;
        }
        else if (state == "Deactivate")
        {
            SelectedSkills[placeIndex].SetActive(false);
            BattleMenuSkills[placeIndex].SetActive(false);
        }
    }

    private void DisplayMarketSkillCostMoneyAndWill(int skill_index)
    {
        float current_skill_money_cost = 0;
        float current_will_cost = 0;

        switch (skill_index)
        {
            case ((int)SkillEnum.BallClamp):
                current_skill_money_cost = _BallClamp.UnlockCost;
                current_will_cost = _BallClamp.WillCost;
                break;
            case ((int)SkillEnum.ShootingGlance):
                current_skill_money_cost = _ShootingGlance.UnlockCost;
                current_will_cost = _ShootingGlance.WillCost;
                break;
            case ((int)SkillEnum.BlackSauna):
                current_skill_money_cost = _BlackSauna.UnlockCost;
                current_will_cost = _BlackSauna.WillCost;
                break;
            case ((int)SkillEnum.ArmorBreak):
                current_skill_money_cost = _ArmorBreak.UnlockCost;
                current_will_cost = _ArmorBreak.WillCost;
                break;
            case ((int)SkillEnum.ZeusAnger):
                current_skill_money_cost = _ZeusAnger.UnlockCost;
                current_will_cost = _ZeusAnger.WillCost;
                break;
            case ((int)SkillEnum.HelpPerun):
                current_skill_money_cost = _HelpPerun.UnlockCost;
                current_will_cost = _HelpPerun.WillCost;
                break;
            case ((int)SkillEnum.PrayerToTheGods):
                current_skill_money_cost = _PrayerToTheGods.UnlockCost;
                current_will_cost = _PrayerToTheGods.WillCost;
                break;
            case ((int)SkillEnum.GeneralFee):
                current_skill_money_cost = _GeneralFee.UnlockCost;
                current_will_cost = _GeneralFee.WillCost;
                break;
        }

        _marketSkillCostText[skill_index].text = $"{current_skill_money_cost} веры";
        _marketSkillWillCostText[skill_index].text = $"{current_will_cost} воли";
    }

    private void ChangeNameText(int placeIndex, int skillIndex)
    { SelectedSkillNameText[placeIndex].text = MarketSkillNameText[skillIndex].text; }

    private void ChangeImage(int placeIndex, int skillIndex)
    { SelectedSkillButton[placeIndex].image.sprite = MarketSkillButton[skillIndex].image.sprite; }

    private void DeselectSkillBackgroundColor(int skillIndex)
    { MarketSkills[skillIndex].GetComponent<Image>().color = Heroes.HighlightColor_Deselect; }

    private void SelectSkillBackgroundColor(int skillIndex)
    { MarketSkills[skillIndex].GetComponent<Image>().color = Heroes.HighlightColor_Select; }

    private void DisplayPlaceCost(int place_index)
    { Facilities.DisplayCostText(PlaceCostText[place_index], PlasesForSkillsCost[place_index], "силы предков"); }

    private void ChangeWillCostText(int placeIndex, float value)
    {
        SelectedSkillWillCostText[placeIndex].text = $"{value} воли";
        BattleMenuSkillWillCostText[placeIndex].text = $"{value}";
    }
    #endregion

    #region Functional
    private void AddListenerToBattleSkillButton(int placeIndex)
    {
        switch (SelectedSkillIndex[placeIndex])
        {
            case ((int)SkillEnum.BallClamp):
                Skills.BattleMenuSkills[placeIndex].GetComponent<Button>().onClick.AddListener(SkillsMethods.BallClamp);
                break;
            case ((int)SkillEnum.ShootingGlance):
                Skills.BattleMenuSkills[placeIndex].GetComponent<Button>().onClick.AddListener(SkillsMethods.ShootingGlance);
                break;
            case ((int)SkillEnum.BlackSauna):
                Skills.BattleMenuSkills[placeIndex].GetComponent<Button>().onClick.AddListener(SkillsMethods.BlackSauna);
                break;
            case ((int)SkillEnum.ArmorBreak):
                Skills.BattleMenuSkills[placeIndex].GetComponent<Button>().onClick.AddListener(SkillsMethods.ArmorBreak);
                break;
            case ((int)SkillEnum.ZeusAnger):
                Skills.BattleMenuSkills[placeIndex].GetComponent<Button>().onClick.AddListener(SkillsMethods.ZeusAnger);
                break;
            case ((int)SkillEnum.HelpPerun):
                Skills.BattleMenuSkills[placeIndex].GetComponent<Button>().onClick.AddListener(SkillsMethods.HelpPerun);
                break;
            case ((int)SkillEnum.PrayerToTheGods):
                Skills.BattleMenuSkills[placeIndex].GetComponent<Button>().onClick.AddListener(SkillsMethods.PrayerToTheGods);
                break;
            case ((int)SkillEnum.GeneralFee):
                Skills.BattleMenuSkills[placeIndex].GetComponent<Button>().onClick.AddListener(SkillsMethods.GeneralFee);
                break;
        }
    }

    private void ActiveChooseComplect(int skillIndex)
    {
        _cachedSkillIndex = skillIndex;

        SwitchEnabledClickableButtonsForSkills(true, skillIndex);
        SubscribeSelectSkill();
    }

    private void NonActiveChooseComplect(int skillIndex)
    {
        SwitchEnabledClickableButtonsForSkills(false, skillIndex);
        UnsubscribeSelectSkill();
    }

    private void SwitchEnabledClickableButtonsForSkills(bool active_type, int skill_index)
    {
        for (int i = 0; i < PlasesForSkillsCount; i++)
        {
            if (SelectedSkillIndex[i] == skill_index)
            {
                return;
            }
        }

        for (int i = 0; i < PlasesForSkillsCount; i++)
        {
            if (PlaseForSkillIsBought[i] == true && active_type == true)
            {
                ShowCircles[i].GetComponent<Image>().color = _green;
                ShowCircles[i].SetActive(active_type);
            }

            else if (PlaseForSkillIsBought[i] == true && active_type == false)
            {
                ShowCircles[i].SetActive(active_type);
            }
        }
    }

    private void SubscribeSelectSkill()
    {
        for (int i = 0; i < PlasesForSkillsCount; i++)
            ShowCircles[i].GetComponent<Button>().onClick.AddListener(SelectSkill);
    }

    private void UnsubscribeSelectSkill()
    {
        for (int i = 0; i < PlasesForSkillsCount; i++)
            ShowCircles[i].GetComponent<Button>().onClick.RemoveListener(SelectSkill);
    }
    #endregion

    #region Delegates
    public void SelectSkill()
    {
        int placeIndex = ReturnedPlaseIndex;
        int skillIndex = _cachedSkillIndex;

        SwitchEnabledClickableButtonsForSkills(false, skillIndex);

        if (SelectedSkillIndex[placeIndex] != -1)
        {
            _cachedSkillIndex = SelectedSkillIndex[placeIndex];
            DeselectSkill();
        }

        SkillIsSelected[skillIndex] = true;
        SelectedSkillIndex[placeIndex] = skillIndex;
        ChangeNameText(placeIndex, skillIndex);
        ChangeImage(placeIndex, skillIndex);
        SelectSkillBackgroundColor(skillIndex);
        BattleMenuSkills[placeIndex].GetComponent<Button>().onClick.RemoveAllListeners();
        AddListenerToBattleSkillButton(placeIndex);
        ChangeCurrentSkillWillCostText(placeIndex, skillIndex);
        NonActiveChooseComplect(skillIndex);
        ChangeSkillsInBattleWindow(placeIndex, "Activate");
    }

    public void DeselectSkill()
    {
        int placeIndex = ReturnedPlaseIndex;
        int tmpSkillIndex = _cachedSkillIndex;

        if (SelectedSkillIndex[placeIndex] != -1)
            _cachedSkillIndex = SelectedSkillIndex[placeIndex];

        SkillIsSelected[_cachedSkillIndex] = false;
        SkillIsChoosing[_cachedSkillIndex] = false;
        ShowCircles[placeIndex].SetActive(false);
        NonActiveChooseComplect(_cachedSkillIndex);
        DeselectSkillBackgroundColor(_cachedSkillIndex);
        ChangeSkillsInBattleWindow(placeIndex, "Deactivate");
        SelectedSkillIndex[placeIndex] = -1;
        _cachedSkillIndex = tmpSkillIndex;
    }

    public void DeleteSkills()
    {
        DeleteChoose = !DeleteChoose;

        if (DeleteChoose == true)
        {
            for (int i = 0; i < PlasesForSkillsCount; i++)
            {
                if (SelectedSkillIndex[i] != -1)
                {
                    ShowCircles[i].GetComponent<Image>().color = _red;
                    ShowCircles[i].SetActive(true);
                    Subscribe(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < PlasesForSkillsCount; i++)
            {
                if (SelectedSkillIndex[i] != -1)
                {
                    ShowCircles[i].SetActive(false);
                    Unsubscribe(i);
                }
            }
        }

        void Subscribe(int i)
        {
            _cachedSkillIndex = SelectedSkillIndex[i];
            ShowCircles[i].GetComponent<Button>().onClick.AddListener(DeselectSkill);
        }

        void Unsubscribe(int i)
        {
            _cachedSkillIndex = SelectedSkillIndex[i];
            ShowCircles[i].GetComponent<Button>().onClick.RemoveListener(DeselectSkill);
        }
    }

    public void SwitchSelectedSkill(int skillIndex)
    {
        DeleteChoose = false;

        for (int tmpSkillIndex = 0; tmpSkillIndex < SkillsCount; tmpSkillIndex++)
        {
            if (tmpSkillIndex != skillIndex && SkillIsSelected[tmpSkillIndex] == false)
            {
                NonActiveChooseComplect(tmpSkillIndex);
                SkillIsChoosing[tmpSkillIndex] = false;
            }
        }

        if (SkillIsChoosing[skillIndex] == false)
        {
            SkillIsChoosing[skillIndex] = true;
            ActiveChooseComplect(skillIndex);
        }
        else
        {
            SkillIsChoosing[skillIndex] = false;
            NonActiveChooseComplect(skillIndex);
        }
    }

    public void PurchaseSkill(int skill_index)
    {
        float current_skill_cost = 0;

        switch (skill_index)
        {
            case ((int)SkillEnum.BallClamp):
                current_skill_cost = _BallClamp.UnlockCost;
                break;
            case ((int)SkillEnum.ShootingGlance):
                current_skill_cost = _ShootingGlance.UnlockCost;
                break;
            case ((int)SkillEnum.BlackSauna):
                current_skill_cost = _BlackSauna.UnlockCost;
                break;
            case ((int)SkillEnum.ArmorBreak):
                current_skill_cost = _ArmorBreak.UnlockCost;
                break;
            case ((int)SkillEnum.ZeusAnger):
                current_skill_cost = _ZeusAnger.UnlockCost;
                break;
            case ((int)SkillEnum.HelpPerun):
                current_skill_cost = _HelpPerun.UnlockCost;
                break;
            case ((int)SkillEnum.PrayerToTheGods):
                current_skill_cost = _PrayerToTheGods.UnlockCost;
                break;
            case ((int)SkillEnum.GeneralFee):
                current_skill_cost = _GeneralFee.UnlockCost;
                break;
        }

        if (Facilities.CheckFaithCurrencyAmount(current_skill_cost))
        {
            AudioEffects.PlayOneShotEffect(SkillNamesEffects[skill_index]);
            Facilities.FaithCurrencyPay(current_skill_cost);
            SkillIsBought[skill_index] = true;
            SwitchSkillExcessComponents(skill_index, false);

            if (Achievements_R_vs_L.SkillIsUnlocked[skill_index] == false)
            {
                Achievements_R_vs_L.SkillIsUnlocked[skill_index] = true;
                Achievements_R_vs_L.AccumulateSkills();
            }
        }
    }

    public void PurchasePlaceForSkill(int place_index)
    {
        if (Facilities.CheckAncestralPowerAmount(PlasesForSkillsCost[place_index]))
        {
            Facilities.AncestralPowerPay(PlasesForSkillsCost[place_index]);
            CloseSkillPlacesExcessComponents(place_index);
            PlaseForSkillIsBought[place_index] = true;

            if (place_index > 0)
                BattleHero.CrossObject[place_index - 1].SetActive(false);
        }
    }

    public void ResetProgress()
    {
        _BallClamp.ResetStats();
        _ShootingGlance.ResetStats();
        _BlackSauna.ResetStats();
        _ArmorBreak.ResetStats();
        _ZeusAnger.ResetStats();
        _HelpPerun.ResetStats();
        _PrayerToTheGods.ResetStats();
        _GeneralFee.ResetStats();

        for (int placeIndex = 0; placeIndex < PlasesForSkillsCount; placeIndex++)
        {
            ChangeSkillsInBattleWindow(placeIndex, "Deactivate");
            SelectedSkillIndex[placeIndex] = -1;
            SavedReloadTimeForSelectedSkill[placeIndex] = 0;
        }

        for (int i = 0; i < SkillsCount; i++)
        {
            if (i != 0)
            {
                SkillIsBought[i] = false;
                SkillIsSelected[i] = false;
                SkillIsChoosing[i] = false;
                DeselectSkillBackgroundColor(i);
                SwitchSkillExcessComponents(i, true);
            }
            else
            {
                SkillIsBought[i] = true;
                SkillIsSelected[i] = true;
                SelectedSkillIndex[0] = 0;
                ChangeNameText(0, i);
                ChangeImage(0, i);
                AddListenerToBattleSkillButton(0);
                ChangeCurrentSkillWillCostText(0, i);
                SwitchSkillExcessComponents(i, false);
                ChangeSkillsInBattleWindow(0, "Activate");
            }

            DisplayMarketSkillCostMoneyAndWill(i);
        }
    }
    #endregion
}