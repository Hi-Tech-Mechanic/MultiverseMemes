using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class ClickADReward : DataStructure
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private Image _timerImage;
    [SerializeField] private Color _buffActiveColor;
    [SerializeField] private Color _buffDeactiveColor;

    public static float BuffTimeRemaining = 0;
    public static float CachedADMultiplier;
    private Button _rewardButton;
    private IEnumerator enumerator;

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += GetClickReward;
        GlobalUpgrades.AdvertisingBuffTimeIsChange += UpdateAdvertisingBuffTime;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= GetClickReward;
        GlobalUpgrades.AdvertisingBuffTimeIsChange -= UpdateAdvertisingBuffTime;
    }

    public void Init() //Start
    {
        _rewardButton = gameObject.GetComponent<Button>();
        _timeText.text = ValuesRounding.GetFormattedTime(GlobalUpgrades.AdvertisingBuffTime);

        UpdateRewardText();
        CheckBuffs();
    }

    private void UpdateRewardText()
    { 
        StopCoroutine(Delay());
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.2f);
            _rewardText.text = $"{Math.Round(GlobalUpgrades.ADRewardMultiplier, 2)}x"; 
        }
    }

    public void WatchADForClickReward(int i)
    {
        YandexGame.RewVideoShow(i);
    }

    private void GetClickReward(int i)
    {
        if (i == (int)Game.RewardIndex.ClickBuff)
        {
            Game.AccumulateWatchedAD();
            ActivateBuff();
            SaveAndLoad.SavePlayerData();
        }        
    }

    private void ActivateBuff()
    {
        float[] cachedClickIncome = new float[Game.ScenesCount];
        enumerator = CountingDown();

        if (BuffTimeRemaining <= 0)
        {
            CachedADMultiplier = GlobalUpgrades.ADRewardMultiplier;
            BuffTimeRemaining = GlobalUpgrades.AdvertisingBuffTime;
        }
        else
        {
            Debuff();
        }

        StartCoroutine(enumerator);

        IEnumerator CountingDown()
        {
            Buff();

            while (BuffTimeRemaining > 0)
            {
                DisplayBuffTime(BuffTimeRemaining);
                BuffTimeRemaining--;
                yield return new WaitForSeconds(1);
            }

            Debuff();
        }

        void Buff()
        {
            _rewardButton.interactable = false;
            _timerImage.color = _buffActiveColor;

            for (int scene = 0; scene < Game.ScenesCount; scene++)
            {
                cachedClickIncome = MoneyMenu.GetClickIncome();
                cachedClickIncome[scene] *= CachedADMultiplier;
            }

            MoneyMenu.SetClickIncome(cachedClickIncome);
            cachedClickIncome = new float[Game.ScenesCount];

            for (int scene = 0; scene < Game.ScenesCount; scene++)
            {
                for (int i = 0; i < Game.MaxCellCountInGame; i++)
                {
                    Upgrades._Click.Income[scene, i] *= CachedADMultiplier;
                    Upgrades._Click.CachedIncome[scene, i] *= CachedADMultiplier;
                    Upgrades._Click.SummIncome[scene, i] *= CachedADMultiplier;
                    Upgrades._Click.DisplayCellAdditivesIncome(i);
                }
            }
        }

        void Debuff()
        {
            _rewardButton.interactable = true;
            _timerImage.color = _buffDeactiveColor;
            DisplayBuffTime(GlobalUpgrades.AdvertisingBuffTime);

            for (int scene = 0; scene < Game.ScenesCount; scene++)
            {
                for (int i = 0; i < Game.MaxCellCountInGame; i++)
                {
                    Upgrades._Click.Income[scene, i] /= CachedADMultiplier;
                    Upgrades._Click.CachedIncome[scene, i] /= CachedADMultiplier;
                    Upgrades._Click.SummIncome[scene, i] /= CachedADMultiplier;
                    Upgrades._Click.DisplayCellAdditivesIncome(i);

                    cachedClickIncome[scene] += Upgrades._Click.SummIncome[scene, i];
                }

                cachedClickIncome[scene] += MoneyMenu.StartClickIncome;
            }
            MoneyMenu.SetClickIncome(cachedClickIncome);
        }
    }

    private void CheckBuffs()
    {
        if (BuffTimeRemaining > 0)
        {
            ActivateBuff();
        }
    }

    private void UpdateAdvertisingBuffTime()
    {
        if (_rewardButton.interactable == true)
        {
            DisplayBuffTime(GlobalUpgrades.AdvertisingBuffTime);
        }
    }

    private void DisplayBuffTime(float value)
    {
        _timeText.text = ValuesRounding.GetFormattedTime(value);
    }
}