using Russians_vs_Lizards;
using System;

[System.Serializable]
public class PlayerData_2
{
    #region Russians_vs_Lizards

    #region Battle
    public int EnemiesInStage;
    public int CurrentStage;
    public int MaxOpenStage;
    public int ResetCount;
    public bool FinalBossIsDead;
    #endregion

    #region Facilities
    public float FaithCurrency;
    public float AncestralPower;
    public float FaithMultiplier;
    #endregion

    #region Achievements
    public int AccumulatedKills;
    public float AccumulatedFaith;
    public float AccumulatedAncestralPower;
    public int AccumulatedHeroes;
    public int AccumulatedSkills;
    public int AccumulatedArmors;
    public int AccumulatedWeapons;
    public int AccumulatedPerks;
    public int AccumulatedStages;

    public bool[] WeaponIsUnlocked = new bool[Weapons.WeaponCount];
    public bool[] ArmorIsUnlocked = new bool[3];
    public bool[] SkillIsUnlocked = new bool[Skills.SkillsCount];
    public bool[] HeroIsUnlocked = new bool[Heroes.HeroCount];
    public int SavedMaxOpenStage;
    #endregion

    #region Items
    public int BaikalWaterCount; 
    public int[] PotionsCount = new int[3];
    #endregion

    #region Buffs
    public float[] BaikalWaterTimer;
    public float[,] AdditiveStatsFromBaikalWater = new float[Heroes.HeroCount, 3];

    public float[,] CachedAdditiveStats_PrayerToTheGods = new float[Heroes.HeroCount, 3];
    public float[] CachedBuffTime_PrayerToTheGods = new float[Heroes.HeroCount];
    public float[,] CachedAdditiveStats_GeneralFee = new float[Heroes.HeroCount, 3];
    public float[] CachedBuffTime_GeneralFee = new float[Heroes.HeroCount];
    #endregion

    #region Armors

    [Serializable]
    public class ArmorSet
    {
        public float[] StatValue;
        public float[] StatUpgradeCost;
        public int[] CurrentUpgradeCount;
        public bool Bought;
        public bool Selected;
    }
    public ArmorSet[] _ArmorSet = new ArmorSet[3];
    #endregion

    #region Weapons
    [Serializable]
    public class Weapons_
    {
        public float[,] StatsValue = new float[Weapons.WeaponCount, Weapons.WeaponStatsCount];
        public float[,] StatsUpgradeCost = new float[Weapons.WeaponCount, Weapons.WeaponStatsCount];
        public int[,] CurrentUpgradeCount = new int[Weapons.WeaponCount, Weapons.WeaponStatsCount];
        public bool[] Bought = new bool[Weapons.WeaponCount];
        public bool[] Selected = new bool[Weapons.WeaponCount];
    }
    public Weapons_ _Weapons = new();
    #endregion

    #region Skills

    public int[] SelectedSkillIndex = new int[Skills.PlasesForSkillsCount];
    public float[] SavedReloadTimeForSelectedSkill = new float[Skills.PlasesForSkillsCount];
    public bool[] BoughtPlasesForSkills = new bool[Skills.PlasesForSkillsCount];
    public bool[] BoughtSkills = new bool[Skills.SkillsCount];
    public bool[] SelectedSkill = new bool[Skills.SkillsCount];

    [Serializable]
    public class BallClamp
    {
        public float Damage;
        public float StunDuration;
        public float ReloadTime;
        public float CurrentPurchasePercent;
        public int WillCost;
        public int SummCurrentPurchaseCount;

        public int[] PurchaseCount = new int[3];
        public int[] Cost = new int[3];
    }
    public BallClamp _BallClamp = new();

    [Serializable]
    public class CumBombardment
    {
        public float Damage;
        public float PeriodicDamage;
        public float Duration;
        public float ReloadTime;
        public float CurrentPurchasePercent;
        public int WillCost;
        public int SummCurrentPurchaseCount;

        public int[] PurchaseCount = new int[4];
        public int[] Cost = new int[4];
    }
    public CumBombardment _ShootingGlance = new();

    [Serializable]
    public class BlackSauna
    {
        public float PeriodicDamage;
        public float DecreasedHealthRegeneration;
        public float Duration;
        public float ReloadTime;
        public float CurrentPurchasePercent;
        public int WillCost;
        public int SummCurrentPurchaseCount;

        public int[] PurchaseCount = new int[4];
        public int[] Cost = new int[4];
    }
    public BlackSauna _BlackSauna = new();

    [Serializable]
    public class ArmorBreak
    {
        public float Damage;
        public float DecreasedArmor;
        public float Duration;
        public float ReloadTime;
        public float CurrentPurchasePercent;
        public int WillCost;
        public int SummCurrentPurchaseCount;

        public int[] PurchaseCount = new int[4];
        public int[] Cost = new int[4];
    }
    public ArmorBreak _ArmorBreak = new();

    [Serializable]
    public class ZeusAnger
    {
        public float PercentDamage;
        public float TimeBetweenAttacks;
        public int AttackCount;
        public float ReloadTime;
        public float CurrentPurchasePercent;
        public int WillCost;
        public int SummCurrentPurchaseCount;

        public int[] PurchaseCount = new int[4];
        public int[] Cost = new int[4];
    }
    public ZeusAnger _ZeusAnger = new();

    [Serializable]
    public class HelpPerun
    {
        public int AttackCount;
        public float ReloadTime;
        public float CurrentPurchasePercent;
        public int WillCost;
        public int SummCurrentPurchaseCount;

        public int[] PurchaseCount = new int[2];
        public int[] Cost = new int[2];
    }
    public HelpPerun _HelpPerun = new();

    [Serializable]
    public class PrayerToTheGods
    {
        public float PercentToStats;
        public float Duration;
        public float ReloadTime;
        public float CurrentPurchasePercent;
        public int WillCost;
        public int SummCurrentPurchaseCount;

        public int[] PurchaseCount = new int[3];
        public int[] Cost = new int[3];
    }
    public PrayerToTheGods _PrayerToTheGods = new();

    [Serializable]
    public class GeneralFee
    {
        public float PercentAttributes;
        public float Duration;
        public float ReloadTime;
        public float CurrentPurchasePercent;
        public int WillCost;
        public int SummCurrentPurchaseCount;

        public int[] PurchaseCount = new int[3];
        public int[] Cost = new int[3];
    }
    public GeneralFee _GeneralFee = new();
    #endregion

    #region PerkTree
    [Serializable] 
    public class PerkTree_
    {
        public float[] Summ = new float[PerkTree.PerkCount];
        public int[] CurrentUpgradeIndex = new int[PerkTree.PerkCount];
    }
    public PerkTree_ _PerkTree = new();
    #endregion

    #region Hero
    public float[] RemainingTimeToResurrationHero = new float[Heroes.HeroCount];
    public int CurrentHeroIndex;
    public bool[] HeroBought = new bool[Heroes.HeroCount];
    public int[] Level = new int[Heroes.HeroCount];
    public int[] PumpingPoints = new int[Heroes.HeroCount];
    public float[] CurrentEXP = new float[Heroes.HeroCount];
    public float[] EXPForNextLevel = new float[Heroes.HeroCount];
    public float[] PriceHero = new float[Heroes.HeroCount];

    public float[] ActualHealth = new float[Heroes.HeroCount];
    public float[] MaxHealthBase = new float[Heroes.HeroCount];
    public float[] MaxHealthFromLevel = new float[Heroes.HeroCount];
    public float[] MaxHealthFromStrength = new float[Heroes.HeroCount];
    public float[] MaxHealthFromArmorSet = new float[Heroes.HeroCount];
    public float[] MaxHealthFromWeapon = new float[Heroes.HeroCount];
    public float[] MaxHealthFromPerk = new float[Heroes.HeroCount];

    public float[] HealthRegenerationBase = new float[Heroes.HeroCount];
    public float[] HealthRegenerationFromStrength = new float[Heroes.HeroCount];
    public float[] HealthRegenerationFromArmorSet = new float[Heroes.HeroCount];
    public float[] HealthRegenerationFromWeapon = new float[Heroes.HeroCount];
    public float[] HealthRegenerationFromPerk = new float[Heroes.HeroCount];

    public float[] CostStaminaToAttack = new float[Heroes.HeroCount];
    public float[] ActualStamina = new float[Heroes.HeroCount];
    public float[] MaxStaminaBase = new float[Heroes.HeroCount];
    public float[] MaxStaminaFromLevel = new float[Heroes.HeroCount];
    public float[] MaxStaminaFromStrength = new float[Heroes.HeroCount];
    public float[] MaxStaminaFromDexterity = new float[Heroes.HeroCount];
    public float[] MaxStaminaFromArmorSet = new float[Heroes.HeroCount];
    public float[] MaxStaminaFromWeapon = new float[Heroes.HeroCount];
    public float[] MaxStaminaFromPerk = new float[Heroes.HeroCount];

    public float[] StaminaRegenerationBase = new float[Heroes.HeroCount];
    public float[] StaminaRegenerationFromStrength = new float[Heroes.HeroCount];
    public float[] StaminaRegenerationFromDexterity = new float[Heroes.HeroCount];
    public float[] StaminaRegenerationFromArmorSet = new float[Heroes.HeroCount];
    public float[] StaminaRegenerationFromWeapon = new float[Heroes.HeroCount];
    public float[] StaminaRegenerationFromPerk = new float[Heroes.HeroCount];

    public float[] ActualWill = new float[Heroes.HeroCount];
    public float[] MaxWillBase = new float[Heroes.HeroCount];
    public float[] MaxWillFromLevel = new float[Heroes.HeroCount];
    public float[] MaxWillFromIntellect = new float[Heroes.HeroCount];
    public float[] MaxWillFromArmorSet = new float[Heroes.HeroCount];
    public float[] MaxWillFromPerk = new float[Heroes.HeroCount];

    public float[] WillRegenerationBase = new float[Heroes.HeroCount];
    public float[] WillRegenerationFromIntellect = new float[Heroes.HeroCount];
    public float[] WillRegenerationFromArmorSet = new float[Heroes.HeroCount];
    public float[] WillRegenerationFromPerk = new float[Heroes.HeroCount];

    public float[] DamageBase = new float[Heroes.HeroCount];
    public float[] DamageFromLevel = new float[Heroes.HeroCount];
    public float[] DamageFromWeapon = new float[Heroes.HeroCount];
    public float[] DamageFromStrength = new float[Heroes.HeroCount];
    public float[] DamageFromArmorSet = new float[Heroes.HeroCount];
    public float[] DamageFromPerk = new float[Heroes.HeroCount];

    public float[] ArmorBase = new float[Heroes.HeroCount];
    public float[] ArmorFromArmorSet = new float[Heroes.HeroCount];
    public float[] ArmorFromWeapon = new float[Heroes.HeroCount];
    public float[] ArmorFromDexterity = new float[Heroes.HeroCount];
    public float[] ArmorFromPerk = new float[Heroes.HeroCount];
    
    public float[] EvasionBase = new float[Heroes.HeroCount];
    public float[] EvasionFromDexterity = new float[Heroes.HeroCount];
    public float[] EvasionFromArmorSet = new float[Heroes.HeroCount];
    public float[] EvasionFromWeapon = new float[Heroes.HeroCount];
    public float[] EvasionFromPerk = new float[Heroes.HeroCount];
    
    public float[] StrengthBase = new float[Heroes.HeroCount];
    public float[] StrengthFromLevel = new float[Heroes.HeroCount];
    public float[] StrengthFromPumpingPoints = new float[Heroes.HeroCount];
    public float[] StrengthFromArmorSet = new float[Heroes.HeroCount];
    public float[] StrengthFromWeapon = new float[Heroes.HeroCount];
    public float[] StrengthFromPerk = new float[Heroes.HeroCount];
    
    public float[] DexterityBase = new float[Heroes.HeroCount];
    public float[] DexterityFromLevel = new float[Heroes.HeroCount];
    public float[] DexterityFromPumpingPoints = new float[Heroes.HeroCount];
    public float[] DexterityFromWeapon = new float[Heroes.HeroCount];
    public float[] DexterityFromArmorSet = new float[Heroes.HeroCount];
    public float[] DexterityFromPerk = new float[Heroes.HeroCount];
    
    public float[] IntellectBase = new float[Heroes.HeroCount];
    public float[] IntellectFromLevel = new float[Heroes.HeroCount];
    public float[] IntellectFromPumpingPoints = new float[Heroes.HeroCount];
    public float[] IntellectFromArmorSet = new float[Heroes.HeroCount];
    public float[] IntellectFromPerk = new float[Heroes.HeroCount];
    #endregion

    #region Learning
    public bool RussiansLessonIsFinished;
    #endregion

    #endregion

    public PlayerData_2( Heroes heroes, ArmorMark_1 mark_1, ArmorMark_2 mark_2, ArmorMark_3 mark_3, Weapons weapons,
        PerkTree perkTree, Skills skills, Facilities facilities, Achievements achievements, Battle battle, Items items, ListOfEffects listOfEffects)
    {
        #region Russians_vs_Lizards

        #region Battle
        EnemiesInStage = battle.CurrentEnemiesInStage;
        if (battle.BattleIsActive)
            CurrentStage = battle.CurrentStage - 1;
        else CurrentStage = battle.CurrentStage;
        MaxOpenStage = battle.MaxOpenStage;
        ResetCount = battle.ResetCount;
        FinalBossIsDead = battle.FinalBossIsDead;
        #endregion

        #region Facilities
        FaithCurrency = facilities.FaithCurrency;
        AncestralPower = facilities.AncestralPower;
        FaithMultiplier = facilities.FaithMultiplier;
        #endregion

        #region Achievements
        AccumulatedKills = achievements.AccumulatedKills;
        AccumulatedFaith = achievements.AccumulatedFaith;
        AccumulatedAncestralPower = achievements.AccumulatedAncestralPower;
        AccumulatedHeroes = achievements.AccumulatedHeroes;
        AccumulatedSkills = achievements.AccumulatedSkills;
        AccumulatedArmors = achievements.AccumulatedArmors;
        AccumulatedWeapons = achievements.AccumulatedWeapons;
        AccumulatedPerks = achievements.AccumulatedPerks;
        AccumulatedStages = achievements.AccumulatedStages;

        WeaponIsUnlocked = achievements.WeaponIsUnlocked;
        ArmorIsUnlocked = achievements.ArmorIsUnlocked;
        SkillIsUnlocked = achievements.SkillIsUnlocked;
        HeroIsUnlocked = achievements.HeroIsUnlocked;
        SavedMaxOpenStage = achievements.SavedMaxOpenStage;
        #endregion

        #region Items
        BaikalWaterCount = items._BaikalWater.Count;
        PotionsCount[0] = items.HealthPotion.Count;
        PotionsCount[1] = items.StaminaPotion.Count;
        PotionsCount[2] = items.WillPotion.Count;
        #endregion

        #region Buffs
        AdditiveStatsFromBaikalWater = items.AdditiveStats;
        BaikalWaterTimer = items.Timer;

        CachedAdditiveStats_PrayerToTheGods = listOfEffects.CachedAdditiveStats_PrayerToTheGods;
        CachedBuffTime_PrayerToTheGods = listOfEffects.CachedBuffTime_PrayerToTheGods;
        CachedAdditiveStats_GeneralFee = listOfEffects.CachedAdditiveStats_GeneralFee;
        CachedBuffTime_GeneralFee = listOfEffects.CachedBuffTime_GeneralFee;
        #endregion

        #region Armors
        ArmorSetsInitialize();

        for (int i = 0; i < ArmorMark_1.StatsCount; i++)
        {
            _ArmorSet[0].StatValue[i] = mark_1.StatValue[i];
            _ArmorSet[0].StatUpgradeCost[i] = mark_1.StatUpgradeCost[i];
            _ArmorSet[0].CurrentUpgradeCount[i] = mark_1.CurrentUpgradeCount[i];
            _ArmorSet[0].Bought = mark_1.Bought;
            _ArmorSet[0].Selected = mark_1.Selected;
        }
        for (int i = 0; i < ArmorMark_2.StatsCount; i++)
        {
            _ArmorSet[1].StatValue[i] = mark_2.StatValue[i];
            _ArmorSet[1].StatUpgradeCost[i] = mark_2.StatUpgradeCost[i];
            _ArmorSet[1].CurrentUpgradeCount[i] = mark_2.CurrentUpgradeCount[i];
            _ArmorSet[1].Bought = mark_2.Bought;
            _ArmorSet[1].Selected = mark_2.Selected;
        }
        for (int i = 0; i < ArmorMark_3.StatsCount; i++)
        {
            _ArmorSet[2].StatValue[i] = mark_3.StatValue[i];
            _ArmorSet[2].StatUpgradeCost[i] = mark_3.StatUpgradeCost[i];
            _ArmorSet[2].CurrentUpgradeCount[i] = mark_3.CurrentUpgradeCount[i];
            _ArmorSet[2].Bought = mark_3.Bought;
            _ArmorSet[2].Selected = mark_3.Selected;
        }
        #endregion

        #region Weapons
        for (int weaponIndex = 0; weaponIndex < Weapons.WeaponCount; weaponIndex++)
        {
            _Weapons.Selected[weaponIndex] = weapons._Weapon[weaponIndex].Selected;
            _Weapons.Bought[weaponIndex] = weapons._Weapon[weaponIndex].Bought;

            for (int statIndex = 0; statIndex < Weapons.WeaponStatsCount; statIndex++)
            {
                _Weapons.StatsValue[weaponIndex, statIndex] = weapons._Weapon[weaponIndex].StatsValue[statIndex];
                _Weapons.CurrentUpgradeCount[weaponIndex, statIndex] = weapons._Weapon[weaponIndex].CurrentUpgradeCount[statIndex];
                _Weapons.StatsUpgradeCost[weaponIndex, statIndex] = weapons._Weapon[weaponIndex].StatsUpgradeCost[statIndex];
            }
        }
        #endregion

        #region PerkTree
        int perkIndex;

        perkIndex = 0;
        _PerkTree.Summ[perkIndex] = perkTree.BaseStrengthAdditional.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.BaseStrengthAdditional.CurrentUpgradeIndex;

        perkIndex = 1;
        _PerkTree.Summ[perkIndex] = perkTree.BaseDexterityAdditional.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.BaseDexterityAdditional.CurrentUpgradeIndex;

        perkIndex = 2;
        _PerkTree.Summ[perkIndex] = perkTree.BaseIntellectAdditional.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.BaseIntellectAdditional.CurrentUpgradeIndex;

        perkIndex = 3;
        _PerkTree.Summ[perkIndex] = perkTree.ArithmeticStrengthAdditional.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.ArithmeticStrengthAdditional.CurrentUpgradeIndex;

        perkIndex = 4;
        _PerkTree.Summ[perkIndex] = perkTree.ArithmeticDexterityAdditional.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.ArithmeticDexterityAdditional.CurrentUpgradeIndex;

        perkIndex = 5;
        _PerkTree.Summ[perkIndex] = perkTree.ArithmeticIntellectAdditional.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.ArithmeticIntellectAdditional.CurrentUpgradeIndex;

        perkIndex = 6;
        _PerkTree.Summ[perkIndex] = perkTree.GeometricStrengthAdditional.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.GeometricStrengthAdditional.CurrentUpgradeIndex;

        perkIndex = 7;
        _PerkTree.Summ[perkIndex] = perkTree.GeometricDexterityAdditional.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.GeometricDexterityAdditional.CurrentUpgradeIndex;

        perkIndex = 8;
        _PerkTree.Summ[perkIndex] = perkTree.GeometricIntellectAdditional.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.GeometricIntellectAdditional.CurrentUpgradeIndex;

        perkIndex = 9;
        _PerkTree.Summ[perkIndex] = perkTree.MaxHealthFromStrength.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.MaxHealthFromStrength.CurrentUpgradeIndex;

        perkIndex = 10;
        _PerkTree.Summ[perkIndex] = perkTree.MaxStaminaFromDexterity.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.MaxStaminaFromDexterity.CurrentUpgradeIndex;

        perkIndex = 11;
        _PerkTree.Summ[perkIndex] = perkTree.MaxWillFromIntellect.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.MaxWillFromIntellect.CurrentUpgradeIndex;

        perkIndex = 12;
        _PerkTree.Summ[perkIndex] = perkTree.HealthRegenerationFromStrength.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.HealthRegenerationFromStrength.CurrentUpgradeIndex;

        perkIndex = 13;
        _PerkTree.Summ[perkIndex] = perkTree.StaminaRegenerationFromDexterity.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.StaminaRegenerationFromDexterity.CurrentUpgradeIndex;

        perkIndex = 14;
        _PerkTree.Summ[perkIndex] = perkTree.WillRegenerationFromIntellect.Summ;
        _PerkTree.CurrentUpgradeIndex[perkIndex] = perkTree.WillRegenerationFromIntellect.CurrentUpgradeIndex;
        #endregion

        #region Skills

        for (int i = 0; i < Skills.PlasesForSkillsCount; i++)
        {
            BoughtPlasesForSkills[i] = skills.PlaseForSkillIsBought[i];
            SelectedSkillIndex[i] = skills.SelectedSkillIndex[i];
            SavedReloadTimeForSelectedSkill[i] = skills.SavedReloadTimeForSelectedSkill[i];
        }

        for (int i = 0; i < Skills.SkillsCount; i++)
        {
            BoughtSkills[i] = skills.SkillIsBought[i];
            SelectedSkill[i] = skills.SkillIsSelected[i];
        }


        _BallClamp.StunDuration = skills._BallClamp.StunDuration;
        _BallClamp.Damage = skills._BallClamp.Damage;
        _BallClamp.ReloadTime = skills._BallClamp.ReloadTime;
        _BallClamp.WillCost = skills._BallClamp.WillCost;
        _BallClamp.CurrentPurchasePercent = skills._BallClamp.CurrentPurchasePercent;
        _BallClamp.SummCurrentPurchaseCount = skills._BallClamp.SummCurrentPurchaseCount;
        for (int i = 0; i < _BallClamp.Cost.Length; i++)
        {
            _BallClamp.Cost[i] = skills._BallClamp.Cost[i];
            _BallClamp.PurchaseCount[i] = skills._BallClamp.PurchaseCount[i];
        }

        _ShootingGlance.Damage = skills._ShootingGlance.Damage;
        _ShootingGlance.PeriodicDamage = skills._ShootingGlance.PeriodicDamage;
        _ShootingGlance.Duration = skills._ShootingGlance.Duration;
        _ShootingGlance.ReloadTime = skills._ShootingGlance.ReloadTime;
        _ShootingGlance.WillCost = skills._ShootingGlance.WillCost;
        _ShootingGlance.CurrentPurchasePercent = skills._ShootingGlance.CurrentPurchasePercent;
        _ShootingGlance.SummCurrentPurchaseCount = skills._ShootingGlance.SummCurrentPurchaseCount;
        for (int i = 0; i < _ShootingGlance.Cost.Length; i++)
        {
            _ShootingGlance.Cost[i] = skills._ShootingGlance.Cost[i];
            _ShootingGlance.PurchaseCount[i] = skills._ShootingGlance.PurchaseCount[i];
        }

        _BlackSauna.PeriodicDamage = skills._BlackSauna.PeriodicDamage;
        _BlackSauna.DecreasedHealthRegeneration = skills._BlackSauna.DecreasedHealthRegeneration;
        _BlackSauna.Duration = skills._BlackSauna.Duration;
        _BlackSauna.ReloadTime = skills._BlackSauna.ReloadTime;
        _BlackSauna.WillCost = skills._BlackSauna.WillCost;
        _BlackSauna.CurrentPurchasePercent = skills._BlackSauna.CurrentPurchasePercent;
        _BlackSauna.SummCurrentPurchaseCount = skills._BlackSauna.SummCurrentPurchaseCount;
        for (int i = 0; i < _BlackSauna.Cost.Length; i++)
        {
            _BlackSauna.Cost[i] = skills._BlackSauna.Cost[i];
            _BlackSauna.PurchaseCount[i] = skills._BlackSauna.PurchaseCount[i];
        }

        _ArmorBreak.Damage = skills._ArmorBreak.Damage;
        _ArmorBreak.DecreasedArmor = skills._ArmorBreak.DecreasedArmor;
        _ArmorBreak.Duration = skills._ArmorBreak.Duration;
        _ArmorBreak.ReloadTime = skills._ArmorBreak.ReloadTime;
        _ArmorBreak.WillCost = skills._ArmorBreak.WillCost;
        _ArmorBreak.CurrentPurchasePercent = skills._ArmorBreak.CurrentPurchasePercent;
        _ArmorBreak.SummCurrentPurchaseCount = skills._ArmorBreak.SummCurrentPurchaseCount;
        for (int i = 0; i < _ArmorBreak.Cost.Length; i++)
        {
            _ArmorBreak.Cost[i] = skills._ArmorBreak.Cost[i];
            _ArmorBreak.PurchaseCount[i] = skills._ArmorBreak.PurchaseCount[i];
        }

        _ZeusAnger.PercentDamage = skills._ZeusAnger.PercentDamage;
        _ZeusAnger.AttackCount = skills._ZeusAnger.AttackCount;
        _ZeusAnger.TimeBetweenAttacks = skills._ZeusAnger.TimeBetweenAttacks;
        _ZeusAnger.ReloadTime = skills._ZeusAnger.ReloadTime;
        _ZeusAnger.WillCost = skills._ZeusAnger.WillCost;
        _ZeusAnger.CurrentPurchasePercent = skills._ZeusAnger.CurrentPurchasePercent;
        _ZeusAnger.SummCurrentPurchaseCount = skills._ZeusAnger.SummCurrentPurchaseCount;
        for (int i = 0; i < _ZeusAnger.Cost.Length; i++)
        {
            _ZeusAnger.Cost[i] = skills._ZeusAnger.Cost[i];
            _ZeusAnger.PurchaseCount[i] = skills._ZeusAnger.PurchaseCount[i];
        }

        _HelpPerun.AttackCount = skills._HelpPerun.AttackCount;
        _HelpPerun.ReloadTime = skills._HelpPerun.ReloadTime;
        _HelpPerun.WillCost = skills._HelpPerun.WillCost;
        _HelpPerun.CurrentPurchasePercent = skills._HelpPerun.CurrentPurchasePercent;
        _HelpPerun.SummCurrentPurchaseCount = skills._HelpPerun.SummCurrentPurchaseCount;
        for (int i = 0; i < _HelpPerun.Cost.Length; i++)
        {
            _HelpPerun.Cost[i] = skills._HelpPerun.Cost[i];
            _HelpPerun.PurchaseCount[i] = skills._HelpPerun.PurchaseCount[i];
        }

        _PrayerToTheGods.PercentToStats = skills._PrayerToTheGods.PercentToStats;
        _PrayerToTheGods.Duration = skills._PrayerToTheGods.Duration;
        _PrayerToTheGods.ReloadTime = skills._PrayerToTheGods.ReloadTime;
        _PrayerToTheGods.WillCost = skills._PrayerToTheGods.WillCost;
        _PrayerToTheGods.CurrentPurchasePercent = skills._PrayerToTheGods.CurrentPurchasePercent;
        _PrayerToTheGods.SummCurrentPurchaseCount = skills._PrayerToTheGods.SummCurrentPurchaseCount;
        for (int i = 0; i < _PrayerToTheGods.Cost.Length; i++)
        {
            _PrayerToTheGods.Cost[i] = skills._PrayerToTheGods.Cost[i];
            _PrayerToTheGods.PurchaseCount[i] = skills._PrayerToTheGods.PurchaseCount[i];
        }

        _GeneralFee.PercentAttributes = skills._GeneralFee.PercentToStats;
        _GeneralFee.Duration = skills._GeneralFee.Duration;
        _GeneralFee.ReloadTime = skills._GeneralFee.ReloadTime;
        _GeneralFee.WillCost = skills._GeneralFee.WillCost;
        _GeneralFee.CurrentPurchasePercent = skills._GeneralFee.CurrentPurchasePercent;
        _GeneralFee.SummCurrentPurchaseCount = skills._GeneralFee.SummCurrentPurchaseCount;
        for (int i = 0; i < _GeneralFee.Cost.Length; i++)
        {
            _GeneralFee.Cost[i] = skills._GeneralFee.Cost[i];
            _GeneralFee.PurchaseCount[i] = skills._GeneralFee.PurchaseCount[i];
        }
        #endregion

        #region Heroes

        CurrentHeroIndex = heroes.CurrentHero.Index;

        for (int heroIndex = 0; heroIndex < Heroes.HeroCount; heroIndex++)
        {
            RemainingTimeToResurrationHero[heroIndex] = heroes.hero[heroIndex].RemainingTimeToResurrationHero;
            Level[heroIndex] = heroes.hero[heroIndex].Level;
            CurrentEXP[heroIndex] = heroes.hero[heroIndex].ActualEXP;
            EXPForNextLevel[heroIndex] = heroes.hero[heroIndex].EXPForNextLevel;
            PumpingPoints[heroIndex] = heroes.hero[heroIndex].PumpingPoints;
            HeroBought[heroIndex] = heroes.hero[heroIndex].Bought;

            ActualHealth[heroIndex] = heroes.hero[heroIndex].ActualHealth;
            MaxHealthBase[heroIndex] = heroes.hero[heroIndex].MaxHealthBase;
            MaxHealthFromLevel[heroIndex] = heroes.hero[heroIndex].MaxHealthFromLevel;
            MaxHealthFromStrength[heroIndex] = heroes.hero[heroIndex].MaxHealthFromStrength;
            MaxHealthFromArmorSet[heroIndex] = heroes.hero[heroIndex].MaxHealthFromArmorSet;
            MaxHealthFromWeapon[heroIndex] = heroes.hero[heroIndex].MaxHealthFromWeapon;
            MaxHealthFromPerk[heroIndex] = heroes.hero[heroIndex].MaxHealthFromPerk;

            HealthRegenerationBase[heroIndex] = heroes.hero[heroIndex].HealthRegenerationBase;
            HealthRegenerationFromStrength[heroIndex] = heroes.hero[heroIndex].HealthRegenerationFromStrength;
            HealthRegenerationFromArmorSet[heroIndex] = heroes.hero[heroIndex].HealthRegenerationFromArmorSet;
            HealthRegenerationFromWeapon[heroIndex] = heroes.hero[heroIndex].HealthRegenerationFromWeapon;
            HealthRegenerationFromPerk[heroIndex] = heroes.hero[heroIndex].HealthRegenerationFromPerk;

            ActualStamina[heroIndex] = heroes.hero[heroIndex].ActualStamina;
            CostStaminaToAttack[heroIndex] = heroes.hero[heroIndex].CostStaminaToAttack;
            MaxStaminaBase[heroIndex] = heroes.hero[heroIndex].MaxStaminaBase;
            MaxStaminaFromLevel[heroIndex] = heroes.hero[heroIndex].MaxStaminaFromLevel;
            MaxStaminaFromStrength[heroIndex] = heroes.hero[heroIndex].MaxStaminaFromStrength;
            MaxStaminaFromDexterity[heroIndex] = heroes.hero[heroIndex].MaxStaminaFromDexterity;
            MaxStaminaFromArmorSet[heroIndex] = heroes.hero[heroIndex].MaxStaminaFromArmorSet;
            MaxStaminaFromWeapon[heroIndex] = heroes.hero[heroIndex].MaxStaminaFromWeapon;
            MaxStaminaFromPerk[heroIndex] = heroes.hero[heroIndex].MaxStaminaFromPerk;

            StaminaRegenerationBase[heroIndex] = heroes.hero[heroIndex].StaminaRegenerationBase;
            StaminaRegenerationFromStrength[heroIndex] = heroes.hero[heroIndex].StaminaRegenerationFromStrength;
            StaminaRegenerationFromDexterity[heroIndex] = heroes.hero[heroIndex].StaminaRegenerationFromDexterity;
            StaminaRegenerationFromArmorSet[heroIndex] = heroes.hero[heroIndex].StaminaRegenerationFromArmorSet;
            StaminaRegenerationFromWeapon[heroIndex] = heroes.hero[heroIndex].StaminaRegenerationFromWeapon;
            StaminaRegenerationFromPerk[heroIndex] = heroes.hero[heroIndex].StaminaRegenerationFromPerk;

            ActualWill[heroIndex] = heroes.hero[heroIndex].ActualWill;
            MaxWillBase[heroIndex] = heroes.hero[heroIndex].MaxWillBase;
            MaxWillFromLevel[heroIndex] = heroes.hero[heroIndex].MaxWillFromLevel;
            MaxWillFromIntellect[heroIndex] = heroes.hero[heroIndex].MaxWillFromIntellect;
            MaxWillFromArmorSet[heroIndex] = heroes.hero[heroIndex].MaxWillFromArmorSet;
            MaxWillFromPerk[heroIndex] = heroes.hero[heroIndex].MaxWillFromPerk;

            WillRegenerationBase[heroIndex] = heroes.hero[heroIndex].WillRegenerationBase;
            WillRegenerationFromIntellect[heroIndex] = heroes.hero[heroIndex].WillRegenerationFromIntellect;
            WillRegenerationFromArmorSet[heroIndex] = heroes.hero[heroIndex].WillRegenerationFromArmorSet;
            WillRegenerationFromPerk[heroIndex] = heroes.hero[heroIndex].WillRegenerationFromPerk;

            DamageBase[heroIndex] = heroes.hero[heroIndex].DamageBase;
            DamageFromLevel[heroIndex] = heroes.hero[heroIndex].DamageFromLevel;
            DamageFromWeapon[heroIndex] = heroes.hero[heroIndex].DamageFromWeapon;
            DamageFromStrength[heroIndex] = heroes.hero[heroIndex].DamageFromStrength;
            DamageFromArmorSet[heroIndex] = heroes.hero[heroIndex].DamageFromArmorSet;
            DamageFromPerk[heroIndex] = heroes.hero[heroIndex].DamageFromPerk;

            ArmorBase[heroIndex] = heroes.hero[heroIndex].ArmorBase;
            ArmorFromArmorSet[heroIndex] = heroes.hero[heroIndex].ArmorFromArmorSet;
            ArmorFromWeapon[heroIndex] = heroes.hero[heroIndex].ArmorFromWeapon;
            ArmorFromDexterity[heroIndex] = heroes.hero[heroIndex].ArmorFromDexterity;
            ArmorFromPerk[heroIndex] = heroes.hero[heroIndex].ArmorFromPerk;

            EvasionBase[heroIndex] = heroes.hero[heroIndex].EvasionBase;
            EvasionFromDexterity[heroIndex] = heroes.hero[heroIndex].EvasionFromDexterity;
            EvasionFromArmorSet[heroIndex] = heroes.hero[heroIndex].EvasionFromArmorSet;
            EvasionFromWeapon[heroIndex] = heroes.hero[heroIndex].EvasionFromWeapon;
            EvasionFromPerk[heroIndex] = heroes.hero[heroIndex].EvasionFromPerk;

            StrengthBase[heroIndex] = heroes.hero[heroIndex].StrengthBase;
            StrengthFromLevel[heroIndex] = heroes.hero[heroIndex].StrengthFromLevel;
            StrengthFromPumpingPoints[heroIndex] = heroes.hero[heroIndex].IntellectFromPumpingPoints;
            StrengthFromArmorSet[heroIndex] = heroes.hero[heroIndex].StrengthFromArmorSet;
            StrengthFromWeapon[heroIndex] = heroes.hero[heroIndex].StrengthFromWeapon;
            StrengthFromPerk[heroIndex] = heroes.hero[heroIndex].StrengthFromPerk;

            DexterityBase[heroIndex] = heroes.hero[heroIndex].DexterityBase;
            DexterityFromLevel[heroIndex] = heroes.hero[heroIndex].DexterityFromLevel;
            DexterityFromPumpingPoints[heroIndex] = heroes.hero[heroIndex].IntellectFromPumpingPoints;
            DexterityFromWeapon[heroIndex] = heroes.hero[heroIndex].DexterityFromWeapon;
            DexterityFromArmorSet[heroIndex] = heroes.hero[heroIndex].DexterityFromArmorSet;
            DexterityFromPerk[heroIndex] = heroes.hero[heroIndex].DexterityFromPerk;

            IntellectBase[heroIndex] = heroes.hero[heroIndex].IntellectBase;
            IntellectFromLevel[heroIndex] = heroes.hero[heroIndex].IntellectFromLevel;
            IntellectFromPumpingPoints[heroIndex] = heroes.hero[heroIndex].IntellectFromPumpingPoints;
            IntellectFromArmorSet[heroIndex] = heroes.hero[heroIndex].IntellectFromArmorSet;
            IntellectFromPerk[heroIndex] = heroes.hero[heroIndex].IntellectFromPerk;
        }
        #endregion

        #region Learning
        RussiansLessonIsFinished = Learning.RussiansLessonIsFinished;
        #endregion

        #endregion
    }

    private void ArmorSetsInitialize()
    {
        for (int i = 0; i < 3; i++)
            _ArmorSet[i] = new();

        _ArmorSet[0].StatValue = new float[ArmorMark_1.StatsCount];
        _ArmorSet[0].StatUpgradeCost = new float[ArmorMark_1.StatsCount];
        _ArmorSet[0].CurrentUpgradeCount = new int[ArmorMark_1.StatsCount];

        _ArmorSet[1].StatValue = new float[ArmorMark_2.StatsCount];
        _ArmorSet[1].StatUpgradeCost = new float[ArmorMark_2.StatsCount];
        _ArmorSet[1].CurrentUpgradeCount = new int[ArmorMark_2.StatsCount];

        _ArmorSet[2].StatValue = new float[ArmorMark_3.StatsCount];
        _ArmorSet[2].StatUpgradeCost = new float[ArmorMark_3.StatsCount];
        _ArmorSet[2].CurrentUpgradeCount = new int[ArmorMark_3.StatsCount];
    }
}