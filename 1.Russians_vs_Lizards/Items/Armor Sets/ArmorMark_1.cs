using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ArmorMark_1 : DataStructure
{
    private enum StatsIndex
    {
        MaxHealth = 0,
        HealthRegeneration = 1,
        Armor = 2
    }

    [Header("Characteristic")]
    [Space(5)] [NonSerialized] public float[] StatValue = new float[StatsCount];
    [Space(5)] [NonSerialized] public float[] StatUpgradeCost = new float[StatsCount];
    [NonSerialized] public int[] CurrentUpgradeCount = new int[StatsCount];
    [NonSerialized] public bool Bought = false;
    [NonSerialized] public bool Selected = false;
    public static readonly int StatsCount = 3;
    private const int _armorUnlockCost = 2500;
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

    private void FindComponents()
    {
        RectTransform[] children = gameObject.GetComponentsInChildren<RectTransform>(includeInactive: true);

        foreach (RectTransform tmp in children)
        {
            switch (tmp.name)
            {
                case ("BuyAmmunitionButton"):
                    _buyButtonObject = tmp.gameObject;
                    break;
                case ("Lock"):
                    _lock = tmp.gameObject;
                    break;
                case ("Armor_Button|Background"):
                    _backgroundWindow = tmp.gameObject;
                    break;
                case ("CostValue"):
                    _ulockCostText = tmp.GetComponent<TextMeshProUGUI>();
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
                case ("Armor"):
                    FindComponentsInComponents(tmp, (int)StatsIndex.Armor);
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
                    case ("UpgradeButton"):
                        _upgradeButton[stat_index] = tmp.GetComponent<Button>();
                        break;
                    case ("UpgradeCount"):
                        UpgradeCountText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
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

    public void SetStartStats()
    {
        Bought = false;
        Selected = false;

        for (int i = 0; i < StatsCount; i++)
            CurrentUpgradeCount[i] = 0;

        StatValue[(int)StatsIndex.MaxHealth] = 150;
        StatUpgradeCost[(int)StatsIndex.MaxHealth] = 50;

        StatValue[(int)StatsIndex.HealthRegeneration] = 1.5f;
        StatUpgradeCost[(int)StatsIndex.HealthRegeneration] = 50;

        StatValue[(int)StatsIndex.Armor] = 5;
        StatUpgradeCost[(int)StatsIndex.Armor] = 50;

        DisplayAllInfo();
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
                case (int)StatsIndex.Armor:
                    Heroes.CurrentHero.ArmorFromArmorSet = StatValue[stat_index];
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

            if (Achievements_R_vs_L.ArmorIsUnlocked[0] == false)
            {
                Achievements_R_vs_L.ArmorIsUnlocked[0] = true;
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
        if (ArmorMark_2.Selected)
            ArmorMark_2.DeselectArmorSet();
        if (ArmorMark_3.Selected)
            ArmorMark_3.DeselectArmorSet();

        if (Bought)
        {
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
        Heroes.CurrentHero.ArmorFromArmorSet = StatValue[(int)StatsIndex.Armor];
    }

    private void UpdateCurrentStatValueText(int stat_index, float value)
    {
        if (stat_index != (int)StatsIndex.HealthRegeneration)
            CurrentStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value)}";
        else CurrentStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value)}/сек.";
    }

    private void UpdateFutureStatValueText(int stat_index, float value)
    {
        if (stat_index != (int)StatsIndex.HealthRegeneration)
            FutureStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value)}";
        else FutureStatValueText[stat_index].text = $"+{ValuesRounding.FormattingValue("", "", value)}/сек.";
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