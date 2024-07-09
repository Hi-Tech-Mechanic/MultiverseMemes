using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ArmorMark_3 : DataStructure
{
    private enum StatsIndex
    {
        MaxHealth = 0,
        HealthRegeneration = 1,
        MaxStamina = 2,
        StaminaRegeneration = 3,
        MaxWill = 4,
        WillRegeneration = 5,
        Damage = 6,
        Armor = 7,
        Evasion = 8,
        Strength = 9,
        Dexterity = 10,
        Intellect = 11
    }

    [Header("Characteristic")]
    [Space(5)] [NonSerialized] public float[] StatValue = new float[StatsCount];
    [Space(5)] [NonSerialized] public float[] StatUpgradeCost = new float[StatsCount];
    [NonSerialized] public int[] CurrentUpgradeCount = new int[StatsCount];
    [NonSerialized] public bool Bought = false;
    [NonSerialized] public bool Selected = false;
    public static readonly int StatsCount = 12;
    private const int _armorUnlockCost = 75000;
    private const int _maxUpgradeCount = 20;
    private const float _additivePercent = 1.1f;

    private readonly TextMeshProUGUI[] UpgradeCostText = new TextMeshProUGUI[StatsCount];
    private readonly TextMeshProUGUI[] CurrentStatValueText = new TextMeshProUGUI[StatsCount];
    private readonly TextMeshProUGUI[] FutureStatValueText = new TextMeshProUGUI[StatsCount];
    private readonly TextMeshProUGUI[] UpgradeCountText = new TextMeshProUGUI[StatsCount];
    private readonly Button[] _upgradeButton = new Button[StatsCount];
    private readonly GameObject[] _arrow = new GameObject[StatsCount];
    private TextMeshProUGUI _ulockCostText;
    private GameObject _buyButtonObject;
    private GameObject _lock;
    private GameObject _backgroundWindow;
    private TextMeshProUGUI _qualityText;
    private Image _headerImage;

    public void Awake()
    {
        FindComponents();

        if (Game.FirstLaunchScene[Game.CurrentScene])
            SetStartStats();
    }

    private void Start()
    {
        DisplayAllInfo();

        if (Bought)
        {
            SwitchUnlockState(false);
            DisplayCurrentQuality();

            for (int i = 0; i < StatsCount; i++)
            {
                if (CurrentUpgradeCount[i] == _maxUpgradeCount)
                {
                    SetMessageMaxLevel(i);
                }
            }

            if (Selected)
            {
                _backgroundWindow.GetComponent<Image>().color = Heroes.HighlightColor_Select;
                SetCharacteriscticsValues();
            }
        }            
    }

    public void SetStartStats()
    {
        int upgradeCost = 850;

        Bought = false;
        Selected = false;

        for (int i = 0; i < StatsCount; i++)
            CurrentUpgradeCount[i] = 0;

        StatValue[(int)StatsIndex.MaxHealth] = 2500;
        StatUpgradeCost[(int)StatsIndex.MaxHealth] = upgradeCost;

        StatValue[(int)StatsIndex.HealthRegeneration] = 20;
        StatUpgradeCost[(int)StatsIndex.HealthRegeneration] = upgradeCost;

        StatValue[(int)StatsIndex.MaxStamina] = 1200;
        StatUpgradeCost[(int)StatsIndex.MaxStamina] = upgradeCost;

        StatValue[(int)StatsIndex.StaminaRegeneration] = 10;
        StatUpgradeCost[(int)StatsIndex.StaminaRegeneration] = upgradeCost;

        StatValue[(int)StatsIndex.MaxWill] = 800;
        StatUpgradeCost[(int)StatsIndex.MaxWill] = upgradeCost;

        StatValue[(int)StatsIndex.WillRegeneration] = 10;
        StatUpgradeCost[(int)StatsIndex.WillRegeneration] = upgradeCost;

        StatValue[(int)StatsIndex.Damage] = 150;
        StatUpgradeCost[(int)StatsIndex.Damage] = upgradeCost;

        StatValue[(int)StatsIndex.Armor] = 45;
        StatUpgradeCost[(int)StatsIndex.Armor] = upgradeCost;

        StatValue[(int)StatsIndex.Evasion] = 0.035f;
        StatUpgradeCost[(int)StatsIndex.Evasion] = upgradeCost;

        StatValue[(int)StatsIndex.Strength] = 45;
        StatUpgradeCost[(int)StatsIndex.Strength] = upgradeCost;

        StatValue[(int)StatsIndex.Dexterity] = 35;
        StatUpgradeCost[(int)StatsIndex.Dexterity] = upgradeCost;

        StatValue[(int)StatsIndex.Intellect] = 25;
        StatUpgradeCost[(int)StatsIndex.Intellect] = upgradeCost;

        DisplayAllInfo();
    }

    private void FindComponents()
    {
        RectTransform[] children = gameObject.GetComponentsInChildren<RectTransform>(includeInactive: true);

        foreach (var tmp in children)
        {
            switch (tmp.name)
            {
                case ("BuyAmmunitionButton"):
                    _buyButtonObject = tmp.gameObject;
                    break;
                case ("CostValue"):
                    _ulockCostText = tmp.GetComponent<TextMeshProUGUI>();
                    break;
                case ("Lock"):
                    _lock = tmp.gameObject;
                    break;
                case ("Armor_Button|Background"):
                    _backgroundWindow = tmp.gameObject;
                    break;
                case ("Header"):
                    _headerImage = tmp.GetComponent<Image>();
                    break;
                case ("QualityText"):
                    _qualityText = tmp.GetComponent<TextMeshProUGUI>();
                    break;
                case ("MaxHealth"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.MaxHealth);
                    break;
                case ("HealthRegeneration"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.HealthRegeneration);
                    break;
                case ("MaxStamina"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.MaxStamina);
                    break;
                case ("StaminaRegeneration"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.StaminaRegeneration);
                    break;
                case ("MaxWill"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.MaxWill);
                    break;
                case ("WillRegeneration"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.WillRegeneration);
                    break;
                case ("Damage"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.Damage);
                    break;
                case ("Armor"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.Armor);
                    break;
                case ("Evasion"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.Evasion);
                    break;
                case ("Strength"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.Strength);
                    break;
                case ("Dexterity"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.Dexterity);
                    break;
                case ("Intellect"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.Intellect);
                    break;
                default: break;
            }
        }

        void FindComponentsInComponents(RectTransform element, int stat_index)
        {
            RectTransform[] arr_2 = element.gameObject.GetComponentsInChildren<RectTransform>(includeInactive: true);

            foreach (RectTransform tmp in arr_2)
            {
                switch (tmp.name)
                {
                    case ("BackgroundArrow"):
                        _arrow[stat_index] = tmp.gameObject;
                        break;
                    case ("UpgradeCost"):
                        UpgradeCostText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("Value"):
                        CurrentStatValueText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("PostUpgradeValue"):
                        FutureStatValueText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("UpgradeCount"):
                        UpgradeCountText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("UpgradeButton"):
                        _upgradeButton[stat_index] = tmp.GetComponent<Button>();
                        break;
                    default: break;
                }
            }
        }
    }

    private void DisplayAllInfo()
    {
        for (int stat_index = 0; stat_index < StatsCount; stat_index++)
        {
            UpdateCurrentStatValueText(stat_index, StatValue[stat_index]);
            UpdateFutureStatValueText(stat_index, StatValue[stat_index] * _additivePercent);
            UpdateCostText(stat_index, StatUpgradeCost[stat_index]);
            UpdateUpgradeCountText(stat_index, CurrentUpgradeCount[stat_index]);
            _arrow[stat_index].SetActive(false);
            FutureStatValueText[stat_index].gameObject.SetActive(false);
        }

        UpdateUlockCostText();
        DisplayCurrentQuality();
    }

    public void IncreaseStat(int stat_index)
    {
        if (Facilities.CheckFaithCurrencyAmount(StatUpgradeCost[stat_index]))
        {
            if (CurrentUpgradeCount[stat_index] != _maxUpgradeCount)
            {
                AudioEffects.PlaySharpeningEffect();
                Facilities.FaithCurrencyPay(StatUpgradeCost[stat_index]);
                CurrentUpgradeCount[stat_index]++;
                StatValue[stat_index] *= _additivePercent;
                StatUpgradeCost[stat_index] *= _additivePercent;
                DisplayCurrentQuality();
                UpdateCurrentStatValueText(stat_index, StatValue[stat_index]);
                UpdateFutureStatValueText(stat_index, StatValue[stat_index] * _additivePercent);
                UpdateCostText(stat_index, StatUpgradeCost[stat_index]);
                UpdateUpgradeCountText(stat_index, CurrentUpgradeCount[stat_index]);

                if (CurrentUpgradeCount[stat_index] == _maxUpgradeCount)
                { SetMessageMaxLevel(stat_index); }
            }
        }
        if (Selected)
        {
            switch (stat_index)
            {
                case (int)StatsIndex.MaxHealth:
                    Heroes.CurrentHero.MaxHealthFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.HealthRegeneration:
                    Heroes.CurrentHero.HealthRegenerationFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.MaxStamina:
                    Heroes.CurrentHero.MaxStaminaFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.StaminaRegeneration:
                    Heroes.CurrentHero.StaminaRegenerationFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.MaxWill:
                    Heroes.CurrentHero.MaxWillFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.WillRegeneration:
                    Heroes.CurrentHero.WillRegenerationFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.Damage:
                    Heroes.CurrentHero.DamageFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.Armor:
                    Heroes.CurrentHero.ArmorFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.Evasion:
                    Heroes.CurrentHero.EvasionFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.Strength:
                    Heroes.CurrentHero.StrengthFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.Dexterity:
                    Heroes.CurrentHero.DexterityFromArmorSet = StatValue[stat_index];
                    break;
                case (int)StatsIndex.Intellect:
                    Heroes.CurrentHero.IntellectFromArmorSet = StatValue[stat_index];
                    break;
            }
        }   
    }

    public void UnlockArmorSet()
    {
        if (Facilities.CheckFaithCurrencyAmount(_armorUnlockCost))
        {
            Facilities.FaithCurrencyPay(_armorUnlockCost);
            Bought = true;
            SwitchUnlockState(false);
            SwitchChoose();

            if (Achievements_R_vs_L.ArmorIsUnlocked[2] == false)
            {
                Achievements_R_vs_L.ArmorIsUnlocked[2] = true;
                Achievements_R_vs_L.AccumulateArmors();
            }
        }
    }

    public void SwitchUnlockState(bool state)
    {
        _lock.SetActive(state);
        _buyButtonObject.SetActive(state);
        _qualityText.gameObject.SetActive(!state);
        _backgroundWindow.GetComponent<Button>().interactable = !state;

        for (int i = 0; i < StatsCount; i++)
        {
            _upgradeButton[i].interactable = !state;
            _arrow[i].SetActive(!state);
            FutureStatValueText[i].gameObject.SetActive(!state);
        }

        if (state == true)
            _backgroundWindow.GetComponent<Image>().color = Heroes.HighlightColor_Deselect;
    }

    public void SwitchChoose()
    {
        if (ArmorMark_1.Selected)
            ArmorMark_1.DeselectArmorSet();
        if (ArmorMark_2.Selected)
            ArmorMark_2.DeselectArmorSet();

        if (Bought)
        {
            AudioEffects.PlayButtonClickEffect();
            if (!Selected) SelectArmorSet();
            else DeselectArmorSet();
        }
    }

    private void SelectArmorSet()
    {
        Selected = true;
        AudioEffects.PlayArsenalItemSelect();
        _backgroundWindow.GetComponent<Image>().color = Heroes.HighlightColor_Select;
        SetCharacteriscticsValues();
    }

    public void DeselectArmorSet()
    {
        if (Selected == true)
        {
            Selected = false;
            AudioEffects.PlayButtonClickEffect();
            _backgroundWindow.GetComponent<Image>().color = Heroes.HighlightColor_Deselect;
            Heroes.CurrentHero.ResetArmorStats();
        }
    }

    public void SetCharacteriscticsValues()
    {
        Heroes.CurrentHero.MaxHealthFromArmorSet = StatValue[(int)StatsIndex.MaxHealth];
        Heroes.CurrentHero.HealthRegenerationFromArmorSet = StatValue[(int)StatsIndex.HealthRegeneration];
        Heroes.CurrentHero.MaxStaminaFromArmorSet = StatValue[(int)StatsIndex.MaxStamina];
        Heroes.CurrentHero.StaminaRegenerationFromArmorSet = StatValue[(int)StatsIndex.StaminaRegeneration];
        Heroes.CurrentHero.MaxWillFromArmorSet = StatValue[(int)StatsIndex.MaxWill];
        Heroes.CurrentHero.WillRegenerationFromArmorSet = StatValue[(int)StatsIndex.WillRegeneration];
        Heroes.CurrentHero.ArmorFromArmorSet = StatValue[(int)StatsIndex.Armor];
        Heroes.CurrentHero.DamageFromArmorSet = StatValue[(int)StatsIndex.Damage];
        Heroes.CurrentHero.EvasionFromArmorSet = StatValue[(int)StatsIndex.Evasion];
        Heroes.CurrentHero.StrengthFromArmorSet = StatValue[(int)StatsIndex.Strength];
        Heroes.CurrentHero.DexterityFromArmorSet = StatValue[(int)StatsIndex.Dexterity];
        Heroes.CurrentHero.IntellectFromArmorSet = StatValue[(int)StatsIndex.Intellect];
    }

    private void UpdateCurrentStatValueText(int stat_index, float value)
    {
        if (stat_index == (int)StatsIndex.HealthRegeneration ||
            stat_index == (int)StatsIndex.WillRegeneration ||
            stat_index == (int)StatsIndex.StaminaRegeneration)
            { CurrentStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value)}/сек."; }
        else if (stat_index == (int)StatsIndex.Evasion)
            { CurrentStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value * 100)}%"; }
        else { CurrentStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value)}"; }
    }

    private void UpdateFutureStatValueText(int stat_index, float value)
    {
        if (stat_index == (int)StatsIndex.HealthRegeneration ||
            stat_index == (int)StatsIndex.WillRegeneration ||
            stat_index == (int)StatsIndex.StaminaRegeneration)
        { FutureStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value)}/сек."; }
        else if (stat_index == (int)StatsIndex.Evasion)
            { FutureStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value * 100)}%"; }
        else { FutureStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value)}"; }
    }

    private void UpdateCostText(int stat_index, float value)
    {
        UpgradeCostText[stat_index].text = $"{ValuesRounding.FormattingValue("", "", value)} веры";
    }

    private void UpdateUpgradeCountText(int stat_index, float current_level)
    {
        UpgradeCountText[stat_index].text = $"{current_level}/{_maxUpgradeCount}";
    }

    private void UpdateUlockCostText()
    {
        _ulockCostText.text = $"{_armorUnlockCost} веры";
    }

    private void SetMessageMaxLevel(int stat_index)
    {
        UpgradeCostText[stat_index].text = "Макс.уровень";
        _arrow[stat_index].SetActive(false);
        FutureStatValueText[stat_index].gameObject.SetActive(false);
    }

    private void DisplayCurrentQuality()
    {
        int maxUpgradeCountForCurrentWeapon = 0;
        int summUpgradeCount = 0;

        foreach (float stat in StatValue)
        {
            if (stat != 0)
            {
                maxUpgradeCountForCurrentWeapon++;
            }
        }
        maxUpgradeCountForCurrentWeapon *= _maxUpgradeCount;

        foreach (int count in CurrentUpgradeCount)
        {
            if (count != 0)
            {
                summUpgradeCount += count;
            }
        }

        Heroes.SetQuality(_headerImage, _qualityText, summUpgradeCount, maxUpgradeCountForCurrentWeapon);
    }
}