using UnityEngine;
using YG;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class SaveAndLoad : DataStructure
{
    public static int CurrentSaveMode = (int)SaveMode.YGSave;

    public static bool IsSavingData_1 = false;
    public static bool IsSavingData_2 = false;

    [SerializeField] private Button _saveButton;
    [SerializeField] private Color _savedColor;
    [SerializeField] private Color _loadingColor;

    private TextMeshProUGUI _saveButtonText;
    private Color _standardColor;
    private Action OnSaved;

    private const float _timeToSave = 30;
    private float _timeToSaveCounter = _timeToSave;

    public enum SaveMode
    {
        BinarySave = 0,
        YGSave = 1 //Надо переделывать архитектуру прогрузки всех скриптов для его использования
    }

    private void Awake()
    {
        _saveButtonText = _saveButton.GetComponentInChildren<TextMeshProUGUI>();
        _standardColor = _saveButton.image.color;
    }

    private void OnEnable() => OnSaved += DisplayCurrentSaveState;

    private void OnDisable() => OnSaved -= DisplayCurrentSaveState;

    private void OnApplicationQuit() => SavePlayerData();

    #region Main
    public void AutoSave()
    {
        if (_timeToSaveCounter > 0)
        {
            _timeToSaveCounter -= Time.deltaTime;
        }
        else
        {
            _timeToSaveCounter = _timeToSave;
            SavePlayerData();
        }
    }

    public void SavePlayerData()
    {
        if (StartLearning.StartLessonIsComplete == true)
        {
            if (CurrentSaveMode == (int)SaveMode.YGSave)
            {
                SavePlayerDataYG();
            }
            else if (CurrentSaveMode == (int)SaveMode.BinarySave)
            {
                SaveBinaryPlayerData();
            }

            Game.AddOrUpdateMainLidearboard();
        }
    }

    public void ResetProgress() //Пока не надо
    {
        YandexGame.GetDataEvent -= SaveAndLoad.CheckSaveYG;
        YandexGame.ResetSaveProgress();

        if (YandexGame.savesData.StartLessonIsComplete)
        {
            YandexGame.savesData.StartLessonIsComplete = true;
        }
        if (YandexGame.savesData.RussiansLessonIsFinished)
        {
            YandexGame.savesData.RussiansLessonIsFinished = true;
        }

        YandexGame.savesData.FirstLaunchGame = false;
        YandexGame.SaveProgress();
        CheckSaveYG();

        Debug.Log("After reset MoneyCapital[0] = " + MoneyMenu.GetMoneyCapital()[0]);
        Debug.Log("First launch game = " + Game.FirstLaunchGame);
    }
    #endregion

    #region YandexGameData
    private void SavePlayerDataYG()
    {
        SavePlayerDataYG_1();

        if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
        {
            SavePlayerDataYG_2();
        }
    }

    public void CheckSaveYG()
    {
        Debug.Log("CheckSaveYG");

        if (CurrentSaveMode == (int)SaveMode.YGSave)
        {
            if (YandexGame.SDKEnabled)
            {
                Debug.Log("SDKEnabled");

                if (YandexGame.savesData.FirstLaunchGame == false)
                {
                    LoadPlayerData_1();

                    if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
                    {
                        if (Game.FirstLaunchScene[(int)Game.BuildIndex.Russians_vs_Lizards] == false)
                            LoadPlayerData_2();
                    }
                }

                InitializeData_1();
            }
        }
    }
    #endregion

    #region BinaryData
    private void SaveBinaryPlayerData()
    {
        SaveSystem.SavePlayerData_1(MoneyMenu, Upgrades, GlobalUpgrades, MemeShop, Music, AudioEffects);

        if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
        {
            SaveSystem.SavePlayerData_2(Heroes, ArmorMark_1, ArmorMark_2, ArmorMark_3,
                Weapons, PerkTree, Skills, Facilities, Achievements_R_vs_L, Battle, Items, ListOfEffects);
        }

        if (IsSavingData_1 && IsSavingData_2 == false)
        {
            Debug.Log("SaveBinaryPlayerData Complete");
        }
    }

    public void CheckBinarySave()
    {
        if (File.Exists(SaveSystem.Path_1) != false)
        {
            Debug.Log("LoadIsCorrect, path = " + SaveSystem.Path_1);
            LoadPlayerData_1();
        }
        else Debug.Log("Save file not found in" + SaveSystem.Path_1);

        if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
        {
            if (File.Exists(SaveSystem.Path_2) != false)
            {
                Debug.Log("LoadIsCorrect, path_2");
                LoadPlayerData_2();
            }
            else Debug.Log("Save file not found in" + SaveSystem.Path_2);
        }
    }
    #endregion

    private void SavePlayerDataYG_1()
    {
        #region System
        FirstLaunchGameInitialize();

        Game.FirstLaunchScene[Game.CurrentScene] = false;
        YandexGame.savesData.FirstLaunchScene = Game.FirstLaunchScene;

        YandexGame.savesData.LastPlayingTime = Game.GetLastPlayingTime();
        YandexGame.savesData.LastLaunchGameTime = Game.LastLaunchGameTime;
        YandexGame.savesData.VisitingDays = Game.VisitingDays;

        YandexGame.savesData.StartLessonIsComplete = StartLearning.StartLessonIsComplete;
        YandexGame.savesData.DisclaimerIsWathced = Disclaimer.DisclaimerIsWathced;
        YandexGame.savesData.MaxCellCountInGame = Game.MaxCellCountInGame;
        #endregion

        #region MoneyMenu
        YandexGame.savesData.MemeCoins = MoneyMenu.GetMemeCoins();
        YandexGame.savesData.MoneyCapital = MoneyMenu.GetMoneyCapital();
        YandexGame.savesData.ClickIncome = MoneyMenu.GetClickIncome();
        YandexGame.savesData.TickIncome = MoneyMenu.GetIdleIncomePerTick();
        YandexGame.savesData.AccumulatedMoney = MoneyMenu.AccumulatedMoney;
        #endregion

        #region GlobalUpgrades
        YandexGame.savesData.UpgradeCostMultiplier = GlobalUpgrades.UpgradeCostMultiplier;
        YandexGame.savesData.IncomeMultiplier = GlobalUpgrades.IncomeMultiplier;
        YandexGame.savesData.TickTime = GlobalUpgrades.TickTime;
        YandexGame.savesData.CritChance = GlobalUpgrades.CritChance;
        YandexGame.savesData.CritStrength = GlobalUpgrades.CritStrength;
        YandexGame.savesData.ADRewardMultiplier = GlobalUpgrades.ADRewardMultiplier;
        YandexGame.savesData.Prices = GlobalUpgrades.Prices;
        YandexGame.savesData.PurchasedCounter = GlobalUpgrades.PurchasedCounter;
        YandexGame.savesData.AbsenceTime = GlobalUpgrades.AbsenceTime;
        YandexGame.savesData.AdvertisingBuffTime = GlobalUpgrades.AdvertisingBuffTime;
        YandexGame.savesData.MemeClipCount = GlobalUpgrades.MemeClipCount;
        #endregion

        #region Buffs
        YandexGame.savesData.ClickBuffTimeRemaining = ClickADReward.BuffTimeRemaining;
        YandexGame.savesData.IdleBuffTimeRemaining = IdleADReward.BuffTimeRemaining;
        YandexGame.savesData.ClickCachedADRewardMultiplier = ClickADReward.CachedADMultiplier;
        YandexGame.savesData.IdleCachedADRewardMultiplier = IdleADReward.CachedADMultiplier;
        #endregion

        #region Achievements
        YandexGame.savesData.AchievementsRewardsIsClaimed = GlobalUpgrades.AchievementsRewardsIsClaimed;
        YandexGame.savesData.CurrentUniverseProgress = Multiverse.CurrentUniverseProgress;
        YandexGame.savesData.AccumulatedTime = Game.AccumulatedTime;
        YandexGame.savesData.AccumulatedClicks = Game.AccumulatedClicks;
        YandexGame.savesData.PurchasedMemeClips = MemeShop.PurchasedMemeClips;
        #endregion

        #region Upgrades
        YandexGame.savesData.AccumulatedPurchasedClickUpgrades = Upgrades.AccumulatedPurchasedClickUpgrades;
        YandexGame.savesData.AccumulatedPurchasedIdleUpgrades = Upgrades.AccumulatedPurchasedIdleUpgrades;

        YandexGame.savesData._Click.Income = Upgrades._Click.Income;
        YandexGame.savesData._Click.SummIncome = Upgrades._Click.SummIncome;
        YandexGame.savesData._Click.Prices = Upgrades._Click.Prices;
        YandexGame.savesData._Click.PurchasedUpgradeAmount = Upgrades._Click.PurchasedUpgradeAmount;
        YandexGame.savesData._Click.CurrentBuyAmount = Upgrades._Click.CurrentBuyAmount;
        YandexGame.savesData._Click.ReceivedLevels = Upgrades._Click.ReceivedLevels;

        YandexGame.savesData._Idle.Income = Upgrades._Idle.Income;
        YandexGame.savesData._Idle.SummIncome = Upgrades._Idle.SummIncome;
        YandexGame.savesData._Idle.Prices = Upgrades._Idle.Prices;
        YandexGame.savesData._Idle.PurchasedUpgradeAmount = Upgrades._Idle.PurchasedUpgradeAmount;
        YandexGame.savesData._Idle.CurrentBuyAmount = Upgrades._Idle.CurrentBuyAmount;
        YandexGame.savesData._Idle.ReceivedLevels = Upgrades._Idle.ReceivedLevels;
        #endregion

        #region MemeShop
        YandexGame.savesData.SelectedClipCount = MemeShop.SelectedClipCount;
        YandexGame.savesData.ClipIsSelected = MemeShop.ClipIsSelected;
        YandexGame.savesData.ClipIsAvailable = MemeShop.ClipIsAvailable;
        YandexGame.savesData.ClipIsSelected_2 = CombinedUniverse.MemeShop.ClipIsSelected;
        #endregion

        #region Calendar
        YandexGame.savesData.CalendarRewardIsClaimed = Calendar.CalendarRewardIsClaimed;
        YandexGame.savesData.RewardIsUnlock = Calendar.RewardIsUnlock;
        #endregion

        #region CasesMenu
        if (CheckNull(CasesMenu))
        {
            YandexGame.savesData.OpenedCases = CasesMenu.OpenedCases;
            YandexGame.savesData.CasesCount = CasesMenu.CasesCount;
            YandexGame.savesData.WatchedVideosInCurrentDay = CasesMenu.WatchedVideosInCurrentDay;
            YandexGame.savesData.EverydayCaseIsTaken = CasesMenu.EverydayCaseIsTaken;
        }
        #endregion

        #region Options
        YandexGame.savesData.CurrentImageIndex = SwitchBackground.CurrentImageIndex;
        YandexGame.savesData.AudioIsOn = MainAudio.AudioIsOn;

        YandexGame.savesData.SelectedMusicIndex = Music.SelectedMusicIndex;
        YandexGame.savesData.CurrentMusicTime = Music.CurrentMusicTime;
        YandexGame.savesData.MusicVolume = Music.MusicVolume;
        YandexGame.savesData.MusicPitch = Music.MusicPitch;
        YandexGame.savesData.MusicIsMute = Music.MusicIsMute;

        YandexGame.savesData.AudioEffectsVolume = AudioEffects.AudioEffectsVolume;
        YandexGame.savesData.AudioEffectsPitch = AudioEffects.AudioEffectsPitch;
        YandexGame.savesData.AudioEffectsIsMute = AudioEffects.AudioEffectsIsMute;
        YandexGame.savesData.IdleAudioEffectIsOn = AudioEffects.IdleAudioEffectsIsOn;
        YandexGame.savesData.AudioEffectsIsLayering = AudioEffects.AudioEffectsIsLayering;
        #endregion

        YandexGame.SaveProgress();
        OnSaved?.Invoke();

        void FirstLaunchGameInitialize()
        {
            if (Game.FirstLaunchGame == true)
            {
                Debug.Log("FIRST LAUNCH");
                Game.FirstLaunchGame = false;
                YandexGame.savesData.FirstLaunchGame = Game.FirstLaunchGame;
            }
        }
    }

    private void SavePlayerDataYG_2()
    {
        #region Battle
        YandexGame.savesData.EnemiesInStage = Battle.CurrentEnemiesInStage;
        YandexGame.savesData.MaxOpenStage = Battle.MaxOpenStage;
        YandexGame.savesData.ResetCount = Battle.ResetCount;

        if (Battle.BattleIsActive)
            YandexGame.savesData.CurrentStage = Battle.CurrentStage - 1;
        else YandexGame.savesData.CurrentStage = Battle.CurrentStage;
        #endregion

        #region Facilities
        YandexGame.savesData.FaithCurrency = Facilities.FaithCurrency;
        YandexGame.savesData.AncestralPower = Facilities.AncestralPower;
        YandexGame.savesData.FaithMultiplier = Facilities.FaithMultiplier;
        #endregion

        #region Achievements
        YandexGame.savesData.AccumulatedKills = Achievements_R_vs_L.AccumulatedKills;
        YandexGame.savesData.AccumulatedFaith = Achievements_R_vs_L.AccumulatedFaith;
        YandexGame.savesData.AccumulatedAncestralPower = Achievements_R_vs_L.AccumulatedAncestralPower;
        YandexGame.savesData.AccumulatedHeroes = Achievements_R_vs_L.AccumulatedHeroes;
        YandexGame.savesData.AccumulatedSkills = Achievements_R_vs_L.AccumulatedSkills;
        YandexGame.savesData.AccumulatedArmors = Achievements_R_vs_L.AccumulatedArmors;
        YandexGame.savesData.AccumulatedWeapons = Achievements_R_vs_L.AccumulatedWeapons;
        YandexGame.savesData.AccumulatedPerks = Achievements_R_vs_L.AccumulatedPerks;
        YandexGame.savesData.AccumulatedStages = Achievements_R_vs_L.AccumulatedStages;
        #endregion

        #region Items
        YandexGame.savesData.BaikalWaterCount = Items._BaikalWater.Count;
        YandexGame.savesData.PotionsCount[0] = Items.HealthPotion.Count;
        YandexGame.savesData.PotionsCount[1] = Items.StaminaPotion.Count;
        YandexGame.savesData.PotionsCount[2] = Items.WillPotion.Count;
        #endregion

        #region Buffs
        YandexGame.savesData.AdditiveStatsFromBaikalWater = Items.AdditiveStats;
        YandexGame.savesData.BaikalWaterTimer = Items.Timer;

        YandexGame.savesData.CachedAdditiveStats_PrayerToTheGods = ListOfEffects.CachedAdditiveStats_PrayerToTheGods;
        YandexGame.savesData.CachedBuffTime_PrayerToTheGods = ListOfEffects.CachedBuffTime_PrayerToTheGods;
        YandexGame.savesData.CachedAdditiveStats_GeneralFee = ListOfEffects.CachedAdditiveStats_GeneralFee;
        YandexGame.savesData.CachedBuffTime_GeneralFee = ListOfEffects.CachedBuffTime_GeneralFee;
        #endregion

        #region Armors
        ArmorSetsInitialize();

        for (int i = 0; i < ArmorMark_1.StatsCount; i++)
        {
            YandexGame.savesData._ArmorSet[0].StatValue[i] = ArmorMark_1.StatValue[i];
            YandexGame.savesData._ArmorSet[0].StatUpgradeCost[i] = ArmorMark_1.StatUpgradeCost[i];
            YandexGame.savesData._ArmorSet[0].CurrentUpgradeCount[i] = ArmorMark_1.CurrentUpgradeCount[i];
            YandexGame.savesData._ArmorSet[0].Bought = ArmorMark_1.Bought;
            YandexGame.savesData._ArmorSet[0].Selected = ArmorMark_1.Selected;
        }
        for (int i = 0; i < ArmorMark_2.StatsCount; i++)
        {
            YandexGame.savesData._ArmorSet[1].StatValue[i] = ArmorMark_2.StatValue[i];
            YandexGame.savesData._ArmorSet[1].StatUpgradeCost[i] = ArmorMark_2.StatUpgradeCost[i];
            YandexGame.savesData._ArmorSet[1].CurrentUpgradeCount[i] = ArmorMark_2.CurrentUpgradeCount[i];
            YandexGame.savesData._ArmorSet[1].Bought = ArmorMark_2.Bought;
            YandexGame.savesData._ArmorSet[1].Selected = ArmorMark_2.Selected;
        }
        for (int i = 0; i < ArmorMark_3.StatsCount; i++)
        {
            YandexGame.savesData._ArmorSet[2].StatValue[i] = ArmorMark_3.StatValue[i];
            YandexGame.savesData._ArmorSet[2].StatUpgradeCost[i] = ArmorMark_3.StatUpgradeCost[i];
            YandexGame.savesData._ArmorSet[2].CurrentUpgradeCount[i] = ArmorMark_3.CurrentUpgradeCount[i];
            YandexGame.savesData._ArmorSet[2].Bought = ArmorMark_3.Bought;
            YandexGame.savesData._ArmorSet[2].Selected = ArmorMark_3.Selected;
        }
        #endregion

        #region Weapons
        for (int weaponIndex = 0; weaponIndex < Weapons.WeaponCount; weaponIndex++)
        {
            YandexGame.savesData._Weapons.Selected[weaponIndex] = Weapons._Weapon[weaponIndex].Selected;
            YandexGame.savesData._Weapons.Bought[weaponIndex] = Weapons._Weapon[weaponIndex].Bought;

            for (int statIndex = 0; statIndex < Weapons.WeaponStatsCount; statIndex++)
            {
                YandexGame.savesData._Weapons.StatsValue[weaponIndex, statIndex] = Weapons._Weapon[weaponIndex].StatsValue[statIndex];
                YandexGame.savesData._Weapons.CurrentUpgradeCount[weaponIndex, statIndex] = Weapons._Weapon[weaponIndex].CurrentUpgradeCount[statIndex];
                YandexGame.savesData._Weapons.StatsUpgradeCost[weaponIndex, statIndex] = Weapons._Weapon[weaponIndex].StatsUpgradeCost[statIndex];
            }
        }
        #endregion

        #region PerkTree
        int perkIndex;

        perkIndex = 0;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.BaseStrengthAdditional.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.BaseStrengthAdditional.CurrentUpgradeIndex;

        perkIndex = 1;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.BaseDexterityAdditional.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.BaseDexterityAdditional.CurrentUpgradeIndex;

        perkIndex = 2;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.BaseIntellectAdditional.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.BaseIntellectAdditional.CurrentUpgradeIndex;

        perkIndex = 3;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.ArithmeticStrengthAdditional.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.ArithmeticStrengthAdditional.CurrentUpgradeIndex;

        perkIndex = 4;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.ArithmeticDexterityAdditional.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.ArithmeticDexterityAdditional.CurrentUpgradeIndex;

        perkIndex = 5;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.ArithmeticIntellectAdditional.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.ArithmeticIntellectAdditional.CurrentUpgradeIndex;

        perkIndex = 6;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.GeometricStrengthAdditional.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.GeometricStrengthAdditional.CurrentUpgradeIndex;

        perkIndex = 7;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.GeometricDexterityAdditional.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.GeometricDexterityAdditional.CurrentUpgradeIndex;

        perkIndex = 8;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.GeometricIntellectAdditional.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.GeometricIntellectAdditional.CurrentUpgradeIndex;

        perkIndex = 9;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.MaxHealthFromStrength.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.MaxHealthFromStrength.CurrentUpgradeIndex;

        perkIndex = 10;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.MaxStaminaFromDexterity.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.MaxStaminaFromDexterity.CurrentUpgradeIndex;

        perkIndex = 11;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.MaxWillFromIntellect.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.MaxWillFromIntellect.CurrentUpgradeIndex;

        perkIndex = 12;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.HealthRegenerationFromStrength.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.HealthRegenerationFromStrength.CurrentUpgradeIndex;

        perkIndex = 13;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.StaminaRegenerationFromDexterity.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.StaminaRegenerationFromDexterity.CurrentUpgradeIndex;

        perkIndex = 14;
        YandexGame.savesData._PerkTree.Summ[perkIndex] = PerkTree.WillRegenerationFromIntellect.Summ;
        YandexGame.savesData._PerkTree.CurrentUpgradeIndex[perkIndex] = PerkTree.WillRegenerationFromIntellect.CurrentUpgradeIndex;
        #endregion

        #region Skills

        for (int i = 0; i < Skills.PlasesForSkillsCount; i++)
        {
            YandexGame.savesData.BoughtPlasesForSkills[i] = Skills.PlaseForSkillIsBought[i];
            YandexGame.savesData.SelectedSkillIndex[i] = Skills.SelectedSkillIndex[i];
            YandexGame.savesData.SavedReloadTimeForSelectedSkill[i] = Skills.SavedReloadTimeForSelectedSkill[i];
        }

        for (int i = 0; i < Skills.SkillsCount; i++)
        {
            YandexGame.savesData.BoughtSkills[i] = Skills.SkillIsBought[i];
            YandexGame.savesData.SelectedSkill[i] = Skills.SkillIsSelected[i];
        }

        YandexGame.savesData._BallClamp.StunDuration = Skills._BallClamp.StunDuration;
        YandexGame.savesData._BallClamp.Damage = Skills._BallClamp.Damage;
        YandexGame.savesData._BallClamp.ReloadTime = Skills._BallClamp.ReloadTime;
        YandexGame.savesData._BallClamp.WillCost = Skills._BallClamp.WillCost;
        YandexGame.savesData._BallClamp.CurrentPurchasePercent = Skills._BallClamp.CurrentPurchasePercent;
        YandexGame.savesData._BallClamp.SummCurrentPurchaseCount = Skills._BallClamp.SummCurrentPurchaseCount;
        for (int i = 0; i < YandexGame.savesData._BallClamp.Cost.Length; i++)
        {
            YandexGame.savesData._BallClamp.Cost[i] = Skills._BallClamp.Cost[i];
            YandexGame.savesData._BallClamp.PurchaseCount[i] = Skills._BallClamp.PurchaseCount[i];
        }

        YandexGame.savesData._ShootingGlance.Damage = Skills._ShootingGlance.Damage;
        YandexGame.savesData._ShootingGlance.PeriodicDamage = Skills._ShootingGlance.PeriodicDamage;
        YandexGame.savesData._ShootingGlance.Duration = Skills._ShootingGlance.Duration;
        YandexGame.savesData._ShootingGlance.ReloadTime = Skills._ShootingGlance.ReloadTime;
        YandexGame.savesData._ShootingGlance.WillCost = Skills._ShootingGlance.WillCost;
        YandexGame.savesData._ShootingGlance.CurrentPurchasePercent = Skills._ShootingGlance.CurrentPurchasePercent;
        YandexGame.savesData._ShootingGlance.SummCurrentPurchaseCount = Skills._ShootingGlance.SummCurrentPurchaseCount;
        for (int i = 0; i < YandexGame.savesData._ShootingGlance.Cost.Length; i++)
        {
            YandexGame.savesData._ShootingGlance.Cost[i] = Skills._ShootingGlance.Cost[i];
            YandexGame.savesData._ShootingGlance.PurchaseCount[i] = Skills._ShootingGlance.PurchaseCount[i];
        }

        YandexGame.savesData._BlackSauna.PeriodicDamage = Skills._BlackSauna.PeriodicDamage;
        YandexGame.savesData._BlackSauna.DecreasedHealthRegeneration = Skills._BlackSauna.DecreasedHealthRegeneration;
        YandexGame.savesData._BlackSauna.Duration = Skills._BlackSauna.Duration;
        YandexGame.savesData._BlackSauna.ReloadTime = Skills._BlackSauna.ReloadTime;
        YandexGame.savesData._BlackSauna.WillCost = Skills._BlackSauna.WillCost;
        YandexGame.savesData._BlackSauna.CurrentPurchasePercent = Skills._BlackSauna.CurrentPurchasePercent;
        YandexGame.savesData._BlackSauna.SummCurrentPurchaseCount = Skills._BlackSauna.SummCurrentPurchaseCount;
        for (int i = 0; i < YandexGame.savesData._BlackSauna.Cost.Length; i++)
        {
            YandexGame.savesData._BlackSauna.Cost[i] = Skills._BlackSauna.Cost[i];
            YandexGame.savesData._BlackSauna.PurchaseCount[i] = Skills._BlackSauna.PurchaseCount[i];
        }

        YandexGame.savesData._ArmorBreak.Damage = Skills._ArmorBreak.Damage;
        YandexGame.savesData._ArmorBreak.DecreasedArmor = Skills._ArmorBreak.DecreasedArmor;
        YandexGame.savesData._ArmorBreak.Duration = Skills._ArmorBreak.Duration;
        YandexGame.savesData._ArmorBreak.ReloadTime = Skills._ArmorBreak.ReloadTime;
        YandexGame.savesData._ArmorBreak.WillCost = Skills._ArmorBreak.WillCost;
        YandexGame.savesData._ArmorBreak.CurrentPurchasePercent = Skills._ArmorBreak.CurrentPurchasePercent;
        YandexGame.savesData._ArmorBreak.SummCurrentPurchaseCount = Skills._ArmorBreak.SummCurrentPurchaseCount;
        for (int i = 0; i < YandexGame.savesData._ArmorBreak.Cost.Length; i++)
        {
            YandexGame.savesData._ArmorBreak.Cost[i] = Skills._ArmorBreak.Cost[i];
            YandexGame.savesData._ArmorBreak.PurchaseCount[i] = Skills._ArmorBreak.PurchaseCount[i];
        }

        YandexGame.savesData._ZeusAnger.PercentDamage = Skills._ZeusAnger.PercentDamage;
        YandexGame.savesData._ZeusAnger.AttackCount = Skills._ZeusAnger.AttackCount;
        YandexGame.savesData._ZeusAnger.TimeBetweenAttacks = Skills._ZeusAnger.TimeBetweenAttacks;
        YandexGame.savesData._ZeusAnger.ReloadTime = Skills._ZeusAnger.ReloadTime;
        YandexGame.savesData._ZeusAnger.WillCost = Skills._ZeusAnger.WillCost;
        YandexGame.savesData._ZeusAnger.CurrentPurchasePercent = Skills._ZeusAnger.CurrentPurchasePercent;
        YandexGame.savesData._ZeusAnger.SummCurrentPurchaseCount = Skills._ZeusAnger.SummCurrentPurchaseCount;
        for (int i = 0; i < YandexGame.savesData._ZeusAnger.Cost.Length; i++)
        {
            YandexGame.savesData._ZeusAnger.Cost[i] = Skills._ZeusAnger.Cost[i];
            YandexGame.savesData._ZeusAnger.PurchaseCount[i] = Skills._ZeusAnger.PurchaseCount[i];
        }

        YandexGame.savesData._HelpPerun.AttackCount = Skills._HelpPerun.AttackCount;
        YandexGame.savesData._HelpPerun.ReloadTime = Skills._HelpPerun.ReloadTime;
        YandexGame.savesData._HelpPerun.WillCost = Skills._HelpPerun.WillCost;
        YandexGame.savesData._HelpPerun.CurrentPurchasePercent = Skills._HelpPerun.CurrentPurchasePercent;
        YandexGame.savesData._HelpPerun.SummCurrentPurchaseCount = Skills._HelpPerun.SummCurrentPurchaseCount;
        for (int i = 0; i < YandexGame.savesData._HelpPerun.Cost.Length; i++)
        {
            YandexGame.savesData._HelpPerun.Cost[i] = Skills._HelpPerun.Cost[i];
            YandexGame.savesData._HelpPerun.PurchaseCount[i] = Skills._HelpPerun.PurchaseCount[i];
        }

        YandexGame.savesData._PrayerToTheGods.PercentToStats = Skills._PrayerToTheGods.PercentToStats;
        YandexGame.savesData._PrayerToTheGods.Duration = Skills._PrayerToTheGods.Duration;
        YandexGame.savesData._PrayerToTheGods.ReloadTime = Skills._PrayerToTheGods.ReloadTime;
        YandexGame.savesData._PrayerToTheGods.WillCost = Skills._PrayerToTheGods.WillCost;
        YandexGame.savesData._PrayerToTheGods.CurrentPurchasePercent = Skills._PrayerToTheGods.CurrentPurchasePercent;
        YandexGame.savesData._PrayerToTheGods.SummCurrentPurchaseCount = Skills._PrayerToTheGods.SummCurrentPurchaseCount;
        for (int i = 0; i < YandexGame.savesData._PrayerToTheGods.Cost.Length; i++)
        {
            YandexGame.savesData._PrayerToTheGods.Cost[i] = Skills._PrayerToTheGods.Cost[i];
            YandexGame.savesData._PrayerToTheGods.PurchaseCount[i] = Skills._PrayerToTheGods.PurchaseCount[i];
        }

        YandexGame.savesData._GeneralFee.PercentAttributes = Skills._GeneralFee.PercentToStats;
        YandexGame.savesData._GeneralFee.Duration = Skills._GeneralFee.Duration;
        YandexGame.savesData._GeneralFee.ReloadTime = Skills._GeneralFee.ReloadTime;
        YandexGame.savesData._GeneralFee.WillCost = Skills._GeneralFee.WillCost;
        YandexGame.savesData._GeneralFee.CurrentPurchasePercent = Skills._GeneralFee.CurrentPurchasePercent;
        YandexGame.savesData._GeneralFee.SummCurrentPurchaseCount = Skills._GeneralFee.SummCurrentPurchaseCount;
        for (int i = 0; i < YandexGame.savesData._GeneralFee.Cost.Length; i++)
        {
            YandexGame.savesData._GeneralFee.Cost[i] = Skills._GeneralFee.Cost[i];
            YandexGame.savesData._GeneralFee.PurchaseCount[i] = Skills._GeneralFee.PurchaseCount[i];
        }
        #endregion

        #region Heroes

        YandexGame.savesData.CurrentHeroIndex = Heroes.CurrentHero.Index;

        for (int heroIndex = 0; heroIndex < Heroes.HeroCount; heroIndex++)
        {
            YandexGame.savesData.RemainingTimeToResurrationHero[heroIndex] = Heroes.hero[heroIndex].RemainingTimeToResurrationHero;
            YandexGame.savesData.Level[heroIndex] = Heroes.hero[heroIndex].Level;
            YandexGame.savesData.CurrentEXP[heroIndex] = Heroes.hero[heroIndex].ActualEXP;
            YandexGame.savesData.EXPForNextLevel[heroIndex] = Heroes.hero[heroIndex].EXPForNextLevel;
            YandexGame.savesData.PumpingPoints[heroIndex] = Heroes.hero[heroIndex].PumpingPoints;
            YandexGame.savesData.HeroBought[heroIndex] = Heroes.hero[heroIndex].Bought;

            YandexGame.savesData.MaxHealthBase[heroIndex] = Heroes.hero[heroIndex].MaxHealthBase;
            YandexGame.savesData.MaxHealthFromLevel[heroIndex] = Heroes.hero[heroIndex].MaxHealthFromLevel;
            YandexGame.savesData.MaxHealthFromStrength[heroIndex] = Heroes.hero[heroIndex].MaxHealthFromStrength;
            YandexGame.savesData.MaxHealthFromArmorSet[heroIndex] = Heroes.hero[heroIndex].MaxHealthFromArmorSet;
            YandexGame.savesData.MaxHealthFromWeapon[heroIndex] = Heroes.hero[heroIndex].MaxHealthFromWeapon;
            YandexGame.savesData.MaxHealthFromPerk[heroIndex] = Heroes.hero[heroIndex].MaxHealthFromPerk;
            YandexGame.savesData.ActualHealth[heroIndex] = Heroes.hero[heroIndex].ActualHealth;

            YandexGame.savesData.HealthRegenerationBase[heroIndex] = Heroes.hero[heroIndex].HealthRegenerationBase;
            YandexGame.savesData.HealthRegenerationFromStrength[heroIndex] = Heroes.hero[heroIndex].HealthRegenerationFromStrength;
            YandexGame.savesData.HealthRegenerationFromArmorSet[heroIndex] = Heroes.hero[heroIndex].HealthRegenerationFromArmorSet;
            YandexGame.savesData.HealthRegenerationFromWeapon[heroIndex] = Heroes.hero[heroIndex].HealthRegenerationFromWeapon;
            YandexGame.savesData.HealthRegenerationFromPerk[heroIndex] = Heroes.hero[heroIndex].HealthRegenerationFromPerk;

            YandexGame.savesData.CostStaminaToAttack[heroIndex] = Heroes.hero[heroIndex].CostStaminaToAttack;
            YandexGame.savesData.MaxStaminaBase[heroIndex] = Heroes.hero[heroIndex].MaxStaminaBase;
            YandexGame.savesData.MaxStaminaFromLevel[heroIndex] = Heroes.hero[heroIndex].MaxStaminaFromLevel;
            YandexGame.savesData.MaxStaminaFromStrength[heroIndex] = Heroes.hero[heroIndex].MaxStaminaFromStrength;
            YandexGame.savesData.MaxStaminaFromDexterity[heroIndex] = Heroes.hero[heroIndex].MaxStaminaFromDexterity;
            YandexGame.savesData.MaxStaminaFromArmorSet[heroIndex] = Heroes.hero[heroIndex].MaxStaminaFromArmorSet;
            YandexGame.savesData.MaxStaminaFromWeapon[heroIndex] = Heroes.hero[heroIndex].MaxStaminaFromWeapon;
            YandexGame.savesData.MaxStaminaFromPerk[heroIndex] = Heroes.hero[heroIndex].MaxStaminaFromPerk;
            YandexGame.savesData.ActualStamina[heroIndex] = Heroes.hero[heroIndex].ActualStamina;

            YandexGame.savesData.StaminaRegenerationBase[heroIndex] = Heroes.hero[heroIndex].StaminaRegenerationBase;
            YandexGame.savesData.StaminaRegenerationFromStrength[heroIndex] = Heroes.hero[heroIndex].StaminaRegenerationFromStrength;
            YandexGame.savesData.StaminaRegenerationFromDexterity[heroIndex] = Heroes.hero[heroIndex].StaminaRegenerationFromDexterity;
            YandexGame.savesData.StaminaRegenerationFromArmorSet[heroIndex] = Heroes.hero[heroIndex].StaminaRegenerationFromArmorSet;
            YandexGame.savesData.StaminaRegenerationFromWeapon[heroIndex] = Heroes.hero[heroIndex].StaminaRegenerationFromWeapon;
            YandexGame.savesData.StaminaRegenerationFromPerk[heroIndex] = Heroes.hero[heroIndex].StaminaRegenerationFromPerk;

            YandexGame.savesData.MaxWillBase[heroIndex] = Heroes.hero[heroIndex].MaxWillBase;
            YandexGame.savesData.MaxWillFromLevel[heroIndex] = Heroes.hero[heroIndex].MaxWillFromLevel;
            YandexGame.savesData.MaxWillFromIntellect[heroIndex] = Heroes.hero[heroIndex].MaxWillFromIntellect;
            YandexGame.savesData.MaxWillFromArmorSet[heroIndex] = Heroes.hero[heroIndex].MaxWillFromArmorSet;
            YandexGame.savesData.MaxWillFromPerk[heroIndex] = Heroes.hero[heroIndex].MaxWillFromPerk;
            YandexGame.savesData.ActualWill[heroIndex] = Heroes.hero[heroIndex].ActualWill;

            YandexGame.savesData.WillRegenerationBase[heroIndex] = Heroes.hero[heroIndex].WillRegenerationBase;
            YandexGame.savesData.WillRegenerationFromIntellect[heroIndex] = Heroes.hero[heroIndex].WillRegenerationFromIntellect;
            YandexGame.savesData.WillRegenerationFromArmorSet[heroIndex] = Heroes.hero[heroIndex].WillRegenerationFromArmorSet;
            YandexGame.savesData.WillRegenerationFromPerk[heroIndex] = Heroes.hero[heroIndex].WillRegenerationFromPerk;

            YandexGame.savesData.DamageBase[heroIndex] = Heroes.hero[heroIndex].DamageBase;
            YandexGame.savesData.DamageFromLevel[heroIndex] = Heroes.hero[heroIndex].DamageFromLevel;
            YandexGame.savesData.DamageFromWeapon[heroIndex] = Heroes.hero[heroIndex].DamageFromWeapon;
            YandexGame.savesData.DamageFromStrength[heroIndex] = Heroes.hero[heroIndex].DamageFromStrength;
            YandexGame.savesData.DamageFromArmorSet[heroIndex] = Heroes.hero[heroIndex].DamageFromArmorSet;
            YandexGame.savesData.DamageFromPerk[heroIndex] = Heroes.hero[heroIndex].DamageFromPerk;

            YandexGame.savesData.ArmorBase[heroIndex] = Heroes.hero[heroIndex].ArmorBase;
            YandexGame.savesData.ArmorFromArmorSet[heroIndex] = Heroes.hero[heroIndex].ArmorFromArmorSet;
            YandexGame.savesData.ArmorFromWeapon[heroIndex] = Heroes.hero[heroIndex].ArmorFromWeapon;
            YandexGame.savesData.ArmorFromDexterity[heroIndex] = Heroes.hero[heroIndex].ArmorFromDexterity;
            YandexGame.savesData.ArmorFromPerk[heroIndex] = Heroes.hero[heroIndex].ArmorFromPerk;

            YandexGame.savesData.EvasionBase[heroIndex] = Heroes.hero[heroIndex].EvasionBase;
            YandexGame.savesData.EvasionFromDexterity[heroIndex] = Heroes.hero[heroIndex].EvasionFromDexterity;
            YandexGame.savesData.EvasionFromArmorSet[heroIndex] = Heroes.hero[heroIndex].EvasionFromArmorSet;
            YandexGame.savesData.EvasionFromWeapon[heroIndex] = Heroes.hero[heroIndex].EvasionFromWeapon;
            YandexGame.savesData.EvasionFromPerk[heroIndex] = Heroes.hero[heroIndex].EvasionFromPerk;

            YandexGame.savesData.StrengthBase[heroIndex] = Heroes.hero[heroIndex].StrengthBase;
            YandexGame.savesData.StrengthFromLevel[heroIndex] = Heroes.hero[heroIndex].StrengthFromLevel;
            YandexGame.savesData.StrengthFromPumpingPoints[heroIndex] = Heroes.hero[heroIndex].StrengthFromPumpingPoints;
            YandexGame.savesData.StrengthFromArmorSet[heroIndex] = Heroes.hero[heroIndex].StrengthFromArmorSet;
            YandexGame.savesData.StrengthFromWeapon[heroIndex] = Heroes.hero[heroIndex].StrengthFromWeapon;
            YandexGame.savesData.StrengthFromPerk[heroIndex] = Heroes.hero[heroIndex].StrengthFromPerk;

            YandexGame.savesData.DexterityBase[heroIndex] = Heroes.hero[heroIndex].DexterityBase;
            YandexGame.savesData.DexterityFromLevel[heroIndex] = Heroes.hero[heroIndex].DexterityFromLevel;
            YandexGame.savesData.DexterityFromPumpingPoints[heroIndex] = Heroes.hero[heroIndex].DexterityFromPumpingPoints;
            YandexGame.savesData.DexterityFromWeapon[heroIndex] = Heroes.hero[heroIndex].DexterityFromWeapon;
            YandexGame.savesData.DexterityFromArmorSet[heroIndex] = Heroes.hero[heroIndex].DexterityFromArmorSet;
            YandexGame.savesData.DexterityFromPerk[heroIndex] = Heroes.hero[heroIndex].DexterityFromPerk;

            YandexGame.savesData.IntellectBase[heroIndex] = Heroes.hero[heroIndex].IntellectBase;
            YandexGame.savesData.IntellectFromLevel[heroIndex] = Heroes.hero[heroIndex].IntellectFromLevel;
            YandexGame.savesData.IntellectFromPumpingPoints[heroIndex] = Heroes.hero[heroIndex].IntellectFromPumpingPoints;
            YandexGame.savesData.IntellectFromArmorSet[heroIndex] = Heroes.hero[heroIndex].IntellectFromArmorSet;
            YandexGame.savesData.IntellectFromPerk[heroIndex] = Heroes.hero[heroIndex].IntellectFromPerk;
        }
        #endregion

        #region Learning
        YandexGame.savesData.RussiansLessonIsFinished = Learning.RussiansLessonIsFinished;
        #endregion

        YandexGame.SaveProgress();
        OnSaved?.Invoke();

        void ArmorSetsInitialize()
        {
            for (int i = 0; i < 3; i++)
                YandexGame.savesData._ArmorSet[i] = new();

            YandexGame.savesData._ArmorSet[0].StatValue = new float[ArmorMark_1.StatsCount];
            YandexGame.savesData._ArmorSet[0].StatUpgradeCost = new float[ArmorMark_1.StatsCount];
            YandexGame.savesData._ArmorSet[0].CurrentUpgradeCount = new int[ArmorMark_1.StatsCount];

            YandexGame.savesData._ArmorSet[1].StatValue = new float[ArmorMark_2.StatsCount];
            YandexGame.savesData._ArmorSet[1].StatUpgradeCost = new float[ArmorMark_2.StatsCount];
            YandexGame.savesData._ArmorSet[1].CurrentUpgradeCount = new int[ArmorMark_2.StatsCount];

            YandexGame.savesData._ArmorSet[2].StatValue = new float[ArmorMark_3.StatsCount];
            YandexGame.savesData._ArmorSet[2].StatUpgradeCost = new float[ArmorMark_3.StatsCount];
            YandexGame.savesData._ArmorSet[2].CurrentUpgradeCount = new int[ArmorMark_3.StatsCount];
        }
    }

    private void LoadPlayerData_1()
    {
        Debug.Log("LOAD_1");

        SavesYG data_1 = YandexGame.savesData;
        //PlayerData_1 data_1 = SaveSystem.LoadPlayer_1(); //BINARY DATA

        #region System
        if (Game.CurrentScene != (int)Game.BuildIndex.Russians_vs_Lizards)

        if (CheckNull(data_1.FirstLaunchGame))
            Game.FirstLaunchGame = data_1.FirstLaunchGame;
        if (CheckNull(data_1.FirstLaunchScene))
            Game.FirstLaunchScene = data_1.FirstLaunchScene;

        if (CheckNull(data_1.LastPlayingTime))
            Game.LastPlayingTime = data_1.LastPlayingTime;
        if (CheckNull(data_1.LastLaunchGameTime))
            Game.LastLaunchGameTime = data_1.LastLaunchGameTime;
        if (CheckNull(data_1.VisitingDays))
            Game.VisitingDays = data_1.VisitingDays;

        if (CheckNull(data_1.StartLessonIsComplete))
            StartLearning.StartLessonIsComplete = data_1.StartLessonIsComplete;
        if (CheckNull(data_1.DisclaimerIsWathced))
            Disclaimer.DisclaimerIsWathced = data_1.DisclaimerIsWathced;
        #endregion

        #region MoneyMenu
        if (CheckNull(data_1.MemeCoins))
            MoneyMenu.SetMemeCoins(data_1.MemeCoins);
        if (CheckNull(data_1.MoneyCapital))
            MoneyMenu.SetMoneyCapital(data_1.MoneyCapital);
        if (CheckNull(data_1.ClickIncome))
            MoneyMenu.SetClickIncome(data_1.ClickIncome);
        if (CheckNull(data_1.TickIncome))
            MoneyMenu.SetTickIncome(data_1.TickIncome);
        if (CheckNull(data_1.AccumulatedMoney))
            MoneyMenu.AccumulatedMoney = data_1.AccumulatedMoney;
        #endregion

        #region Buffs
        if (CheckNull(data_1.ClickBuffTimeRemaining))
            ClickADReward.BuffTimeRemaining = data_1.ClickBuffTimeRemaining;
        if (CheckNull(data_1.IdleBuffTimeRemaining))
            IdleADReward.BuffTimeRemaining = data_1.IdleBuffTimeRemaining;
        if (CheckNull(data_1.ClickCachedADRewardMultiplier))
            ClickADReward.CachedADMultiplier = data_1.ClickCachedADRewardMultiplier;
        if (CheckNull(data_1.IdleCachedADRewardMultiplier))
            IdleADReward.CachedADMultiplier = data_1.IdleCachedADRewardMultiplier;
        #endregion

        #region Upgrades

        int sceneCount = Game.ScenesCount;

        for (int scenes = 0; scenes < Game.ScenesCount; scenes++)
        {
            if (CheckNull(data_1.AccumulatedPurchasedClickUpgrades))
            {
                for (int i = 0; i < data_1.AccumulatedPurchasedClickUpgrades.Length / sceneCount; i++)
                    Upgrades.AccumulatedPurchasedClickUpgrades[scenes, i] = data_1.AccumulatedPurchasedClickUpgrades[scenes, i];
            }
            if (CheckNull(data_1.AccumulatedPurchasedIdleUpgrades))
            {
                for (int i = 0; i < data_1.AccumulatedPurchasedIdleUpgrades.Length / sceneCount; i++)
                    Upgrades.AccumulatedPurchasedIdleUpgrades[scenes, i] = data_1.AccumulatedPurchasedIdleUpgrades[scenes, i];
            }


            if (CheckNull(data_1._Click.Income))
            {
                for (int i = 0; i < data_1._Click.Income.Length / sceneCount; i++)
                    Upgrades._Click.Income[scenes, i] = data_1._Click.Income[scenes, i];
            }
            if (CheckNull(data_1._Click.SummIncome))
            {
                for (int i = 0; i < data_1._Click.SummIncome.Length / sceneCount; i++)
                    Upgrades._Click.SummIncome[scenes, i] = data_1._Click.SummIncome[scenes, i];
            }
            if (CheckNull(data_1._Click.Prices))
            {
                for (int i = 0; i < data_1._Click.Prices.Length / sceneCount; i++)
                    Upgrades._Click.Prices[scenes, i] = data_1._Click.Prices[scenes, i];
            }
            if (CheckNull(data_1._Click.PurchasedUpgradeAmount))
            {
                for (int i = 0; i < data_1._Click.PurchasedUpgradeAmount.Length / sceneCount; i++)
                    Upgrades._Click.PurchasedUpgradeAmount[scenes, i] = data_1._Click.PurchasedUpgradeAmount[scenes, i];
            }
            if (CheckNull(data_1._Click.ReceivedLevels))
            {
                for (int i = 0; i < data_1._Click.ReceivedLevels.Length / sceneCount; i++)
                    Upgrades._Click.ReceivedLevels[scenes, i] = data_1._Click.ReceivedLevels[scenes, i];
            }

            if (CheckNull(data_1._Idle.Income))
            {
                for (int i = 0; i < data_1._Idle.Income.Length / sceneCount; i++)
                    Upgrades._Idle.Income[scenes, i] = data_1._Idle.Income[scenes, i];
            }
            if (CheckNull(data_1._Idle.SummIncome))
            {
                for (int i = 0; i < data_1._Idle.SummIncome.Length / sceneCount; i++)
                    Upgrades._Idle.SummIncome[scenes, i] = data_1._Idle.SummIncome[scenes, i];
            }
            if (CheckNull(data_1._Idle.Prices))
            {
                for (int i = 0; i < data_1._Idle.Prices.Length / sceneCount; i++)
                    Upgrades._Idle.Prices[scenes, i] = data_1._Idle.Prices[scenes, i];
            }
            if (CheckNull(data_1._Idle.PurchasedUpgradeAmount))
            {
                for (int i = 0; i < data_1._Idle.PurchasedUpgradeAmount.Length / sceneCount; i++)
                    Upgrades._Idle.PurchasedUpgradeAmount[scenes, i] = data_1._Idle.PurchasedUpgradeAmount[scenes, i];
            }
            if (CheckNull(data_1._Idle.ReceivedLevels))
            {
                for (int i = 0; i < data_1._Idle.ReceivedLevels.Length / sceneCount; i++)
                    Upgrades._Idle.ReceivedLevels[scenes, i] = data_1._Idle.ReceivedLevels[scenes, i];
            }
        }
        #endregion

        #region GlobalUpgrades
        if (CheckNull(data_1.UpgradeCostMultiplier))
            GlobalUpgrades.UpgradeCostMultiplier = data_1.UpgradeCostMultiplier;
        if (CheckNull(data_1.IncomeMultiplier))
            GlobalUpgrades.IncomeMultiplier = data_1.IncomeMultiplier;
        if (CheckNull(data_1.TickTime))
            GlobalUpgrades.TickTime = data_1.TickTime;
        if (CheckNull(data_1.CritChance))
            GlobalUpgrades.CritChance = data_1.CritChance;
        if (CheckNull(data_1.CritStrength))
            GlobalUpgrades.CritStrength = data_1.CritStrength;
        if (CheckNull(data_1.ADRewardMultiplier))
            GlobalUpgrades.ADRewardMultiplier = data_1.ADRewardMultiplier;
        if (CheckNull(data_1.AbsenceTime))
            GlobalUpgrades.AbsenceTime = data_1.AbsenceTime;
        if (CheckNull(data_1.AdvertisingBuffTime))
            GlobalUpgrades.AdvertisingBuffTime = data_1.AdvertisingBuffTime;
        if (CheckNull(data_1.Prices))
            GlobalUpgrades.Prices = data_1.Prices;
        if (CheckNull(data_1.PurchasedCounter))
            GlobalUpgrades.PurchasedCounter = data_1.PurchasedCounter;
        if (CheckNull(data_1.MemeClipCount))
            GlobalUpgrades.MemeClipCount = data_1.MemeClipCount;        
        #endregion

        #region Achievements
        if (CheckNull(data_1.AchievementsRewardsIsClaimed))
            GlobalUpgrades.AchievementsRewardsIsClaimed = data_1.AchievementsRewardsIsClaimed;
        if (CheckNull(data_1.CurrentUniverseProgress))
            Multiverse.CurrentUniverseProgress = data_1.CurrentUniverseProgress;        
        if (CheckNull(data_1.AccumulatedTime))
            Game.AccumulatedTime = data_1.AccumulatedTime;
        if (CheckNull(data_1.AccumulatedClicks))
            Game.AccumulatedClicks = data_1.AccumulatedClicks;
        if (CheckNull(data_1.PurchasedMemeClips))
            MemeShop.PurchasedMemeClips = data_1.PurchasedMemeClips;
        if (CheckNull(data_1.CountWatchedAD))
            Game.CountWatchedAD = data_1.CountWatchedAD;
        #endregion

        #region MemeShop
        for (int i = 0; i < MemeShop.SelectedClipCount.Length; i++)
        {
            if (CheckNull(data_1.SelectedClipCount))
                MemeShop.SelectedClipCount[i] = data_1.SelectedClipCount[i];
        }

        if (CheckNull(data_1.ClipIsAvailable))
            MemeShop.ClipIsAvailable = data_1.ClipIsAvailable;
        if (CheckNull(data_1.ClipIsSelected))
            MemeShop.ClipIsSelected = data_1.ClipIsSelected;
        if (CheckNull(data_1.ClipIsSelected_2))
            CombinedUniverse.MemeShop.ClipIsSelected = data_1.ClipIsSelected_2;
        #endregion

        #region Calendar
        if (CheckNull(data_1.CalendarRewardIsClaimed))
            Calendar.CalendarRewardIsClaimed = data_1.CalendarRewardIsClaimed;
        if (CheckNull(data_1.RewardIsUnlock))
            Calendar.RewardIsUnlock = data_1.RewardIsUnlock;
        #endregion

        #region CasesMenu
        if (CheckNull(CasesMenu))
        {
            if (CheckNull(data_1.OpenedCases))
                CasesMenu.OpenedCases = data_1.OpenedCases;
            if (CheckNull(data_1.RewardIsUnlock))
                CasesMenu.CasesCount = data_1.CasesCount;
            if (CheckNull(data_1.WatchedVideosInCurrentDay))
                CasesMenu.WatchedVideosInCurrentDay = data_1.WatchedVideosInCurrentDay;
            if (CheckNull(data_1.EverydayCaseIsTaken))
                CasesMenu.EverydayCaseIsTaken = data_1.EverydayCaseIsTaken;            
        }
        #endregion

        #region Options
        if (CheckNull(data_1.CurrentImageIndex))
            SwitchBackground.CurrentImageIndex = data_1.CurrentImageIndex;
        if (CheckNull(data_1.AudioIsOn))
            MainAudio.AudioIsOn = data_1.AudioIsOn;
        if (CheckNull(data_1.SelectedMusicIndex))
            Music.SelectedMusicIndex = data_1.SelectedMusicIndex;
        if (CheckNull(data_1.CurrentMusicTime))
            Music.CurrentMusicTime = data_1.CurrentMusicTime;
        if (CheckNull(data_1.MusicVolume))
            Music.MusicVolume = data_1.MusicVolume;
        if (CheckNull(data_1.MusicPitch))
            Music.MusicPitch = data_1.MusicPitch;
        if (CheckNull(data_1.MusicIsMute))
            Music.MusicIsMute = data_1.MusicIsMute;

        if (CheckNull(data_1.AudioEffectsVolume))
            AudioEffects.AudioEffectsVolume = data_1.AudioEffectsVolume;
        if (CheckNull(data_1.AudioEffectsPitch))
            AudioEffects.AudioEffectsPitch = data_1.AudioEffectsPitch;
        if (CheckNull(data_1.AudioEffectsIsMute))
            AudioEffects.AudioEffectsIsMute = data_1.AudioEffectsIsMute;
        if (CheckNull(data_1.IdleAudioEffectIsOn))
            AudioEffects.IdleAudioEffectsIsOn = data_1.IdleAudioEffectIsOn;
        if (CheckNull(data_1.AudioEffectsIsLayering))
            AudioEffects.AudioEffectsIsLayering = data_1.AudioEffectsIsLayering;
        #endregion
    }

    private void LoadPlayerData_2()
    {
        Debug.Log("LOAD_2");

        SavesYG data_2 = YandexGame.savesData;
        //PlayerData_2 data_2 = SaveSystem.LoadPlayer_2(); //BINARY DATA

        #region Russians_vs_Lizards

        #region Battle
        if (CheckNull(data_2.EnemiesInStage))
            Battle.CurrentEnemiesInStage = data_2.EnemiesInStage;
        if (CheckNull(data_2.CurrentStage))
            Battle.CurrentStage = data_2.CurrentStage;
        if (CheckNull(data_2.MaxOpenStage))
            Battle.MaxOpenStage = data_2.MaxOpenStage;
        if (CheckNull(data_2.ResetCount))
            Battle.ResetCount = data_2.ResetCount;
        if (CheckNull(data_2.FinalBossIsDead))
             Battle.FinalBossIsDead = data_2.FinalBossIsDead;
        #endregion

        #region Facilities
        if (CheckNull(data_2.FaithCurrency))
            Facilities.FaithCurrency = data_2.FaithCurrency;
        if (CheckNull(data_2.AncestralPower))
            Facilities.AncestralPower = data_2.AncestralPower;
        if (CheckNull(data_2.FaithMultiplier))
        {
            if (data_2.FaithMultiplier < 1 || data_2.FaithMultiplier == 0)
                Facilities.FaithMultiplier = 1;
            else Facilities.FaithMultiplier = data_2.FaithMultiplier;
        }
        #endregion

        #region Achievements
        if (CheckNull(data_2.AccumulatedKills))
            Achievements_R_vs_L.AccumulatedKills = data_2.AccumulatedKills;
        if (CheckNull(data_2.AccumulatedFaith))
            Achievements_R_vs_L.AccumulatedFaith = data_2.AccumulatedFaith;
        if (CheckNull(data_2.AccumulatedAncestralPower))
            Achievements_R_vs_L.AccumulatedAncestralPower = data_2.AccumulatedAncestralPower;
        if (CheckNull(data_2.AccumulatedHeroes))
            Achievements_R_vs_L.AccumulatedHeroes = data_2.AccumulatedHeroes;
        if (CheckNull(data_2.AccumulatedSkills))
            Achievements_R_vs_L.AccumulatedSkills = data_2.AccumulatedSkills;
        if (CheckNull(data_2.AccumulatedArmors))
            Achievements_R_vs_L.AccumulatedArmors = data_2.AccumulatedArmors;
        if (CheckNull(data_2.AccumulatedWeapons))
            Achievements_R_vs_L.AccumulatedWeapons = data_2.AccumulatedWeapons;
        if (CheckNull(data_2.AccumulatedPerks))
            Achievements_R_vs_L.AccumulatedPerks = data_2.AccumulatedPerks;
        if (CheckNull(data_2.AccumulatedStages))
            Achievements_R_vs_L.AccumulatedStages = data_2.AccumulatedStages;

        if (CheckNull(data_2.WeaponIsUnlocked))
            Achievements_R_vs_L.WeaponIsUnlocked = data_2.WeaponIsUnlocked;
        if (CheckNull(data_2.ArmorIsUnlocked))
            Achievements_R_vs_L.ArmorIsUnlocked = data_2.ArmorIsUnlocked;
        if (CheckNull(data_2.SkillIsUnlocked))
            Achievements_R_vs_L.SkillIsUnlocked = data_2.SkillIsUnlocked;
        if (CheckNull(data_2.HeroIsUnlocked))
            Achievements_R_vs_L.HeroIsUnlocked = data_2.HeroIsUnlocked;
        if (CheckNull(data_2.SavedMaxOpenStage))
            Achievements_R_vs_L.SavedMaxOpenStage = data_2.SavedMaxOpenStage;        
        #endregion

        #region Items
        if (CheckNull(data_2.BaikalWaterCount))
            Items._BaikalWater.Count = data_2.BaikalWaterCount;

        if (CheckNull(data_2.PotionsCount[0]))
            Items.HealthPotion.Count = data_2.PotionsCount[0];
        if (CheckNull(data_2.PotionsCount[1]))
            Items.StaminaPotion.Count = data_2.PotionsCount[1];
        if (CheckNull(data_2.PotionsCount[2]))
            Items.WillPotion.Count = data_2.PotionsCount[2];
        #endregion

        #region Buffs
        if (CheckNull(data_2.AdditiveStatsFromBaikalWater))
            Items.AdditiveStats = data_2.AdditiveStatsFromBaikalWater;
        if (CheckNull(data_2.BaikalWaterTimer))
            Items.Timer = data_2.BaikalWaterTimer;

        if (CheckNull(data_2.CachedAdditiveStats_GeneralFee))
            ListOfEffects.CachedAdditiveStats_GeneralFee = data_2.CachedAdditiveStats_GeneralFee;
        if (CheckNull(data_2.CachedBuffTime_GeneralFee))
            ListOfEffects.CachedBuffTime_GeneralFee = data_2.CachedBuffTime_GeneralFee;

        if (CheckNull(data_2.CachedAdditiveStats_PrayerToTheGods))
            ListOfEffects.CachedAdditiveStats_PrayerToTheGods = data_2.CachedAdditiveStats_PrayerToTheGods;
        if (CheckNull(data_2.CachedBuffTime_PrayerToTheGods))
            ListOfEffects.CachedBuffTime_PrayerToTheGods = data_2.CachedBuffTime_PrayerToTheGods;
        #endregion

        #region Armors
        for (int i = 0; i < ArmorMark_1.StatsCount; i++)
        {
            if (CheckNull(data_2._ArmorSet[0].StatValue[i]))
                ArmorMark_1.StatValue[i] = data_2._ArmorSet[0].StatValue[i];
            if (CheckNull(data_2._ArmorSet[0].StatUpgradeCost[i]))
                ArmorMark_1.StatUpgradeCost[i] = data_2._ArmorSet[0].StatUpgradeCost[i];
            if (CheckNull(data_2._ArmorSet[0].CurrentUpgradeCount[i]))
                ArmorMark_1.CurrentUpgradeCount[i] = data_2._ArmorSet[0].CurrentUpgradeCount[i];
            if (CheckNull(data_2._ArmorSet[0].Bought))
                ArmorMark_1.Bought = data_2._ArmorSet[0].Bought;
            if (CheckNull(data_2._ArmorSet[0].Selected))
                ArmorMark_1.Selected = data_2._ArmorSet[0].Selected;
        }
        for (int i = 0; i < ArmorMark_2.StatsCount; i++)
        {
            if (CheckNull(data_2._ArmorSet[1].StatValue[i]))
                ArmorMark_2.StatValue[i] = data_2._ArmorSet[1].StatValue[i];
            if (CheckNull(data_2._ArmorSet[1].StatUpgradeCost[i]))
                ArmorMark_2.StatUpgradeCost[i] = data_2._ArmorSet[1].StatUpgradeCost[i];
            if (CheckNull(data_2._ArmorSet[1].CurrentUpgradeCount[i]))
                ArmorMark_2.CurrentUpgradeCount[i] = data_2._ArmorSet[1].CurrentUpgradeCount[i];
            if (CheckNull(data_2._ArmorSet[1].Bought))
                ArmorMark_2.Bought = data_2._ArmorSet[1].Bought;
            if (CheckNull(data_2._ArmorSet[1].Selected))
                ArmorMark_2.Selected = data_2._ArmorSet[1].Selected;
        }
        for (int i = 0; i < ArmorMark_3.StatsCount; i++)
        {
            if (CheckNull(data_2._ArmorSet[2].StatValue[i]))
                ArmorMark_3.StatValue[i] = data_2._ArmorSet[2].StatValue[i];
            if (CheckNull(data_2._ArmorSet[2].StatUpgradeCost[i]))
                ArmorMark_3.StatUpgradeCost[i] = data_2._ArmorSet[2].StatUpgradeCost[i];
            if (CheckNull(data_2._ArmorSet[2].CurrentUpgradeCount[i]))
                ArmorMark_3.CurrentUpgradeCount[i] = data_2._ArmorSet[2].CurrentUpgradeCount[i];
            if (CheckNull(data_2._ArmorSet[2].Bought))
                ArmorMark_3.Bought = data_2._ArmorSet[2].Bought;
            if (CheckNull(data_2._ArmorSet[2].Selected))
                ArmorMark_3.Selected = data_2._ArmorSet[2].Selected;
        }
        #endregion

        #region Weapons
        for (int weaponIndex = 0; weaponIndex < Weapons.WeaponCount; weaponIndex++)
        {
            if (CheckNull(data_2._Weapons.Selected[weaponIndex]))
                Weapons._Weapon[weaponIndex].Selected = data_2._Weapons.Selected[weaponIndex];
            if (CheckNull(data_2._Weapons.Bought[weaponIndex]))
                Weapons._Weapon[weaponIndex].Bought = data_2._Weapons.Bought[weaponIndex];

            for (int statIndex = 0; statIndex < Weapons.WeaponStatsCount; statIndex++)
            {
                if (CheckNull(data_2._Weapons.StatsValue[weaponIndex, statIndex]))
                    Weapons._Weapon[weaponIndex].StatsValue[statIndex] = data_2._Weapons.StatsValue[weaponIndex, statIndex];
                if (CheckNull(data_2._Weapons.CurrentUpgradeCount[weaponIndex, statIndex]))
                    Weapons._Weapon[weaponIndex].CurrentUpgradeCount[statIndex] = data_2._Weapons.CurrentUpgradeCount[weaponIndex, statIndex];
                if (CheckNull(data_2._Weapons.StatsUpgradeCost[weaponIndex, statIndex]))
                    Weapons._Weapon[weaponIndex].StatsUpgradeCost[statIndex] = data_2._Weapons.StatsUpgradeCost[weaponIndex, statIndex];
            }
        }
        #endregion

        #region PerkTree
        int perkIndex;

        perkIndex = 0;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.BaseStrengthAdditional.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.BaseStrengthAdditional.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.BaseStrengthAdditional.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 1;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.BaseDexterityAdditional.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.BaseDexterityAdditional.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.BaseDexterityAdditional.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 2;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.BaseIntellectAdditional.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.BaseIntellectAdditional.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.BaseIntellectAdditional.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 3;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.ArithmeticStrengthAdditional.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.ArithmeticStrengthAdditional.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.ArithmeticStrengthAdditional.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 4;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.ArithmeticDexterityAdditional.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.ArithmeticDexterityAdditional.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.ArithmeticDexterityAdditional.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 5;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.ArithmeticIntellectAdditional.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.ArithmeticIntellectAdditional.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.ArithmeticIntellectAdditional.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 6;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.GeometricStrengthAdditional.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.GeometricStrengthAdditional.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.GeometricStrengthAdditional.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 7;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.GeometricDexterityAdditional.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.GeometricDexterityAdditional.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.GeometricDexterityAdditional.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 8;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.GeometricIntellectAdditional.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.GeometricIntellectAdditional.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.GeometricIntellectAdditional.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 9;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.MaxHealthFromStrength.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.MaxHealthFromStrength.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.MaxHealthFromStrength.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 10;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.MaxStaminaFromDexterity.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.MaxStaminaFromDexterity.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.MaxStaminaFromDexterity.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 11;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.MaxWillFromIntellect.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.MaxWillFromIntellect.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.MaxWillFromIntellect.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 12;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.HealthRegenerationFromStrength.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.HealthRegenerationFromStrength.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.HealthRegenerationFromStrength.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 13;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.StaminaRegenerationFromDexterity.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.StaminaRegenerationFromDexterity.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.StaminaRegenerationFromDexterity.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];

        perkIndex = 14;
        if (CheckNull(data_2._PerkTree.Summ[perkIndex]))
            PerkTree.WillRegenerationFromIntellect.Summ = data_2._PerkTree.Summ[perkIndex];
        //if (CheckNull(data_2._PerkTree.Cost[perkIndex]))
        //    PerkTree.WillRegenerationFromIntellect.Cost = data_2._PerkTree.Cost[perkIndex];
        if (CheckNull(data_2._PerkTree.CurrentUpgradeIndex[perkIndex]))
            PerkTree.WillRegenerationFromIntellect.CurrentUpgradeIndex = data_2._PerkTree.CurrentUpgradeIndex[perkIndex];
        #endregion

        #region Skills

        for (int i = 0; i < Skills.PlasesForSkillsCount; i++)
        {
            if (CheckNull(data_2.BoughtPlasesForSkills[i]))
                Skills.PlaseForSkillIsBought[i] = data_2.BoughtPlasesForSkills[i];
            if (CheckNull(data_2.SelectedSkillIndex[i]))
                Skills.SelectedSkillIndex[i] = data_2.SelectedSkillIndex[i];
            if (CheckNull(data_2.SavedReloadTimeForSelectedSkill[i]))
                Skills.SavedReloadTimeForSelectedSkill[i] = data_2.SavedReloadTimeForSelectedSkill[i];
        }

        for (int i = 0; i < Skills.SkillsCount; i++)
        {
            if (CheckNull(data_2.BoughtSkills[i]))
                Skills.SkillIsBought[i] = data_2.BoughtSkills[i];
            if (CheckNull(data_2.SelectedSkill[i]))
                Skills.SkillIsSelected[i] = data_2.SelectedSkill[i];
        }

        #region BallClamp
        if (CheckNull(data_2._BallClamp.StunDuration))
            Skills._BallClamp.StunDuration = data_2._BallClamp.StunDuration;
        if (CheckNull(data_2._BallClamp.Damage))
            Skills._BallClamp.Damage = data_2._BallClamp.Damage;
        if (CheckNull(data_2._BallClamp.ReloadTime))
            Skills._BallClamp.ReloadTime = data_2._BallClamp.ReloadTime;
        if (CheckNull(data_2._BallClamp.WillCost))
            Skills._BallClamp.WillCost = data_2._BallClamp.WillCost;
        if (CheckNull(data_2._BallClamp.CurrentPurchasePercent))
            Skills._BallClamp.CurrentPurchasePercent = data_2._BallClamp.CurrentPurchasePercent;
        if (CheckNull(data_2._BallClamp.SummCurrentPurchaseCount))
            Skills._BallClamp.SummCurrentPurchaseCount = data_2._BallClamp.SummCurrentPurchaseCount;

        for (int i = 0; i < Skills._BallClamp.Cost.Length; i++)
        {
            if (CheckNull(data_2._BallClamp.Cost[i]))
                Skills._BallClamp.Cost[i] = data_2._BallClamp.Cost[i];
            if (CheckNull(data_2._BallClamp.PurchaseCount[i]))
                Skills._BallClamp.PurchaseCount[i] = data_2._BallClamp.PurchaseCount[i];
        }
        #endregion

        #region CumBombardment
        if (CheckNull(data_2._ShootingGlance.Damage))
            Skills._ShootingGlance.Damage = data_2._ShootingGlance.Damage;
        if (CheckNull(data_2._ShootingGlance.PeriodicDamage))
            Skills._ShootingGlance.PeriodicDamage = data_2._ShootingGlance.PeriodicDamage;
        if (CheckNull(data_2._ShootingGlance.Duration))
            Skills._ShootingGlance.Duration = data_2._ShootingGlance.Duration;
        if (CheckNull(data_2._ShootingGlance.ReloadTime))
            Skills._ShootingGlance.ReloadTime = data_2._ShootingGlance.ReloadTime;
        if (CheckNull(data_2._ShootingGlance.WillCost))
            Skills._ShootingGlance.WillCost = data_2._ShootingGlance.WillCost;
        if (CheckNull(data_2._ShootingGlance.CurrentPurchasePercent))
            Skills._ShootingGlance.CurrentPurchasePercent = data_2._ShootingGlance.CurrentPurchasePercent;
        if (CheckNull(data_2._ShootingGlance.SummCurrentPurchaseCount))
            Skills._ShootingGlance.SummCurrentPurchaseCount = data_2._ShootingGlance.SummCurrentPurchaseCount;

        for (int i = 0; i < Skills._ShootingGlance.Cost.Length; i++)
        {
            if (CheckNull(data_2._ShootingGlance.Cost[i]))
                Skills._ShootingGlance.Cost[i] = data_2._ShootingGlance.Cost[i];
            if (CheckNull(data_2._ShootingGlance.PurchaseCount[i]))
                Skills._ShootingGlance.PurchaseCount[i] = data_2._ShootingGlance.PurchaseCount[i];
        }
        #endregion

        #region BlackSauna
        if (CheckNull(data_2._BlackSauna.PeriodicDamage))
            Skills._BlackSauna.PeriodicDamage = data_2._BlackSauna.PeriodicDamage;
        if (CheckNull(data_2._BlackSauna.DecreasedHealthRegeneration))
            Skills._BlackSauna.DecreasedHealthRegeneration = data_2._BlackSauna.DecreasedHealthRegeneration;
        if (CheckNull(data_2._BlackSauna.Duration))
            Skills._BlackSauna.Duration = data_2._BlackSauna.Duration;
        if (CheckNull(data_2._BlackSauna.ReloadTime))
            Skills._BlackSauna.ReloadTime = data_2._BlackSauna.ReloadTime;
        if (CheckNull(data_2._BlackSauna.WillCost))
            Skills._BlackSauna.WillCost = data_2._BlackSauna.WillCost;
        if (CheckNull(data_2._BlackSauna.CurrentPurchasePercent))
            Skills._BlackSauna.CurrentPurchasePercent = data_2._BlackSauna.CurrentPurchasePercent;
        if (CheckNull(data_2._BlackSauna.SummCurrentPurchaseCount))
            Skills._BlackSauna.SummCurrentPurchaseCount = data_2._BlackSauna.SummCurrentPurchaseCount;

        for (int i = 0; i < Skills._BlackSauna.Cost.Length; i++)
        {
            if (CheckNull(data_2._BlackSauna.Cost[i]))
                Skills._BlackSauna.Cost[i] = data_2._BlackSauna.Cost[i];
            if (CheckNull(data_2._BlackSauna.PurchaseCount[i]))
                Skills._BlackSauna.PurchaseCount[i] = data_2._BlackSauna.PurchaseCount[i];
        }
        #endregion

        #region ArmorBreak
        if (CheckNull(data_2._ArmorBreak.Damage))
            Skills._ArmorBreak.Damage = data_2._ArmorBreak.Damage;
        if (CheckNull(data_2._ArmorBreak.DecreasedArmor))
            Skills._ArmorBreak.DecreasedArmor = data_2._ArmorBreak.DecreasedArmor;
        if (CheckNull(data_2._ArmorBreak.Duration))
            Skills._ArmorBreak.Duration = data_2._ArmorBreak.Duration;
        if (CheckNull(data_2._ArmorBreak.ReloadTime))
            Skills._ArmorBreak.ReloadTime = data_2._ArmorBreak.ReloadTime;
        if (CheckNull(data_2._ArmorBreak.WillCost))
            Skills._ArmorBreak.WillCost = data_2._ArmorBreak.WillCost;
        if (CheckNull(data_2._ArmorBreak.CurrentPurchasePercent))
            Skills._ArmorBreak.CurrentPurchasePercent = data_2._ArmorBreak.CurrentPurchasePercent;
        if (CheckNull(data_2._ArmorBreak.SummCurrentPurchaseCount))
            Skills._ArmorBreak.SummCurrentPurchaseCount = data_2._ArmorBreak.SummCurrentPurchaseCount;

        for (int i = 0; i < Skills._ArmorBreak.Cost.Length; i++)
        {
            if (CheckNull(data_2._ArmorBreak.Cost[i]))
                Skills._ArmorBreak.Cost[i] = data_2._ArmorBreak.Cost[i];
            if (CheckNull(data_2._ArmorBreak.PurchaseCount[i]))
                Skills._ArmorBreak.PurchaseCount[i] = data_2._ArmorBreak.PurchaseCount[i];
        }
        #endregion

        #region ZeusAnger
        if (CheckNull(data_2._ZeusAnger.PercentDamage))
            Skills._ZeusAnger.PercentDamage = data_2._ZeusAnger.PercentDamage;
        if (CheckNull(data_2._ZeusAnger.AttackCount))
            Skills._ZeusAnger.AttackCount = data_2._ZeusAnger.AttackCount;
        if (CheckNull(data_2._ZeusAnger.TimeBetweenAttacks))
            Skills._ZeusAnger.TimeBetweenAttacks = data_2._ZeusAnger.TimeBetweenAttacks;
        if (CheckNull(data_2._ZeusAnger.ReloadTime))
            Skills._ZeusAnger.ReloadTime = data_2._ZeusAnger.ReloadTime;
        if (CheckNull(data_2._ZeusAnger.WillCost))
            Skills._ZeusAnger.WillCost = data_2._ZeusAnger.WillCost;
        if (CheckNull(data_2._ZeusAnger.CurrentPurchasePercent))
            Skills._ZeusAnger.CurrentPurchasePercent = data_2._ZeusAnger.CurrentPurchasePercent;
        if (CheckNull(data_2._ZeusAnger.SummCurrentPurchaseCount))
            Skills._ZeusAnger.SummCurrentPurchaseCount = data_2._ZeusAnger.SummCurrentPurchaseCount;

        for (int i = 0; i < Skills._ZeusAnger.Cost.Length; i++)
        {
            if (CheckNull(data_2._ZeusAnger.Cost[i]))
                Skills._ZeusAnger.Cost[i] = data_2._ZeusAnger.Cost[i];
            if (CheckNull(data_2._ZeusAnger.PurchaseCount[i]))
                Skills._ZeusAnger.PurchaseCount[i] = data_2._ZeusAnger.PurchaseCount[i];
        }
        #endregion

        #region HelpPerun
        if (CheckNull(data_2._HelpPerun.AttackCount))
            Skills._HelpPerun.AttackCount = data_2._HelpPerun.AttackCount;
        if (CheckNull(data_2._HelpPerun.ReloadTime))
            Skills._HelpPerun.ReloadTime = data_2._HelpPerun.ReloadTime;
        if (CheckNull(data_2._HelpPerun.WillCost))
            Skills._HelpPerun.WillCost = data_2._HelpPerun.WillCost;
        if (CheckNull(data_2._HelpPerun.CurrentPurchasePercent))
            Skills._HelpPerun.CurrentPurchasePercent = data_2._HelpPerun.CurrentPurchasePercent;
        if (CheckNull(data_2._HelpPerun.SummCurrentPurchaseCount))
            Skills._HelpPerun.SummCurrentPurchaseCount = data_2._HelpPerun.SummCurrentPurchaseCount;

        for (int i = 0; i < Skills._HelpPerun.Cost.Length; i++)
        {
            if (CheckNull(data_2._HelpPerun.Cost[i]))
                Skills._HelpPerun.Cost[i] = data_2._HelpPerun.Cost[i];
            if (CheckNull(data_2._HelpPerun.PurchaseCount[i]))
                Skills._HelpPerun.PurchaseCount[i] = data_2._HelpPerun.PurchaseCount[i];
        }
        #endregion

        #region PrayerToTheGods
        if (CheckNull(data_2._PrayerToTheGods.PercentToStats))
            Skills._PrayerToTheGods.PercentToStats = data_2._PrayerToTheGods.PercentToStats;
        if (CheckNull(data_2._PrayerToTheGods.Duration))
            Skills._PrayerToTheGods.Duration = data_2._PrayerToTheGods.Duration;
        if (CheckNull(data_2._PrayerToTheGods.ReloadTime))
            Skills._PrayerToTheGods.ReloadTime = data_2._PrayerToTheGods.ReloadTime;
        if (CheckNull(data_2._PrayerToTheGods.WillCost))
            Skills._PrayerToTheGods.WillCost = data_2._PrayerToTheGods.WillCost;
        if (CheckNull(data_2._PrayerToTheGods.CurrentPurchasePercent))
            Skills._PrayerToTheGods.CurrentPurchasePercent = data_2._PrayerToTheGods.CurrentPurchasePercent;
        if (CheckNull(data_2._PrayerToTheGods.SummCurrentPurchaseCount))
            Skills._PrayerToTheGods.SummCurrentPurchaseCount = data_2._PrayerToTheGods.SummCurrentPurchaseCount;

        for (int i = 0; i < Skills._PrayerToTheGods.Cost.Length; i++)
        {
            if (CheckNull(data_2._PrayerToTheGods.Cost[i]))
                Skills._PrayerToTheGods.Cost[i] = data_2._PrayerToTheGods.Cost[i];
            if (CheckNull(data_2._PrayerToTheGods.PurchaseCount[i]))
                Skills._PrayerToTheGods.PurchaseCount[i] = data_2._PrayerToTheGods.PurchaseCount[i];
        }
        #endregion

        #region GeneralFee
        if (CheckNull(data_2._GeneralFee.PercentAttributes))
            Skills._GeneralFee.PercentToStats = data_2._GeneralFee.PercentAttributes;
        if (CheckNull(data_2._GeneralFee.Duration))
            Skills._GeneralFee.Duration = data_2._GeneralFee.Duration;
        if (CheckNull(data_2._GeneralFee.ReloadTime))
            Skills._GeneralFee.ReloadTime = data_2._GeneralFee.ReloadTime;
        if (CheckNull(data_2._GeneralFee.WillCost))
            Skills._GeneralFee.WillCost = data_2._GeneralFee.WillCost;
        if (CheckNull(data_2._GeneralFee.CurrentPurchasePercent))
            Skills._GeneralFee.CurrentPurchasePercent = data_2._GeneralFee.CurrentPurchasePercent;
        if (CheckNull(data_2._GeneralFee.SummCurrentPurchaseCount))
            Skills._GeneralFee.SummCurrentPurchaseCount = data_2._GeneralFee.SummCurrentPurchaseCount;

        for (int i = 0; i < Skills._GeneralFee.Cost.Length; i++)
        {
            if (CheckNull(data_2._GeneralFee.Cost[i]))
                Skills._GeneralFee.Cost[i] = data_2._GeneralFee.Cost[i];
            if (CheckNull(data_2._GeneralFee.PurchaseCount[i]))
                Skills._GeneralFee.PurchaseCount[i] = data_2._GeneralFee.PurchaseCount[i];
        }
        #endregion

        #endregion

        #region Heroes

        if (CheckNull(data_2.CurrentHeroIndex))
            Heroes.CurrentHeroIndex = data_2.CurrentHeroIndex;

        for (int heroIndex = 0; heroIndex < Heroes.HeroCount; heroIndex++)
        {
            if (CheckNull(data_2.RemainingTimeToResurrationHero[heroIndex]))
                Heroes.hero[heroIndex].RemainingTimeToResurrationHero = data_2.RemainingTimeToResurrationHero[heroIndex];
            if (CheckNull(data_2.Level[heroIndex]))
                Heroes.hero[heroIndex].Level = data_2.Level[heroIndex];
            if (CheckNull(data_2.EXPForNextLevel[heroIndex]))
                Heroes.hero[heroIndex].EXPForNextLevel = data_2.EXPForNextLevel[heroIndex];
            if (CheckNull(data_2.CurrentEXP[heroIndex]))
                Heroes.hero[heroIndex].ActualEXP = data_2.CurrentEXP[heroIndex];
            if (CheckNull(data_2.PumpingPoints[heroIndex]))
                Heroes.hero[heroIndex].PumpingPoints = data_2.PumpingPoints[heroIndex];

            if (CheckNull(data_2.MaxHealthBase[heroIndex]))
                Heroes.hero[heroIndex].MaxHealthBase = data_2.MaxHealthBase[heroIndex];
            if (CheckNull(data_2.MaxHealthFromLevel[heroIndex]))
                Heroes.hero[heroIndex].MaxHealthFromLevel = data_2.MaxHealthFromLevel[heroIndex];
            if (CheckNull(data_2.MaxHealthFromStrength[heroIndex]))
                Heroes.hero[heroIndex].MaxHealthFromStrength = data_2.MaxHealthFromStrength[heroIndex];
            if (CheckNull(data_2.MaxHealthFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].MaxHealthFromArmorSet = data_2.MaxHealthFromArmorSet[heroIndex];
            if (CheckNull(data_2.MaxHealthFromWeapon[heroIndex]))
                Heroes.hero[heroIndex].MaxHealthFromWeapon = data_2.MaxHealthFromWeapon[heroIndex];
            if (CheckNull(data_2.MaxHealthFromPerk[heroIndex]))
                Heroes.hero[heroIndex].MaxHealthFromPerk = data_2.MaxHealthFromPerk[heroIndex];
            if (CheckNull(data_2.ActualHealth[heroIndex]))
                Heroes.hero[heroIndex].ActualHealth = data_2.ActualHealth[heroIndex];

            if (CheckNull(data_2.HealthRegenerationBase[heroIndex]))
                Heroes.hero[heroIndex].HealthRegenerationBase = data_2.HealthRegenerationBase[heroIndex];
            if (CheckNull(data_2.HealthRegenerationFromStrength[heroIndex]))
                Heroes.hero[heroIndex].HealthRegenerationFromStrength = data_2.HealthRegenerationFromStrength[heroIndex];
            if (CheckNull(data_2.HealthRegenerationFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].HealthRegenerationFromArmorSet = data_2.HealthRegenerationFromArmorSet[heroIndex];
            if (CheckNull(data_2.HealthRegenerationFromWeapon[heroIndex]))
                Heroes.hero[heroIndex].HealthRegenerationFromWeapon = data_2.HealthRegenerationFromWeapon[heroIndex];
            if (CheckNull(data_2.HealthRegenerationFromPerk[heroIndex]))
                Heroes.hero[heroIndex].HealthRegenerationFromPerk = data_2.HealthRegenerationFromPerk[heroIndex];

            if (CheckNull(data_2.MaxStaminaBase[heroIndex]))
                Heroes.hero[heroIndex].MaxStaminaBase = data_2.MaxStaminaBase[heroIndex];
            if (CheckNull(data_2.MaxStaminaFromLevel[heroIndex]))
                Heroes.hero[heroIndex].MaxStaminaFromLevel = data_2.MaxStaminaFromLevel[heroIndex];
            if (CheckNull(data_2.MaxStaminaFromStrength[heroIndex]))
                Heroes.hero[heroIndex].MaxStaminaFromStrength = data_2.MaxStaminaFromStrength[heroIndex];
            if (CheckNull(data_2.MaxStaminaFromDexterity[heroIndex]))
                Heroes.hero[heroIndex].MaxStaminaFromDexterity = data_2.MaxStaminaFromDexterity[heroIndex];
            if (CheckNull(data_2.MaxStaminaFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].MaxStaminaFromArmorSet = data_2.MaxStaminaFromArmorSet[heroIndex];
            if (CheckNull(data_2.MaxStaminaFromWeapon[heroIndex]))
                Heroes.hero[heroIndex].MaxStaminaFromWeapon = data_2.MaxStaminaFromWeapon[heroIndex];
            if (CheckNull(data_2.MaxStaminaFromPerk[heroIndex]))
                Heroes.hero[heroIndex].MaxStaminaFromPerk = data_2.MaxStaminaFromPerk[heroIndex];
            if (CheckNull(data_2.ActualStamina[heroIndex]))
                Heroes.hero[heroIndex].ActualStamina = data_2.ActualStamina[heroIndex];

            if (CheckNull(data_2.CostStaminaToAttack[heroIndex]))
                Heroes.hero[heroIndex].CostStaminaToAttack = data_2.CostStaminaToAttack[heroIndex];
            if (CheckNull(data_2.StaminaRegenerationBase[heroIndex]))
                Heroes.hero[heroIndex].StaminaRegenerationBase = data_2.StaminaRegenerationBase[heroIndex];
            if (CheckNull(data_2.StaminaRegenerationFromStrength[heroIndex]))
                Heroes.hero[heroIndex].StaminaRegenerationFromStrength = data_2.StaminaRegenerationFromStrength[heroIndex];
            if (CheckNull(data_2.StaminaRegenerationFromDexterity[heroIndex]))
                Heroes.hero[heroIndex].StaminaRegenerationFromDexterity = data_2.StaminaRegenerationFromDexterity[heroIndex];
            if (CheckNull(data_2.StaminaRegenerationFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].StaminaRegenerationFromArmorSet = data_2.StaminaRegenerationFromArmorSet[heroIndex];
            if (CheckNull(data_2.StaminaRegenerationFromWeapon[heroIndex]))
                Heroes.hero[heroIndex].StaminaRegenerationFromWeapon = data_2.StaminaRegenerationFromWeapon[heroIndex];
            if (CheckNull(data_2.StaminaRegenerationFromPerk[heroIndex]))
                Heroes.hero[heroIndex].StaminaRegenerationFromPerk = data_2.StaminaRegenerationFromPerk[heroIndex];

            if (CheckNull(data_2.MaxWillBase[heroIndex]))
                Heroes.hero[heroIndex].MaxWillBase = data_2.MaxWillBase[heroIndex];
            if (CheckNull(data_2.MaxWillFromLevel[heroIndex]))
                Heroes.hero[heroIndex].MaxWillFromLevel = data_2.MaxWillFromLevel[heroIndex];
            if (CheckNull(data_2.MaxWillFromIntellect[heroIndex]))
                Heroes.hero[heroIndex].MaxWillFromIntellect = data_2.MaxWillFromIntellect[heroIndex];
            if (CheckNull(data_2.MaxWillFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].MaxWillFromArmorSet = data_2.MaxWillFromArmorSet[heroIndex];
            if (CheckNull(data_2.MaxWillFromPerk[heroIndex]))
                Heroes.hero[heroIndex].MaxWillFromPerk = data_2.MaxWillFromPerk[heroIndex];
            if (CheckNull(data_2.ActualWill[heroIndex]))
                Heroes.hero[heroIndex].ActualWill = data_2.ActualWill[heroIndex];

            if (CheckNull(data_2.WillRegenerationBase[heroIndex]))
                Heroes.hero[heroIndex].WillRegenerationBase = data_2.WillRegenerationBase[heroIndex];
            if (CheckNull(data_2.WillRegenerationFromIntellect[heroIndex]))
                Heroes.hero[heroIndex].WillRegenerationFromIntellect = data_2.WillRegenerationFromIntellect[heroIndex];
            if (CheckNull(data_2.WillRegenerationFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].WillRegenerationFromArmorSet = data_2.WillRegenerationFromArmorSet[heroIndex];
            if (CheckNull(data_2.WillRegenerationFromPerk[heroIndex]))
                Heroes.hero[heroIndex].WillRegenerationFromPerk = data_2.WillRegenerationFromPerk[heroIndex];

            if (CheckNull(data_2.DamageBase[heroIndex]))
                Heroes.hero[heroIndex].DamageBase = data_2.DamageBase[heroIndex];
            if (CheckNull(data_2.DamageFromLevel[heroIndex]))
                Heroes.hero[heroIndex].DamageFromLevel = data_2.DamageFromLevel[heroIndex];
            if (CheckNull(data_2.DamageFromWeapon[heroIndex]))
                Heroes.hero[heroIndex].DamageFromWeapon = data_2.DamageFromWeapon[heroIndex];
            if (CheckNull(data_2.DamageFromStrength[heroIndex]))
                Heroes.hero[heroIndex].DamageFromStrength = data_2.DamageFromStrength[heroIndex];
            if (CheckNull(data_2.DamageFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].DamageFromArmorSet = data_2.DamageFromArmorSet[heroIndex];
            if (CheckNull(data_2.DamageFromPerk[heroIndex]))
                Heroes.hero[heroIndex].DamageFromPerk = data_2.DamageFromPerk[heroIndex];

            if (CheckNull(data_2.ArmorBase[heroIndex]))
                Heroes.hero[heroIndex].ArmorBase = data_2.ArmorBase[heroIndex];
            if (CheckNull(data_2.ArmorFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].ArmorFromArmorSet = data_2.ArmorFromArmorSet[heroIndex];
            if (CheckNull(data_2.ArmorFromWeapon[heroIndex]))
                Heroes.hero[heroIndex].ArmorFromWeapon = data_2.ArmorFromWeapon[heroIndex];
            if (CheckNull(data_2.ArmorFromDexterity[heroIndex]))
                Heroes.hero[heroIndex].ArmorFromDexterity = data_2.ArmorFromDexterity[heroIndex];
            if (CheckNull(data_2.ArmorFromPerk[heroIndex]))
                Heroes.hero[heroIndex].ArmorFromPerk = data_2.ArmorFromPerk[heroIndex];

            if (CheckNull(data_2.EvasionBase[heroIndex]))
                Heroes.hero[heroIndex].EvasionBase = data_2.EvasionBase[heroIndex];
            if (CheckNull(data_2.EvasionFromDexterity[heroIndex]))
                Heroes.hero[heroIndex].EvasionFromDexterity = data_2.EvasionFromDexterity[heroIndex];
            if (CheckNull(data_2.EvasionFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].EvasionFromArmorSet = data_2.EvasionFromArmorSet[heroIndex];
            if (CheckNull(data_2.EvasionFromWeapon[heroIndex]))
                Heroes.hero[heroIndex].EvasionFromWeapon = data_2.EvasionFromWeapon[heroIndex];
            if (CheckNull(data_2.EvasionFromPerk[heroIndex]))
                Heroes.hero[heroIndex].EvasionFromPerk = data_2.EvasionFromPerk[heroIndex];

            if (CheckNull(data_2.StrengthBase[heroIndex]))
                Heroes.hero[heroIndex].StrengthBase = data_2.StrengthBase[heroIndex];
            if (CheckNull(data_2.StrengthFromLevel[heroIndex]))
                Heroes.hero[heroIndex].StrengthFromLevel = data_2.StrengthFromLevel[heroIndex];
            if (CheckNull(data_2.StrengthFromPumpingPoints[heroIndex]))
                Heroes.hero[heroIndex].StrengthFromPumpingPoints = data_2.StrengthFromPumpingPoints[heroIndex];
            if (CheckNull(data_2.StrengthFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].StrengthFromArmorSet = data_2.StrengthFromArmorSet[heroIndex];
            if (CheckNull(data_2.StrengthFromWeapon[heroIndex]))
                Heroes.hero[heroIndex].StrengthFromWeapon = data_2.StrengthFromWeapon[heroIndex];
            if (CheckNull(data_2.StrengthFromPerk[heroIndex]))
                Heroes.hero[heroIndex].StrengthFromPerk = data_2.StrengthFromPerk[heroIndex];

            if (CheckNull(data_2.DexterityBase[heroIndex]))
                Heroes.hero[heroIndex].DexterityBase = data_2.DexterityBase[heroIndex];
            if (CheckNull(data_2.DexterityFromLevel[heroIndex]))
                Heroes.hero[heroIndex].DexterityFromLevel = data_2.DexterityFromLevel[heroIndex];
            if (CheckNull(data_2.DexterityFromPumpingPoints[heroIndex]))
                Heroes.hero[heroIndex].DexterityFromPumpingPoints = data_2.DexterityFromPumpingPoints[heroIndex];
            if (CheckNull(data_2.DexterityFromWeapon[heroIndex]))
                Heroes.hero[heroIndex].DexterityFromWeapon = data_2.DexterityFromWeapon[heroIndex];
            if (CheckNull(data_2.DexterityFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].DexterityFromArmorSet = data_2.DexterityFromArmorSet[heroIndex];
            if (CheckNull(data_2.DexterityFromPerk[heroIndex]))
                Heroes.hero[heroIndex].DexterityFromPerk = data_2.DexterityFromPerk[heroIndex];

            if (CheckNull(data_2.IntellectBase[heroIndex]))
                Heroes.hero[heroIndex].IntellectBase = data_2.IntellectBase[heroIndex];
            if (CheckNull(data_2.IntellectFromLevel[heroIndex]))
                Heroes.hero[heroIndex].IntellectFromLevel = data_2.IntellectFromLevel[heroIndex];
            if (CheckNull(data_2.IntellectFromPumpingPoints[heroIndex]))
                Heroes.hero[heroIndex].IntellectFromPumpingPoints = data_2.IntellectFromPumpingPoints[heroIndex];
            if (CheckNull(data_2.IntellectFromArmorSet[heroIndex]))
                Heroes.hero[heroIndex].IntellectFromArmorSet = data_2.IntellectFromArmorSet[heroIndex];
            if (CheckNull(data_2.IntellectFromPerk[heroIndex]))
                Heroes.hero[heroIndex].IntellectFromPerk = data_2.IntellectFromPerk[heroIndex];
            if (CheckNull(data_2.HeroBought[heroIndex]))
                Heroes.hero[heroIndex].Bought = data_2.HeroBought[heroIndex];
        }
        #endregion

        #region Learning
        if (CheckNull(data_2.RussiansLessonIsFinished))
            Learning.RussiansLessonIsFinished = data_2.RussiansLessonIsFinished;
        #endregion

        #endregion
    }

    private void DisplayCurrentSaveState()
    {
        StartCoroutine(WaitSaving());

        //if (IsSavingData_1 || IsSavingData_2)
        //{
        //    StartCoroutine(WaitSaving());
        //}
        //else
        //{
        //    StartCoroutine(DisplayCompletedMessage());
        //}

        IEnumerator WaitSaving()
        {
            _saveButton.interactable = false;
            _saveButton.image.color = _loadingColor;

            for (int i = 0; i < 2; i++)
            {
                _saveButtonText.text = $"Подождите.";
                yield return new WaitForSeconds(0.2f);
                _saveButtonText.text = $"Подождите..";
                yield return new WaitForSeconds(0.2f);
                _saveButtonText.text = $"Подождите...";
                yield return new WaitForSeconds(0.2f);
            }

            StartCoroutine(DisplayCompletedMessage());
        }

        IEnumerator DisplayCompletedMessage()
        {
            _saveButtonText.text = $"Сохранено";
            _saveButton.image.color = _savedColor;
            _saveButton.interactable = false;

            yield return new WaitForSeconds(1f);

            _saveButtonText.text = $"Сохранить";
            _saveButton.image.color = _standardColor;
            _saveButton.interactable = true;
        }
    }

    private void InitializeData_1()
    {
        Game.Init();
        Disclaimer?.Init();
        GlobalUpgrades?.Init();
        MainAudio?.Init();
        Music?.Init();
        AudioEffects?.Init();
        MoneyMenu?.Init();

        Upgrades?.Init();
        MemeShop?.Init();
        ClickADReward?.Init();
        IdleADReward?.Init();

        Achievements_CombinedUniverse?.Init();
        Achievements_R_vs_L?.Init();
        Achievements_GenaBukinUniverse?.Init();
        Achievements_GenaGorinUniverse?.Init();
        Achievements_PapichUniverse?.Init();
        Achievements_UniverUniverse?.Init();
        Achievements_ParanormalUniverse?.Init();
        Achievements_RockUniverse?.Init();  
        Achievements_ShrekUniverse?.Init();

        Multiverse?.Init();

        MainMemeShop?.Init();
        Calendar?.Init();
        CasesMenu?.Init();

        Debug.Log("DATA_1 INITIALIZED");
    }

    private bool CheckNull(object obj)
    {
        if (obj != null)
            return true;
        return false;
    }
}