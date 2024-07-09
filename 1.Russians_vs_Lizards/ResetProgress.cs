using TMPro;
using UnityEngine;
using YG;

public class ResetProgress : DataStructure
{
    [SerializeField] private TextMeshProUGUI _ancestralPowerSummText;
    [SerializeField] private TextMeshProUGUI _ADRewardText;
    [SerializeField] private TextMeshProUGUI _minStageMessageText;
    [SerializeField] private TextMeshProUGUI _summFaithMultiplierText;

    private Animator _animator;
    private int _minRequiredStage = 10;
    private const float _multiplier = 1.015f;
    private const int _startStageReward = 10;
    private float _ancestralPowerReward;
    private float _stageReward = _startStageReward;

    private bool _windowIsOpen = false;

    private void Awake()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        CalcReward();
        CalcMinRequiredStage();
        _summFaithMultiplierText.text = $"Суммарный множитель веры = {ValuesRounding.FormattingValue("", "", Facilities.FaithMultiplier * 100)}%";
        _minStageMessageText.text = $"Минимальная требуемая полянка: {_minRequiredStage}";
        _ancestralPowerSummText.text = $"Ты получишь <color=red>" +
            $"{ValuesRounding.ExtendedAccuracyFormattingValue("", "", _ancestralPowerReward)}</color> силы предков" +
            $"и <color=green>{ValuesRounding.FormattingValue("+", "", (_ancestralPowerReward / 100) * 2)}%</color> к множителю веры";
        _ADRewardText.text = $"{GlobalUpgrades.ADRewardMultiplier}X";

        YandexGame.RewardVideoEvent += ADReward;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= ADReward;
    }

    public void SwitchActiveMenu()
    {
        _windowIsOpen = gameObject.activeInHierarchy;

        if (!_windowIsOpen)
        {
            gameObject.SetActive(true);
            _animator.SetTrigger("Open");
        }
        else _animator.SetTrigger("Close");

        _windowIsOpen = !_windowIsOpen;
    }

    public void GetAverageReward()
    {
        if (_minRequiredStage <= Battle.MaxOpenStage)
        {
            ProgressReset();
            GetMoneyAnimation.CreateAndAddCoins(_ancestralPowerReward, "AncestralPower");
            Facilities.FaithMultiplier += ((_ancestralPowerReward / 100) / 100) * 2;
            _animator.SetTrigger("Close");
        }
    }

    public void GetADReward()
    {
        if (_minRequiredStage <= Battle.MaxOpenStage)
        {
            YandexGame.RewVideoShow((int)Game.RewardIndex.PrestigeReward);
        }
    }

    private void ADReward(int i)
    {
        if (i == (int)Game.RewardIndex.PrestigeReward)
        {
            Game.AccumulateWatchedAD();

            ProgressReset();
            GetMoneyAnimation.CreateAndAddCoins(_ancestralPowerReward * GlobalUpgrades.ADRewardMultiplier, "AncestralPower");
            Facilities.FaithMultiplier += (((_ancestralPowerReward * GlobalUpgrades.ADRewardMultiplier) / 100) / 100) * 2;
            _animator.SetTrigger("Close");

            SaveAndLoad.SavePlayerData();
        }
    }

    private void CalcMinRequiredStage()
    {
        _minRequiredStage = 10 + (Battle.ResetCount * 5);
    }

    private void CalcReward()
    {
        _ancestralPowerReward = 0;
        _stageReward = _startStageReward;
        _stageReward += 0.1f * Battle.MaxOpenStage;

        for (int i = 0; i < Battle.MaxOpenStage; i++)
        {
            _ancestralPowerReward += _stageReward;
            _stageReward *= _multiplier;

            if (i % 5 == 0)
                _stageReward += 5;
        }
    }

    private void ProgressReset()
    {
        StagesMenu.DisablePrestigeAttentionSign();
        Battle.ResetCount++;

        Facilities.FaithCurrency = 0;

        Battle.CurrentStage = 0;
        Battle.MaxOpenStage = 0;

        #region Skills
        Skills.ResetProgress();
        #endregion

        #region Armors
        ArmorMark_1.SwitchUnlockState(true);
        ArmorMark_2.SwitchUnlockState(true);
        ArmorMark_3.SwitchUnlockState(true);
        ArmorMark_1.SetStartStats();
        ArmorMark_2.SetStartStats();
        ArmorMark_3.SetStartStats();
        #endregion

        #region Weapons
        Weapons.SetStartWeaponsStats();
        Weapons.DisplayAllWeaponInfo();

        for (int i = 0; i < Weapons.WeaponCount; i++)
        {
            Weapons._Weapon[i].SwitchLockState(false);
            Weapons._Weapon[i].BackgroundImage.color = Heroes.HighlightColor_Deselect;
        }
        #endregion

        #region Heroes
        float[] BaseStr = new float[Heroes.HeroCount];
        float[] BaseDex = new float[Heroes.HeroCount];
        float[] BaseInt = new float[Heroes.HeroCount];

        for (int i = 0; i < Heroes.HeroCount; i++)
        {
            BaseStr[i] = Heroes.hero[i].StrengthBase;
            BaseDex[i] = Heroes.hero[i].DexterityBase;
            BaseInt[i] = Heroes.hero[i].IntellectBase;

            Heroes.hero[i].ResetArmorStats();
            Heroes.hero[i].ResetWeaponStats();
            Heroes.hero[i].ResetOtherStats();

            if (i != 0)
            {
                Heroes.hero[i].RedactHeroCardToLockState();
            }
        }

        Heroes.SetStartValuesFromHeroes();

        for (int i = 0; i < Heroes.HeroCount; i++)
        {
            Heroes.hero[i].StrengthBase = BaseStr[i];
            Heroes.hero[i].DexterityBase = BaseDex[i];
            Heroes.hero[i].IntellectBase = BaseInt[i];
            Heroes.hero[i].SetActualStocksToMAX();
        }

        Heroes.ChooseHero(0);
        Heroes.UpdateAllHeroPriceValues();
        BattleHero.UpdateAllInfo();
        #endregion
    }
}
