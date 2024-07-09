using System;
using TMPro;
using UnityEngine;

namespace Russians_vs_Lizards
{
    public class Achievements : AchievementsParent
    {
        [Header("Система достижений")]
        [Space(15)]
        [SerializeField] private TextMeshProUGUI _textMemeCoins;

        [SerializeField] private Sprite _killSprite;
        [SerializeField] private Sprite _faithSprite;
        [SerializeField] private Sprite _ancestralPowerSprite;
        [SerializeField] private Sprite _heroSprite;
        [SerializeField] private Sprite _skillSprite;
        [SerializeField] private Sprite _armorSprite;
        [SerializeField] private Sprite _weaponSprite;
        [SerializeField] private Sprite _perkSprite;
        [SerializeField] private Sprite _stageSprite;

        [NonSerialized] private AchievementUnit[] _killsAchievements;
        [NonSerialized] private AchievementUnit[] _faithAchievements;
        [NonSerialized] private AchievementUnit[] _ancestralPowerAchievements;
        [NonSerialized] private AchievementUnit[] _heroesAchievements;
        [NonSerialized] private AchievementUnit[] _skillsAchievements;
        [NonSerialized] private AchievementUnit[] _weaponsAchievements;
        [NonSerialized] private AchievementUnit[] _armorsAchievements;
        [NonSerialized] private AchievementUnit[] _perksAchievements;
        [NonSerialized] private AchievementUnit[] _stagesAchievements;

        [Header("Подсчет общего количества убийств")]
        [Range(0, 600)] [SerializeField] private int _accumulatedKills = 0;
        [NonSerialized] public readonly int[] _neededKills = { 5, 10, 25, 50, 100, 150, 200, 300, 400, 500, 600, 750 };
        [NonSerialized] private int[] _rewardFromKills;
        [NonSerialized] private const float _unitsForCoin_1 = 0.5f;
        public int AccumulatedKills { get { return _accumulatedKills; } set { _accumulatedKills = value; } }

        [Header("Подсчет общего количества веры")]
        [Range(0, 1000000)] [SerializeField] private float _accumulatedFaith = 0;
        [NonSerialized] private readonly float[] _neededFaith = { 500, 1000, 2500, 5000, 10000, 25000, 50000, 100000, 150000, 250000, 450000, 650000, 850000, 1000000 };
        [NonSerialized] private int[] _rewardFromFaith = { 5, 10, 20, 40, 80, 140, 200, 300, 440, 600, 750, 850, 1100, 1400};
        public float AccumulatedFaith { get { return _accumulatedFaith; } set { _accumulatedFaith = value; } }

        [Header("Подсчет общего количества силы предков")]
        [Range(0, 100000)] [SerializeField] private float _accumulatedAncestralPower = 0;
        [NonSerialized] private readonly float[] _neededAncestralPower = { 100, 500, 1000, 2500, 5000, 10000, 25000, 50000, 100000 };
        [NonSerialized] private int[] _rewardFromAncestralPower = { 20, 40, 80, 150, 300, 625, 1250, 2500, 5000 };
        public float AccumulatedAncestralPower { get { return _accumulatedAncestralPower; } set { _accumulatedAncestralPower = value; } }

        [Header("Подсчет купленных богатырей")]
        [Range(0, 4)][SerializeField] private int _accumulatedHeroes = 0;
        [NonSerialized] private readonly int[] _neededHeroes = { 1, 2, 3, 4 };
        [NonSerialized] private readonly int[] _rewardFromHeroes = { 50, 200, 400, 1600 };
        [NonSerialized] public bool[] HeroIsUnlocked = new bool[Heroes.HeroCount];
        public int AccumulatedHeroes { get { return _accumulatedHeroes; } set { _accumulatedHeroes = value; } }

        [Header("Подсчет купленных способностей")]
        [Range(0, 8)] [SerializeField] private int _accumulatedSkills = 0;
        [NonSerialized] private readonly int[] _neededSkills = { 1, 2, 3, 4, 5, 6, 7 };
        [NonSerialized] private int[] _rewardFromSkills;
        [NonSerialized] private const float _unitsForCoin_4 = 0.02f;
        [NonSerialized] public bool[] SkillIsUnlocked = new bool[Skills.SkillsCount];
        public int AccumulatedSkills { get { return _accumulatedSkills; } set { _accumulatedSkills = value; } }

        [Header("Подсчет купленных комплектов брони")]
        [Range(0, 3)] [SerializeField] private int _accumulatedArmors = 0;
        [NonSerialized] private readonly int[] _neededArmors = { 1, 2, 3};
        [NonSerialized] private readonly int[] _rewardFromArmors = { 100, 400, 1600 };
        [NonSerialized] public bool[] ArmorIsUnlocked = new bool[3];
        public int AccumulatedArmors { get { return _accumulatedArmors; } set { _accumulatedArmors = value; } }

        [Header("Подсчет купленного оружия")]
        [Range(0, 9)] [SerializeField] private int _accumulatedWeapons = 0;
        [NonSerialized] private readonly int[] _neededWeapons = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        [NonSerialized] private readonly int[] _rewardFromWeapons = { 30, 70, 110, 170, 230, 290, 380, 460, 540 };
        [NonSerialized] public bool[] WeaponIsUnlocked = new bool[Weapons.WeaponCount];
        public int AccumulatedWeapons { get { return _accumulatedWeapons; } set { _accumulatedWeapons = value; } }

        [Header("Подсчет купленных перков")]
        [Range(0, 216)][SerializeField] private int _accumulatedPerks = 0;
        [NonSerialized] private readonly int[] _neededPerks = { 5, 10, 20, 40, 80, 120, 160, 216 };
        [NonSerialized] private int[] _rewardFromPerks;
        [NonSerialized] private const float _unitsForCoin_5 = 0.1f;
        public int AccumulatedPerks { get { return _accumulatedPerks; } set { _accumulatedPerks = value; } }

        [Header("Подсчет пройденых этапов")]
        [Range(0, 150)][SerializeField] private int _accumulatedStages = 0;
        [NonSerialized] private readonly int[] _neededStages = { 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150 };
        [NonSerialized] private int[] _rewardFromStages;
        [NonSerialized] private const float _unitsForCoin_6 = 0.2f;
        [NonSerialized] public int SavedMaxOpenStage;
        public int AccumulatedStages { get { return _accumulatedStages; } set { _accumulatedStages = value; } }

        public override void Init()
        {
            if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
                base.Init();
        }

        #region InitialPreparations
        protected override void InitializeFields()
        {
            AchievementsBuffer = new GameObject[ContentObject.transform.childCount];
            AchievementsScripts = new AchievementUnit[AchievementsBuffer.Length];

            _killsAchievements = new AchievementUnit[_neededKills.Length];
            _faithAchievements = new AchievementUnit[_neededFaith.Length];
            _ancestralPowerAchievements = new AchievementUnit[_neededAncestralPower.Length];
            _heroesAchievements = new AchievementUnit[_neededHeroes.Length];
            _skillsAchievements = new AchievementUnit[_neededSkills.Length];
            _weaponsAchievements = new AchievementUnit[_neededWeapons.Length];
            _armorsAchievements = new AchievementUnit[_neededArmors.Length];
            _perksAchievements = new AchievementUnit[_neededPerks.Length];
            _stagesAchievements = new AchievementUnit[_neededStages.Length];

            _rewardFromKills = new int[_neededKills.Length];
            _rewardFromSkills = new int[_neededSkills.Length];
            _rewardFromPerks = new int[_neededPerks.Length];
            _rewardFromStages = new int[_neededStages.Length];
        }

        protected override void DesignateFields()
        {
            for (int i = 0; i < AchievementsBuffer.Length; i++)
            {
                AchievementsBuffer[i] = ContentObject.transform.GetChild(i).gameObject;
            }

            int counter = 0;

            foreach (GameObject tmp in AchievementsBuffer)
            {
                AchievementsScripts[counter] = tmp.GetComponent<AchievementUnit>();
                AchievementsScripts[counter].RewardID = counter + Enum.GetNames(typeof(Game.RewardIndex)).Length;
                counter++;
            }

            counter = 0;

            for (int i = 0; i < _neededKills.Length; i++)
            {
                _killsAchievements[i] = AchievementsScripts[counter];
                IncrementCounter();
            }         
            for (int i = 0; i < _neededFaith.Length; i++)
            {
                _faithAchievements[i] = AchievementsScripts[counter];
                IncrementCounter();
            }         
            for (int i = 0; i < _neededAncestralPower.Length; i++)
            {
                _ancestralPowerAchievements[i] = AchievementsScripts[counter];
                IncrementCounter();
            }
            for (int i = 0; i < _neededHeroes.Length; i++)
            {
                _heroesAchievements[i] = AchievementsScripts[counter];
                IncrementCounter();
            }
            for (int i = 0; i < _neededSkills.Length; i++)
            {
                _skillsAchievements[i] = AchievementsScripts[counter];
                IncrementCounter();
            }
            for (int i = 0; i < _neededWeapons.Length; i++)
            {
                _weaponsAchievements[i] = AchievementsScripts[counter];
                IncrementCounter();
            }
            for (int i = 0; i < _neededArmors.Length; i++)
            {
                _armorsAchievements[i] = AchievementsScripts[counter];
                IncrementCounter();
            }
            for (int i = 0; i < _neededPerks.Length; i++)
            {
                _perksAchievements[i] = AchievementsScripts[counter];
                IncrementCounter();
            }
            for (int i = 0; i < _neededStages.Length; i++)
            {
                _stagesAchievements[i] = AchievementsScripts[counter];
                IncrementCounter();
            }

            CheckSavedData();

            void IncrementCounter()
            { counter++; }

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
            CalcRewards();
            SetAchievementArray_1();
            SetAchievementArray_2();
            SetAchievementArray_3();
            SetAchievementArray_4();
            SetAchievementArray_5();
            SetAchievementArray_6();
            SetAchievementArray_7();
            SetAchievementArray_8();
            SetAchievementArray_9();

            void SetAchievementArray_1()
            {
                for (int i = 0; i < _neededKills.Length; i++)
                {
                    _killsAchievements[i].SetDescription($"Убить {_neededKills[i]} ящеров");
                    _killsAchievements[i].SetAchievementImage(_killSprite);
                    _killsAchievements[i].SetTargetProgress(_neededKills[i]);
                    _killsAchievements[i].SetCurrentProgress(_accumulatedKills);
                    _killsAchievements[i].SetReward(_rewardFromKills[i]);
                }
            }

            void SetAchievementArray_2()
            {
                for (int i = 0; i < _neededFaith.Length; i++)
                {
                    _faithAchievements[i].SetDescription($"Заработать {_neededFaith[i]} веры");
                    _faithAchievements[i].SetAchievementImage(_faithSprite);
                    _faithAchievements[i].SetTargetProgress(_neededFaith[i]);
                    _faithAchievements[i].SetCurrentProgress(_accumulatedFaith);
                    _faithAchievements[i].SetReward(_rewardFromFaith[i]);
                }
            }

            void SetAchievementArray_3()
            {
                for (int i = 0; i < _neededAncestralPower.Length; i++)
                {
                    _ancestralPowerAchievements[i].SetDescription($"Заработать {_neededAncestralPower[i]} силы предков");
                    _ancestralPowerAchievements[i].SetAchievementImage(_ancestralPowerSprite);
                    _ancestralPowerAchievements[i].SetTargetProgress(_neededAncestralPower[i]);
                    _ancestralPowerAchievements[i].SetCurrentProgress(_accumulatedAncestralPower);
                    _ancestralPowerAchievements[i].SetReward(_rewardFromAncestralPower[i]);
                }
            }

            void SetAchievementArray_4()
            {
                for (int i = 0; i < _neededHeroes.Length; i++)
                {
                    if (_neededHeroes[i] == 1)
                        _heroesAchievements[i].SetDescription($"Позвать {_neededHeroes[i]} богатыря");
                    else _heroesAchievements[i].SetDescription($"Позвать {_neededHeroes[i]} богатырей");
                    _heroesAchievements[i].SetAchievementImage(_heroSprite);
                    _heroesAchievements[i].SetTargetProgress(_neededHeroes[i]);
                    _heroesAchievements[i].SetCurrentProgress(_accumulatedHeroes);
                    _heroesAchievements[i].SetReward(_rewardFromHeroes[i]);
                }
            }

            void SetAchievementArray_5()
            {
                for (int i = 0; i < _neededSkills.Length; i++)
                {
                    string postfix;
                    if (_neededSkills[i] == 1)
                        postfix = "способность";
                    else if (_neededSkills[i] > 1 && _neededSkills[i] <= 4)
                        postfix = "способности";
                    else postfix = "способностей";

                    _skillsAchievements[i].SetDescription($"Купить {_neededSkills[i]} {postfix}");
                    _skillsAchievements[i].SetAchievementImage(_skillSprite);
                    _skillsAchievements[i].SetTargetProgress(_neededSkills[i]);
                    _skillsAchievements[i].SetCurrentProgress(_accumulatedSkills);
                    _skillsAchievements[i].SetReward(_rewardFromSkills[i]);
                }
            }

            void SetAchievementArray_6()
            {
                for (int i = 0; i < _neededWeapons.Length; i++)
                {
                    _weaponsAchievements[i].SetDescription($"Купить {_neededWeapons[i]} ед. оружия");
                    _weaponsAchievements[i].SetAchievementImage(_weaponSprite);
                    _weaponsAchievements[i].SetTargetProgress(_neededWeapons[i]);
                    _weaponsAchievements[i].SetCurrentProgress(_accumulatedWeapons);
                    _weaponsAchievements[i].SetReward(_rewardFromWeapons[i]);
                }
            }

            void SetAchievementArray_7()
            {
                for (int i = 0; i < _neededArmors.Length; i++)
                {
                    if (_neededHeroes[i] == 1)
                        _armorsAchievements[i].SetDescription($"Купить {_neededArmors[i]} комплект брони");
                    else _armorsAchievements[i].SetDescription($"Купить {_neededArmors[i]} комплекта брони");
                    _armorsAchievements[i].SetAchievementImage(_armorSprite);
                    _armorsAchievements[i].SetTargetProgress(_neededArmors[i]);
                    _armorsAchievements[i].SetCurrentProgress(_accumulatedArmors);
                    _armorsAchievements[i].SetReward(_rewardFromArmors[i]);
                }
            }

            void SetAchievementArray_8()
            {
                for (int i = 0; i < _neededPerks.Length; i++)
                {
                    _perksAchievements[i].SetDescription($"Изучить {_neededPerks[i]} ед. навыков");
                    _perksAchievements[i].SetAchievementImage(_perkSprite);
                    _perksAchievements[i].SetTargetProgress(_neededPerks[i]);
                    _perksAchievements[i].SetCurrentProgress(_accumulatedPerks);
                    _perksAchievements[i].SetReward(_rewardFromPerks[i]);
                }
            }

            void SetAchievementArray_9()
            {
                for (int i = 0; i < _neededStages.Length; i++)
                {
                    _stagesAchievements[i].SetDescription($"Дойти до {_neededStages[i]}-ой поляны");
                    _stagesAchievements[i].SetAchievementImage(_stageSprite);
                    _stagesAchievements[i].SetTargetProgress(_neededStages[i]);
                    _stagesAchievements[i].SetCurrentProgress(_accumulatedStages);
                    _stagesAchievements[i].SetReward(_rewardFromStages[i]);
                }
            }

            void CalcRewards()
            {
                for (int i = 0; i < _neededKills.Length; i++)
                    _rewardFromKills[i] = (int)Math.Round(_neededKills[i] / _unitsForCoin_1);
                //for (int i = 0; i < _neededFaith.Length; i++)
                //    _rewardFromFaith[i] = (int)Math.Round(_neededFaith[i] / _unitsForCoin_2);
                //for (int i = 0; i < _neededAncestralPower.Length; i++)
                //    _rewardFromAncestralPower[i] = (int)Math.Round(_neededAncestralPower[i] / _unitsForCoin_3);
                for (int i = 0; i < _neededSkills.Length; i++)
                    _rewardFromSkills[i] = (int)Math.Round(_neededSkills[i] / _unitsForCoin_4);
                for (int i = 0; i < _neededPerks.Length; i++)
                    _rewardFromPerks[i] = (int)Math.Round(_neededPerks[i] / _unitsForCoin_5);
                for (int i = 0; i < _neededStages.Length; i++)
                    _rewardFromStages[i] = (int)Math.Round(_neededStages[i] / _unitsForCoin_6);
            }
        }
        #endregion

        #region ButtonEvent
        public override void SwitchActiveWindow()
        {
            base.SwitchActiveWindow();
            //UpdateAllInfo();
        }
        #endregion

        #region TaskRegistration
        public void AccumulateKills()
        {
            _accumulatedKills++;
            if (WindowIsActive)
                UpdateKillsProgress();
        }

        public void AccumulateFaith(float income)
        {
            _accumulatedFaith += income;
            if (WindowIsActive)
                UpdateFaithProgress();
        }

        public void AccumulateAncestralPower(float income)
        {
            _accumulatedAncestralPower += income;
            if (WindowIsActive)
                UpdateAncestralPowerProgress();
        }

        public void AccumulateHeroes()
        {
            _accumulatedHeroes++;
        }

        public void AccumulateSkills()
        {
            _accumulatedSkills++;
        }

        public void AccumulateWeapons()
        {
            _accumulatedWeapons++;
        }

        public void AccumulateArmors()
        {
            _accumulatedArmors++;
        }

        public void AccumulatePerks()
        {
            _accumulatedPerks++;
        }

        public void AccumulateStages()
        {
            _accumulatedStages++;
        }
        #endregion

        #region UpdateForEachTask
        public override void UpdateAllInfo()
        {
            UpdateKillsProgress();
            UpdateFaithProgress();
            UpdateAncestralPowerProgress();
            UpdateHeroesProgress();
            UpdateSkillsProgress();
            UpdateWeaponsProgress();
            UpdateArmorsProgress();
            UpdatePerksProgress();
            UpdateStagesProgress();
        }

        private void UpdateKillsProgress()
        {
            foreach (AchievementUnit achievement in _killsAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(_accumulatedKills);
            }
        }

        private void UpdateFaithProgress()
        {
            foreach (AchievementUnit achievement in _faithAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(_accumulatedFaith);
            }
        }

        private void UpdateAncestralPowerProgress()
        {
            foreach (AchievementUnit achievement in _ancestralPowerAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(_accumulatedAncestralPower);
            }
        }

        private void UpdateHeroesProgress()
        {
            foreach (AchievementUnit achievement in _heroesAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(_accumulatedHeroes);
            }
        }

        private void UpdateSkillsProgress()
        {
            foreach (AchievementUnit achievement in _skillsAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(_accumulatedSkills);
            }
        }

        private void UpdateWeaponsProgress()
        {
            foreach (AchievementUnit achievement in _weaponsAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(_accumulatedWeapons);
            }
        }

        private void UpdateArmorsProgress()
        {
            foreach (AchievementUnit achievement in _armorsAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(_accumulatedArmors);
            }
        }

        private void UpdatePerksProgress()
        {
            foreach (AchievementUnit achievement in _perksAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(_accumulatedPerks);
            }
        }

        private void UpdateStagesProgress()
        {
            foreach (AchievementUnit achievement in _stagesAchievements)
            {
                if (!achievement.RewardIsReady && !achievement.Claimed)
                    achievement.SetCurrentProgress(_accumulatedStages);
            }
        }
        #endregion

        #region Other
        protected override void CalcAndDisplayRequiredUnitsCount()
        {
            int summ = 0;
            summ += _neededKills.Length + _neededFaith.Length + _neededAncestralPower.Length
                + _neededSkills.Length + _neededWeapons.Length + _neededArmors.Length;
            Debug.Log("Required achievements units count = " + summ);
        }
        #endregion
    }
}