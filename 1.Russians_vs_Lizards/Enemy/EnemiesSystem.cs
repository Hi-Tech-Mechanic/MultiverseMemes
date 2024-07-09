using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemiesSystem : DataStructure
{
    #region SerializeField
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private GameObject[] _enemyCreater;
    [SerializeField] private Sprite[] _enemySprites;

    [Header("Звуковые эффекты")]
    [Tooltip("Получение урона")] public AudioClip[] GetDamageClip;
    [Tooltip("Нанесение урона")] public AudioClip[] MakeDamageClip;
    [Tooltip("Насмешка")] public AudioClip[] MockeryClip;
    [Tooltip("Звук смерти")][SerializeField] private AudioClip[] DeadClip;
    #endregion

    #region Functional
    [NonSerialized] public Enemy enemy = new();
    [NonSerialized] public GameObject EnemyObject;
    [NonSerialized] private readonly Animator[] _enemyCreaterAnimator = new Animator[Battle.MaxEnemiesInStage];

    private const int _AttackAnimationsCount = 4;
    private const int _ReceivedDamageAnimationsCount = 4;
    [NonSerialized] public bool BossIsAlive = false;
    #endregion

    #region CharacteristicsValues
    private const float _MultiplicateCharacteristics = 0.55f;

    private const float _constMaxHealth = 100;
    private const float _constHealthRegeneration = 1f;
    private const float _constDamage = 40;
    private const float _constArmor = 1;
    private const float _maxEvasion = 0.60f;
    private const float _maxCritChance = 0.75f;
    private const float _constCritDamage = 0.1f;
    private const float _constAttackSpeed = 4f;
    private const float _maxAttackSpeed = 0.8f;
    private const float _constExperienceReward = 75;
    private const float _constFaithCurrencyReward = 50;
    private float СonstCritChance => _maxCritChance / Battle.MaxStage;
    private float СonstEvasion => (_maxEvasion - 0.1f) / Battle.MaxStage;
    private float DeductibleAttackSpeed => (_constAttackSpeed - _maxAttackSpeed - 0.4f) / Battle.MaxStage;

    [NonSerialized] private float _actualMaxHealth;
    [NonSerialized] private float _actualHealthRegeneration;
    [NonSerialized] private float _actualDamage;
    [NonSerialized] private float _actualArmor;
    [NonSerialized] private float _actualEvasion;
    [NonSerialized] private float _actualCritChance;
    [NonSerialized] private float _actualCritDamage;
    [NonSerialized] private float _actualAttackSpeed;
    [NonSerialized] private float _actualExperienceReward;
    [NonSerialized] private float _actualFaithCurrencyReward;
    #endregion

    public class Enemy
    {
        #region Characteristics
        public float ActualHealth
        {
            get { return _actualHealth; }
            set { _actualHealth = value; UpdateHealth(); }
        }
        private float _actualHealth;

        public float MaxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = value; UpdateHealth(); }
        }
        private float _maxHealth;

        public float HealthRegeneration
        {
            get { return _healthRegeneration; }
            set { _healthRegeneration = value; UpdateHealthRegenerationText(); }
        }
        private float _healthRegeneration;

        public float Armor
        {
            get { return _armor; }
            set { _armor = value; UpdateCharacteristicsText(); }
        }
        private float _armor;

        public float Evasion
        {
            get { return _evasion; }
            set { _evasion = value; UpdateCharacteristicsText(); }
        }
        private float _evasion;

        public float Damage;
        public float AttackSpeed;
        public float CritChance;
        public float CritStrength;
        public int Level;
        #endregion

        #region Reward
        public float ExperienceReward;
        public float FaithCurrencyReward;
        #endregion

        #region Functional
        public bool IsAlive = false;
        public float ReceivedStun = 0;
        #endregion

        #region UI
        public Image EnemyImage;
        public Transform PassiveEffectsParent;
        public Transform DebuffEffectsParent;
        public Animator Animator;
        private Slider _healthBar;
        private Slider _healthBarSecond;
        private TextMeshProUGUI _healthText;
        private TextMeshProUGUI _healthRegenerationText;
        private TextMeshProUGUI _healthPercentText;

        private GameObject _damageIcon;
        private GameObject _armorIcon;
        private GameObject _evasionIcon;
        private GameObject _attackSpeedIcon;
        private GameObject _critChanceIcon;
        private GameObject _critDamageIcon;
        private TextMeshProUGUI _damageText;
        private TextMeshProUGUI _armorText;
        private TextMeshProUGUI _evasionText;
        private TextMeshProUGUI _attackSpeedText;
        private TextMeshProUGUI _critChanceText;
        private TextMeshProUGUI _critDamageText;
        #endregion

        public void FindComponentsSets(GameObject enemyObject)
        {
            Animator = enemyObject.GetComponent<Animator>();

            RectTransform[] components = enemyObject.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform var in components)
            {
                switch (var.name)
                {
                    case ("Image_Enemy"):
                        EnemyImage = var.GetComponent<Image>();
                        break;
                    case ("PassiveEffectsParent"):
                        PassiveEffectsParent = var.gameObject.transform;
                        break;
                    case ("DebuffEffectsParent"):
                        DebuffEffectsParent = var.gameObject.transform;
                        break;
                    case ("Damage"):
                        _damageIcon = var.gameObject;
                        break;
                    case ("Armor"):
                        _armorIcon = var.gameObject;
                        break;
                    case ("Evasion"):
                        _evasionIcon = var.gameObject;
                        break;
                    case ("AttackSpeed"):
                        _attackSpeedIcon = var.gameObject;
                        break;
                    case ("CritChance"):
                        _critChanceIcon = var.gameObject;
                        break;
                    case ("CritDamage"):
                        _critDamageIcon = var.gameObject;
                        break;
                    case ("Slider_EnemyHealth"):
                        _healthBar = var.GetComponent<Slider>();
                        break;
                    case ("SecondSlider_EnemyHealth"):
                        _healthBarSecond = var.GetComponent<Slider>();
                        break;
                    case ("HealthText"):
                        _healthText = var.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("HealthRegenerationValue"):
                        _healthRegenerationText = var.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("HealthPercentValue"):
                        _healthPercentText = var.GetComponent<TextMeshProUGUI>();
                        break;

                    default: break;
                }
            }

            _damageText = _damageIcon.GetComponentInChildren<TextMeshProUGUI>();
            _armorText = _armorIcon.GetComponentInChildren<TextMeshProUGUI>();
            _evasionText = _evasionIcon.GetComponentInChildren<TextMeshProUGUI>();
            _attackSpeedText = _attackSpeedIcon.GetComponentInChildren<TextMeshProUGUI>();
            _critChanceText = _critChanceIcon.GetComponentInChildren<TextMeshProUGUI>();
            _critDamageText = _critDamageIcon.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void UpdateCharacteristicsText()
        {
            _damageText.text = $"{ValuesRounding.FormattingValue("", "", Damage)}";
            _armorText.text = $"{Math.Round(Armor, 1)}";
            _evasionText.text = $"{Math.Round(Evasion * 100, 2)}%";
            _attackSpeedText.text = $"{Math.Round(AttackSpeed, 2)}сек.";
            _critChanceText.text = $"{Math.Round(CritChance * 100, 2)}%";
            _critDamageText.text = $"{Math.Round((1 + CritStrength) * 100)}%";
        }

        public void UpdateHealth()
        {
            if (IsAlive)
            {
                UpdateHealthText();
                UpdatePercentHealthText();
                UpdateHealthBar();
            }

            void UpdateHealthText()
            {
                _healthText.text = $"{ValuesRounding.FormattingValue("", "", _actualHealth)}/{ValuesRounding.FormattingValue("", "", _maxHealth)}";
            }

            void UpdatePercentHealthText()
            {
                _healthPercentText.text = $"({Math.Round((_actualHealth / _maxHealth) * 100, 2)}%):";
            }
            
            void UpdateHealthBar()
            { 
                _healthBar.value = _actualHealth / _maxHealth; 
            }
        }

        private void UpdateHealthRegenerationText()
        {
            _healthRegenerationText.text = $"{Math.Round(_healthRegeneration, 2)}/сек.";
        }

        public IEnumerator AnimatedSlider()
        {
            float part = (_healthBarSecond.value - _healthBar.value) / 20;
            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < 20; i++)
            {
                if (_healthBarSecond.value != 0)
                    _healthBarSecond.value -= part;
                yield return new WaitForSeconds(0.01f);
            }
            _healthBarSecond.value = _healthBar.value;
        }

        public void GetDamageAnimation()
        {
            int animIndex = Random.Range(0, _ReceivedDamageAnimationsCount);

            for (int i = 0; i < _ReceivedDamageAnimationsCount; i++)
            {
                if (Animator != null)
                    Animator.ResetTrigger($"GetDamage_{i}");
            }

            if (Animator != null)
                Animator.SetTrigger($"GetDamage_{animIndex}");
        }

        public void AttackAnimation()
        {
            int animIndex = Random.Range(0, _AttackAnimationsCount);

            for (int i = 0; i < _AttackAnimationsCount; i++)
            {
                if (Animator != null)
                    Animator.ResetTrigger($"Attack_{i}");
            }

            if (Animator != null)
                Animator.SetTrigger($"Attack_{animIndex}");
        }
    }

    private void Awake()
    {
        for (int i = 0; i < Battle.MaxEnemiesInStage; i++)
        {
            _enemyCreaterAnimator[i] = _enemyCreater[i].GetComponent<Animator>();
            _enemyCreater[i].SetActive(false);
        }
        _enemyCreater[0].SetActive(true);
    }

    private void Update()
    {
        if (enemy.IsAlive)
        {
            if (enemy.ActualHealth < enemy.MaxHealth)
            {
                enemy.ActualHealth += enemy.HealthRegeneration / (1 / Time.deltaTime);
            }
            else enemy.ActualHealth = enemy.MaxHealth;
        }
    }

    #region BattleMethods
    public void StartAttackHeroCoroutine()
    {
        StopAllCoroutines();
        StartCoroutine(LoopAttack());

        IEnumerator LoopAttack()
        {
            float timer = 0;

            while (enemy.IsAlive && Heroes.CurrentHero.IsAlive)
            {
                if (enemy.ReceivedStun <= 0)
                {
                    timer += Time.deltaTime;

                    if (timer >= enemy.AttackSpeed)
                    {
                        enemy.AttackAnimation();
                        timer = 0;
                    }
                    yield return null;
                }
                else
                {
                    StartCoroutine(StunControl());
                    yield return new WaitForSeconds(enemy.ReceivedStun);
                }
            }
        }
    }

    public void GetSkillDamage(float quantityDamage)
    {
        float finalDamage = Battle.CalcDealDamage(quantityDamage, EnemiesSystem.enemy.Armor);
        float volatileFactor = Random.Range(0.97f, 1.03f);
        finalDamage *= volatileFactor;

        CheckAndDecreaseEnemyHealth(finalDamage);
    }

    public void GetPureDamage(float finalDamage)
    {
        CheckAndDecreaseEnemyHealth(finalDamage);
    }

    private void CheckAndDecreaseEnemyHealth(float finalDamage)
    {
        if (EnemyObject != null)
        {
            enemy.ActualHealth -= finalDamage;
            StartCoroutine(EnemiesSystem.enemy.AnimatedSlider());
            EnemiesSystem.enemy.GetDamageAnimation();

            int rnd = Random.Range(0, 4);
            if (rnd == 0) AudioEffects.PlayEntitiesAudioEffects(BattleHero.MakeDamageClip, "Play");
            else AudioEffects.PlayEntitiesAudioEffects(EnemiesSystem.GetDamageClip, "Play");

            if (enemy.ActualHealth <= 0 && enemy.IsAlive)
            {
                EnemyIsDead();
            }
            Battle.CreateDamagePrefab(finalDamage, EnemyObject.transform);
        }
    }
    #endregion

    #region UpdateVariables
    private IEnumerator StunControl()
    {
        while (enemy.ReceivedStun > 0)
        {
            enemy.ReceivedStun -= Time.deltaTime;
            if (enemy.ReceivedStun < 0 || enemy.IsAlive == false)
                enemy.ReceivedStun = 0;
            yield return null;
        }
    }
    #endregion

    #region CreateAndKilleEnemy
    public void CreateEnemy()
    {
        InstantiateEnemyObject();
        SetNewImage();
        CalcActualCharacteristics();
        SetNewCharacteristics();
        StartAttackHeroCoroutine();        

        void SetNewCharacteristics()
        {
            enemy.ReceivedStun = 0;
            ListOfEffects.CreatedDebuffCount = 0;
            enemy.IsAlive = true;
            enemy.MaxHealth = RandomizeCurrentStat(_actualMaxHealth);
            enemy.HealthRegeneration = RandomizeCurrentStat(_actualHealthRegeneration);
            enemy.Damage = RandomizeCurrentStat(_actualDamage);
            enemy.Armor = RandomizeCurrentStat(_actualArmor);
            enemy.CritStrength = RandomizeCurrentStat(_actualCritDamage);
            enemy.ExperienceReward = RandomizeCurrentStat(_actualExperienceReward);
            enemy.FaithCurrencyReward = RandomizeCurrentStat(_actualFaithCurrencyReward);
            enemy.AttackSpeed = RandomizeCurrentStat(_actualAttackSpeed);
            enemy.Evasion = _actualEvasion;
            enemy.CritChance = _actualCritChance;

            CheckEvasionToMaxValue();
            CheckCritChanceToMaxValue();
            CheckAttackSpeedToMaxValue();

            ListOfEffects.RandomActivatePassiveEffects();
            enemy.ActualHealth = enemy.MaxHealth;

            float RandomizeCurrentStat(float stat)
            {
                float summ;
                float random_multiplier = Random.Range(1.05f, 1.1f);
                int increase_or_decrease = Random.Range(1, 2);

                if (increase_or_decrease == 1)
                    summ = stat * random_multiplier;
                else summ = stat  / random_multiplier;

                return summ;
            }
        }

        void InstantiateEnemyObject()
        {
            for (int i = 0; i < _enemyCreater.Length; i++)
            {
                if (_enemyCreater[i].transform.childCount == 0)
                {
                    _enemyCreater[i].SetActive(true);
                    EnemyObject = Instantiate(EnemyPrefab, _enemyCreater[i].transform);
                    _enemyCreaterAnimator[i].SetTrigger("Create");
                    break;
                }
            }

            enemy.FindComponentsSets(EnemyObject);
            EnemyObject.name = $"Enemy {Achievements_R_vs_L.AccumulatedKills}";
        }

        void SetNewImage()
        {
            int random_sprite = Random.Range(0, _enemySprites.Length - 1);
            enemy.EnemyImage.sprite = _enemySprites[random_sprite];
        }
    }

    public void EnemyIsDead()
    {
        DestroyEnemy();
        Battle.EnemiesInStageKilled++;
        Battle.CheckKilledEnemiesInStage();

        if (Battle.CurrentStageIsBossStage())
        {
            BossIsAlive = false;

            if (Battle.CurrentStage == Battle.MaxStage && Battle.FinalBossIsDead == false)
            {
                Battle.FinalBossIsDead = true;
                Learning.PlayFinalMessage();
            }
        }
        
        Achievements_R_vs_L.AccumulateKills();
        Heroes.CurrentHero.ActualEXP += enemy.ExperienceReward;
        BattleHero.ComplexUpdateEXP();

        AudioEffects.PlayEntitiesAudioEffects(EnemiesSystem.DeadClip, "PlayOneShot");
        AudioEffects.PlayEntitiesAudioEffects(BattleHero.MockeryClip, "PlayOneShot");
        GetMoneyAnimation.CreateAndAddCoins(EnemiesSystem.enemy.FaithCurrencyReward, "FaithCoin");

        if (Battle.BattleIsActive == true)
        {
            CreateEnemy();
        }
    }

    public void DestroyEnemy()
    {
        enemy.IsAlive = false;

        for (int i = 0; i < _enemyCreater.Length; i++)
        {
            if (_enemyCreater[i].transform.childCount > 0)
                _enemyCreaterAnimator[i].SetTrigger("Destroy");
        }
    }

    private void CalcActualCharacteristics()
    {
        _actualMaxHealth = Expression(_constMaxHealth);
        _actualHealthRegeneration = Expression(_constHealthRegeneration);
        _actualDamage = Expression(_constDamage);
        _actualArmor = Expression(_constArmor);
        _actualCritDamage = Expression(_constCritDamage);
        _actualExperienceReward = Expression(_constExperienceReward);
        _actualFaithCurrencyReward = Expression(_constFaithCurrencyReward * Facilities.FaithMultiplier);

        _actualEvasion = СonstEvasion * (Battle.CurrentStage + 1);
        _actualAttackSpeed = _constAttackSpeed - (DeductibleAttackSpeed * (Battle.CurrentStage + 1));
        _actualCritChance = СonstCritChance * (Battle.CurrentStage + 1);

        float Expression(float value)
        {
            float summ = value * ((_MultiplicateCharacteristics * Battle.CurrentStage) + 1);
            return summ;
        }
    }
    #endregion

    public void CheckEvasionToMaxValue()
    {
        if (enemy.Evasion > _maxEvasion)
            enemy.Evasion = _maxEvasion;
    }

    public void CheckAttackSpeedToMaxValue()
    {
        if (enemy.AttackSpeed < _maxAttackSpeed)
            enemy.AttackSpeed = _maxAttackSpeed;
    }

    public void CheckCritChanceToMaxValue()
    {
        if (enemy.CritChance > _maxCritChance)
            enemy.CritChance = _maxCritChance;
    }
}