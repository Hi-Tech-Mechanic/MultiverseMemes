using System;
using UnityEngine;

public class AchievementsParent : DataStructure
{
    #region System
    [SerializeField] protected GameObject ContentObject;
    [NonSerialized] protected GameObject[] AchievementsBuffer;

    [Header("Спрайты достижений")]
    [Space(5)]
    [SerializeField] protected Sprite[] ClickSprites;
    [SerializeField] protected Sprite[] IdleSprites;
    [SerializeField] protected Sprite MoneySprite;
    [SerializeField] protected Sprite MemeClipSprite;
    [NonSerialized] protected AchievementUnit[] AchievementsScripts;
    [NonSerialized] protected bool WindowIsActive = false;
    #endregion

    #region Currency
    [NonSerialized] protected AchievementUnit[] AchievementsForMoney;
    protected double[] NeededMoney = { Math.Pow(10, 2), Math.Pow(10, 3),
    Math.Pow(10, 4), Math.Pow(10, 5), Math.Pow(10, 6),
    Math.Pow(10, 7), Math.Pow(10, 8), Math.Pow(10, 9),
    Math.Pow(10, 10), Math.Pow(10, 11), Math.Pow(10, 12),
    Math.Pow(10, 13), Math.Pow(10, 14), Math.Pow(10, 15) };
    [NonSerialized] protected float[] RewardFromMoney = { 25, 60, 100, 175, 275, 400, 550, 725, 925, 1150, 1400, 1675, 1975, 2300};
    #endregion

    #region Upgrades
    [NonSerialized] protected AchievementUnit[,] ClickUpgrades;
    [NonSerialized] protected AchievementUnit[,] IdleUpgrades;
    [NonSerialized] protected int ClickCellsCount;
    [NonSerialized] protected int IdleCellsCount;
    [NonSerialized] protected int[] NeededPurchasedUpgrades = { 10, 25, 50, 100, 150, 250, 350, 500 };
    [NonSerialized] protected int[,] ClickRewards;
    [NonSerialized] protected int[,] IdleRewards;
    [NonSerialized] protected const float UnitsUpgradesForCoin = 5;
    #endregion

    #region MemeClips
    [NonSerialized] protected AchievementUnit[] AchievementsForMemeClips;
    [NonSerialized] protected int[] NeededPurchasedMemeClips = { 1, 5, 10, 15, 20, 25 };
    [NonSerialized] protected float[] RewardFromMemeClips;
    [NonSerialized] protected float UnitsMemeClipsForCoin = 0.1f;
    #endregion

    protected virtual void OnDisable()
    {
        foreach (AchievementUnit unit in AchievementsScripts)
            GlobalUpgrades.OnChangedADRewardMultiplier -= unit.DisplayRewardMultiplier;
    }

    public virtual void Init()
    {
        InitializeFields();
        DesignateFields();

        foreach (AchievementUnit unit in AchievementsScripts)
        {
            unit.Initialize();
            GlobalUpgrades.OnChangedADRewardMultiplier += unit.DisplayRewardMultiplier;
        }

        SetDataInAchievementUnit();
        UpdateAllInfo();

        //OutputAllRewardsSumm(); //hide
    }

    protected virtual void InitializeFields()
    {
        AchievementsBuffer = new GameObject[ContentObject.transform.childCount];
        AchievementsScripts = new AchievementUnit[AchievementsBuffer.Length];

        ClickCellsCount = ClickSprites.Length;
        IdleCellsCount = IdleSprites.Length;
        ClickUpgrades = new AchievementUnit[ClickCellsCount, NeededPurchasedUpgrades.Length];
        IdleUpgrades = new AchievementUnit[IdleCellsCount, NeededPurchasedUpgrades.Length];
        AchievementsForMoney = new AchievementUnit[NeededMoney.Length];
        AchievementsForMemeClips = new AchievementUnit[NeededPurchasedMemeClips.Length];
        ClickRewards = new int[ClickCellsCount, NeededPurchasedUpgrades.Length];
        IdleRewards = new int[IdleCellsCount, NeededPurchasedUpgrades.Length];
        RewardFromMemeClips = new float[NeededPurchasedMemeClips.Length];
    }

    protected virtual void DesignateFields()
    {
        for (int i = 0; i < AchievementsBuffer.Length; i++)
        {
            AchievementsBuffer[i] = ContentObject.transform.GetChild(i).gameObject;
        }

        int counter = 0;
        int rows;
        int columns;

        foreach (GameObject tmp in AchievementsBuffer)
        {
            AchievementsScripts[counter] = tmp.GetComponent<AchievementUnit>();
            AchievementsScripts[counter].RewardID = counter + Enum.GetNames(typeof(Game.RewardIndex)).Length;
            counter++;
        }

        counter = 0;

        for (int i = 0; i < NeededMoney.Length; i++)
        {
            AchievementsForMoney[i] = AchievementsScripts[counter];
            counter++;
        }

        for (int i = 0; i < NeededPurchasedMemeClips.Length; i++)
        {
            AchievementsForMemeClips[i] = AchievementsScripts[counter];
            counter++;
        }

        rows = ClickUpgrades.GetUpperBound(0) + 1;
        columns = ClickUpgrades.Length / rows;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                ClickUpgrades[row, column] = AchievementsScripts[counter];
                counter++;
            }
        }

        rows = IdleUpgrades.GetUpperBound(0) + 1;
        columns = IdleUpgrades.Length / rows;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                IdleUpgrades[row, column] = AchievementsScripts[counter];
                counter++;
            }
        }

        CheckSavedData();

        void CheckSavedData()
        {
            for (int i = 0; i < AchievementsScripts.Length; i++)
            {
                AchievementsScripts[i].UnitIndex = i;

                if (GlobalUpgrades.AchievementsRewardsIsClaimed[Game.CurrentScene, i] == true)
                    AchievementsScripts[i].Claimed = true;
            }
        }
    }

    #region ButtonEvents
    public virtual void SwitchActiveWindow()
    {
        WindowIsActive = !WindowIsActive;
        UpdateAllInfo();
    }

    public void SortingPerType()
    {
        Debug.Log("PerType");
    }

    public void SortingPerProgress()
    {
        Debug.Log("PerProgress");
        float[] progressArr = new float[AchievementsScripts.Length];
        int[] indexArr = new int[AchievementsScripts.Length];
        int i = 0;
        float temp;

        foreach (var tmp in AchievementsScripts)
        {
            progressArr[i] = tmp.GetProgress();
            i++;
        }

        for (int j = 0; j < progressArr.Length - 1; j++)
        {
            for (int k = j + 1; k < progressArr.Length; k++)
            {
                if (progressArr[j] > progressArr[k])
                {
                    temp = progressArr[j];
                    progressArr[j] = progressArr[k];
                    progressArr[k] = temp;
                    indexArr[j] = k;
                    AchievementsBuffer[k].transform.SetAsFirstSibling();
                }
            }
        }
    }

    public void SortingPerReward()
    {
        Debug.Log("PerReward");
        //foreach (var tmp in AchievementsScripts)
        //{
        //}
    }
    #endregion

    #region FillingAchievement
    protected virtual void SetDataInAchievementUnit()
    {
        CalcRewards();
        SetData();

        void SetData()
        {
            string currencyPostfix = "";

            switch (Game.CurrentScene)
            {
                case 0:
                    currencyPostfix = "валюты";
                    break;
                case 1:
                    currencyPostfix = "веры";
                    break;
                case 2:
                    currencyPostfix = "букикоинов";
                    break;
                case 3:
                    currencyPostfix = "генкоинов";
                    break;
                case 4:
                    currencyPostfix = "аскеткоинов";
                    break;
                case 5:
                    currencyPostfix = "униксов";
                    break;
                case 6:
                    currencyPostfix = "аномалий";
                    break;
                case 7:
                    currencyPostfix = "дуэйнов";
                    break;
                case 8:
                    currencyPostfix = "шрексов";
                    break;
            }

            int rows;
            int columns;

            for (int i = 0; i < NeededMoney.Length; i++)
            {
                AchievementsForMoney[i].SetDescription($"Заработать {ValuesRounding.FormattingValue("", "", NeededMoney[i])} {currencyPostfix}");
                AchievementsForMoney[i].SetAchievementImage(MoneySprite);
                AchievementsForMoney[i].SetTargetProgress(NeededMoney[i]);
                AchievementsForMoney[i].SetCurrentProgress(MoneyMenu.AccumulatedMoney[Game.CurrentScene]);
                AchievementsForMoney[i].SetReward(RewardFromMoney[i]);
            }

            for (int i = 0; i < NeededPurchasedMemeClips.Length; i++)
            {
                string postfix;

                if ((NeededPurchasedMemeClips[i] % 10) == 1)
                    postfix = "прикол";
                else if ((NeededPurchasedMemeClips[i] % 10) > 1 && ((NeededPurchasedMemeClips[i] % 10) <= 4))
                    postfix = "прикола";
                else postfix = "приколов";

                AchievementsForMemeClips[i].SetDescription($"Выкупить {ValuesRounding.FormattingValue("", "", NeededPurchasedMemeClips[i])} {postfix}");
                AchievementsForMemeClips[i].SetAchievementImage(MemeClipSprite);
                AchievementsForMemeClips[i].SetTargetProgress(NeededPurchasedMemeClips[i]);
                AchievementsForMemeClips[i].SetCurrentProgress(MemeShop.PurchasedMemeClips[Game.CurrentScene]);
                AchievementsForMemeClips[i].SetReward(RewardFromMemeClips[i]);
            }

            rows = ClickUpgrades.GetUpperBound(0) + 1;
            columns = ClickUpgrades.Length / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    ClickUpgrades[row, column].SetDescription($"Купить улучшение '{ClickSprites[row].name}' {NeededPurchasedUpgrades[column]} раз");
                    ClickUpgrades[row, column].SetAchievementImage(ClickSprites[row]);
                    ClickUpgrades[row, column].SetTargetProgress(NeededPurchasedUpgrades[column]);
                    ClickUpgrades[row, column].SetCurrentProgress(Upgrades.AccumulatedPurchasedClickUpgrades[Game.CurrentScene, row]);
                    ClickUpgrades[row, column].SetReward(ClickRewards[row, column]);
                }
            }

            rows = IdleUpgrades.GetUpperBound(0) + 1;
            columns = IdleUpgrades.Length / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    IdleUpgrades[row, column].SetDescription($"Купить улучшение '{IdleSprites[row].name}' {NeededPurchasedUpgrades[column]} раз");
                    IdleUpgrades[row, column].SetAchievementImage(IdleSprites[row]);
                    IdleUpgrades[row, column].SetTargetProgress(NeededPurchasedUpgrades[column]);
                    IdleUpgrades[row, column].SetCurrentProgress(Upgrades.AccumulatedPurchasedIdleUpgrades[Game.CurrentScene, row]);
                    IdleUpgrades[row, column].SetReward(IdleRewards[row, column]);
                }
            }
        }

        void CalcRewards()
        {
            float divider;
            int rows;
            int columns;

            //for (int i = 0; i < NeededMoney.Length; i++)
            //{
            //    RewardFromMoney[i] = (int)Math.Round(NeededMoney[i] /(UnitsMoneyForCoin));
            //    UnitsMoneyForCoin *= 6f; // + (i);// / 1.5f);
            //}

            for (int i = 0; i < NeededPurchasedMemeClips.Length; i++)
            {
                RewardFromMemeClips[i] = (int)Math.Round(NeededPurchasedMemeClips[i] / (UnitsMemeClipsForCoin));
            }

            if (Game.CurrentScene != (int)Game.BuildIndex.Paranormal_Universe && Game.CurrentScene != (int)Game.BuildIndex.Papich_Universe)
                divider = 1;
            else divider = 1.5f;

            rows = ClickRewards.GetUpperBound(0) + 1;
            columns = ClickRewards.Length / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    ClickRewards[row, column] =
                        (int)Math.Round(NeededPurchasedUpgrades[column] / (UnitsUpgradesForCoin - ((UnitsUpgradesForCoin / (ClickCellsCount + 1)) * (row + 1))) / divider);
                }
            }

            rows = IdleRewards.GetUpperBound(0) + 1;
            columns = IdleRewards.Length / rows;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    IdleRewards[row, column] =
                        (int)Math.Round(NeededPurchasedUpgrades[column] / (UnitsUpgradesForCoin - ((UnitsUpgradesForCoin / (IdleCellsCount + 1)) * (row + 1))) / divider);
                }
            }
        }
    }
    #endregion

    #region UpdateForEachTask
    public virtual void UpdateAllInfo()
    {
        UpdateProgressUpgrade();
        UpdateMoneyAchievements();
        UpdateMemeClipsAchievements();
    }

    protected virtual void UpdateProgressUpgrade()
    {
        int counter = 0;
        int iter = 0;

        foreach (AchievementUnit achievement in ClickUpgrades)
        {
            if (!achievement.RewardIsReady && !achievement.Claimed)
            {
                achievement.SetCurrentProgress(Upgrades.AccumulatedPurchasedClickUpgrades[Game.CurrentScene, counter]);             
            }
            iter++;
            if (iter == NeededPurchasedUpgrades.Length)
            {
                counter++;
                iter = 0;
            }
        }

        counter = 0;
        iter = 0;

        foreach (AchievementUnit achievement in IdleUpgrades)
        {
            if (!achievement.RewardIsReady && !achievement.Claimed)
            {
                achievement.SetCurrentProgress(Upgrades.AccumulatedPurchasedIdleUpgrades[Game.CurrentScene, counter]);
            }
            iter++;
            if (iter == NeededPurchasedUpgrades.Length)
            {
                counter++;
                iter = 0;
            }
        }
    }

    protected virtual void UpdateMoneyAchievements()
    {
        foreach (AchievementUnit achievement in AchievementsForMoney)
        {
            if (!achievement.RewardIsReady && !achievement.Claimed)
            {
                achievement.SetCurrentProgress(MoneyMenu.AccumulatedMoney[Game.CurrentScene]);
            }
        }
    }

    protected virtual void UpdateMemeClipsAchievements()
    {
        foreach (AchievementUnit achievement in AchievementsForMemeClips)
        {
            if (!achievement.RewardIsReady && !achievement.Claimed)
            {
                achievement.SetCurrentProgress(MemeShop.PurchasedMemeClips[Game.CurrentScene]);
            }
        }
    }
    #endregion

    #region Other
    protected virtual void CalcAndDisplayRequiredUnitsCount()
    {
        int summ = ((IdleCellsCount + ClickCellsCount) * NeededPurchasedUpgrades.Length) + (NeededMoney.Length);
        Debug.Log("Required achievements units count = " + summ);
    }

    protected virtual void OutputAllRewardsSumm()
    {
        float summ = 0;

        foreach (int i in ClickRewards)
            summ += i;
        foreach (int i in IdleRewards)
            summ += i;
        foreach (int i in RewardFromMemeClips)
            summ += i;
        foreach (int i in RewardFromMoney)
            summ += i;

        Debug.Log($"All reward = {summ}");
    }

    public virtual AchievementUnit[] GetAchievementsScripts()
    {
        return AchievementsScripts;
    }
    #endregion
}