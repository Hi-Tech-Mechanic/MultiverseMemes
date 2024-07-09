using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Battle : DataStructure
{
    #region SerializeField
    public GameObject BattleWindow;
    public GameObject StagesWindow;
    public Button ButtleButton;

    [SerializeField] private Button _directoryButton;
    [SerializeField] private GameObject _averageExitButton;
    [SerializeField] private GameObject _battleExitButton;

    [SerializeField] private GameObject _exitChooseMenu;

    [SerializeField] private TextMeshProUGUI TimeToBossBattleText;
    [SerializeField] private GameObject TimeToBossBattleObject;
    [SerializeField] private GameObject GoToNextStageButton;
    [SerializeField] private Animator NextStageButtonAnimator;

    [SerializeField] private GameObject _damagePrefab;
    [SerializeField] private GameObject _critDamagePrefab;
    [SerializeField] private GameObject _evasionPrefab;
    #endregion

    public const float TickTime = 0.05f;
    public const float TicksInSecond = 20;
    public int ResetCount;

    #region NonSerialized
    [NonSerialized] public bool BattleIsActive = false;
    [NonSerialized] public bool FinalBossIsDead = false;
    [NonSerialized] public const int MaxEnemiesInStage = 6;
    [NonSerialized] public int CurrentEnemiesInStage = 1;
    [NonSerialized] public int EnemiesInStageKilled = 0;
    [NonSerialized] public int CurrentStage = 0;
    [NonSerialized] public int MaxOpenStage = 0;
    [NonSerialized] public const int MaxStage = 150;
    [NonSerialized] private const float _timeToBattleWithBoss = 30;
    [NonSerialized] private const float _armorFormulaFactor = 0.06f;
    #endregion

    public enum EntityType
    {
        Hero = 0,
        Enemy = 1
    }

    public IEnumerator PeriodicDamage(float damage, float duration)
    {
        float timer = 0;
        int enemyIndex = Achievements_R_vs_L.AccumulatedKills;

        while (duration > 0 && Achievements_R_vs_L.AccumulatedKills == enemyIndex && EnemiesSystem.enemy.IsAlive)
        {
            duration -= Time.deltaTime;
            timer += Time.deltaTime;

            if (timer >= 1)
            {
                timer--;
                EnemiesSystem.GetSkillDamage(damage);
            }

            yield return null;
        }
    }

    public void StartBossBattle()
    {
        EnemiesSystem.BossIsAlive = true;
        ListOfEffects.CreateBossBuff();
        EnemiesSystem.enemy.FaithCurrencyReward *= ListOfEffects._PassiveMultipliers.BossMultiplier;
        EnemiesSystem.enemy.ExperienceReward *= ListOfEffects._PassiveMultipliers.BossMultiplier;
        StartCoroutine(DecreaseBattleTime());

        IEnumerator DecreaseBattleTime()
        {
            float timer = _timeToBattleWithBoss;
            TimeToBossBattleObject.SetActive(true);

            while (timer > 0 && EnemiesSystem.enemy.IsAlive)
            {
                TimeToBossBattleText.text = $"{Math.Round(timer, 1)} сек.";
                timer -= Time.deltaTime;
                yield return null;
            }

            TimeToBossBattleObject.SetActive(false);

            if (EnemiesSystem.BossIsAlive)
            {
                CurrentStage--;
                SwitchBattleMode("Deactive");
            }
        }
    }

    public void CheckKilledEnemiesInStage()
    {
        if (EnemiesInStageKilled == CurrentEnemiesInStage || CurrentStageIsBossStage() == true)
        {
            if (CurrentStage >= MaxOpenStage) 
            {
                MaxOpenStage++;

                if (Achievements_R_vs_L.SavedMaxOpenStage < MaxOpenStage)
                {
                    Achievements_R_vs_L.SavedMaxOpenStage = MaxOpenStage;
                    Achievements_R_vs_L.AccumulateStages();
                }
            }

            SwitchBattleMode("Deactive");
        }
    }

    public void SwitchBattleMode(string state)
    {
        if (state == "Active")
        {
            BattleIsActive = true;
            _directoryButton.interactable = false;
            EnemiesInStageKilled = 0;

            if (_averageExitButton.activeInHierarchy)
            {
                StagesWindow.GetComponent<Animator>().SetTrigger("Close");
                _battleExitButton.SetActive(true);
                _averageExitButton.SetActive(false);
            }

            GoToNextStageButton.SetActive(false);
            TimeToBossBattleObject.SetActive(false);
            NextStageButtonAnimator.SetTrigger("Set");
        }
        else if (state == "Deactive")
        {
            BattleIsActive = false;
            _directoryButton.interactable = true;

            if (_exitChooseMenu.activeInHierarchy)
                _exitChooseMenu.SetActive(false);

            EnemiesInStageKilled = 0;

            if (_battleExitButton.activeInHierarchy)
            {
                StagesWindow.SetActive(true);
                StagesWindow.GetComponent<Animator>().SetTrigger("Open");
                _battleExitButton.SetActive(false);
                _averageExitButton.SetActive(true);
            }

            GoToNextStageButton.SetActive(true);
            TimeToBossBattleObject.SetActive(false);
            NextStageButtonAnimator.SetTrigger("Set");

            if (EnemiesSystem.EnemyObject != null)
            {
                EnemiesSystem.DestroyEnemy();
            }
        }
    }

    static public float CalcDealDamage(float damage, float target_armor)
    {
        float summ = damage * (1 - ((_armorFormulaFactor * target_armor) / (1 + (_armorFormulaFactor * target_armor))));
        return summ;
    }

    public void CreateDamagePrefab(float final_damage, Transform parent_transform)
    {
        GameObject obj;
        obj = Instantiate(_damagePrefab, parent_transform);
        obj.GetComponent<TextMeshProUGUI>().text = $"-{ValuesRounding.FormattingValue("", "", final_damage)}";
    }

    public void CreateCritDamagePrefab(float final_damage, Transform parent_transform)
    {
        GameObject obj;
        obj = Instantiate(_critDamagePrefab, parent_transform);
        obj.GetComponent<TextMeshProUGUI>().text = $"-{ValuesRounding.FormattingValue("", "", final_damage)}";
    }

    public void ProbabilityOfDifferentVersionsOfAttack(float damage, int entityThatAttacks)
    {
        if (IsDodge() == false)
        {
            if (AttackIsCrit())
            {
                CauseDamage(damage, entityThatAttacks, true);
            }
            else
            {
                CauseDamage(damage, entityThatAttacks, false);
            }
        }
        else
        {
            if (entityThatAttacks == (int)EntityType.Hero)
            {
                CreateEvasionPrefab(EnemiesSystem.EnemyObject.transform);
            }
            else
            {
                CreateEvasionPrefab(BattleHero.HeroTransform);
            }
        }

        bool IsDodge()
        {
            float rnd = Random.Range(0f, 1f);

            if (entityThatAttacks == (int)EntityType.Hero)
            {
                return EnemiesSystem.enemy.Evasion > rnd;
            }
            else
            {
                return Heroes.CurrentHero.Evasion > rnd;
            }
        }

        bool AttackIsCrit()
        {
            float rnd = Random.Range(0f, 1f);

            if (entityThatAttacks == (int)EntityType.Hero)
            {
                if (Heroes.CurrentHero.CritChance > rnd)
                {
                    damage *= Heroes.CurrentHero.CritDamage;
                    return true;
                }
                return false;
            }
            else
            {
                if (EnemiesSystem.enemy.CritChance > rnd)
                {
                    damage *= EnemiesSystem.enemy.CritStrength;
                    return true;
                }
                return false;
            }
        }

        void CauseDamage(float damage, int entityThatAttacks, bool critState)
        {
            float volatileFactor = Random.Range(0.97f, 1.03f);

            if (entityThatAttacks == (int)EntityType.Enemy)
            {
                float finalDamage = CalcDealDamage(damage, Heroes.CurrentHero.Armor);
                finalDamage *= volatileFactor;
                Heroes.CurrentHero.ActualHealth -= finalDamage;

                if (critState == true)
                    CreateCritDamagePrefab(finalDamage, BattleHero.HeroTransform);
                else CreateDamagePrefab(finalDamage, BattleHero.HeroTransform);

                BattleHero.GetDamageAnimation(CalcDealDamage(EnemiesSystem.enemy.Damage, Heroes.CurrentHero.Armor));

                if (Heroes.CurrentHero.ActualHealth <= 0 && Heroes.CurrentHero.IsAlive == true)
                {
                    BattleHero.HeroIsDead();
                }
            }
            else
            {
                if (EnemiesSystem.EnemyObject != null)
                {
                    float finalDamage = CalcDealDamage(damage, EnemiesSystem.enemy.Armor);
                    finalDamage *= volatileFactor;
                    EnemiesSystem.enemy.ActualHealth -= finalDamage;

                    if (critState == true)
                        CreateCritDamagePrefab(finalDamage, EnemiesSystem.EnemyObject.transform);
                    else CreateDamagePrefab(finalDamage, EnemiesSystem.EnemyObject.transform);

                    StartCoroutine(EnemiesSystem.enemy.AnimatedSlider());
                    EnemiesSystem.enemy.GetDamageAnimation();

                    if (EnemiesSystem.enemy.ActualHealth <= 0 && EnemiesSystem.enemy.IsAlive)
                    {
                        EnemiesSystem.enemy.ActualHealth = 0;
                        EnemiesSystem.EnemyIsDead();
                    }

                    int rnd = Random.Range(0, 4);
                    if (rnd == 0) AudioEffects.PlayEntitiesAudioEffects(BattleHero.MakeDamageClip, "Play");
                    else AudioEffects.PlayEntitiesAudioEffects(EnemiesSystem.GetDamageClip, "Play");

                    if (EnemiesSystem.enemy.ActualHealth <= 0 && EnemiesSystem.enemy.IsAlive == true)
                    {
                        EnemiesSystem.EnemyIsDead();
                    }
                }
            }
        }
    }
    
    public bool CurrentStageIsBossStage()
    {
        if (CurrentStage % 10 == 0 && CurrentStage != 0)
        {
            return true;
        }
        else return false;
    }

    public void CheckEnemiesInStage()
    {
        if (CurrentStage >= 0 && CurrentStage < 20)
            CurrentEnemiesInStage = 1;
        else if (CurrentStage >= 20 && CurrentStage < 40)
            CurrentEnemiesInStage = 2;
        else if (CurrentStage >= 40 && CurrentStage < 70)
            CurrentEnemiesInStage = 3;
        else if (CurrentStage >= 70 && CurrentStage < 100)
            CurrentEnemiesInStage = 4;
        else if (CurrentStage >= 100 && CurrentStage < 120)
            CurrentEnemiesInStage = 5;
        else if (CurrentStage >= 120)
            CurrentEnemiesInStage = 6;
    }

    private void CreateEvasionPrefab(Transform parentTransform)
    {
        Instantiate(_evasionPrefab, parentTransform);
    }
}