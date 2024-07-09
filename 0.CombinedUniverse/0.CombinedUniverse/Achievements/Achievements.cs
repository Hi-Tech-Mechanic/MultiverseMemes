using UnityEngine;
using System;
using System.Collections;

namespace CombinedUniverse
{
    public class Achievements : AchievementsParent
    {
        [SerializeField] private Sprite _clickSprite;
        [SerializeField] private Sprite _timeSprite;
        [SerializeField] private Sprite _memeCoinsSprite;
        [SerializeField] private Sprite _multiplierIncomeSprite;
        [SerializeField] private Sprite _upgradeCostMultiplierSprite;
        [SerializeField] private Sprite _tickTimeSprite;
        [SerializeField] private Sprite _critChanceSprite;
        [SerializeField] private Sprite _critStrengthSprite;
        [SerializeField] private Sprite _ADRewardMultiplierSprite;
        [SerializeField] private Sprite _advertisingBuffTimeSprite;
        [SerializeField] private Sprite _absenceTimeSprite;
        [SerializeField] private Sprite _memeClipCountSprite;
        [SerializeField] private Sprite _watchedADSprite;

        private AchievementUnit[] _clickAchievements;
        private AchievementUnit[] _timeAchievements;
        private AchievementUnit[] _memeCoinsAchievements;
        private AchievementUnit[] _multiplierIncomeAchievements;
        private AchievementUnit[] _upgradeCostMultiplierAchievements;
        private AchievementUnit[] _tickTimeAchievements;
        private AchievementUnit[] _critChanceAchievements;
        private AchievementUnit[] _critStrengthAchievements;
        private AchievementUnit[] _ADRewardMultiplierAchievements;
        private AchievementUnit[] _advertisingBuffTimeAchievements;
        private AchievementUnit[] _absenceTimeAchievements;
        private AchievementUnit[] _memeClipCountAchievements;
        private AchievementUnit[] _watchedADAchievements;

        private readonly int[] _neededClicks = { 50, 250, 500, 1000, 1500, 2000, 3000, 4500, 7500, 10000 };
        private float[] _rewardFromClicks;
        private const float _clicksPerCoin = 10;

        private readonly float[] _neededPlayTime = { 1, 5, 10, 15, 30, 60, 120, 180, 300, 480 };
        private float[] _rewardFromTime;
        private const float _timePerCoin = 0.1f;

        private readonly float[] _neededMemeCoins = { 100, 500, 1000, 5000, 10000, 50000, 100000, 350000};
        private float[] _rewardFromMemeCoins = { 20, 70, 150, 300, 600, 1000, 2000, 5000 };

        private readonly float[] _neededMultiplierIncome = { 5, 10, 20, 40, 60, 80, 100, 120, 140, 155 };
        private float[] _rewardFromMultiplierIncome = { 20, 40, 80, 160, 240, 320, 400, 480, 560, 620 };

        private readonly float[] _neededUpgradeCostMultiplier = { 5, 10, 20, 40, 60, 75 };
        private float[] _rewardFromUpgradeCostMultiplier = { 30, 60, 120, 240, 360, 450 };

        private readonly float[] _neededTickTime = { 5, 10, 20, 40, 60, 80 };
        private float[] _rewardFromTickTime = { 30, 60, 120, 240, 360, 480 };

        private readonly float[] _neededCritChance = { 5, 10, 20, 40, 64 };
        private float[] _rewardFromCritChance = { 35, 70, 140, 280, 450 };

        private readonly float[] _neededCritStrength = { 5, 10, 20, 35, 50, 70 };
        private float[] _rewardFromCritStrength = { 30, 60, 120, 240, 360, 420 };

        private readonly float[] _neededADRewardMultiplier = { 5, 10, 20, 35, 50 };
        private float[] _rewardFromADRewardMultiplier = { 40, 80, 160, 280, 400 };

        private readonly float[] _neededAdvertisingBuffTime = { 5, 10, 20, 40 };
        private float[] _rewardFromAdvertisingBuffTime = { 30, 60, 120, 240 };

        private readonly float[] _neededAbsenceTime = { 5, 10, 20, 40, 60, 80, 100, 123 };
        private float[] _rewardFromAbsenceTime = { 30, 60, 120, 240, 360, 480, 600, 750 };

        private readonly float[] _neededMemeClipCount = { 5, 10, 15, 25 };
        private float[] _rewardFromMemeClipCount = { 50, 100, 150, 250 };

        private readonly float[] _neededWatchedAD = { 1, 5, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100};
        private float[] _rewardFromWatchedAD = { 20, 100, 200, 300, 400, 600, 800, 1000, 1200, 1400, 1600, 1800, 2000 };

        private int[] _neededPurchasedMemeClips = { 1, 5, 10, 15, 20, 25, 31 };
        //protected double[] _neededMoney = { Math.Pow(10, 2), Math.Pow(10, 3),
        //Math.Pow(10, 4), Math.Pow(10, 5), Math.Pow(10, 6),
        //Math.Pow(10, 7), Math.Pow(10, 8), Math.Pow(10, 9),
        //Math.Pow(10, 10), Math.Pow(10, 11), Math.Pow(10, 12), Math.Pow(10, 13) };

        #region InitialPreparations
        public override void Init()
        {
            NeededPurchasedMemeClips = _neededPurchasedMemeClips;
            base.Init();

            //float summ = 0; // hide

            //foreach (int i in ClickRewards)
            //    summ += i;
            //foreach (int i in IdleRewards)
            //    summ += i;
            //foreach (int i in _rewardFromClicks)
            //    summ += i;
            //foreach (int i in _rewardFromClicks)
            //    summ += i;
            //foreach (int i in RewardFromMemeClips)
            //    summ += i;
            //foreach (int i in RewardFromMoney)
            //    summ += i;

            //Debug.Log($"All reward = {summ}");
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            _clickAchievements = new AchievementUnit[_neededClicks.Length];
            _timeAchievements = new AchievementUnit[_neededPlayTime.Length];
            _memeCoinsAchievements = new AchievementUnit[_neededMemeCoins.Length];
            _rewardFromClicks = new float[_neededClicks.Length];
            _rewardFromTime = new float[_neededPlayTime.Length];

            _multiplierIncomeAchievements = new AchievementUnit[_neededMultiplierIncome.Length];
            _upgradeCostMultiplierAchievements = new AchievementUnit[_neededUpgradeCostMultiplier.Length];
            _tickTimeAchievements = new AchievementUnit[_neededTickTime.Length];
            _critChanceAchievements = new AchievementUnit[_neededCritChance.Length];
            _critStrengthAchievements = new AchievementUnit[_neededCritStrength.Length];
            _ADRewardMultiplierAchievements = new AchievementUnit[_neededADRewardMultiplier.Length];
            _advertisingBuffTimeAchievements = new AchievementUnit[_neededAdvertisingBuffTime.Length];
            _absenceTimeAchievements = new AchievementUnit[_neededAbsenceTime.Length];
            _memeClipCountAchievements = new AchievementUnit[_neededMemeClipCount.Length];
            _watchedADAchievements = new AchievementUnit[_neededWatchedAD.Length];
        }

        protected override void DesignateFields()
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

            for (int i = 0; i < NeededMoney.Length; i++, counter++)
            {
                AchievementsForMoney[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < NeededPurchasedMemeClips.Length; i++, counter++)
            {
                AchievementsForMemeClips[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededClicks.Length; i++, counter++)
            {
                _clickAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededPlayTime.Length; i++, counter++)
            {
                _timeAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededMemeCoins.Length; i++, counter++)
            {
                _memeCoinsAchievements[i] = AchievementsScripts[counter];
            }

            for (int i = 0; i < _neededMultiplierIncome.Length; i++, counter++)
            {
                _multiplierIncomeAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededUpgradeCostMultiplier.Length; i++, counter++)
            {
                _upgradeCostMultiplierAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededTickTime.Length; i++, counter++)
            {
                _tickTimeAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededCritChance.Length; i++, counter++)
            {
                _critChanceAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededCritStrength.Length; i++, counter++)
            {
                _critStrengthAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededADRewardMultiplier.Length; i++, counter++)
            {
                _ADRewardMultiplierAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededAdvertisingBuffTime.Length; i++, counter++)
            {
                _advertisingBuffTimeAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededAbsenceTime.Length; i++, counter++)
            {
                _absenceTimeAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededMemeClipCount.Length; i++, counter++)
            {
                _memeClipCountAchievements[i] = AchievementsScripts[counter];
            }
            for (int i = 0; i < _neededWatchedAD.Length; i++, counter++)
            {
                _watchedADAchievements[i] = AchievementsScripts[counter];
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

        protected override void SetDataInAchievementUnit()
        {
            base.SetDataInAchievementUnit();

            CalcRewards();
            SetAchievements_1();
            SetAchievements_2();
            SetAchievements_3();
            SetAchievements_3_1();
            SetAchievements_4();
            SetAchievements_5();
            SetAchievements_6();
            SetAchievements_7();
            SetAchievements_8();
            SetAchievements_9();
            SetAchievements_10();
            SetAchievements_11();
            SetAchievements_12();

            void SetAchievements_1()
            {
                for (int i = 0; i < _neededClicks.Length; i++)
                {
                    _clickAchievements[i].SetDescription($"Кликнуть {_neededClicks[i]} раз");
                    _clickAchievements[i].SetAchievementImage(_clickSprite);
                    _clickAchievements[i].SetTargetProgress(_neededClicks[i]);
                    _clickAchievements[i].SetCurrentProgress(Game.AccumulatedClicks);
                    _clickAchievements[i].SetReward(_rewardFromClicks[i]);
                }
            }

            void SetAchievements_2()
            {
                for (int i = 0; i < _neededPlayTime.Length; i++)
                {
                    if ((_neededPlayTime[i] % 10) == 1)
                        _timeAchievements[i].SetDescription($"Отыграть {_neededPlayTime[i]} минуту");
                    else if ((_neededPlayTime[i] % 10) > 1 && (_neededPlayTime[i] % 10) <= 4)
                        _timeAchievements[i].SetDescription($"Отыграть {_neededPlayTime[i]} минуты");
                    else _timeAchievements[i].SetDescription($"Отыграть {_neededPlayTime[i]} минут");

                    _timeAchievements[i].SetAchievementImage(_timeSprite);
                    _timeAchievements[i].SetTargetProgress(_neededPlayTime[i]);
                    _timeAchievements[i].SetCurrentProgress(Game.AccumulatedTime);
                    _timeAchievements[i].SetReward(_rewardFromTime[i]);
                }
            }

            void SetAchievements_3_1()
            {
                for (int i = 0; i < _neededMemeCoins.Length; i++)
                {
                    _memeCoinsAchievements[i].SetDescription($"Заработать {_neededMemeCoins[i]} мем коинов");
                    _memeCoinsAchievements[i].SetAchievementImage(_memeCoinsSprite);
                    _memeCoinsAchievements[i].SetTargetProgress(_neededMemeCoins[i]);
                    _memeCoinsAchievements[i].SetCurrentProgress(MoneyMenu.AccumulatedMemeCoins);
                    _memeCoinsAchievements[i].SetReward(_rewardFromMemeCoins[i]);
                }
            }

            void SetAchievements_3()
            {
                for (int i = 0; i < _neededMultiplierIncome.Length; i++)
                {
                    _multiplierIncomeAchievements[i].SetDescription($"Купить мультипликатор дохода {_neededMultiplierIncome[i]} раз");
                    _multiplierIncomeAchievements[i].SetAchievementImage(_multiplierIncomeSprite);
                    _multiplierIncomeAchievements[i].SetTargetProgress(_neededMultiplierIncome[i]);
                    _multiplierIncomeAchievements[i].SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.IncomeMultiplier]);
                    _multiplierIncomeAchievements[i].SetReward(_rewardFromMultiplierIncome[i]);
                }
            }

            void SetAchievements_4()
            {
                for (int i = 0; i < _neededUpgradeCostMultiplier.Length; i++)
                {
                    _upgradeCostMultiplierAchievements[i].SetDescription($"Сократить стоимость улучшений {_neededUpgradeCostMultiplier[i]} раз");
                    _upgradeCostMultiplierAchievements[i].SetAchievementImage(_upgradeCostMultiplierSprite);
                    _upgradeCostMultiplierAchievements[i].SetTargetProgress(_neededUpgradeCostMultiplier[i]);
                    _upgradeCostMultiplierAchievements[i].SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.UpgradeCostMultiplier]);
                    _upgradeCostMultiplierAchievements[i].SetReward(_rewardFromUpgradeCostMultiplier[i]);
                }
            }

            void SetAchievements_5()
            {
                for (int i = 0; i < _neededTickTime.Length; i++)
                {
                    _tickTimeAchievements[i].SetDescription($"Сократить время тика {_neededTickTime[i]} раз");
                    _tickTimeAchievements[i].SetAchievementImage(_tickTimeSprite);
                    _tickTimeAchievements[i].SetTargetProgress(_neededTickTime[i]);
                    _tickTimeAchievements[i].SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.TickTime]);
                    _tickTimeAchievements[i].SetReward(_rewardFromTickTime[i]);
                }
            }

            void SetAchievements_6()
            {
                for (int i = 0; i < _neededCritChance.Length; i++)
                {
                    _critChanceAchievements[i].SetDescription($"Увеличить вероятность крита {_neededCritChance[i]} раз");
                    _critChanceAchievements[i].SetAchievementImage(_critChanceSprite);
                    _critChanceAchievements[i].SetTargetProgress(_neededCritChance[i]);
                    _critChanceAchievements[i].SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.CritChance]);
                    _critChanceAchievements[i].SetReward(_rewardFromCritChance[i]);
                }
            }

            void SetAchievements_7()
            {
                for (int i = 0; i < _neededCritStrength.Length; i++)
                {
                    _critStrengthAchievements[i].SetDescription($"Увеличить силу крита {_neededCritStrength[i]} раз");
                    _critStrengthAchievements[i].SetAchievementImage(_critStrengthSprite);
                    _critStrengthAchievements[i].SetTargetProgress(_neededCritStrength[i]);
                    _critStrengthAchievements[i].SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.CritStrength]);
                    _critStrengthAchievements[i].SetReward(_rewardFromCritStrength[i]);
                }
            }

            void SetAchievements_8()
            {
                for (int i = 0; i < _neededADRewardMultiplier.Length; i++)
                {
                    _ADRewardMultiplierAchievements[i].SetDescription($"Увеличить вознаграждение за рекламу {_neededADRewardMultiplier[i]} раз");
                    _ADRewardMultiplierAchievements[i].SetAchievementImage(_ADRewardMultiplierSprite);
                    _ADRewardMultiplierAchievements[i].SetTargetProgress(_neededADRewardMultiplier[i]);
                    _ADRewardMultiplierAchievements[i].SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.ADRewardMultiplier]);
                    _ADRewardMultiplierAchievements[i].SetReward(_rewardFromADRewardMultiplier[i]);
                }
            }

            void SetAchievements_9()
            {
                for (int i = 0; i < _neededAdvertisingBuffTime.Length; i++)
                {
                    _advertisingBuffTimeAchievements[i].SetDescription($"Увеличить рекламный усилитель {_neededAdvertisingBuffTime[i]} раз");
                    _advertisingBuffTimeAchievements[i].SetAchievementImage(_advertisingBuffTimeSprite);
                    _advertisingBuffTimeAchievements[i].SetTargetProgress(_neededAdvertisingBuffTime[i]);
                    _advertisingBuffTimeAchievements[i].SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.AdvertisingBuffTime]);
                    _advertisingBuffTimeAchievements[i].SetReward(_rewardFromAdvertisingBuffTime[i]);
                }
            }

            void SetAchievements_10()
            {
                for (int i = 0; i < _neededAbsenceTime.Length; i++)
                {
                    _absenceTimeAchievements[i].SetDescription($"Увеличить макс. оффлайн время {_neededAbsenceTime[i]} раз");
                    _absenceTimeAchievements[i].SetAchievementImage(_absenceTimeSprite);
                    _absenceTimeAchievements[i].SetTargetProgress(_neededAbsenceTime[i]);
                    _absenceTimeAchievements[i].SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.AbsenceTime]);
                    _absenceTimeAchievements[i].SetReward(_rewardFromAbsenceTime[i]);
                }
            }

            void SetAchievements_11()
            {
                for (int i = 0; i < _neededMemeClipCount.Length; i++)
                {
                    _memeClipCountAchievements[i].SetDescription($"Расширить буфер мем звуков {_neededMemeClipCount[i]} раз");
                    _memeClipCountAchievements[i].SetAchievementImage(_memeClipCountSprite);
                    _memeClipCountAchievements[i].SetTargetProgress(_neededMemeClipCount[i]);
                    _memeClipCountAchievements[i].SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.MemeClipCount]);
                    _memeClipCountAchievements[i].SetReward(_rewardFromMemeClipCount[i]);
                }
            }

            void SetAchievements_12()
            {
                for (int i = 0; i < _neededWatchedAD.Length; i++)
                {
                    _watchedADAchievements[i].SetDescription($"Посмотреть рекламу {_neededWatchedAD[i]} раз");
                    _watchedADAchievements[i].SetAchievementImage(_watchedADSprite);
                    _watchedADAchievements[i].SetTargetProgress(_neededWatchedAD[i]);
                    _watchedADAchievements[i].SetCurrentProgress(Game.CountWatchedAD);
                    _watchedADAchievements[i].SetReward(_rewardFromWatchedAD[i]);
                }
            }

            void CalcRewards()
            {
                for (int i = 0; i < _neededClicks.Length; i++)
                    _rewardFromClicks[i] = (float)Math.Round(_neededClicks[i] / _clicksPerCoin);
                for (int i = 0; i < _neededPlayTime.Length; i++)
                    _rewardFromTime[i] = (float)Math.Round(_neededPlayTime[i] / _timePerCoin);
            }
        }
        #endregion

        #region ButtonEvent
        public override void SwitchActiveWindow()
        {
            base.SwitchActiveWindow();
        }
        #endregion

        #region TaskRegistration 
        private IEnumerator DisplayingTimeProgress()
        {
            while (WindowIsActive)
            {
                UpdateTimeProgress();
                yield return null;
            }
        }
        #endregion

        #region UpdateForEachTask
        public override void UpdateAllInfo()
        {
            base.UpdateAllInfo();

            UpdateClickProgress();
            UpdateTimeProgress();
            UpdateMemeCoinsProgress();
            UpdateIncomeMultiplierProgress();
            UpdateUpgradeCostMultiplierProgress();
            UpdateTickTimeProgress();
            UpdateCritChanceProgress();
            UpdateCritStrengthProgress();
            UpdateADRewardMultiplierProgress();
            UpdateAdvertisingBuffTimeProgress();
            UpdateAbsenceTimeProgress();
            UpdateMemeClipCountProgress();
            UpdateWatchedADProgress();

            if (WindowIsActive)
            {
                StartCoroutine(DisplayingTimeProgress());
            }
        }

        private void UpdateClickProgress()
        {
            foreach (AchievementUnit achievement in _clickAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(Game.AccumulatedClicks);
            }
        }

        private void UpdateTimeProgress()
        {
            foreach (AchievementUnit achievement in _timeAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(Game.AccumulatedTime);
            }
        }

        private void UpdateMemeCoinsProgress()
        {
            foreach (AchievementUnit achievement in _memeCoinsAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(MoneyMenu.AccumulatedMemeCoins);
            }
        }

        private void UpdateIncomeMultiplierProgress()
        {
            foreach (AchievementUnit achievement in _multiplierIncomeAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.IncomeMultiplier]);
            }
        }

        private void UpdateUpgradeCostMultiplierProgress()
        {
            foreach (AchievementUnit achievement in _upgradeCostMultiplierAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.UpgradeCostMultiplier]);
            }
        }

        private void UpdateTickTimeProgress()
        {
            foreach (AchievementUnit achievement in _tickTimeAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.TickTime]);
            }
        }

        private void UpdateCritChanceProgress()
        {
            foreach (AchievementUnit achievement in _critChanceAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.CritChance]);
            }
        }

        private void UpdateCritStrengthProgress()
        {
            foreach (AchievementUnit achievement in _critStrengthAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.CritStrength]);
            }
        }

        private void UpdateADRewardMultiplierProgress()
        {
            foreach (AchievementUnit achievement in _ADRewardMultiplierAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.ADRewardMultiplier]);
            }
        }

        private void UpdateAdvertisingBuffTimeProgress()
        {
            foreach (AchievementUnit achievement in _advertisingBuffTimeAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.AdvertisingBuffTime]);
            }
        }

        private void UpdateAbsenceTimeProgress()
        {
            foreach (AchievementUnit achievement in _absenceTimeAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.AbsenceTime]);
            }
        }

        private void UpdateMemeClipCountProgress()
        {
            foreach (AchievementUnit achievement in _memeClipCountAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(GlobalUpgrades.PurchasedCounter[(int)GlobalUpgrades.GlobalUpgradesIndex.MemeClipCount]);
            }
        }

        private void UpdateWatchedADProgress()
        {
            foreach (AchievementUnit achievement in _watchedADAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(Game.CountWatchedAD);
            }
        }
        #endregion

        #region Other
        protected override void CalcAndDisplayRequiredUnitsCount()
        {
            int summ = 0;
            summ += _neededClicks.Length + _neededPlayTime.Length + NeededMoney.Length;
            Debug.Log("Required achievements units count = " + summ);
        }
        #endregion
    }
}