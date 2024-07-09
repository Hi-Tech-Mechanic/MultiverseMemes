using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Weapons : DataStructure
{
    [SerializeField] private GameObject[] _weaponsObjects;

    [NonSerialized] public Weapon CurrentWeapon = null;
    public readonly static int WeaponCount = 9;
    public readonly static int WeaponStatsCount = Enum.GetNames(typeof(StatsIndex)).Length;
    private const float _additivePercent = 1.1f;
    //private const float _maxStatValueMultiplier = 5;
    //private const float _maxUpgradeCostMultiplier = 7;
    private const int _maxUpgradeCount = 20;
    private readonly int[] _startWeaponsStaminaCost = { 8, 12, 16, 32, 48, 64, 128, 192, 256};
    private static int _currentUpgradeWeaponIndex;
    private const float _unlockCostRank1 = 1200;
    private const float _unlockCostRank2 = 10500;
    private const float _unlockCostRank3 = 55000;

    private enum WeaponsID
    {
        knife_1 = 0,
        mace_1 = 1,
        hammer_1 = 2,
        knife_2 = 3, 
        mace_2 = 4,
        hammer_2 = 5,
        knife_3 = 6,        
        mace_3 = 7,        
        hammer_3 = 8
    }

    public enum StatsIndex
    {
        Damage = 0,
        MaxHealth = 1,
        HealthRegeneration = 2,
        MaxStamina = 3,
        StaminaRegeneration = 4,
        Armor = 5,
        Evasion = 6,
        Strength = 7,
        Dexterity = 8,
        PercentFromEnemyMaxHealthToDamage = 9,
        ChanceToCrit = 10,
        CritDamage = 11,
        ChanceToStun = 12,
        StunDuration = 13,
        ChanceToBleeding = 14,
        BleedingDamage = 15,
        BleedingDuration = 16,
        StaminaCost = 17
        //MaxWill = 4, //TODO добавить оружие - посох
        //WillRegeneration = 5,
        //Intellect = 11
    }

    public class Weapon
    {
        public float[] StatsValue = new float[WeaponStatsCount];
        public float[] StatsUpgradeCost = new float[WeaponStatsCount];
        public float[] DeductibleStaminaCost = new float[WeaponCount];
        public int[] CurrentUpgradeCount = new int[WeaponStatsCount];

        public bool Bought = false;
        public bool Selected = false;
        public float UnlockCost = 0;

        public TextMeshProUGUI UnlockCostText;
        public TextMeshProUGUI QualityText;
        public TextMeshProUGUI[] UpgradeCostText = new TextMeshProUGUI[WeaponStatsCount];
        public TextMeshProUGUI[] CurrentStatValueText = new TextMeshProUGUI[WeaponStatsCount];
        public TextMeshProUGUI[] FutureStatValueText = new TextMeshProUGUI[WeaponStatsCount];
        public TextMeshProUGUI[] UpgradeCountText = new TextMeshProUGUI[WeaponStatsCount];
        public GameObject[] UpgradeButton = new GameObject[WeaponStatsCount];
        public GameObject[] ArrowObject = new GameObject[WeaponStatsCount];
        public GameObject BuyButtonObject;
        public GameObject Lock;
        public Button BuyButton;
        public Image BackgroundImage;
        public Image HeaderImage;

        #region Displaying

        public void UpdateCurrentStatValueText(int stat_index, float value, string postfix)
        {
            if (CurrentStatValueText[stat_index] != null)
                CurrentStatValueText[stat_index].text = $"+{GetRoundedValue(value)}{postfix}";
        }

        public void UpdateFutureStatValueText(int stat_index, float value, string postfix)
        {
            if (FutureStatValueText[stat_index] != null)
            {
                if (stat_index != (int)StatsIndex.StaminaCost)
                {
                    FutureStatValueText[stat_index].text = $"+{GetRoundedValue(value * _additivePercent /*GetAdditiveStatPercent(stat_index)*/)}{postfix}";
                }
                else 
                {
                    FutureStatValueText[stat_index].text = $"+{GetRoundedValue(value - DeductibleStaminaCost[_currentUpgradeWeaponIndex])}{postfix}";
                }
            }
        }

        public void UpdateCostText(int stat_index, float value)
        {
            if (UpgradeCostText[stat_index] != null)
                Facilities.DisplayCostText(UpgradeCostText[stat_index], value, "веры");
        }

        public void UpdateUnlockText()
        {
            if (UnlockCostText != null)
                UnlockCostText.text = $"{UnlockCost} веры";
        }

        public void UpdateTextCurrentUpgradeCount(int stat_index)
        {
            if (UpgradeCountText[stat_index] != null)
                UpgradeCountText[stat_index].text = $"{CurrentUpgradeCount[stat_index]}/{_maxUpgradeCount}";
        }
        #endregion

        public void SwitchLockState(bool state)
        {
            Bought = state;
            Lock.SetActive(!state);
            BuyButtonObject.SetActive(!state);
            QualityText.gameObject.SetActive(state);
            BackgroundImage.GetComponent<Button>().interactable = state;

            for (int i = 0; i < WeaponStatsCount; i++)
            {
                if (UpgradeButton[i] != null)
                {
                    UpgradeButton[i].GetComponent<Button>().interactable = state;
                    ArrowObject[i].SetActive(state);
                    FutureStatValueText[i].gameObject.SetActive(state);
                }
            }
        }

        public void SetMaxLevelActions(int stat_index)
        {
            UpgradeCostText[stat_index].fontSizeMin = 30;
            UpgradeCostText[stat_index].text = "Макс.уровень";
            ArrowObject[stat_index].SetActive(false);
            FutureStatValueText[stat_index].gameObject.SetActive(false);
        }

        public void CheckStatAndUpdate(int stat_index)
        {
            if (stat_index == (int)StatsIndex.StaminaRegeneration || stat_index == (int)StatsIndex.HealthRegeneration ||
                stat_index == (int)StatsIndex.BleedingDuration || stat_index == (int)StatsIndex.StunDuration ||
                stat_index == (int)StatsIndex.BleedingDamage)
            {
                UpdateCurrentStatValueText(stat_index, StatsValue[stat_index], "/сек.");
                UpdateFutureStatValueText(stat_index, StatsValue[stat_index], "/сек.");
            }
            else if (stat_index == (int)StatsIndex.ChanceToBleeding || stat_index == (int)StatsIndex.ChanceToCrit ||
                        stat_index == (int)StatsIndex.ChanceToStun || stat_index == (int)StatsIndex.Evasion ||
                        stat_index == (int)StatsIndex.CritDamage || stat_index == (int)StatsIndex.PercentFromEnemyMaxHealthToDamage)
            {
                UpdateCurrentStatValueText(stat_index, StatsValue[stat_index] * 100, "%");
                UpdateFutureStatValueText(stat_index, StatsValue[stat_index] * 100, "%");
            }
            else
            {
                UpdateCurrentStatValueText(stat_index, StatsValue[stat_index], "");
                UpdateFutureStatValueText(stat_index, StatsValue[stat_index], "");
            }

            UpdateCostText(stat_index, StatsUpgradeCost[stat_index]);
        }

        private string GetRoundedValue(float value)
        {
            string message;

            message = $"{ValuesRounding.FormattingValue("", "", value)}";
            return message;
        }

        //internal float GetAdditiveStatPercent(int statIndex)
        //{
        //    int currentUpgradeCount = CurrentUpgradeCount[statIndex];

        //    if (currentUpgradeCount != 0)
        //    {
        //        currentUpgradeCount -= 1;
        //    }
        //    return 1 + ((_maxStatValueMultiplier / _maxUpgradeCount * 2) / (_maxUpgradeCount - currentUpgradeCount));
        //}

        //internal float GetAdditiveCostPercent(int statIndex)
        //{
        //    int currentUpgradeCount = CurrentUpgradeCount[statIndex];

        //    if (currentUpgradeCount != 0)
        //    {
        //        currentUpgradeCount -= 1;
        //    }
        //    return 1 + ((_maxUpgradeCostMultiplier / _maxUpgradeCount * 2) / (_maxUpgradeCount - currentUpgradeCount));
        //}
    }
    public Weapon[] _Weapon = new Weapon[WeaponCount];

    private void Awake()
    {
        InitializeFields();
        FindWeaponComponents();

        if (Game.FirstLaunchScene[Game.CurrentScene])
            SetStartWeaponsStats();
    }

    private void Start()
    {
        DisplayAllWeaponInfo();
        CheckSavedData();
    }

    #region ButtonEvents
    public void UnlockWeapon(int weaponIndex)
    {
        if (Facilities.CheckFaithCurrencyAmount(_Weapon[weaponIndex].UnlockCost))
        {
            Pay(_Weapon[weaponIndex].UnlockCost);
            _Weapon[weaponIndex].SwitchLockState(true);
            SwitchChoose(weaponIndex);

            if (Achievements_R_vs_L.WeaponIsUnlocked[weaponIndex] == false)
            {
                Achievements_R_vs_L.WeaponIsUnlocked[weaponIndex] = true;
                Achievements_R_vs_L.AccumulateWeapons();
            }
        }
    }

    public void SwitchChoose(int weapon_index)
    {
        if (_Weapon[weapon_index].Bought)
        {
            if (CurrentWeapon != null)
            {
                if (_Weapon[weapon_index].GetHashCode() != CurrentWeapon.GetHashCode())
                {
                    CurrentWeapon = null;

                    foreach (Weapon weapon in _Weapon)
                    {
                        if (weapon.Selected)
                        {
                            weapon.Selected = false;
                            weapon.BackgroundImage.color = Heroes.HighlightColor_Deselect;
                        }
                    }
                }
            }

            if (CurrentWeapon == null) SelectWeapon();
            else DeselectWeapon();

            SetWeaponStatsToHero();
        }

        void SelectWeapon()
        {
            AudioEffects.PlayArsenalItemSelect();
            _Weapon[weapon_index].Selected = true;
            CurrentWeapon = _Weapon[weapon_index];
            CurrentWeapon.BackgroundImage.color = Heroes.HighlightColor_Select;
        }

        void DeselectWeapon()
        {
            AudioEffects.PlayButtonClickEffect();
            CurrentWeapon.BackgroundImage.color = Heroes.HighlightColor_Deselect;
            _Weapon[weapon_index].Selected = false;
            CurrentWeapon = null;
        }
    }

    public void UpgradeStat(string stat_name, RectTransform[] weapon_parent)
    {
        int weapon_index = 0;
        int stat_index;

        weapon_index = FindParent(weapon_parent);

        if (_Weapon[weapon_index].Bought)
        {
            switch (stat_name)
            {
                case ("Damage"):
                    stat_index = (int)StatsIndex.Damage;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    Heroes.CurrentHero.DamageFromWeapon = _Weapon[weapon_index].StatsValue[stat_index];
                    break;
                case ("MaxHealth"):
                    stat_index = (int)StatsIndex.MaxHealth;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    Heroes.CurrentHero.MaxHealthFromWeapon = _Weapon[weapon_index].StatsValue[stat_index];
                    break;
                case ("HealthRegeneration"):
                    stat_index = (int)StatsIndex.HealthRegeneration;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    Heroes.CurrentHero.HealthRegenerationFromWeapon = _Weapon[weapon_index].StatsValue[stat_index];
                    break;
                case ("MaxStamina"):
                    stat_index = (int)StatsIndex.MaxStamina;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    Heroes.CurrentHero.MaxStaminaFromWeapon = _Weapon[weapon_index].StatsValue[stat_index];
                    break;
                case ("StaminaRegeneration"):
                    stat_index = (int)StatsIndex.StaminaRegeneration;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    Heroes.CurrentHero.StaminaRegenerationFromWeapon = _Weapon[weapon_index].StatsValue[stat_index];
                    break;
                case ("Strength"):
                    stat_index = (int)StatsIndex.Strength;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    Heroes.CurrentHero.StrengthFromWeapon = _Weapon[weapon_index].StatsValue[stat_index];
                    break;
                case ("Dexterity"):
                    stat_index = (int)StatsIndex.Dexterity;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    Heroes.CurrentHero.DexterityFromWeapon = _Weapon[weapon_index].StatsValue[stat_index];
                    break;
                case ("Armor"):
                    stat_index = (int)StatsIndex.Armor;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    Heroes.CurrentHero.ArmorFromWeapon = _Weapon[weapon_index].StatsValue[stat_index];
                    break;
                case ("Evasion"):
                    stat_index = (int)StatsIndex.Evasion;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    Heroes.CurrentHero.EvasionFromWeapon = _Weapon[weapon_index].StatsValue[stat_index];
                    break;
                case ("BleedingDamage"):
                    stat_index = (int)StatsIndex.BleedingDamage;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    break;
                case ("ChanceToBleeding"):
                    stat_index = (int)StatsIndex.ChanceToBleeding;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    break;
                case ("BleedingDuration"):
                    stat_index = (int)StatsIndex.BleedingDuration;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    break;
                case ("ChanceToStun"):
                    stat_index = (int)StatsIndex.ChanceToStun;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    break;
                case ("StunDuration"):
                    stat_index = (int)StatsIndex.StunDuration;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    break;
                case ("ChanceToCrit"):
                    stat_index = (int)StatsIndex.ChanceToCrit;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    break;
                case ("CritDamage"):
                    stat_index = (int)StatsIndex.CritDamage;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    break;
                case ("DamageFromMaxHealth"):
                    stat_index = (int)StatsIndex.PercentFromEnemyMaxHealthToDamage;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    break;
                case ("StaminaCost"):
                    stat_index = (int)StatsIndex.StaminaCost;
                    SampleUpgradeMethod(weapon_index, stat_index);
                    break;
                default:
                    break;
            }

            SetWeaponStatsToHero();
        }

        void SampleUpgradeMethod(int weapon_index, int stat_index)
        {
            if (Facilities.CheckFaithCurrencyAmount(_Weapon[weapon_index].StatsUpgradeCost[stat_index]))
            {
                if (_Weapon[weapon_index].CurrentUpgradeCount[stat_index] != _maxUpgradeCount)
                {
                    AudioEffects.PlaySharpeningEffect();
                    _Weapon[weapon_index].CurrentUpgradeCount[stat_index]++;
                    _Weapon[weapon_index].UpdateTextCurrentUpgradeCount(stat_index);
                    DisplayCurrentQuality(weapon_index);
                    Pay(_Weapon[weapon_index].StatsUpgradeCost[stat_index]);

                    if (stat_index != (int)StatsIndex.StaminaCost)
                        _Weapon[weapon_index].StatsValue[stat_index] *= _additivePercent; // _Weapon[weapon_index].GetAdditiveStatPercent(stat_index);
                    else _Weapon[weapon_index].StatsValue[stat_index] -= _Weapon[weapon_index].DeductibleStaminaCost[weapon_index];

                    _Weapon[weapon_index].StatsUpgradeCost[stat_index] *= _additivePercent; // _Weapon[weapon_index].GetAdditiveCostPercent(stat_index);

                    _currentUpgradeWeaponIndex = weapon_index;
                    _Weapon[weapon_index].CheckStatAndUpdate(stat_index);

                    if (_Weapon[weapon_index].CurrentUpgradeCount[stat_index] == _maxUpgradeCount)
                        _Weapon[weapon_index].SetMaxLevelActions(stat_index);
                }
            }
        }

        int FindParent(RectTransform[] current_weapon_parent)
        {
            foreach (RectTransform var in current_weapon_parent)
            {
                switch (var.name)
                {
                    case ("Knife_1"):
                        weapon_index = (int)WeaponsID.knife_1;
                        break;
                    case ("Knife_2"):
                        weapon_index = (int)WeaponsID.knife_2;
                        break;
                    case ("Knife_3"):
                        weapon_index = (int)WeaponsID.knife_3;
                        break;
                    case ("Mace_1"):
                        weapon_index = (int)WeaponsID.mace_1;
                        break;
                    case ("Mace_2"):
                        weapon_index = (int)WeaponsID.mace_2;
                        break;
                    case ("Mace_3"):
                        weapon_index = (int)WeaponsID.mace_3;
                        break;
                    case ("Hammer_1"):
                        weapon_index = (int)WeaponsID.hammer_1;
                        break;
                    case ("Hammer_2"):
                        weapon_index = (int)WeaponsID.hammer_2;
                        break;
                    case ("Hammer_3"):
                        weapon_index = (int)WeaponsID.hammer_3;
                        break;

                    default: break;
                }
            }
            return weapon_index;
        }
    }
    #endregion

    public void SetWeaponStatsToHero()
    {
        if (CurrentWeapon != null)
        {
            Heroes.CurrentHero.CostStaminaToAttackFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.StaminaCost];
            Heroes.CurrentHero.StrengthFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.Strength];
            Heroes.CurrentHero.DexterityFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.Dexterity];

            Heroes.CurrentHero.MaxHealthFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.MaxHealth];
            Heroes.CurrentHero.HealthRegenerationFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.HealthRegeneration];
            Heroes.CurrentHero.MaxStaminaFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.MaxStamina];
            Heroes.CurrentHero.StaminaRegenerationFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.StaminaRegeneration];
            Heroes.CurrentHero.DamageFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.Damage];
            Heroes.CurrentHero.EvasionFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.Evasion];
            Heroes.CurrentHero.ArmorFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.Armor];

            Heroes.CurrentHero.CritChanceFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.ChanceToCrit];
            Heroes.CurrentHero.CritDamageFromWeapon = CurrentWeapon.StatsValue[(int)StatsIndex.CritDamage];
        }
        else
        {
            Heroes.CurrentHero.ResetWeaponStats();
        }
    }

    public void DisplayAllWeaponInfo()
    {
        for (int weapon_index = 0; weapon_index < WeaponCount; weapon_index++)
        {
            DisplayCurrentQuality(weapon_index);

            if (_Weapon[weapon_index].UnlockCostText != null)
                _Weapon[weapon_index].UpdateUnlockText();

            for (int stat_index = 0; stat_index < WeaponStatsCount; stat_index++)
            {
                _currentUpgradeWeaponIndex = weapon_index;
                _Weapon[weapon_index].CheckStatAndUpdate(stat_index);
                _Weapon[weapon_index].UpdateTextCurrentUpgradeCount(stat_index);
            }
        }
    }

    public void SetStartWeaponsStats()
    {
        CurrentWeapon = null;

        for (int i = 0; i < WeaponCount; i++)
        {
            _Weapon[i].Bought = false;
            _Weapon[i].Selected = false;

            for (int k = 0; k < WeaponStatsCount; k ++)
            {
                _Weapon[i].CurrentUpgradeCount[k] = 0;
            }
        }

        int weapon_index;
        int cost;

        #region WeaponsEnumaration
        #region knife_1
        cost = 50;
        weapon_index = (int)WeaponsID.knife_1;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Damage] = 10;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Evasion] = 0.01f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaCost] = _startWeaponsStaminaCost[weapon_index];
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Damage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Evasion] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaCost] = cost;
        #endregion

        #region knife_2
        weapon_index = (int)WeaponsID.knife_2;
        cost = 150;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Damage] = 55;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Evasion] = 0.015f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.BleedingDamage] = 35;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.BleedingDuration] = 1f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.ChanceToBleeding] = 0.04f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Dexterity] = 12;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaCost] = _startWeaponsStaminaCost[weapon_index];
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Damage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Evasion] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.BleedingDamage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.BleedingDuration] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.ChanceToBleeding] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Dexterity] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaCost] = cost;
        #endregion

        #region knife_3
        weapon_index = (int)WeaponsID.knife_3;
        cost = 450;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Damage] = 220;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Evasion] = 0.03f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Dexterity] = 25f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.ChanceToBleeding] = 0.07f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.BleedingDamage] = 85;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.BleedingDuration] = 1.8f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.ChanceToCrit] = 0.02f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.CritDamage] = 1.2f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaCost] = _startWeaponsStaminaCost[weapon_index];
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Damage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Evasion] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Dexterity] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.ChanceToBleeding] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.BleedingDamage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.BleedingDuration] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.ChanceToCrit] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.CritDamage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaCost] = cost;
        #endregion

        #region Mace_1
        weapon_index = (int)WeaponsID.mace_1;
        cost = 50;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Damage] = 18;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.MaxHealth] = 50f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaCost] = _startWeaponsStaminaCost[weapon_index];
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Damage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.MaxHealth] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaCost] = cost;
        #endregion

        #region Mace_2 
        weapon_index = (int)WeaponsID.mace_2;
        cost = 150;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Damage] = 85;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.MaxHealth] = 200;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.HealthRegeneration] = 3;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Strength] = 10;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaCost] = _startWeaponsStaminaCost[weapon_index];
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Damage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.MaxHealth] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.HealthRegeneration] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Strength] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaCost] = cost;
        #endregion

        #region Mace_3
        weapon_index = (int)WeaponsID.mace_3;
        cost = 450;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Damage] = 215;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.MaxHealth] = 650;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.HealthRegeneration] = 8;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Strength] = 30;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.PercentFromEnemyMaxHealthToDamage] = 0.02f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaCost] = _startWeaponsStaminaCost[weapon_index];
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Damage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.MaxHealth] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.HealthRegeneration] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Strength] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.PercentFromEnemyMaxHealthToDamage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaCost] = cost;
        #endregion

        #region hammer_1
        weapon_index = (int)WeaponsID.hammer_1;
        cost = 50;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Damage] = 25;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Armor] = 4f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaCost] = _startWeaponsStaminaCost[weapon_index];
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Damage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Armor] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaCost] = cost;
        #endregion

        #region hammer_2
        weapon_index = (int)WeaponsID.hammer_2;
        cost = 150;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Damage] = 120;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.MaxStamina] = 300f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaRegeneration] = 4f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Armor] = 8f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Strength] = 16f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaCost] = _startWeaponsStaminaCost[weapon_index];
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Damage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.MaxStamina] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaRegeneration] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Armor] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Strength] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaCost] = cost;
        #endregion

        #region hammer_3
        weapon_index = (int)WeaponsID.hammer_3;
        cost = 450;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Damage] = 310;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Armor] = 14f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.MaxStamina] = 650f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaRegeneration] = 9f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.Strength] = 45f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.ChanceToStun] = 0.05f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StunDuration] = 0.6f;
        _Weapon[weapon_index].StatsValue[(int)StatsIndex.StaminaCost] = _startWeaponsStaminaCost[weapon_index];
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Damage] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Armor] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.MaxStamina] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaRegeneration] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.Strength] = cost;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.ChanceToStun] = 650;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StunDuration] = 650;
        _Weapon[weapon_index].StatsUpgradeCost[(int)StatsIndex.StaminaCost] = cost;
        #endregion
        #endregion
    }

    private void FindWeaponComponents()
    {
        RectTransform[] arr;
        int index = 0;

        foreach (var weapon in _weaponsObjects)
        {
            arr = weapon.GetComponentsInChildren<RectTransform>(includeInactive: true);
            foreach (var element in arr)
            {
                switch (element.name)
                {
                    case ("Weapon_Button|Background"):
                        _Weapon[index].BackgroundImage = element.GetComponent<Image>();
                        break;
                    case ("Weapon_Name"):
                        _Weapon[index].HeaderImage = element.GetComponent<Image>();
                        break;                        
                    case ("BuyAmmunitionButton"):
                        _Weapon[index].BuyButton = element.GetComponent<Button>();
                        _Weapon[index].BuyButtonObject = element.gameObject;
                        break;
                    case ("CostValue"):
                        _Weapon[index].UnlockCostText = element.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("QualityText"):
                        _Weapon[index].QualityText = element.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("Lock"):
                        _Weapon[index].Lock = element.gameObject;
                        break;                       
                    case ("Damage"):
                        find_components_in_components(element, (int)StatsIndex.Damage);
                        break;
                    case ("MaxHealth"):
                        find_components_in_components(element, (int)StatsIndex.MaxHealth);
                        break;
                    case ("HealthRegeneration"):
                        find_components_in_components(element, (int)StatsIndex.HealthRegeneration);
                        break;
                    case ("MaxStamina"):
                        find_components_in_components(element, (int)StatsIndex.MaxStamina);
                        break;
                    case ("StaminaRegeneration"):
                        find_components_in_components(element, (int)StatsIndex.StaminaRegeneration);
                        break;
                    case ("Armor"):
                        find_components_in_components(element, (int)StatsIndex.Armor);
                        break;
                    case ("Evasion"):
                        find_components_in_components(element, (int)StatsIndex.Evasion);
                        break;
                    case ("Strength"):
                        find_components_in_components(element, (int)StatsIndex.Strength);
                        break;
                    case ("Dexterity"):
                        find_components_in_components(element, (int)StatsIndex.Dexterity);
                        break;
                    case ("BleedingDamage"):
                        find_components_in_components(element, (int)StatsIndex.BleedingDamage);
                        break;
                    case ("ChanceToBleeding"):
                        find_components_in_components(element, (int)StatsIndex.ChanceToBleeding);
                        break;
                    case ("BleedingDuration"):
                        find_components_in_components(element, (int)StatsIndex.BleedingDuration);
                        break;
                    case ("ChanceToStun"):
                        find_components_in_components(element, (int)StatsIndex.ChanceToStun);
                        break;
                    case ("StunDuration"):
                        find_components_in_components(element, (int)StatsIndex.StunDuration);
                        break;
                    case ("ChanceToCrit"):
                        find_components_in_components(element, (int)StatsIndex.ChanceToCrit);
                        break;
                    case ("CritDamage"):
                        find_components_in_components(element, (int)StatsIndex.CritDamage);
                        break;
                    case ("DamageFromMaxHealth"):
                        find_components_in_components(element, (int)StatsIndex.PercentFromEnemyMaxHealthToDamage);
                        break;
                    case ("StaminaCost"):
                        find_components_in_components(element, (int)StatsIndex.StaminaCost);
                        break;
                    default: break;
                }
            }
            index++;
        }

        void find_components_in_components(RectTransform element, int stat_index)
        {
            RectTransform[] arr_2 = null;

            arr_2 = element.gameObject.GetComponentsInChildren<RectTransform>(includeInactive: true);
            foreach (RectTransform tmp in arr_2)
            {
                switch (tmp.name)
                {
                    case ("UpgradeCost"):
                        _Weapon[index].UpgradeCostText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("Value"):
                        _Weapon[index].CurrentStatValueText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("PostUpgradeValue"):
                        _Weapon[index].FutureStatValueText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("UpgradeCount"):
                        _Weapon[index].UpgradeCountText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("UpgradeButton"):
                        _Weapon[index].UpgradeButton[stat_index] = tmp.gameObject;
                        break;
                    case ("BackgroundArrow"):
                        _Weapon[index].ArrowObject[stat_index] = tmp.gameObject;
                        break;
                }
            }
        }
    }

    private void InitializeFields()
    {
        for (int i = 0; i < WeaponCount; i++)
        {
            if (_Weapon[i] == null)
                _Weapon[i] = new Weapon();
        }

        #region SetCosts
        int weapon_index;

        weapon_index = (int)WeaponsID.knife_1;
        _Weapon[weapon_index].DeductibleStaminaCost[weapon_index] = (_startWeaponsStaminaCost[weapon_index] / 2f) / _maxUpgradeCount;
        _Weapon[weapon_index].UnlockCost = _unlockCostRank1;

        weapon_index = (int)WeaponsID.knife_2;
        _Weapon[weapon_index].DeductibleStaminaCost[weapon_index] = (_startWeaponsStaminaCost[weapon_index] / 2f) / _maxUpgradeCount;
        _Weapon[weapon_index].UnlockCost = _unlockCostRank2;

        weapon_index = (int)WeaponsID.knife_3;
        _Weapon[weapon_index].DeductibleStaminaCost[weapon_index] = (_startWeaponsStaminaCost[weapon_index] / 2f) / _maxUpgradeCount;
        _Weapon[weapon_index].UnlockCost = _unlockCostRank3;

        weapon_index = (int)WeaponsID.mace_1;
        _Weapon[weapon_index].DeductibleStaminaCost[weapon_index] = (_startWeaponsStaminaCost[weapon_index] / 2f) / _maxUpgradeCount;
        _Weapon[weapon_index].UnlockCost = _unlockCostRank1;

        weapon_index = (int)WeaponsID.mace_2;
        _Weapon[weapon_index].DeductibleStaminaCost[weapon_index] = (_startWeaponsStaminaCost[weapon_index] / 2f) / _maxUpgradeCount;
        _Weapon[weapon_index].UnlockCost = _unlockCostRank2;

        weapon_index = (int)WeaponsID.mace_3;
        _Weapon[weapon_index].DeductibleStaminaCost[weapon_index] = (_startWeaponsStaminaCost[weapon_index] / 2f) / _maxUpgradeCount;
        _Weapon[weapon_index].UnlockCost = _unlockCostRank3;

        weapon_index = (int)WeaponsID.hammer_1;
        _Weapon[weapon_index].DeductibleStaminaCost[weapon_index] = (_startWeaponsStaminaCost[weapon_index] / 2f) / _maxUpgradeCount;
        _Weapon[weapon_index].UnlockCost = _unlockCostRank1;

        weapon_index = (int)WeaponsID.hammer_2;
        _Weapon[weapon_index].DeductibleStaminaCost[weapon_index] = (_startWeaponsStaminaCost[weapon_index] / 2f) / _maxUpgradeCount;
        _Weapon[weapon_index].UnlockCost = _unlockCostRank2;

        weapon_index = (int)WeaponsID.hammer_3;
        _Weapon[weapon_index].DeductibleStaminaCost[weapon_index] = (_startWeaponsStaminaCost[weapon_index] / 2f) / _maxUpgradeCount;
        _Weapon[weapon_index].UnlockCost = _unlockCostRank3;
        #endregion
    }

    private void CheckSavedData()
    {
        for (int weaponIndex = 0; weaponIndex < WeaponCount; weaponIndex++)
        {
            if (_Weapon[weaponIndex].Bought)
            {
                _Weapon[weaponIndex].SwitchLockState(true);
                DisplayCurrentQuality(weaponIndex);

                for (int statIndex = 0; statIndex < WeaponStatsCount; statIndex++)
                {
                    if (_Weapon[weaponIndex].CurrentUpgradeCount[statIndex] == _maxUpgradeCount)
                        _Weapon[weaponIndex].SetMaxLevelActions(statIndex);
                }

                if (_Weapon[weaponIndex].Selected)
                {
                    CurrentWeapon = _Weapon[weaponIndex];
                    CurrentWeapon.BackgroundImage.color = Heroes.HighlightColor_Select;
                    //SetStartWeaponsStats();
                }
            }
        }
    }

    private void Pay(float cost)
    {
        if (Facilities.CheckFaithCurrencyAmount(cost))
            Facilities.FaithCurrencyPay(cost);
        Facilities.UpdateFaithCurrencyText();
    }

    private void DisplayCurrentQuality(int weaponIndex)
    {
        int maxUpgradeCountForCurrentWeapon = 0;
        int summUpgradeCount = 0;

        foreach (float stat in _Weapon[weaponIndex].StatsValue)
        {
            if (stat != 0)
            {
                maxUpgradeCountForCurrentWeapon++;
            }
        }
        maxUpgradeCountForCurrentWeapon *= _maxUpgradeCount;

        foreach (int count in _Weapon[weaponIndex].CurrentUpgradeCount)
        {
            if (count != 0)
            {
                summUpgradeCount += count;
            }
        }

        Heroes.SetQuality(_Weapon[weaponIndex].HeaderImage, _Weapon[weaponIndex].QualityText, summUpgradeCount, maxUpgradeCountForCurrentWeapon);
    }
}