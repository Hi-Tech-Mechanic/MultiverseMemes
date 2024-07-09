using UnityEngine;

[System.Serializable]
public class PlayerData_1
{
    #region System
    public bool[] FirstLaunchScene = { true, true, true, true, true, true, true, true, true };
    public bool FirstLauntGame;
    public long[] LastPlayingTime = new long[Game.ScenesCount];
    public long LastLaunchGameTime;
    public int VisitingDays;

    public bool StartLessonIsComplete;
    #endregion

    #region Options
    public int[] CurrentImageIndex = new int[Game.ScenesCount];
    public bool AudioIsOn;

    public int SelectedMusicIndex;
    public float CurrentMusicTime;
    public float MusicVolume;
    public float MusicPitch;
    public bool MusicIsMute;

    public float AudioEffectsVolume;
    public float AudioEffectsPitch;
    public bool AudioEffectsIsMute;
    public bool IdleAudioEffectIsOn;
    public bool AudioEffectsIsLayering;
    #endregion

    #region MoneyMenu 
    public float MemeCoins;
    public float[] MoneyCapital = new float[Game.ScenesCount];
    public float[] ClickIncome = new float[Game.ScenesCount];
    public float[] TickIncome = new float[Game.ScenesCount];
    public float[] AccumulatedMoney = new float[Game.ScenesCount];
    #endregion

    #region Upgrades
    public int[,] AccumulatedPurchasedClickUpgrades = new int[Game.ScenesCount, Game.MaxCellCountInGame];
    public int[,] AccumulatedPurchasedIdleUpgrades = new int[Game.ScenesCount, Game.MaxCellCountInGame];

    [System.Serializable]
    public class Sample
    {
        public float[,] Income = new float[Game.ScenesCount, Game.MaxCellCountInGame];
        public float[,] SummIncome = new float[Game.ScenesCount, Game.MaxCellCountInGame];
        public float[,] Prices = new float[Game.ScenesCount, Game.MaxCellCountInGame];
        public int[,] MaxUpgradeAmount = new int[Game.ScenesCount, Game.MaxCellCountInGame];
        public int[,] PurchasedUpgradeAmount = new int[Game.ScenesCount, Game.MaxCellCountInGame];
        public int[,] CurrentBuyAmount = new int[Game.ScenesCount, Game.MaxCellCountInGame];
        public int[,] ReceivedLevels = new int[Game.ScenesCount, Game.MaxCellCountInGame];
    }

    [System.Serializable]
    public class Click : Sample { }
    public Click _Click = new();

    [System.Serializable]
    public class Idle : Sample { }
    public Idle _Idle = new();
    #endregion

    #region GlobalUpgrades
    public float UpgradeCostMultiplier;
    public float IncomeMultiplier;
    public float TickTime;
    public float CritChance;
    public float CritStrength;
    public float ADRewardMultiplier;
    public int AbsenceTime;
    public int AdvertisingBuffTime;
    public int MemeClipCount;
    public float[] Prices = new float[GlobalUpgrades.GlobalUpgradesCount];
    public int[] PurchasedCounter = new int[GlobalUpgrades.GlobalUpgradesCount];
    #endregion

    #region Achievements
    public bool[,] AchievementsRewardsIsClaimed = new bool[Game.ScenesCount, Game.MaxAchievementsCountInGame];
    public float[] CurrentUniverseProgress = new float[Game.ScenesCount];
    public int[] PurchasedMemeClips = new int[Game.ScenesCount];
    public int CountWatchedAD;
    public float AccumulatedTime;
    public float AccumulatedClicks;
    #endregion

    #region Buffs
    public float ClickBuffTimeRemaining;
    public float IdleBuffTimeRemaining;
    public float IdleCachedADRewardMultiplier;
    public float ClickCachedADRewardMultiplier;
    #endregion

    #region MemeShop
    public int[] SelectedClipCount = new int[Game.ScenesCount];
    public bool[,] ClipIsSelected = new bool[Game.ScenesCount, MemeShop.MaxClipLength];
    public bool[,] ClipIsAvailable = new bool[Game.ScenesCount, MemeShop.MaxClipLength];
    public bool[,] ClipIsSelected_2 = new bool[Game.ScenesCount, MemeShop.MaxClipLength];
    #endregion

    #region Calendar
    public bool[] CalendarRewardIsClaimed = new bool[7];
    public bool[] RewardIsUnlock = new bool[7];
    #endregion

    public PlayerData_1(MoneyMenu moneyMenu, Upgrades upgrades, GlobalUpgrades globalUpgrades,
        MemeShop memeShop, MusicOptions musicOptions, AudioEffectsOptions audioEffectsOptions)
    {
        #region System
        FirstLaunchGameInitialize();
        Game.FirstLaunchScene[Game.CurrentScene] = false;
        FirstLaunchScene = Game.FirstLaunchScene;
        LastPlayingTime = Game.GetLastPlayingTime();
        LastLaunchGameTime = Game.LastLaunchGameTime;
        VisitingDays = Game.VisitingDays;

        StartLessonIsComplete = StartLearning.StartLessonIsComplete;
        #endregion

        #region Options
        CurrentImageIndex = SwitchBackground.CurrentImageIndex;
        AudioIsOn = MainAudio.AudioIsOn;

        SelectedMusicIndex = musicOptions.SelectedMusicIndex;
        CurrentMusicTime = musicOptions.CurrentMusicTime;
        MusicVolume = musicOptions.MusicVolume;
        MusicPitch = musicOptions.MusicPitch;
        MusicIsMute = musicOptions.MusicIsMute;

        AudioEffectsVolume = audioEffectsOptions.AudioEffectsVolume;
        AudioEffectsPitch = audioEffectsOptions.AudioEffectsPitch;
        AudioEffectsIsMute = audioEffectsOptions.AudioEffectsIsMute;
        IdleAudioEffectIsOn = audioEffectsOptions.IdleAudioEffectsIsOn;
        AudioEffectsIsLayering = audioEffectsOptions.AudioEffectsIsLayering;
        #endregion

        #region Buffs
        ClickBuffTimeRemaining = ClickADReward.BuffTimeRemaining;
        IdleBuffTimeRemaining = IdleADReward.BuffTimeRemaining;
        ClickCachedADRewardMultiplier = ClickADReward.CachedADMultiplier;
        IdleCachedADRewardMultiplier = IdleADReward.CachedADMultiplier;
        #endregion

        #region GlobalUpgrades
        UpgradeCostMultiplier = globalUpgrades.UpgradeCostMultiplier;
        IncomeMultiplier = globalUpgrades.IncomeMultiplier;
        TickTime = globalUpgrades.TickTime;
        CritChance = globalUpgrades.CritChance;
        CritStrength = globalUpgrades.CritStrength;
        ADRewardMultiplier = globalUpgrades.ADRewardMultiplier;
        Prices = globalUpgrades.Prices;
        PurchasedCounter = globalUpgrades.PurchasedCounter;
        AbsenceTime = globalUpgrades.AbsenceTime;
        AdvertisingBuffTime = globalUpgrades.AdvertisingBuffTime;
        MemeClipCount = globalUpgrades.MemeClipCount;
        #endregion

        #region Achievements
        AchievementsRewardsIsClaimed = GlobalUpgrades.AchievementsRewardsIsClaimed;
        CurrentUniverseProgress = Multiverse.CurrentUniverseProgress;
        AccumulatedTime = Game.AccumulatedTime;
        AccumulatedClicks = Game.AccumulatedClicks;
        PurchasedMemeClips = memeShop.PurchasedMemeClips;
        CountWatchedAD = Game.CountWatchedAD;
        #endregion

        #region MoneyMenu
        MemeCoins = moneyMenu.GetMemeCoins();
        MoneyCapital = moneyMenu.GetMoneyCapital();
        ClickIncome = moneyMenu.GetClickIncome();
        TickIncome = moneyMenu.GetIdleIncomePerTick();
        AccumulatedMoney = moneyMenu.AccumulatedMoney;
        #endregion

        #region Upgrades
        AccumulatedPurchasedClickUpgrades = upgrades.AccumulatedPurchasedClickUpgrades;
        AccumulatedPurchasedIdleUpgrades = upgrades.AccumulatedPurchasedIdleUpgrades;

        _Click.Income = upgrades._Click.Income;
        _Click.SummIncome = upgrades._Click.SummIncome;
        _Click.Prices = upgrades._Click.Prices;
        _Click.PurchasedUpgradeAmount = upgrades._Click.PurchasedUpgradeAmount;
        _Click.CurrentBuyAmount = upgrades._Click.CurrentBuyAmount;
        _Click.ReceivedLevels = upgrades._Click.ReceivedLevels;

        _Idle.Income = upgrades._Idle.Income;
        _Idle.SummIncome = upgrades._Idle.SummIncome;
        _Idle.Prices = upgrades._Idle.Prices;
        _Idle.PurchasedUpgradeAmount = upgrades._Idle.PurchasedUpgradeAmount;
        _Idle.CurrentBuyAmount = upgrades._Idle.CurrentBuyAmount;
        _Idle.ReceivedLevels = upgrades._Idle.ReceivedLevels;
        #endregion

        #region MemeShop
        SelectedClipCount = memeShop.SelectedClipCount;
        ClipIsSelected = memeShop.ClipIsSelected;
        ClipIsAvailable = memeShop.ClipIsAvailable;
        ClipIsSelected_2 = CombinedUniverse.MemeShop.ClipIsSelected;
        #endregion

        #region Calendar
        CalendarRewardIsClaimed = Calendar.CalendarRewardIsClaimed;
        RewardIsUnlock = Calendar.RewardIsUnlock;
        #endregion
    }

    private void FirstLaunchGameInitialize()
    {
        if (Game.FirstLaunchGame == true)
        {
            Debug.Log("FIRST LAUNCH");
            Game.FirstLaunchGame = false;
            FirstLauntGame = Game.FirstLaunchGame;
        }
    }
}