using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : DataStructure
{
    [SerializeField] private GameObject _attentionObject;
    [SerializeField] private GameObject[] _checkmarks;
    [SerializeField] private Button[] _cellButtons;

    [SerializeField] private TextMeshProUGUI _visitingDaysText;

    [NonSerialized] public static bool[] CalendarRewardIsClaimed = new bool[7];
    [NonSerialized] public static bool[] RewardIsUnlock = new bool[7];

    private readonly int[] _memeCoinsReward = { 200, 400, 800, 1500, 2500, 3500, 5000 };

    public void Init() //Start
    {
        ÑheckElapsedTime();
        DisplayVisitingDays();

        for (int i = 0; i < 7; i++)
        {
            if (CalendarRewardIsClaimed[i])
            {
                DisplayClaimedState(i);
            }
            else if (RewardIsUnlock[i])
            {
                UnlockReward(i);
            }
        }
    }

    #region ButtonEvents
    public void ClaimDay_1()
    {
        GetMoneyAnimation.CreateAndAddCoins(_memeCoinsReward[0], "Calendar");
        GlobalUpgrades.IcreaseIncomeMultiplier(0.1f);
        DisplayClaimedState(0);
    }

    public void ClaimDay_2()
    {
        GetMoneyAnimation.CreateAndAddCoins(_memeCoinsReward[1], "Calendar");
        GlobalUpgrades.AbsenceTime += 1800;
        DisplayClaimedState(1);
    }

    public void ClaimDay_3()
    {
        GetMoneyAnimation.CreateAndAddCoins(_memeCoinsReward[2], "Calendar");
        GlobalUpgrades.TickTime -= 0.25f;
        DisplayClaimedState(2);
    }

    public void ClaimDay_4()
    {
        GetMoneyAnimation.CreateAndAddCoins(_memeCoinsReward[3], "Calendar");
        GlobalUpgrades.DecreaseUpgradeCostMultiplier(0.005f);
        DisplayClaimedState(3);
    }

    public void ClaimDay_5()
    {
        GetMoneyAnimation.CreateAndAddCoins(_memeCoinsReward[4], "Calendar");
        GlobalUpgrades.IcreaseIncomeMultiplier(0.5f);
        DisplayClaimedState(4);
    }

    public void ClaimDay_6()
    {
        GetMoneyAnimation.CreateAndAddCoins(_memeCoinsReward[5], "Calendar");
        GlobalUpgrades.AbsenceTime += 3600;
        DisplayClaimedState(5);
    }

    public void ClaimDay_7()
    {
        GetMoneyAnimation.CreateAndAddCoins(_memeCoinsReward[6], "Calendar");
        GlobalUpgrades.DecreaseUpgradeCostMultiplier(0.0025f);
        GlobalUpgrades.IcreaseIncomeMultiplier(0.2f);
        GlobalUpgrades.TickTime -= 0.1f;
        GlobalUpgrades.CritChance += 0.05f;
        GlobalUpgrades.CritStrength += 0.4f;
        DisplayClaimedState(6);
    }
    #endregion

    private void DisplayClaimedState(int day)
    {
        CalendarRewardIsClaimed[day] = true;
        _cellButtons[day].interactable = false;
        _checkmarks[day].SetActive(true);

        int counter = 0;
        foreach (Button button in _cellButtons)
        {
            if (button.interactable == false)
            {
                counter++;
            }
            if (counter == _cellButtons.Length)
            {
                _attentionObject.SetActive(false);
            }
        }
    }

    private void UnlockReward(int day)
    {
        RewardIsUnlock[day] = true;
        _cellButtons[day].interactable = true;
        _attentionObject.SetActive(true);
    }

    private void DisplayVisitingDays() 
    { _visitingDaysText.text = $"Äíåé ïîñåùåíî:{Game.VisitingDays}"; }

    private void ÑheckElapsedTime()
    {
        if (Game.VisitingDays >= 1)
        {
            UnlockReward(0);
        }
        if (Game.VisitingDays >= 2)
        {
            UnlockReward(1);
        }
        if (Game.VisitingDays >= 3)
        {
            UnlockReward(2);
        }
        if (Game.VisitingDays >= 4)
        {
            UnlockReward(3);
        }
        if (Game.VisitingDays >= 5)
        {
            UnlockReward(4);
        }
        if (Game.VisitingDays >= 6)
        {
            UnlockReward(5);
        }
        if (Game.VisitingDays >= 7)
        {
            UnlockReward(6);
        }
    }
}