using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ListOfEffects : DataStructure
{
    [SerializeField] private GameObject[] Buffs;
    [SerializeField] private GameObject[] Debuffs;
    [SerializeField] private GameObject[] PassiveBuffs;
    [SerializeField] private GameObject BossBuff;
    [SerializeField] private GameObject buffsParent;

    [NonSerialized] public bool[] DebuffCreated = new bool[Enum.GetValues(typeof(DebuffEnum)).Length];
    [NonSerialized] public const int PassiveEffectsLength = 6;
    [NonSerialized] public int CreatedDebuffCount = 0;

    [NonSerialized] private readonly GameObject[] _debuffBuffer = new GameObject[10];
    [NonSerialized] private readonly GameObject[] _buffBuffer = new GameObject[10];
    [NonSerialized] private readonly GameObject[] _passiveEffectsBuffer = new GameObject[Enum.GetValues(typeof(PassiveEffectsEnum)).Length];

    [NonSerialized] private GameObject _bossBuffBuffer = null;
    [NonSerialized] private int _createdBuffCount = 0;
    [NonSerialized] private int _createdPassiveBuffCount = 0;

    [NonSerialized] public float[,] CachedAdditiveStats_PrayerToTheGods = new float[Heroes.HeroCount, 3];
    [NonSerialized] public float[] CachedBuffTime_PrayerToTheGods = new float[Heroes.HeroCount];
    [NonSerialized] public float[,] CachedAdditiveStats_GeneralFee = new float[Heroes.HeroCount, 3];
    [NonSerialized] public float[] CachedBuffTime_GeneralFee = new float[Heroes.HeroCount];

    private const int _standardEffectWidth = 100;
    private const int _standardEffectHeigth = 100;
    private readonly int[] UsedPassiveBuffs = new int[PassiveEffectsLength];

    public class PassiveMultipliers
    {
        public float Damage = 1.5f;
        public float MaxHealth = 1.5f;
        public float HealthRegeneration = 1.5f;
        public float Armor = 1.35f;
        public float Evasion = 1.2f;
        public float AttackSpeed = 1.5f;
        public float BossMultiplier = 2.5f;
    }
    public PassiveMultipliers _PassiveMultipliers = new();

    public enum DebuffEnum
    {
        Bleeding = 0,
        Burning = 1,
        ArmorBreak = 2,
        DecreaseHealthRegeneration = 3,
        PsiAttack = 4,
        Stun = 5
    }

    public enum BuffEnum
    {
        Health = 0,
        Damage = 1,
        Armor = 2,
        WaterAttributes = 3,
        Attributes = 4
    }

    public enum PassiveEffectsEnum
    {
        Health,
        HealthRegeneration,
        Armor,
        Damage,
        Evasion,
        AttackSpeed
    }

    public void CreateDebuffEffect(int effect_index, float duration)
    {
        _debuffBuffer[effect_index] = Instantiate(Debuffs[effect_index], EnemiesSystem.enemy.DebuffEffectsParent);
        SuperimprosedEffects effect = _debuffBuffer[effect_index].GetComponent<SuperimprosedEffects>();
        effect.Duration = duration;
        effect.EffectType = "Debuff";
        EffectsBias(effect_index, "Debuff");
        ListOfEffects.CreatedDebuffCount++;
    }

    public void CreateBuffEffect(int effect_index, float duration)
    {
        _buffBuffer[effect_index] =
            Instantiate(Buffs[effect_index], buffsParent.transform);
        SuperimprosedEffects effect = _buffBuffer[effect_index].GetComponent<SuperimprosedEffects>();
        effect.Duration = duration;
        effect.EffectType = "Buff";
        EffectsBias(effect_index, "Buff");
        ListOfEffects._createdBuffCount++;
    }

    public GameObject[] GetDebuffBuffer()
    {
        return _debuffBuffer;
    }

    public void RandomActivatePassiveEffects()
    {
        DestroyPassiveEffects();

        for (int iter = 0; iter < PassiveEffectsLength; iter++)
        {
            UsedPassiveBuffs[iter] = -1;
        }

        for (int iter = 0; iter < PassiveEffectsLength; iter++)
        {
            int passive_effect_index = Random.Range(0, PassiveEffectsLength);
            int counter = 0;

            for (int i = 0; i < PassiveEffectsLength; i++)
            {
                if (passive_effect_index == UsedPassiveBuffs[i])
                {
                    counter++;
                }
            }

            float probability = Random.Range(0f, 1f);

            if (probability >= 0.5f && counter == 0)
            {
                CreatePassiveEffect(passive_effect_index, "Enemy");
                UsedPassiveBuffs[iter] = passive_effect_index;
            }
        }
    }

    public void CreatePassiveEffect(int effect_index, string type)
    {
        _passiveEffectsBuffer[effect_index] = 
            Instantiate(PassiveBuffs[effect_index], EnemiesSystem.enemy.PassiveEffectsParent);
        EffectsBias();
        _createdPassiveBuffCount++;

        switch (effect_index)
        {
            case (int)PassiveEffectsEnum.Damage:
                PassiveBuffDamage();
                break;
            case (int)PassiveEffectsEnum.Health:
                PassiveBuffHealth();
                break;
            case (int)PassiveEffectsEnum.HealthRegeneration:
                PassiveBuffHealthRegeneration();
                break;
            case (int)PassiveEffectsEnum.Armor:
                PassiveBuffArmor();
                break;
            case (int)PassiveEffectsEnum.Evasion:
                PassiveBuffEvasion();
                break;
            case (int)PassiveEffectsEnum.AttackSpeed:
                PassiveBuffAttackSpeed();
                break;
        }

        void PassiveBuffDamage()
        {
            if (type == "Enemy")
                EnemiesSystem.enemy.Damage *= _PassiveMultipliers.Damage;
            else if (type == "Hero")
                Heroes.CurrentHero.Damage *= _PassiveMultipliers.Damage;
        }

        void PassiveBuffArmor()
        {
            if (type == "Enemy")
                EnemiesSystem.enemy.Armor *= _PassiveMultipliers.Armor;
            else if (type == "Hero")
                Heroes.CurrentHero.Armor *= _PassiveMultipliers.Armor;
        }

        void PassiveBuffEvasion()
        {
            if (type == "Enemy")
            {
                EnemiesSystem.enemy.Evasion *= _PassiveMultipliers.Evasion;
                EnemiesSystem.CheckEvasionToMaxValue();
            }
            else if (type == "Hero")
                Heroes.CurrentHero.Evasion *= _PassiveMultipliers.Evasion;
        }

        void PassiveBuffAttackSpeed()
        {
            if (type == "Enemy")
            {
                EnemiesSystem.enemy.AttackSpeed /= _PassiveMultipliers.AttackSpeed;
                EnemiesSystem.CheckAttackSpeedToMaxValue();
            }
        }

        void PassiveBuffHealth()
        {
            if (type == "Enemy")
                EnemiesSystem.enemy.MaxHealth *= _PassiveMultipliers.MaxHealth;
            else if (type == "Hero")
                Heroes.CurrentHero.MaxHealth *= _PassiveMultipliers.MaxHealth;
        }

        void PassiveBuffHealthRegeneration()
        {
            if (type == "Enemy")
                EnemiesSystem.enemy.HealthRegeneration *= _PassiveMultipliers.HealthRegeneration;
            else if (type == "Hero")
                Heroes.CurrentHero.HealthRegeneration *= _PassiveMultipliers.HealthRegeneration;
        }

        void EffectsBias()
        {
            float x = (_createdPassiveBuffCount * _standardEffectWidth);
            _passiveEffectsBuffer[effect_index].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
        }
    }

    public void CreateBossBuff()
    {
        _bossBuffBuffer = Instantiate(BossBuff, EnemiesSystem.enemy.PassiveEffectsParent);
        _bossBuffBuffer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        IncreaseCharacteristics();
        _createdPassiveBuffCount++;

        void IncreaseCharacteristics()
        {
            EnemiesSystem.enemy.Damage *= _PassiveMultipliers.BossMultiplier;
            EnemiesSystem.enemy.Armor *= _PassiveMultipliers.BossMultiplier;
            EnemiesSystem.enemy.MaxHealth *= _PassiveMultipliers.BossMultiplier;
            EnemiesSystem.enemy.HealthRegeneration *= _PassiveMultipliers.BossMultiplier;

            EnemiesSystem.enemy.ActualHealth = EnemiesSystem.enemy.MaxHealth;
        }
    }

    public void DestroyPassiveEffects()
    {
        for (int effect_index = 0; effect_index < _passiveEffectsBuffer.Length; effect_index++)
        {
            Destroy(_passiveEffectsBuffer[effect_index]);
        }
        _createdPassiveBuffCount = 0;
    }

    public void DestroySelectedBuffEffect(int effect_index)
    {
        if (_buffBuffer[effect_index] != null)
        {
            Destroy(_buffBuffer[effect_index]);
            _createdBuffCount--;
        }
    }

    public void DestroyBuffEffects()
    {
        for (int effect_index = 0; effect_index < _buffBuffer.Length; effect_index++)
        {
            Destroy(_buffBuffer[effect_index]);
        }
        _createdBuffCount = 0;
    }

    public void DecreaseCreatedBuffCount()
    { _createdBuffCount--; }

    public void DecreaseCreatedDebuffCount()
    {  CreatedDebuffCount--; }

    private void EffectsBias(int effect_index, string state)
    {
        if (state == "Debuff")
        {
            float y = (CreatedDebuffCount * _standardEffectHeigth) * -1;
            _debuffBuffer[effect_index].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
        }
        else if (state == "Buff")
        {
            float y = (_createdBuffCount * _standardEffectHeigth) * -1;
            _buffBuffer[effect_index].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);
        }
    }
}