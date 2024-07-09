using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class IdleADReward : DataStructure
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private Image _timerImage;
    [SerializeField] private Color _buffActiveColor;
    [SerializeField] private Color _buffDeactiveColor;
    [SerializeField] private Transform _errorMessageParent;

    public static float BuffTimeRemaining = 0;
    public static float CachedADMultiplier;
    private Button _rewardButton;
    private IEnumerator enumerator;

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += GetIdleReward;
        GlobalUpgrades.AdvertisingBuffTimeIsChange += UpdateAdvertisingBuffTime;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= GetIdleReward;
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

    public void WatchADForIdleReward(int i)
    {
        if (MoneyMenu.GetIdleIncomePerTick()[Game.CurrentScene] > 0)
        {
            YandexGame.RewVideoShow(i);
        }
        else
        {
            Instantiate(FloatPrefabs.InfoOutlier, _errorMessageParent).GetComponent<Outlier>().DesignateInfoOutlier("Up", "Усиление не доступно пока пассивный доход = 0", Color.white);
        }
    }

    public void GetIdleReward(int i)
    {
        if (i == (int)Game.RewardIndex.IdleBuff)
        {
            Game.AccumulateWatchedAD();
            ActivateBuff();
            SaveAndLoad.SavePlayerData();
        }       
    }

    private void ActivateBuff()
    {
        float[] cachedIdleIncome = new float[Game.ScenesCount];
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
                cachedIdleIncome = MoneyMenu.GetIdleIncomePerTick();
                cachedIdleIncome[scene] *= CachedADMultiplier;
            }

            MoneyMenu.SetTickIncome(cachedIdleIncome);
            cachedIdleIncome = new float[Game.ScenesCount];

            for (int scene = 0; scene < Game.ScenesCount; scene++)
            {
                for (int i = 0; i < Game.MaxCellCountInGame; i++)
                {
                    Upgrades._Idle.Income[scene, i] *= CachedADMultiplier;
                    Upgrades._Idle.CachedIncome[scene, i] *= CachedADMultiplier;
                    Upgrades._Idle.SummIncome[scene, i] *= CachedADMultiplier;
                    Upgrades._Idle.DisplayCellAdditivesIncome(i);
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
                    Upgrades._Idle.Income[scene, i] /= CachedADMultiplier;
                    Upgrades._Idle.CachedIncome[scene, i] /= CachedADMultiplier;
                    Upgrades._Idle.SummIncome[scene, i] /= CachedADMultiplier;
                    Upgrades._Idle.DisplayCellAdditivesIncome(i);

                    cachedIdleIncome[scene] += Upgrades._Idle.SummIncome[scene, i];
                }
            }
            MoneyMenu.SetTickIncome(cachedIdleIncome);
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