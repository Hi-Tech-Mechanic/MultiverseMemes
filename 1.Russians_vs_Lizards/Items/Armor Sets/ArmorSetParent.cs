//using UnityEngine;
//using TMPro;
//using System;
//using UnityEngine.UI;

//public class ArmorSetParent : DataStructure
//{
//    private enum StatsIndex
//    {
//        MaxHealth = 0,
//        HealthRegeneration = 1,
//        MaxStamina = 2,
//        StaminaRegeneration = 3,
//        MaxWill = 4,
//        WillRegeneration = 5,
//        Damage = 6,
//        Armor = 7,
//        Evasion = 8,
//        Strength = 9,
//        Dexterity = 10,
//        Intellect = 11
//    }

//    [Serializable]
//    public struct Armors
//    {
//        [Header("Characteristic")]
//        [Space(5)]
//        public GameObject ArmorObject;
//        public int ArmorUnlockCost;
//        public float[] StatValue;
//        public float[] StatUpgradeCost;
//        public int _statsCount;
//        private int[] _currentUpgradeCount;
//        private const int _maxUpgradeCount = 20;
//        private const float _additivePercent = 1.1f;
//        private bool _bought;
//        private bool _selected;
//        private TextMeshProUGUI[] UpgradeCostText;
//        private TextMeshProUGUI[] CurrentStatValueText;
//        private TextMeshProUGUI[] FutureStatValueText;
//        private TextMeshProUGUI[] UpgradeCountText;
//        private Button[] _upgradeButton;
//        private GameObject[] _arrow;
//        private TextMeshProUGUI _ulockCostText;
//        private GameObject _buyButtonObject;
//        private GameObject _lock;
//        private GameObject _backgroundWindow;

//        private void InitializeFields()
//        {
//            UpgradeCostText = new TextMeshProUGUI[_statsCount];
//            CurrentStatValueText = new TextMeshProUGUI[_statsCount];
//            FutureStatValueText = new TextMeshProUGUI[_statsCount];
//            UpgradeCountText = new TextMeshProUGUI[_statsCount];
//            _upgradeButton = new Button[_statsCount];
//            _arrow = new GameObject[_statsCount];

//            _currentUpgradeCount = new int[_statsCount];
//            StatUpgradeCost = new float[_statsCount];
//            StatValue = new float[_statsCount];
//        }

//        private void FindComponents()
//        {
//            RectTransform[] children = ArmorObject.GetComponentsInChildren<RectTransform>(includeInactive: true);

//            foreach (var tmp in children)
//            {
//                switch (tmp.name)
//                {
//                    case ("BuyAmmunitionButton"):
//                        _buyButtonObject = tmp.gameObject;
//                        break;
//                    case ("CostValue"):
//                        _ulockCostText = tmp.GetComponent<TextMeshProUGUI>();
//                        break;
//                    case ("Lock"):
//                        _lock = tmp.gameObject;
//                        break;
//                    case ("Armor_Button|Background"):
//                        _backgroundWindow = tmp.gameObject;
//                        break;
//                    case ("MaxHealth"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.MaxHealth);
//                        break;
//                    case ("HealthRegeneration"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.HealthRegeneration);
//                        break;
//                    case ("MaxStamina"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.MaxStamina);
//                        break;
//                    case ("StaminaRegeneration"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.StaminaRegeneration);
//                        break;
//                    case ("MaxWill"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.MaxWill);
//                        break;
//                    case ("WillRegeneration"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.WillRegeneration);
//                        break;
//                    case ("Damage"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.Damage);
//                        break;
//                    case ("Armor"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.Armor);
//                        break;
//                    case ("Evasion"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.Evasion);
//                        break;
//                    case ("Strength"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.Strength);
//                        break;
//                    case ("Dexterity"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.Dexterity);
//                        break;
//                    case ("Intellect"):
//                        FindComponentsInComponents(tmp, (int)StatsIndex.Intellect);
//                        break;
//                    default: break;
//                }
//            }
//        }

//        private void FindComponentsInComponents(RectTransform element, int stat_index)
//        {
//            RectTransform[] arr_2 = element.gameObject.GetComponentsInChildren<RectTransform>(includeInactive: true);

//            foreach (RectTransform tmp in arr_2)
//            {
//                switch (tmp.name)
//                {
//                    case ("BackgroundArrow"):
//                        _arrow[stat_index] = tmp.gameObject;
//                        break;
//                    case ("UpgradeCost"):
//                        UpgradeCostText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
//                        break;
//                    case ("Value"):
//                        CurrentStatValueText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
//                        break;
//                    case ("PostUpgradeValue"):
//                        FutureStatValueText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
//                        break;
//                    case ("UpgradeButton"):
//                        _upgradeButton[stat_index] = tmp.GetComponent<Button>();
//                        break;
//                    case ("UpgradeCount"):
//                        UpgradeCountText[stat_index] = tmp.GetComponent<TextMeshProUGUI>();
//                        break;
//                    default: break;
//                }
//            }
//        }

//        private void StartDisplaying()
//        {
//            for (int stat_index = 0; stat_index < _statsCount; stat_index++)
//            {
//                UpdateCurrentStatValueText(stat_index, StatValue[stat_index]);
//                UpdateFutureStatValueText(stat_index, StatValue[stat_index] * _additivePercent);
//                UpdateCostText(stat_index, StatUpgradeCost[stat_index]);
//                UpdateUpgradeCountText(stat_index, _currentUpgradeCount[stat_index]);
//                _arrow[stat_index].SetActive(false);
//                FutureStatValueText[stat_index].gameObject.SetActive(false);
//            }
//            UpdateUlockCostText();
//        }

//        private void UpdateCurrentStatValueText(int stat_index, float value)
//        {
//            if (stat_index != (int)StatsIndex.HealthRegeneration)
//                CurrentStatValueText[stat_index].text = GetRoundedValue(value, 0);
//            else CurrentStatValueText[stat_index].text = GetRoundedValue(value, 2) + "/сек.";
//        }

//        private void UpdateFutureStatValueText(int stat_index, float value)
//        {
//            if (stat_index != (int)StatsIndex.HealthRegeneration)
//                FutureStatValueText[stat_index].text = GetRoundedValue(value, 0);
//            else FutureStatValueText[stat_index].text = GetRoundedValue(value, 2) + "/сек.";
//        }

//        private void UpdateCostText(int stat_index, float value)
//        {
//            UpgradeCostText[stat_index].text = GetRoundedValue(value, 0) + " веры";
//        }

//        private void UpdateUpgradeCountText(int stat_index, float current_level)
//        {
//            UpgradeCountText[stat_index].text = $"{current_level}/{_maxUpgradeCount}";
//        }

//        private void UpdateUlockCostText()
//        {
//            _ulockCostText.text = $"{ArmorUnlockCost} веры";
//        }

//        private void SetMessageMaxLevel(int stat_index)
//        {
//            UpgradeCostText[stat_index].text = "Макс.уровень";
//            _arrow[stat_index].SetActive(false);
//            FutureStatValueText[stat_index].gameObject.SetActive(false);
//        }

//        private string GetRoundedValue(float value, int decimal_places)
//        {
//            string message;

//            message = $"+{Math.Round(value, decimal_places)}";
//            return message;
//        }
//    }
//    Armors[] ArmorBuffer = new Armors[3];

//    private void Awake()
//    {
//        //SetStatsCount();
//        InitializeFields();
//        FindComponents();
//        StartDisplaying();
//    }

//    //protected virtual void SetStatsCount()
//    //{ _statsCount = 12; }

//    public void IncreaseStat(int stat_index)
//    {
//        if (Facilities.FaithCurrency >= ArmorBuffer[].StatUpgradeCost[stat_index])
//        {
//            if (_currentUpgradeCount[stat_index] != _maxUpgradeCount)
//            {
//                AudioEffects.PlayUpgradeEffect();
//                Facilities.FaithCurrencyPay(StatUpgradeCost[stat_index]);
//                _currentUpgradeCount[stat_index]++;
//                StatValue[stat_index] *= _additivePercent;
//                StatUpgradeCost[stat_index] *= _additivePercent;
//                UpdateCurrentStatValueText(stat_index, StatValue[stat_index]);
//                UpdateFutureStatValueText(stat_index, StatValue[stat_index] * _additivePercent);
//                UpdateCostText(stat_index, StatUpgradeCost[stat_index]);
//                UpdateUpgradeCountText(stat_index, _currentUpgradeCount[stat_index]);

//                if (_currentUpgradeCount[stat_index] == _maxUpgradeCount)
//                { SetMessageMaxLevel(stat_index); }
//            }
//        }
//        if (_selected)
//        {
//            switch (stat_index)
//            {
//                case (int)StatsIndex.MaxHealth:
//                    Heroes.CurrentHero.MaxHealthFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.HealthRegeneration:
//                    Heroes.CurrentHero.HealthRegenerationFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.MaxStamina:
//                    Heroes.CurrentHero.MaxStaminaFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.StaminaRegeneration:
//                    Heroes.CurrentHero.StaminaRegenerationFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.MaxWill:
//                    Heroes.CurrentHero.MaxWillFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.WillRegeneration:
//                    Heroes.CurrentHero.WillRegenerationFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Damage:
//                    Heroes.CurrentHero.DamageFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Armor:
//                    Heroes.CurrentHero.ArmorFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Evasion:
//                    Heroes.CurrentHero.EvasionFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Strength:
//                    Heroes.CurrentHero.StrengthFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Dexterity:
//                    Heroes.CurrentHero.DexterityFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Intellect:
//                    Heroes.CurrentHero.IntellectFromArmorSet = StatValue[stat_index];
//                    break;
//            }
//        }
//    }

//    public void UnlockArmorSet()
//    {
//        if (ArmorUnlockCost <= Facilities.FaithCurrency)
//        {
//            AudioEffects.PlayUpgradeEffect();
//            Facilities.FaithCurrencyPay(ArmorUnlockCost);
//            _bought = true;
//            _buyButtonObject.gameObject.SetActive(false);
//            _lock.gameObject.SetActive(false);
//            _backgroundWindow.GetComponent<Button>().interactable = true;

//            for (int i = 0; i < _statsCount; i++)
//            {
//                _upgradeButton[i].interactable = true;
//                _arrow[i].SetActive(true);
//                FutureStatValueText[i].gameObject.SetActive(true);
//            }
//        }
//    }

//    public void SwitchChoose()
//    {
//        if (_bought)
//        {
//            if (!_selected) SelectArmorSet();
//            else DeselectArmorSet();

//            _selected = !_selected;
//        }
//    }

//    private void SelectArmorSet()
//    {
//        int hero_stat_index = 0;

//        AudioEffects.PlayArsenalItemSelect();
//        _backgroundWindow.GetComponent<Image>().color = Heroes.HighlightColor_Select;
//        SetCharacteriscticsValues();

//        foreach (int stat_index in Enum.GetValues(typeof(StatsIndex)))
//        {
//            switch (stat_index)
//            {
//                case (int)StatsIndex.MaxHealth:
//                    hero_stat_index = (int)Heroes.StatsIndex.MaxHealth;
//                    Heroes.CurrentHero.MaxHealthFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.HealthRegeneration:
//                    Heroes.CurrentHero.HealthRegenerationFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.MaxStamina:
//                    Heroes.CurrentHero.MaxStamina = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.StaminaRegeneration:
//                    Heroes.CurrentHero.StaminaRegeneration = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.MaxWill:
//                    Heroes.CurrentHero.MaxWill = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.WillRegeneration:
//                    Heroes.CurrentHero.WillRegeneration = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Damage:
//                    Heroes.CurrentHero.Damage = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Armor:
//                    Heroes.CurrentHero.ArmorFromArmorSet = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Evasion:
//                    Heroes.CurrentHero.Evasion = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Strength:
//                    Heroes.CurrentHero.Strength = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Dexterity:
//                    Heroes.CurrentHero.Dexterity = StatValue[stat_index];
//                    break;
//                case (int)StatsIndex.Intellect:
//                    Heroes.CurrentHero.Intellect = StatValue[stat_index];
//                    break;
//            }            
//        }

//        Heroes.CurrentHero.CalcSet_MaxHealth();
//        Heroes.CurrentHero.CalcSet_HealthRegeneration();
//        Heroes.CurrentHero.CalcSet_Armor();
//    }

//    private void DeselectArmorSet()
//    {
//        if (_selected == true)
//        {
//            AudioEffects.PlayButtonClickEffect();
//            _backgroundWindow.GetComponent<Image>().color = Heroes.HighlightColor_Deselect;
//            Heroes.StatsFromArmorSet[(int)Heroes.StatsIndex.MaxHealth] = 0;
//            Heroes.StatsFromArmorSet[(int)Heroes.StatsIndex.HealthRegeneration] = 0;
//            Heroes.StatsFromArmorSet[(int)Heroes.StatsIndex.Armor] = 0;

//            Heroes.CurrentHero.CalcSet_MaxHealth();
//            Heroes.CurrentHero.CalcSet_HealthRegeneration();
//            Heroes.CurrentHero.CalcSet_Armor();
//        }
//    }

//    private void SetCharacteriscticsValues()
//    {
//        Heroes.StatsFromArmorSet[(int)Heroes.StatsIndex.MaxHealth] = StatValue[(int)StatsIndex.MaxHealth];
//        Heroes.StatsFromArmorSet[(int)Heroes.StatsIndex.HealthRegeneration] = StatValue[(int)StatsIndex.HealthRegeneration];
//        Heroes.StatsFromArmorSet[(int)Heroes.StatsIndex.Armor] = StatValue[(int)StatsIndex.Armor];
//    }
//}
