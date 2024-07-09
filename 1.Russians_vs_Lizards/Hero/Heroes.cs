using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ListOfEffects;

public class Heroes : DataStructure
{
    #region MainFields

    #region SerializeField
    [Header("Элементы со сцены")]
    [Space(15)]
    [SerializeField] private GameObject[] HeroObject;
    public Color HighlightColor_Select;
    public Color HighlightColor_Deselect;
    public Color[] QualityColor;
    [Header("Кнопки переключения магазина")]
    [Space(10)]
    [SerializeField] private GameObject HeroesWindow;
    [SerializeField] private GameObject WeaponWindow;
    [SerializeField] private GameObject ArmorWindow;
    public Button ChooseHeroButton;
    public Button ChooseArmorButton;
    public Button ChooseWeaponButton;
    public Button ExitButton;

    [Header("Звуки покупки богатырей")]
    [Space(10)]
    public AudioClip AudioMessage_hero_2;
    public AudioClip AudioMessage_hero_3;
    public AudioClip AudioMessage_hero_4;
    public AudioClip AudioMessage_hero_5;
    public AudioClip PrayerAfterPurchase;
    #endregion

    #region NonSerialized
    public const int StatsCount = 12;

    public enum QualityColorEnum
    {
        Average,
        Unusual,
        Rare,
        Epic,
        Legendary
    }

    public enum StatsIndex
    {
        MaxHealth = 0,
        HealthRegeneration = 1,
        MaxStamina = 2,
        StaminaRegeneration = 3,
        MaxWill = 4,
        WillRegeneration = 5,
        Damage = 6,
        Armor = 7,
        Evasion = 8,
        Strength = 9,
        Dexterity = 10,
        Intellect = 11
    }

    [Tooltip("Инициализирует массив класса героя")]
    [NonSerialized] public Hero[] hero;
    [Tooltip("Определяет какой герой сейчас выбран")]
    [NonSerialized] public Hero CurrentHero;
    [Tooltip("Количество героев, нужно для длины массивов")]
    [NonSerialized] public const int HeroCount = 5;
    [NonSerialized] public int CurrentHeroIndex;
    [NonSerialized] public bool ChooseHero_Flag = true;
    [NonSerialized] public bool ChooseArmor_Flag = false;
    [NonSerialized] public bool ChooseWeapon_Flag = false;

    #region NonPecentAdditives

    #region OfLevel
    public const int AdditiveStatFromPumpingPoint = 1;
    private const int AdditivePumpingPoints = 6;

    private const int _additiveStrengthFromLevel = 3;
    private const int _additiveDexterityFromLevel = 3;
    private const int _additiveIntellectFromLevel = 3;
    private const int _additiveMaxHealthFromLevel = 100;
    private const int _additiveDamageFromLevel = 5;
    private const int _additiveMaxStaminaFromLevel = 50;
    private const int _additiveMaxWillFromLevel = 25;
    #endregion

    #region ConstGetOfStats
    [NonSerialized] public static float AdditiveMaxHealthFromStrength = 20f;
    [NonSerialized] public static float AdditiveMaxStaminaFromStrength = 5f;
    [NonSerialized] public static float AdditiveHealthRegenerationFromStrength = 0.1f;
    [NonSerialized] public static float AdditiveStaminaRegenerationFromStrength = 0.025f;
    [NonSerialized] public static float AdditiveDamageFromStrength = 1.25f;

    [NonSerialized] public static float AdditiveArmorFromDexterity = 0.167f;
    [NonSerialized] public static float AdditiveMaxStaminaFromDexterity = 10f;
    [NonSerialized] public static float AdditiveStaminaRegenerationFromDexterity = 0.05f;

    [NonSerialized] public static float AdditiveMaxWillFromIntellect = 10f;
    [NonSerialized] public static float AdditiveWillRegenerationFromIntellect = 0.05f;
    #endregion

    #endregion

    #region PercentAdditives
    [NonSerialized] public static float _getMaxHealthFromPerk = 0.20f;
    [NonSerialized] public static float _getHealthRegenerationFromPerk = 0.20f;

    [NonSerialized] public static float _getMaxStaminaFromPerk = 0.20f;
    [NonSerialized] public static float _getStaminaRegenerationFromPerk = 0.20f;

    [NonSerialized] public static float _getMaxIntellectFromPerk = 0.20f;
    [NonSerialized] public static float _getWillRegenerationFromPerk = 0.20f;

    [NonSerialized] public static float _getEvasionFromDexterity = 0.0005f;
    [NonSerialized] public static float _getAddDamageFromPerk = 0.15f;
    [NonSerialized] public static float _getAddArmorFromPerk = 0.16f;
    [NonSerialized] public static float _getEvasionFromPerk = 0.17f;
    [NonSerialized] public static float _getMaxWillFromPerk = 0.20f;
    [NonSerialized] private const float _multiplierPercentForNextLevel = 0.25f;
    #endregion

    #endregion

    #endregion

    #region TransformConstForHeroesWindows
    [Header("Редактирование карточек героев")]
    [NonSerialized] public Vector2 TransformPozitionHeroHealthBar = new(-180f, 481);
    [NonSerialized] public Vector2 TransformDeltaHeroHealthBar = new(340, 70);
    [NonSerialized] public Quaternion StandardArrowRotation = Quaternion.Euler(0, 0, 270);
    [NonSerialized] public Quaternion DiscloseArrowRotation = Quaternion.Euler(0, 0, 180);
    [NonSerialized] public static Color Red = new(0.5f, 0, 0, 1);
    [NonSerialized] public static Color White = new(1, 1, 1, 1);
    #endregion

    #region MessagesForOutputOnScreen
    private const string _baseName = "база";
    private const string _fromStrengthName = "от силушки";
    private const string _fromDexterityName = "от проворства";
    private const string _fromIntellectName = "от разума";
    private const string _fromLevelName = "от уровня";
    private const string _fromWeaponName = "от оружия";
    private const string _fromArmorSetName = "от комплекта брони";
    private const string _fromPerkName = "от навыков";
    private const string _fromPumpingPointsName = "от очков прокачки";
    private const string _perSecondName = "/сек.";
    #endregion

    public class Hero
    {
        #region HERO_NUMERIC_ELEMENTS

        #region ExperienceControl
        public float ActualEXP
        {
            get { return _actualEXP; }
            set { _actualEXP = value; ComplexUpdateEXP(); CheckHeroLevelUp(); }
        }
        private float _actualEXP;

        public float EXPForNextLevel
        {
            get { return _EXPForNextLevel; }
            set { _EXPForNextLevel = value; ComplexUpdateEXP(); }
        }
        private float _EXPForNextLevel;

        public int Level
        {
            get { return _level; }
            set { _level = value; UpdateLevelText(); }
        }
        private int _level;

        public int PumpingPoints
        {
            get { return _pumpingPoints; }
            set { _pumpingPoints = value; UpdatePumpingPointsText(); }
        }
        private int _pumpingPoints;
        #endregion

        #region PriceControl
        public float PriceHero;
        public int NeedLizards;
        #endregion

        #region Health
        public float ActualHealth
        {
            get { return _actualHealth; }
            set { _actualHealth = value; ComplexUpdateHealth(); }
        }
        private float _actualHealth;

        public float MaxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = value; CalcSet_MaxHealth(); ComplexUpdateHealth(); }
        }
        private float _maxHealth;

        public float MaxHealthBase
        {
            get { return _maxHealthBase; }
            set { _maxHealthBase = value; CalcValue_MaxHealth(); UpdateText_MaxHealthBase(); ComplexUpdateHealth(); }
        }
        private float _maxHealthBase;

        public float MaxHealthFromWeapon
        {
            get { return _maxHealthFromWeapon; }
            set { _maxHealthFromWeapon = value; CalcValue_MaxHealth(); UpdateText_MaxHealthFromWeapon(); }
        }
        private float _maxHealthFromWeapon;

        public float MaxHealthFromLevel
        {
            get { return _maxHealthFromLevel; }
            set { _maxHealthFromLevel = value; CalcValue_MaxHealth(); UpdateText_MaxHealthFromLevel(); }
        }
        private float _maxHealthFromLevel;

        public float MaxHealthFromStrength
        {
            get { return _maxHealthFromStrength; }
            set { _maxHealthFromStrength = value; UpdateText_MaxHealthFromStrength(); }
        }
        private float _maxHealthFromStrength;

        public float MaxHealthFromArmorSet
        {
            get { return _maxHealthFromArmorSet; }
            set { _maxHealthFromArmorSet = value; CalcValue_MaxHealth(); UpdateText_MaxHealthFromArmorSet(); }
        }
        private float _maxHealthFromArmorSet;

        public float MaxHealthFromPerk
        {
            get { return _maxHealthFromPerk; }
            set { _maxHealthFromPerk = value; CalcValue_MaxHealth(); UpdateText_MaxHealthFromPerk(); }
        }
        private float _maxHealthFromPerk;
        #endregion

        #region HealthRegeneration
        public float HealthRegeneration
        {
            get { return _healthRegeneration; }
            set { _healthRegeneration = value; CalcSet_HealthRegeneration(); }
        }
        private float _healthRegeneration;

        public float HealthRegenerationBase
        {
            get { return _healthRegenerationBase; }
            set { _healthRegenerationBase = value; CalcValue_HealthRegeneration(); UpdateText_HealthRegenerationBase(); }
        }
        private float _healthRegenerationBase;

        public float HealthRegenerationFromWeapon
        {
            get { return _healthRegenerationFromWeapon; }
            set { _healthRegenerationFromWeapon = value; CalcValue_HealthRegeneration(); UpdateText_HealthRegenerationFromWeapon(); }
        }
        private float _healthRegenerationFromWeapon;

        public float HealthRegenerationFromStrength
        {
            get { return _healthRegenerationFromStrength; }
            set { _healthRegenerationFromStrength = value; UpdateText_HealthRegenerationFromStrength(); }
        }
        private float _healthRegenerationFromStrength;

        public float HealthRegenerationFromArmorSet
        {
            get { return _healthRegenerationFromArmorSet; }
            set { _healthRegenerationFromArmorSet = value; CalcValue_HealthRegeneration(); UpdateText_HealthRegenerationFromArmorSet(); }
        }
        private float _healthRegenerationFromArmorSet;

        public float HealthRegenerationFromPerk
        {
            get { return _healthRegenerationFromPerk; }
            set { _healthRegenerationFromPerk = value; CalcValue_HealthRegeneration(); UpdateText_HealthRegenerationFromPerk(); }
        }
        private float _healthRegenerationFromPerk;
        #endregion

        #region CostStaminaToAttack
        public float CostStaminaToAttack
        {
            get { return _costStaminaToAttack; }
            set { _costStaminaToAttack = value; CalcValue_CostStaminaToAttack(); }
        }
        private float _costStaminaToAttack;

        public float CostStaminaToAttackBase
        {
            get { return _costStaminaToAttackBase; }
            set { _costStaminaToAttackBase = value; CalcValue_CostStaminaToAttack(); }
        }
        private float _costStaminaToAttackBase;

        public float CostStaminaToAttackFromWeapon
        {
            get { return _costStaminaToAttackFromWeapon; }
            set { _costStaminaToAttackFromWeapon = value; CalcValue_CostStaminaToAttack(); }
        }
        private float _costStaminaToAttackFromWeapon;
        #endregion

        #region Stamina
        public float ActualStamina
        {
            get { return _actualStamina; }
            set { _actualStamina = value; ComplexUpdateStamina(); }
        }
        private float _actualStamina;

        public float MaxStamina
        {
            get { return _maxStamina; }
            set { _maxStamina = value; CalcSet_MaxStamina(); ComplexUpdateStamina(); }
        }
        private float _maxStamina;

        public float MaxStaminaBase
        {
            get { return _maxStaminaBase; }
            set { _maxStaminaBase = value; CalcValue_MaxStamina(); UpdateText_MaxStaminaBase(); ComplexUpdateStamina(); }
        }
        private float _maxStaminaBase;

        public float MaxStaminaFromWeapon
        {
            get { return _maxStaminaFromWeapon; }
            set { _maxStaminaFromWeapon = value; CalcValue_MaxStamina(); UpdateText_MaxStaminaFromWeapon(); }
        }
        private float _maxStaminaFromWeapon;

        public float MaxStaminaFromLevel
        {
            get { return _maxStaminaFromLevel; }
            set { _maxStaminaFromLevel = value; CalcValue_MaxStamina(); UpdateText_MaxStaminaFromLevel(); }
        }
        private float _maxStaminaFromLevel;

        public float MaxStaminaFromStrength
        {
            get { return _maxStaminaFromStrength; }
            set { _maxStaminaFromStrength = value; UpdateText_MaxStaminaFromStrength(); }
        }
        private float _maxStaminaFromStrength;

        public float MaxStaminaFromDexterity
        {
            get { return _maxStaminaFromDexterity; }
            set { _maxStaminaFromDexterity = value; UpdateText_MaxStaminaFromDexterity(); }
        }
        private float _maxStaminaFromDexterity;

        public float MaxStaminaFromArmorSet
        {
            get { return _maxStaminaFromArmorSet; }
            set { _maxStaminaFromArmorSet = value; CalcValue_MaxStamina(); UpdateText_MaxStaminaFromArmorSet(); }
        }
        private float _maxStaminaFromArmorSet;

        public float MaxStaminaFromPerk
        {
            get { return _maxStaminaFromPerk; }
            set { _maxStaminaFromPerk = value; CalcValue_MaxStamina(); UpdateText_MaxStaminaFromPerk(); }
        }
        private float _maxStaminaFromPerk;
        #endregion

        #region StaminaRegeneration
        public float StaminaRegeneration
        {
            get { return _staminaRegeneration; }
            set { _staminaRegeneration = value; CalcSet_StaminaRegeneration(); }
        }
        private float _staminaRegeneration;

        public float StaminaRegenerationBase
        {
            get { return _staminaRegenerationBase; }
            set { _staminaRegenerationBase = value; CalcValue_StaminaRegeneration(); UpdateText_StaminaRegenerationBase(); }
        }
        private float _staminaRegenerationBase;

        public float StaminaRegenerationFromWeapon
        {
            get { return _staminaRegenerationFromWeapon; }
            set { _staminaRegenerationFromWeapon = value; CalcValue_StaminaRegeneration(); UpdateText_StaminaRegenerationFromWeapon(); }
        }
        private float _staminaRegenerationFromWeapon;

        public float StaminaRegenerationFromStrength
        {
            get { return _staminaRegenerationFromStrength; }
            set { _staminaRegenerationFromStrength = value; UpdateText_StaminaRegenerationFromStrength(); }
        }
        private float _staminaRegenerationFromStrength;

        public float StaminaRegenerationFromDexterity
        {
            get { return _staminaRegenerationFromDexterity; }
            set { _staminaRegenerationFromDexterity = value; UpdateText_StaminaRegenerationFromDexterity(); }
        }
        private float _staminaRegenerationFromDexterity;

        public float StaminaRegenerationFromArmorSet
        {
            get { return _staminaRegenerationFromArmorSet; }
            set { _staminaRegenerationFromArmorSet = value; CalcValue_StaminaRegeneration(); UpdateText_StaminaRegenerationFromArmorSet(); }
        }
        private float _staminaRegenerationFromArmorSet;

        public float StaminaRegenerationFromPerk
        {
            get { return _staminaRegenerationFromPerk; }
            set { _staminaRegenerationFromPerk = value; CalcValue_StaminaRegeneration(); UpdateText_StaminaRegenerationFromPerk(); }
        }
        private float _staminaRegenerationFromPerk;
        #endregion

        #region Will
        public float ActualWill
        {
            get { return _actualWill; }
            set { _actualWill = value; ComplexUpdateWill(); }
        }
        private float _actualWill;

        public float MaxWill
        {
            get { return _maxWill; }
            set { _maxWill = value; CalcSet_MaxWill(); ComplexUpdateWill(); }
        }
        private float _maxWill;

        public float MaxWillBase
        {
            get { return _maxWillBase; }
            set { _maxWillBase = value; CalcValue_MaxWill(); UpdateText_MaxWillBase(); ComplexUpdateWill(); }
        }
        private float _maxWillBase;

        public float MaxWillFromLevel
        {
            get { return _maxWillFromLevel; }
            set { _maxWillFromLevel = value; CalcValue_MaxWill(); UpdateText_MaxWillFromLevel(); }
        }
        private float _maxWillFromLevel;

        public float MaxWillFromIntellect
        {
            get { return _maxWillFromIntellect; }
            set { _maxWillFromIntellect = value; UpdateText_MaxWillFromIntellect(); }
        }
        private float _maxWillFromIntellect;

        public float MaxWillFromArmorSet
        {
            get { return _maxWillFromArmorSet; }
            set { _maxWillFromArmorSet = value; CalcValue_MaxWill(); UpdateText_MaxWillFromArmorSet(); }
        }
        private float _maxWillFromArmorSet;

        public float MaxWillFromPerk
        {
            get { return _maxWillFromPerk; }
            set { _maxWillFromPerk = value; CalcValue_MaxWill(); UpdateText_MaxWillFromPerk(); }
        }
        private float _maxWillFromPerk;
        #endregion

        #region WillRegeneration
        public float WillRegeneration
        {
            get { return _willRegeneration; }
            set { _willRegeneration = value; CalcSet_WillRegeneration(); }
        }
        private float _willRegeneration;

        public float WillRegenerationBase
        {
            get { return _willRegenerationBase; }
            set { _willRegenerationBase = value; CalcValue_WillRegeneration(); UpdateText_WillRegenerationBase(); }
        }
        private float _willRegenerationBase;

        public float WillRegenerationFromIntellect
        {
            get { return _willRegenerationFromIntellect; }
            set { _willRegenerationFromIntellect = value; UpdateText_WillRegenerationFromIntellect(); }
        }
        private float _willRegenerationFromIntellect;

        public float WillRegenerationFromArmorSet
        {
            get { return _willRegenerationFromArmorSet; }
            set { _willRegenerationFromArmorSet = value; CalcValue_WillRegeneration(); UpdateText_WillRegenerationFromArmorSet(); }
        }
        private float _willRegenerationFromArmorSet;

        public float WillRegenerationFromPerk
        {
            get { return _willRegenerationFromPerk; }
            set { _willRegenerationFromPerk = value; CalcValue_WillRegeneration(); UpdateText_WillRegenerationFromPerk(); }
        }
        private float _willRegenerationFromPerk;
        #endregion

        #region Damage
        public float Damage
        {
            get { return _damage; }
            set { _damage = value; CalcSet_Damage(); }
        }
        private float _damage;

        public float DamageBase
        {
            get { return _damageBase; }
            set { _damageBase = value; CalcValue_Damage(); UpdateText_DamageBase(); }
        }
        private float _damageBase;

        public float DamageFromWeapon
        {
            get { return _damageFromWeapon; }
            set { _damageFromWeapon = value; CalcValue_Damage(); UpdateText_DamageFromWeapon(); }
        }
        private float _damageFromWeapon;

        public float DamageFromLevel
        {
            get { return _damageFromLevel; }
            set { _damageFromLevel = value; CalcValue_Damage(); UpdateText_DamageFromLevel(); }
        }
        private float _damageFromLevel;

        public float DamageFromStrength
        {
            get { return _damageFromStrength; }
            set { _damageFromStrength = value; UpdateText_DamageFromStrength(); }
        }
        private float _damageFromStrength;

        public float DamageFromArmorSet
        {
            get { return _damageFromArmorSet; }
            set { _damageFromArmorSet = value; CalcValue_Damage(); UpdateText_DamageFromArmorSet(); }
        }
        private float _damageFromArmorSet;

        public float DamageFromPerk
        {
            get { return _damageFromPerk; }
            set { _damageFromPerk = value; CalcValue_Damage(); UpdateText_DamageFromPerk(); }
        }
        private float _damageFromPerk;
        #endregion

        #region Armor
        public float Armor
        {
            get { return _armor; }
            set { _armor = value; CalcSet_Armor(); }
        }
        private float _armor;

        public float ArmorBase
        {
            get { return _armorBase; }
            set { _armorBase = value; CalcValue_Armor(); UpdateText_ArmorBase(); }
        }
        private float _armorBase;

        public float ArmorFromWeapon
        {
            get { return _armorFromWeapon; }
            set { _armorFromWeapon = value; CalcValue_Armor(); UpdateText_ArmorFromWeapon(); }
        }
        private float _armorFromWeapon;

        public float ArmorFromArmorSet
        {
            get { return _armorFromArmorSet; }
            set { _armorFromArmorSet = value; CalcValue_Armor(); UpdateText_ArmorFromArmorSet(); }
        }
        private float _armorFromArmorSet;

        public float ArmorFromDexterity
        {
            get { return _armorFromDexterity; }
            set { _armorFromDexterity = value; UpdateText_ArmorFromDexterity(); }
        }
        private float _armorFromDexterity;

        public float ArmorFromPerk
        {
            get { return _armorFromPerk; }
            set { _armorFromPerk = value; CalcValue_Armor(); UpdateText_ArmorFromPerk(); }
        }
        private float _armorFromPerk;
        #endregion

        #region Evasion
        public float Evasion
        {
            get { return _evasion; }
            set { _evasion = value; CalcSet_Evasion(); }
        }
        private float _evasion;

        public float EvasionBase
        {
            get { return _evasionBase; }
            set { _evasionBase = value; CalcValue_Evasion(); UpdateText_EvasionBase(); }
        }
        private float _evasionBase;

        public float EvasionFromWeapon
        {
            get { return _evasionFromWeapon; }
            set { _evasionFromWeapon = value; CalcValue_Evasion(); UpdateText_EvasionFromWeapon(); }
        }
        private float _evasionFromWeapon;

        public float EvasionFromDexterity
        {
            get { return _evasionFromDexterity; }
            set { _evasionFromDexterity = value; UpdateText_EvasionFromDexterity(); }
        }
        private float _evasionFromDexterity;

        public float EvasionFromArmorSet
        {
            get { return _evasionFromArmorSet; }
            set { _evasionFromArmorSet = value; CalcValue_Evasion(); UpdateText_EvasionFromArmorSet(); }
        }
        private float _evasionFromArmorSet;

        public float EvasionFromPerk
        {
            get { return _evasionFromPerk; }
            set { _evasionFromPerk = value; CalcValue_Evasion(); UpdateText_EvasionFromPerk(); }
        }
        private float _evasionFromPerk;
        #endregion

        #region CritChance
        public float CritChance
        {
            get { return _critChance; }
            set { _critChance = value; CalcValue_CritChance(); }
        }
        private float _critChance;

        public float CritChanceBase
        {
            get { return _critChanceBase; }
            set { _critChanceBase = value; CalcValue_CritChance(); }
        }
        private float _critChanceBase;

        public float CritChanceFromWeapon
        {
            get { return _critChanceFromWeapon; }
            set { _critChanceFromWeapon = value; CalcValue_CritChance(); }
        }
        private float _critChanceFromWeapon;
        #endregion

        #region CritDamage
        public float CritDamage
        {
            get { return _critDamage; }
            set { _critDamage = value; CalcValue_CritDamage(); }
        }
        private float _critDamage;

        public float CritDamageBase
        {
            get { return _critDamageBase; }
            set { _critDamageBase = value; CalcValue_CritDamage(); }
        }
        private float _critDamageBase;

        public float CritDamageFromWeapon
        {
            get { return _critDamageFromWeapon; }
            set { _critDamageFromWeapon = value; CalcValue_CritDamage(); }
        }
        private float _critDamageFromWeapon;
        #endregion

        #region Strength
        public float Strength
        {
            get { return _strength; }
            set { _strength = value; CalcSet_Strength(); }
        }
        private float _strength;

        public float StrengthBase
        {
            get { return _strengthBase; }
            set { _strengthBase = value; CalcSet_Strength(); UpdateText_StrengthBase(); }
        }
        private float _strengthBase;

        public float StrengthFromWeapon
        {
            get { return _strengthFromWeapon; }
            set { _strengthFromWeapon = value; CalcSet_Strength(); UpdateText_StrengthFromWeapon(); }
        }
        private float _strengthFromWeapon;

        public float StrengthFromPerk
        {
            get { return _strengthFromPerk; }
            set { _strengthFromPerk = value; CalcSet_Strength(); UpdateText_StrengthFromPerk(); }
        }
        private float _strengthFromPerk;

        public float StrengthFromLevel
        {
            get { return _strengthFromLevel; }
            set { _strengthFromLevel = value; CalcSet_Strength(); UpdateText_StrengthFromLevel(); }
        }
        private float _strengthFromLevel;

        public float StrengthFromPumpingPoints
        {
            get { return _strengthFromPumpingPoints; }
            set { _strengthFromPumpingPoints = value; CalcSet_Strength(); UpdateText_StrengthFromPumpingPoints(); }
        }
        private float _strengthFromPumpingPoints;

        public float StrengthFromArmorSet
        {
            get { return _strengthFromArmorSet; }
            set { _strengthFromArmorSet = value; CalcSet_Strength(); UpdateText_StrengthFromArmorSet(); }
        }
        private float _strengthFromArmorSet;
        #endregion

        #region Dexterity
        public float Dexterity
        {
            get { return _dexterity; }
            set { _dexterity = value; CalcSet_Dexterity(); }
        }
        private float _dexterity;

        public float DexterityBase
        {
            get { return _dexterityBase; }
            set { _dexterityBase = value; CalcSet_Dexterity(); UpdateText_DexterityBase(); }
        }
        private float _dexterityBase;

        public float DexterityFromWeapon
        {
            get { return _dexterityFromWeapon; }
            set { _dexterityFromWeapon = value; CalcSet_Dexterity(); UpdateText_DexterityFromWeapon(); }
        }
        private float _dexterityFromWeapon;
        public float DexterityFromPerk
        {
            get { return _dexterityFromPerk; }
            set { _dexterityFromPerk = value; CalcSet_Dexterity(); UpdateText_DexterityFromPerk(); }
        }
        private float _dexterityFromPerk;

        public float DexterityFromLevel
        {
            get { return _dexterityFromLevel; }
            set { _dexterityFromLevel = value; CalcSet_Dexterity(); UpdateText_DexterityFromLevel(); }
        }
        private float _dexterityFromLevel;
        public float DexterityFromPumpingPoints
        {
            get { return _dexterityFromPumpingPoints; }
            set { _dexterityFromPumpingPoints = value; CalcSet_Dexterity(); UpdateText_DexterityFromPumpingPoints(); }
        }
        private float _dexterityFromPumpingPoints;

        public float DexterityFromArmorSet
        {
            get { return _dexterityFromArmorSet; }
            set { _dexterityFromArmorSet = value; CalcSet_Dexterity(); UpdateText_DexterityFromArmorSet(); }
        }
        private float _dexterityFromArmorSet;

        #endregion

        #region Intellect
        public float Intellect
        {
            get { return _intellect; }
            set { _intellect = value; CalcSet_Intellect(); }
        }
        private float _intellect;

        public float IntellectBase
        {
            get { return _intellectBase; }
            set { _intellectBase = value; CalcSet_Intellect(); UpdateText_IntellectBase(); }
        }
        private float _intellectBase;

        public float IntellectFromLevel
        {
            get { return _intellectFromLevel; }
            set { _intellectFromLevel = value; CalcSet_Intellect(); UpdateText_IntellectFromLevel(); }
        }
        private float _intellectFromLevel;

        public float IntellectFromPumpingPoints
        {
            get { return _intellectFromPumpingPoints; }
            set { _intellectFromPumpingPoints = value; CalcSet_Intellect(); UpdateText_IntellectFromPumpingPoints(); }
        }
        private float _intellectFromPumpingPoints;

        public float IntellectFromArmorSet
        {
            get { return _intellectFromArmorSet; }
            set { _intellectFromArmorSet = value; CalcSet_Intellect(); UpdateText_IntellectFromArmorSet(); }
        }
        private float _intellectFromArmorSet;

        public float IntellectFromPerk
        {
            get { return _intellectFromPerk; }
            set { _intellectFromPerk = value; CalcSet_Intellect(); UpdateText_IntellectFromPerk(); }
        }
        private float _intellectFromPerk;
        #endregion

        #region Functional
        public bool Bought;
        public int Index;
        public float TimeToResurration;
        public float RemainingTimeToResurrationHero;
        public bool IsAlive;
        #endregion

        #endregion

        #region HERO_UI_ELEMENTS

        #region GameObjectComponent
        public GameObject[] ExtendedStats_MaxHealth = new GameObject[6];
        public GameObject[] ExtendedStats_HealthRegeneration = new GameObject[5];
        public GameObject[] ExtendedStats_MaxStamina = new GameObject[7];
        public GameObject[] ExtendedStats_StaminaRegeneration = new GameObject[6];
        public GameObject[] ExtendedStats_MaxWill = new GameObject[5];
        public GameObject[] ExtendedStats_WillRegeneration = new GameObject[4];
        public GameObject[] ExtendedStats_Damage = new GameObject[6];
        public GameObject[] ExtendedStats_Armor = new GameObject[5];
        public GameObject[] ExtendedStats_Evasion = new GameObject[5];
        public GameObject[] ExtendedStats_Strength = new GameObject[6];
        public GameObject[] ExtendedStats_Dexterity = new GameObject[6];
        public GameObject[] ExtendedStats_Intellect = new GameObject[5];
        //public GameObject[,] ExtendedStatsBuffer = new GameObject[StatsCount, 7];
        #endregion

        #region SliderComponent
        public Slider ExperienceBar;
        public Slider HealthBar;
        public Slider StaminaBar;
        public Slider WillBar;
        #endregion

        #region ButtonComponent
        public Button BuyHeroButton;
        public Button SelectHeroButton;
        public Button WindowSwipeButton;
        public Button[] StatButtons = new Button[StatsCount];
        public Button StrengthPumpingPointButton;
        public Button DexterityPumpingPointButton;
        public Button IntellectPumpingPointButton;
        #endregion

        #region ImageComponent
        public Image LockImage;
        public Image HeroBackgroundHighLight;
        public Image СharacteristicsWindow;
        public Image DeadImage;

        #region StatsArrows
        public Image WindowArrow;
        ///////////////////////////////////////
        public Image MaxHealth_ButtonImage;
        public Image MaxStamina_ButtonImage;
        public Image MaxWill_ButtonImage;
        public Image HealthRegeneration_ButtonImage;
        public Image StaminaRegeneration_ButtonImage;
        public Image WillRegeneration_ButtonImage;
        public Image Damage_ButtonImage;
        public Image Armor_ButtonImage;
        public Image Evasion_ButtonImage;
        public Image Strength_ButtonImage;
        public Image Dexterity_ButtonImage;
        public Image Intellect_ButtonImage;
        public Image[] StatButtonsImages = new Image[StatsCount];
        ///////////////////////////////////////
        public Image ButtonBackground_MaxHealth;
        public Image ButtonBackground_MaxStamina;
        public Image ButtonBackground_MaxWill;
        public Image ButtonBackground_HealthRegeneration;
        public Image ButtonBackground_StaminaRegeneration;
        public Image ButtonBackground_WillRegeneration;
        public Image ButtonBackground_Damage;
        public Image ButtonBackground_Armor;
        public Image ButtonBackground_Evasion;
        public Image ButtonBackground_Strength;
        public Image ButtonBackground_Dexterity;
        public Image ButtonBackground_Intellect;
        public Image[] StatButtonsBackgrounds = new Image[StatsCount];
        #endregion
        #endregion

        #region TextComponent
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI HealthText;
        public TextMeshProUGUI HealthPercentText;
        public TextMeshProUGUI HealthRegenerationTextForSlider;
        public TextMeshProUGUI StaminaText;
        public TextMeshProUGUI StaminaPercentText;
        public TextMeshProUGUI StaminaRegenerationTextForSlider;
        public TextMeshProUGUI WillText;
        public TextMeshProUGUI WillPercentText;
        public TextMeshProUGUI WillRegenerationTextForSlider;
        public TextMeshProUGUI ExperienceText;
        public TextMeshProUGUI ExperiencePercentText;
        public TextMeshProUGUI LevelText;
        public TextMeshProUGUI PumpingPointsText;
        public TextMeshProUGUI PriceNarrationText;
        public TextMeshProUGUI TimeToResurractionText;

        #region MoreStatsText
        public TextMeshProUGUI[] ArrStatsName = new TextMeshProUGUI[Heroes.StatsCount];

        public TextMeshProUGUI MaxHealthText;
        public TextMeshProUGUI MaxHealthBaseText;
        public TextMeshProUGUI MaxHealthFromLevelText;
        public TextMeshProUGUI MaxHealthFromStrengthText;
        public TextMeshProUGUI MaxHealthFromArmorSetText;
        public TextMeshProUGUI MaxHealthFromWeaponText;
        public TextMeshProUGUI MaxHealthFromPerkText;

        public TextMeshProUGUI HealthRegenerationText;
        public TextMeshProUGUI HealthRegenerationBaseText;
        public TextMeshProUGUI HealthRegenerationFromStrengthText;
        public TextMeshProUGUI HealthRegenerationFromArmorSetText;
        public TextMeshProUGUI HealthRegenerationFromWeaponText;
        public TextMeshProUGUI HealthRegenerationFromPerkText;

        public TextMeshProUGUI MaxStaminaText;
        public TextMeshProUGUI MaxStaminaBaseText;
        public TextMeshProUGUI MaxStaminaFromLevelText;
        public TextMeshProUGUI MaxStaminaFromStrengthText;
        public TextMeshProUGUI MaxStaminaFromDexterityText;
        public TextMeshProUGUI MaxStaminaFromArmorSetText;
        public TextMeshProUGUI MaxStaminaFromWeaponText;
        public TextMeshProUGUI MaxStaminaFromPerkText;

        public TextMeshProUGUI StaminaRegenerationText;
        public TextMeshProUGUI StaminaRegenerationBaseText;
        public TextMeshProUGUI StaminaRegenerationFromStrengthText;
        public TextMeshProUGUI StaminaRegenerationFromDexterityText;
        public TextMeshProUGUI StaminaRegenerationFromArmorSetText;
        public TextMeshProUGUI StaminaRegenerationFromWeaponText;
        public TextMeshProUGUI StaminaRegenerationFromPerkText;

        public TextMeshProUGUI MaxWillText;
        public TextMeshProUGUI MaxWillBaseText;
        public TextMeshProUGUI MaxWillFromLevelText;
        public TextMeshProUGUI MaxWillFromIntellectText;
        public TextMeshProUGUI MaxWillFromArmorSetText;
        public TextMeshProUGUI MaxWillFromWeaponText;
        public TextMeshProUGUI MaxWillFromPerkText;

        public TextMeshProUGUI WillRegenerationText;
        public TextMeshProUGUI WillRegenerationBaseText;
        public TextMeshProUGUI WillRegenerationFromIntellectText;
        public TextMeshProUGUI WillRegenerationFromArmorSetText;
        public TextMeshProUGUI WillRegenerationFromPerkText;

        public TextMeshProUGUI DamageText;
        public TextMeshProUGUI DamageBaseText;
        public TextMeshProUGUI DamageFromLevelText;
        public TextMeshProUGUI DamageFromWeaponText;
        public TextMeshProUGUI DamageFromStrengthText;
        public TextMeshProUGUI DamageFromArmorSetText;
        public TextMeshProUGUI DamageFromPerkText;

        public TextMeshProUGUI ArmorText;
        public TextMeshProUGUI ArmorBaseText;
        public TextMeshProUGUI ArmorFromArmorSetText;
        public TextMeshProUGUI ArmorFromDexterityText;
        public TextMeshProUGUI ArmorFromWeaponText;
        public TextMeshProUGUI ArmorFromPerkText;

        public TextMeshProUGUI EvasionText;
        public TextMeshProUGUI EvasionBaseText;
        public TextMeshProUGUI EvasionFromDexterityText;
        public TextMeshProUGUI EvasionFromArmorSetText;
        public TextMeshProUGUI EvasionFromWeaponText;
        public TextMeshProUGUI EvasionFromPerkText;

        public TextMeshProUGUI StrengthText;
        public TextMeshProUGUI StrengthBaseText;
        public TextMeshProUGUI StrengthFromLevelText;
        public TextMeshProUGUI StrengthFromPumpingPointsText;
        public TextMeshProUGUI StrengthFromArmorSetText;
        public TextMeshProUGUI StrengthFromWeaponText;
        public TextMeshProUGUI StrengthFromPerkText;

        public TextMeshProUGUI DexterityText;
        public TextMeshProUGUI DexterityBaseText;
        public TextMeshProUGUI DexterityFromLevelText;
        public TextMeshProUGUI DexterityFromPumpingPointsText;
        public TextMeshProUGUI DexterityFromArmorSetText;
        public TextMeshProUGUI DexterityFromWeaponText;
        public TextMeshProUGUI DexterityFromPerkText;

        public TextMeshProUGUI IntellectText;
        public TextMeshProUGUI IntellectBaseText;
        public TextMeshProUGUI IntellectFromLevelText;
        public TextMeshProUGUI IntellectFromPumpingPointsText;
        public TextMeshProUGUI IntellectFromArmorSetText;
        public TextMeshProUGUI IntellectFromPerkText;

        #endregion

        #endregion

        #region RectTransformComponent
        public RectTransform[] StatSetsBuffer = new RectTransform[StatsCount];
        public RectTransform CharacteristicsBody;
        public RectTransform MaxHealthSet;
        public RectTransform HealthRegenerationSet;
        public RectTransform MaxStaminaSet;
        public RectTransform StaminaRegenerationSet;
        public RectTransform MaxWillSet;
        public RectTransform WillRegenerationSet;
        public RectTransform DamageSet;
        public RectTransform ArmorSet;
        public RectTransform EvasionSet;
        public RectTransform StrengthSet;
        public RectTransform DexteritySet;
        public RectTransform IntellectSet;
        #endregion

        #endregion

        #region Calculating

        #region MaxHealth
        private void CalcSet_MaxHealth()
        {
            CalcValue_MaxHealthFromStrength();
            CalcValue_MaxHealth();
        }

        private void CalcValue_MaxHealth()
        {
            _maxHealth =
            (_maxHealthBase +
            MaxHealthFromLevel +
            MaxHealthFromStrength +
            MaxHealthFromArmorSet +
            MaxHealthFromWeapon) *
            (MaxHealthFromPerk + 1);
            ComplexUpdateHealth();
            UpdateText_MaxHealth();

            if (_actualHealth >= _maxHealth)
            {
                _actualHealth = _maxHealth;
            }
        }

        private void CalcValue_MaxHealthFromStrength()
        {
            MaxHealthFromStrength = Strength * AdditiveMaxHealthFromStrength;
        }
        #endregion

        #region HealthRegeneration
        private void CalcSet_HealthRegeneration()
        {
            CalcValue_HealthRegenerationFromStrength();
            CalcValue_HealthRegeneration();
        }

        private void CalcValue_HealthRegeneration()
        {
            _healthRegeneration =
            (_healthRegenerationBase +
            HealthRegenerationFromStrength +
            HealthRegenerationFromArmorSet +
            HealthRegenerationFromWeapon) *
            (HealthRegenerationFromPerk + 1);
            UpdateText_HealthRegeneration();
            UpdateText_HealthRegenerationForSlider();
        }

        private void CalcValue_HealthRegenerationFromStrength()
        {
            HealthRegenerationFromStrength = Strength * AdditiveHealthRegenerationFromStrength;
        }
        #endregion

        #region AttackCost
        private void CalcValue_CostStaminaToAttack()
        {
            _costStaminaToAttack =
            (_costStaminaToAttackBase +
            _costStaminaToAttackFromWeapon);
        }
        #endregion

        #region MaxStamina
        private void CalcSet_MaxStamina()
        {
            CalcValue_MaxStaminaFromDexterity();
            CalcValue_MaxStaminaFromStrength();
            CalcValue_MaxStamina();
        }
        private void CalcValue_MaxStamina()
        {
            _maxStamina =
            (_maxStaminaBase +
            MaxStaminaFromLevel +
            MaxStaminaFromStrength +
            MaxStaminaFromDexterity +
            MaxStaminaFromArmorSet +
            MaxStaminaFromWeapon) *
            (MaxStaminaFromPerk + 1);
            UpdateText_MaxStamina();
            ComplexUpdateStamina();

            if (_actualStamina >= _maxStamina)
            {
                _actualStamina = _maxStamina;
            }
        }

        private void CalcValue_MaxStaminaFromDexterity()
        {
            MaxStaminaFromDexterity = Dexterity * AdditiveMaxStaminaFromDexterity;
        }

        private void CalcValue_MaxStaminaFromStrength()
        {
            MaxStaminaFromStrength = Strength * AdditiveMaxStaminaFromStrength;
        }
        #endregion

        #region StaminaRegeneration
        private void CalcSet_StaminaRegeneration()
        {
            CalcValue_StaminaRegenerationFromDexterity();
            CalcValue_StaminaRegenerationFromStrength();
            CalcValue_StaminaRegeneration();
        }

        private void CalcValue_StaminaRegeneration()
        {
            _staminaRegeneration =
            (_staminaRegenerationBase +
            StaminaRegenerationFromDexterity +
            StaminaRegenerationFromStrength +
            StaminaRegenerationFromArmorSet +
            StaminaRegenerationFromWeapon) *
            (StaminaRegenerationFromPerk + 1);
            UpdateText_StaminaRegeneration();
            UpdateText_StaminaRegenerationForSlider();
        }

        private void CalcValue_StaminaRegenerationFromDexterity()
        {
            StaminaRegenerationFromDexterity =
                Dexterity * AdditiveStaminaRegenerationFromDexterity;
        }

        private void CalcValue_StaminaRegenerationFromStrength()
        {
            StaminaRegenerationFromStrength =
                Strength * AdditiveStaminaRegenerationFromStrength;
        }
        #endregion

        #region MaxWill
        private void CalcSet_MaxWill()
        {
            CalcValue_MaxWillFromIntellect();
            CalcValue_MaxWill();
        }

        private void CalcValue_MaxWill()
        {
            _maxWill =
            (_maxWillBase +
            MaxWillFromLevel +
            MaxWillFromIntellect +
            MaxWillFromArmorSet) *
            (MaxWillFromPerk + 1);
            UpdateText_MaxWill();
            ComplexUpdateWill();

            if (_actualWill >= _maxWill)
            {
                _actualWill = _maxWill;
            }
        }

        private void CalcValue_MaxWillFromIntellect()
        {
            MaxWillFromIntellect = Intellect * AdditiveMaxWillFromIntellect;
        }
        #endregion

        #region WillRegeneration
        private void CalcSet_WillRegeneration()
        {
            CalcValue_WillRegenerationFromIntellect();
            CalcValue_WillRegeneration();
        }

        private void CalcValue_WillRegeneration()
        {
            _willRegeneration =
            (_willRegenerationBase +
            WillRegenerationFromArmorSet +
            WillRegenerationFromIntellect) *
            (WillRegenerationFromPerk + 1);
            UpdateText_WillRegeneration();
            UpdateText_WillRegenerationForSlider();
        }

        private void CalcValue_WillRegenerationFromIntellect()
        {
            WillRegenerationFromIntellect = Intellect * AdditiveWillRegenerationFromIntellect;
        }
        #endregion

        #region Damage
        private void CalcSet_Damage()
        {
            CalcValue_DamageFromStrength();
            CalcValue_Damage();
        }

        private void CalcValue_Damage()
        {
            _damage =
            (_damageBase +
            DamageFromLevel +
            DamageFromWeapon +
            DamageFromStrength +
            DamageFromArmorSet) *
            (DamageFromPerk + 1);
            UpdateText_Damage();
        }

        private void CalcValue_DamageFromStrength()
        {
            DamageFromStrength = Strength * AdditiveDamageFromStrength;
        }
        #endregion

        #region Armor
        private void CalcSet_Armor()
        {
            CalcValue_ArmorFromDexterity();
            CalcValue_Armor();
        }

        private void CalcValue_Armor()
        {
            _armor =
            (_armorBase +
            ArmorFromArmorSet +
            ArmorFromWeapon +
            ArmorFromDexterity) *
            (ArmorFromPerk + 1);
            UpdateText_Armor();
        }

        private void CalcValue_ArmorFromDexterity()
        {
            ArmorFromDexterity = Dexterity * AdditiveArmorFromDexterity;
        }
        #endregion

        #region Evasion
        private void CalcSet_Evasion()
        {
            CalcValue_EvasionFromDexterity();
            CalcValue_Evasion();
        }

        private void CalcValue_Evasion()
        {
            _evasion =
            (_evasionBase +
            EvasionFromDexterity +
            EvasionFromArmorSet +
            EvasionFromWeapon) *
            (EvasionFromPerk + 1);
            UpdateText_Evasion();
        }

        private void CalcValue_EvasionFromDexterity()
        {
            EvasionFromDexterity = Dexterity * _getEvasionFromDexterity;
        }
        #endregion

        #region CritChance
        private void CalcValue_CritChance()
        {
            _critChance =
            (_critChanceBase +
            _critChanceFromWeapon);
        }
        #endregion

        #region CritDamage
        private void CalcValue_CritDamage()
        {
            _critDamage =
            (_critDamageBase +
            CritDamageFromWeapon);
        }
        #endregion

        #region Strength
        public void CalcSet_Strength()
        {
            CalcValue_Strength();
            CalcValue_MaxHealthFromStrength();
            CalcValue_MaxHealth();
            CalcValue_HealthRegenerationFromStrength();
            CalcValue_HealthRegeneration();
            CalcValue_MaxStaminaFromStrength();
            CalcValue_MaxStamina();
            CalcValue_StaminaRegenerationFromStrength();
            CalcValue_StaminaRegeneration();
            CalcValue_DamageFromStrength();
            CalcValue_Damage();
        }

        private void CalcValue_Strength()
        {
            _strength =
            (_strengthBase +
            StrengthFromLevel +
            StrengthFromPumpingPoints +
            StrengthFromArmorSet +
            StrengthFromWeapon) *
            (StrengthFromPerk + 1);
            UpdateText_Strength();
            UpdateText_StrengthFromPerk();
        }
        #endregion

        #region Dexterity
        public void CalcSet_Dexterity()
        {
            CalcValue_Dexterity();
            CalcValue_MaxStaminaFromDexterity();
            CalcValue_MaxStamina();
            CalcValue_StaminaRegenerationFromDexterity();
            CalcValue_StaminaRegeneration();
            CalcValue_ArmorFromDexterity();
            CalcValue_Armor();
            CalcValue_EvasionFromDexterity();
            CalcValue_Evasion();
        }

        private void CalcValue_Dexterity()
        {
            _dexterity =
            (_dexterityBase +
            DexterityFromArmorSet +
            DexterityFromWeapon +
            DexterityFromLevel + 
            DexterityFromPumpingPoints) *
            (DexterityFromPerk + 1);
            UpdateText_Dexterity();
            UpdateText_DexterityFromPerk();
        }
        #endregion

        #region Intellect
        public void CalcSet_Intellect()
        {
            CalcValue_Intellect();
            CalcValue_MaxWillFromIntellect();
            CalcValue_MaxWill();
            CalcValue_WillRegenerationFromIntellect();
            CalcValue_WillRegeneration();
        }

        private void CalcValue_Intellect()
        {
            _intellect =
            (_intellectBase +
            IntellectFromLevel +
            IntellectFromArmorSet +
            IntellectFromPumpingPoints) *
            (IntellectFromPerk + 1);
            UpdateText_Intellect();
            UpdateText_IntellectFromPerk();
        }
        #endregion
        #endregion

        #region UpdatingText

        public void UpdateAllText()
        {
            ComplexUpdateEXP();
            ComplexUpdateHealth();
            ComplexUpdateStamina();
            ComplexUpdateWill();

            UpdateText_MaxHealth();
            UpdateText_MaxHealthBase();
            UpdateText_MaxHealthFromLevel();
            UpdateText_MaxHealthFromStrength();
            UpdateText_MaxHealthFromArmorSet();
            UpdateText_MaxHealthFromWeapon();
            UpdateText_MaxHealthFromPerk();

            UpdateText_HealthRegeneration();
            UpdateText_HealthRegenerationBase();
            UpdateText_HealthRegenerationFromStrength();
            UpdateText_HealthRegenerationFromArmorSet();
            UpdateText_HealthRegenerationFromWeapon();
            UpdateText_HealthRegenerationFromPerk();

            UpdateText_MaxStamina();
            UpdateText_MaxStaminaBase();
            UpdateText_MaxStaminaFromLevel();
            UpdateText_MaxStaminaFromStrength();
            UpdateText_MaxStaminaFromDexterity();
            UpdateText_MaxStaminaFromArmorSet();
            UpdateText_MaxStaminaFromWeapon();
            UpdateText_MaxStaminaFromPerk();

            UpdateText_StaminaRegeneration();
            UpdateText_StaminaRegenerationBase();
            UpdateText_StaminaRegenerationFromStrength();
            UpdateText_StaminaRegenerationFromDexterity();
            UpdateText_StaminaRegenerationFromArmorSet();
            UpdateText_StaminaRegenerationFromWeapon();
            UpdateText_StaminaRegenerationFromPerk();

            UpdateText_MaxWill();
            UpdateText_MaxWillBase();
            UpdateText_MaxWillFromLevel();
            UpdateText_MaxWillFromIntellect();
            UpdateText_MaxWillFromArmorSet();
            UpdateText_MaxWillFromPerk();

            UpdateText_WillRegeneration();
            UpdateText_WillRegenerationBase();
            UpdateText_WillRegenerationFromIntellect();
            UpdateText_WillRegenerationFromArmorSet();
            UpdateText_WillRegenerationFromPerk();

            UpdateText_Damage();
            UpdateText_DamageBase();
            UpdateText_DamageFromLevel();
            UpdateText_DamageFromWeapon();
            UpdateText_DamageFromStrength();
            UpdateText_DamageFromArmorSet();
            UpdateText_DamageFromPerk();

            UpdateText_Armor();
            UpdateText_ArmorBase();
            UpdateText_ArmorFromArmorSet();
            UpdateText_ArmorFromWeapon();
            UpdateText_ArmorFromDexterity();
            UpdateText_ArmorFromPerk();

            UpdateText_Evasion();
            UpdateText_EvasionBase();
            UpdateText_EvasionFromDexterity();
            UpdateText_EvasionFromArmorSet();
            UpdateText_EvasionFromWeapon();
            UpdateText_EvasionFromPerk();

            UpdateText_Strength();
            UpdateText_StrengthBase();
            UpdateText_StrengthFromLevel();
            UpdateText_StrengthFromPumpingPoints();
            UpdateText_StrengthFromArmorSet();
            UpdateText_StrengthFromWeapon();
            UpdateText_StrengthFromPerk();

            UpdateText_Dexterity();
            UpdateText_DexterityBase();
            UpdateText_DexterityFromLevel();
            UpdateText_DexterityFromPumpingPoints();
            UpdateText_DexterityFromArmorSet();
            UpdateText_DexterityFromWeapon();
            UpdateText_DexterityFromPerk();

            UpdateText_Intellect();
            UpdateText_IntellectBase();
            UpdateText_IntellectFromLevel();
            UpdateText_IntellectFromArmorSet();
            UpdateText_IntellectFromPumpingPoints();
            UpdateText_IntellectFromPerk();
        }

        #region Experience
        public void ComplexUpdateEXP()
        {
            UpdateText_EXP();
            UpdateSliderValue_EXP();
            UpdateText_PercentEXP();

            void UpdateText_EXP()
            {
                ExperienceText.text = $"{ValuesRounding.FormattingValue("", "", _actualEXP)}/" +
                    $"{ValuesRounding.FormattingValue("", "", _EXPForNextLevel)}";
            }

            void UpdateSliderValue_EXP()
            {
                ExperienceBar.value = (_actualEXP / _EXPForNextLevel);
            }

            void UpdateText_PercentEXP()
            {
                double expression = Math.Round(((_actualEXP / _EXPForNextLevel) * 100), 1);
                ExperiencePercentText.text = $"({expression}%):";
            }
        }

        private void UpdateLevelText()
        {
            LevelText.text = $"Уровень: {Level}";
        }

        private void UpdatePumpingPointsText()
        {
            PumpingPointsText.text = $"Очки прокачки: {PumpingPoints}";
        }
        #endregion

        #region CurrentHealth
        public void ComplexUpdateHealth()
        {
            UpdateSliderValue_Health();
            UpdateText_Health();
            UpdateText_PercentHealth();
            UpdateText_HealthRegenerationForSlider();

            void UpdateSliderValue_Health()
            {
                HealthBar.value = _actualHealth / _maxHealth;
            }

            void UpdateText_Health()
            {
                HealthText.text = $"{ValuesRounding.FormattingValue("", "", _actualHealth)}/" +
                    $"{ValuesRounding.FormattingValue("", "", _maxHealth)}";
            }

            void UpdateText_PercentHealth()
            {
                double expression = Math.Round(((_actualHealth / _maxHealth) * 100), 1);
                HealthPercentText.text = $"({expression}%):";
            }
        }

        private void UpdateText_HealthRegenerationForSlider()
        {
            HealthRegenerationTextForSlider.text = $"{ValuesRounding.UltraAccuracyFormattingValue("", "", _healthRegeneration)}{_perSecondName}";
        }
        #endregion

        #region CurrentStamina
        public void ComplexUpdateStamina()
        {
            UpdateText_Stamina();
            UpdateSliderValue_Stamina();
            UpdateText_PercentStamina();

            void UpdateText_Stamina()
            {
                StaminaText.text = $"{ValuesRounding.FormattingValue("", "", _actualStamina)}/" +
                    $"{ValuesRounding.FormattingValue("", "", _maxStamina)} ";
            }

            void UpdateSliderValue_Stamina()
            {
                StaminaBar.value = (_actualStamina / _maxStamina);
            }

            void UpdateText_PercentStamina()
            {
                double expression = Math.Round(((_actualStamina / _maxStamina) * 100), 1);
                StaminaPercentText.text = $"({expression}%):";
            }
        }

        private void UpdateText_StaminaRegenerationForSlider()
        {
            StaminaRegenerationTextForSlider.text = $"{ValuesRounding.UltraAccuracyFormattingValue("", "", _staminaRegeneration)}{_perSecondName}";
        }
        #endregion

        #region CurrentWill
        public void ComplexUpdateWill()
        {
            UpdateText_Will();
            UpdateSliderValue_Will();
            UpdateText_PercentWill();

            void UpdateText_Will()
            {
                WillText.text = $"{ValuesRounding.FormattingValue("", "", _actualWill)}/" +
                    $"{ValuesRounding.FormattingValue("", "", _maxWill)} ";
            }

            void UpdateSliderValue_Will()
            {
                WillBar.value = (_actualWill / _maxWill);
            }

            void UpdateText_PercentWill()
            {
                double expression = Math.Round(((_actualWill / _maxWill) * 100), 1);
                WillPercentText.text = $"({expression}%):";
            }
        }

        private void UpdateText_WillRegenerationForSlider()
        {
            WillRegenerationTextForSlider.text = $"{ValuesRounding.UltraAccuracyFormattingValue("", "", _willRegeneration)}{_perSecondName}";
        }
        #endregion

        #region MaxHealth

        private void UpdateText_MaxHealth()
        {
            MaxHealthText.text = $"Макс.здоровье: {RoundValue(_maxHealth)}";
        }
        private void UpdateText_MaxHealthBase()
        {
            MaxHealthBaseText.text =
                $"{RoundValue(MaxHealthBase)} {_baseName}";
        }
        private void UpdateText_MaxHealthFromLevel()
        {
            MaxHealthFromLevelText.text =
                $"+{RoundValue(MaxHealthFromLevel)} {_fromLevelName}";
        }
        private void UpdateText_MaxHealthFromStrength()
        {
            MaxHealthFromStrengthText.text =
                $"+{RoundValue(MaxHealthFromStrength)} {_fromStrengthName}";
        }
        private void UpdateText_MaxHealthFromArmorSet()
        {
            MaxHealthFromArmorSetText.text =
                $"+{RoundValue(MaxHealthFromArmorSet)} {_fromArmorSetName}";
        }
        private void UpdateText_MaxHealthFromWeapon()
        {
            MaxHealthFromWeaponText.text =
                $"+{RoundValue(MaxHealthFromWeapon)} {_fromWeaponName}";
        }
        private void UpdateText_MaxHealthFromPerk()
        {
            MaxHealthFromPerkText.text = $"+{PercentValueFormatting(MaxHealthFromPerk)}% {_fromPerkName} (+{AdditiveSumm()})";

            float AdditiveSumm()
            {
                float MaxHealth =
                    (MaxHealthBase +
                    MaxHealthFromLevel +
                    MaxHealthFromStrength +
                    MaxHealthFromArmorSet +
                    MaxHealthFromWeapon) *
                    MaxHealthFromPerk;

                return RoundValue(MaxHealth);
            }
        }
        #endregion

        #region HealthRegeneration
        private void UpdateText_HealthRegeneration()
        {
            HealthRegenerationText.text = $"Реген. здоровья: {RoundValue(HealthRegeneration)}{_perSecondName}";
        }
        private void UpdateText_HealthRegenerationBase()
        {
            HealthRegenerationBaseText.text =
                $"{RoundValue(HealthRegenerationBase)}{_perSecondName} {_baseName}";
        }
        private void UpdateText_HealthRegenerationFromStrength()
        {
            HealthRegenerationFromStrengthText.text =
                $"+{RoundValue(HealthRegenerationFromStrength)}{_perSecondName} {_fromStrengthName}";
        }
        private void UpdateText_HealthRegenerationFromArmorSet()
        {
            HealthRegenerationFromArmorSetText.text =
                $"+{RoundValue(HealthRegenerationFromArmorSet)}{_perSecondName} {_fromArmorSetName}";
        }
        private void UpdateText_HealthRegenerationFromWeapon()
        {
            HealthRegenerationFromWeaponText.text =
                $"+{RoundValue(HealthRegenerationFromWeapon)}{_perSecondName} {_fromWeaponName}";
        }
        private void UpdateText_HealthRegenerationFromPerk()
        {
            HealthRegenerationFromPerkText.text = $"+{PercentValueFormatting(HealthRegenerationFromPerk)}% {_fromPerkName} (+{AdditiveSumm() + _perSecondName})";

            float AdditiveSumm()
            {
                float HealthRegeneration =
                    (HealthRegenerationBase +
                    HealthRegenerationFromStrength +
                    HealthRegenerationFromArmorSet +
                    HealthRegenerationFromWeapon) *
                    HealthRegenerationFromPerk;

                return RoundValue(HealthRegeneration);
            }
        }
        #endregion

        #region MaxStamina
        private void UpdateText_MaxStamina()
        {
            MaxStaminaText.text = $"Макс.выносливость: {RoundValue(MaxStamina)}";
        }
        private void UpdateText_MaxStaminaBase()
        {
            MaxStaminaBaseText.text =
                $"{RoundValue(MaxStaminaBase)} {_baseName}";
        }
        private void UpdateText_MaxStaminaFromLevel()
        {
            MaxStaminaFromLevelText.text =
                $"+{RoundValue(MaxStaminaFromLevel)} {_fromLevelName}";
        }
        private void UpdateText_MaxStaminaFromStrength()
        {
            MaxStaminaFromStrengthText.text =
                $"+{RoundValue(MaxStaminaFromStrength)} {_fromStrengthName}";
        }
        private void UpdateText_MaxStaminaFromDexterity()
        {
            MaxStaminaFromDexterityText.text =
                $"+{RoundValue(MaxStaminaFromDexterity)} {_fromDexterityName}";
        }
        private void UpdateText_MaxStaminaFromArmorSet()
        {
            MaxStaminaFromArmorSetText.text =
                $"+{RoundValue(MaxStaminaFromArmorSet)} {_fromArmorSetName}";
        }
        private void UpdateText_MaxStaminaFromWeapon()
        {
            MaxStaminaFromWeaponText.text =
                $"+{RoundValue(MaxStaminaFromWeapon)} {_fromWeaponName}";
        }
        private void UpdateText_MaxStaminaFromPerk()
        {
            MaxStaminaFromPerkText.text = $"+{PercentValueFormatting(MaxStaminaFromPerk)}% {_fromPerkName} (+{AdditiveSumm()})";

            float AdditiveSumm()
            {
                float MaxStamina =
                    (MaxStaminaBase +
                    MaxStaminaFromLevel +
                    MaxStaminaFromStrength +
                    MaxStaminaFromDexterity +
                    MaxStaminaFromArmorSet +
                    MaxStaminaFromWeapon) *
                    MaxStaminaFromPerk;

                return RoundValue(MaxStamina);
            }
        }
        #endregion

        #region StaminaRegeneration
        private void UpdateText_StaminaRegeneration()
        {
            StaminaRegenerationText.text = $"Реген. выносливости: {RoundValue(StaminaRegeneration)}{_perSecondName}";
        }
        private void UpdateText_StaminaRegenerationBase()
        {
            StaminaRegenerationBaseText.text =
                $"{RoundValue(StaminaRegenerationBase)}{_perSecondName} {_baseName}";
        }
        private void UpdateText_StaminaRegenerationFromStrength()
        {
            StaminaRegenerationFromStrengthText.text =
                $"+{RoundValue(StaminaRegenerationFromStrength)}{_perSecondName} {_fromStrengthName}";
        }
        private void UpdateText_StaminaRegenerationFromDexterity()
        {
            StaminaRegenerationFromDexterityText.text =
                $"+{RoundValue(StaminaRegenerationFromDexterity)}{_perSecondName} {_fromDexterityName}";
        }
        private void UpdateText_StaminaRegenerationFromArmorSet()
        {
            StaminaRegenerationFromArmorSetText.text =
                $"+{RoundValue(StaminaRegenerationFromArmorSet)}{_perSecondName} {_fromArmorSetName}";
        }
        private void UpdateText_StaminaRegenerationFromWeapon()
        {
            StaminaRegenerationFromWeaponText.text =
                $"+{RoundValue(StaminaRegenerationFromWeapon)}{_perSecondName} {_fromWeaponName}";
        }
        private void UpdateText_StaminaRegenerationFromPerk()
        {
            StaminaRegenerationFromPerkText.text = $"+{PercentValueFormatting(StaminaRegenerationFromPerk)}% {_fromPerkName} (+{AdditiveSumm() + _perSecondName})";

            float AdditiveSumm()
            {
                float StaminaRegeneration =
                    (StaminaRegenerationBase +
                    StaminaRegenerationFromStrength +
                    StaminaRegenerationFromDexterity +
                    StaminaRegenerationFromArmorSet +
                    StaminaRegenerationFromWeapon) *
                    StaminaRegenerationFromPerk;

                return RoundValue(StaminaRegeneration);
            }
        }
        #endregion

        #region MaxWill
        private void UpdateText_MaxWill()
        {
            MaxWillText.text = $"Макс.воля: {RoundValue(MaxWill)}";
        }
        private void UpdateText_MaxWillBase()
        {
            MaxWillBaseText.text =
                $"{RoundValue(MaxWillBase)} {_baseName}";
        }
        private void UpdateText_MaxWillFromLevel()
        {
            MaxWillFromLevelText.text =
                $"+{RoundValue(MaxWillFromLevel)} {_fromLevelName}";
        }
        private void UpdateText_MaxWillFromIntellect()
        {
            MaxWillFromIntellectText.text =
                $"+{RoundValue(MaxWillFromIntellect)} {_fromIntellectName}";
        }
        private void UpdateText_MaxWillFromArmorSet()
        {
            MaxWillFromArmorSetText.text =
                $"+{RoundValue(MaxWillFromArmorSet)} {_fromArmorSetName}";
        }
        private void UpdateText_MaxWillFromPerk()
        {
            MaxWillFromPerkText.text = $"+{PercentValueFormatting(MaxWillFromPerk)}% {_fromPerkName} (+{AdditiveSumm()})";

            float AdditiveSumm()
            {
                float MaxWill =
                    (MaxWillBase +
                    MaxWillFromLevel +
                    MaxWillFromIntellect +
                    MaxWillFromArmorSet) *
                    MaxWillFromPerk;

                return RoundValue(MaxWill);
            }
        }
        #endregion

        #region WillRegeneration
        private void UpdateText_WillRegeneration()
        {
            WillRegenerationText.text = $"Реген. воли: {RoundValue(WillRegeneration)}{_perSecondName}";
        }
        private void UpdateText_WillRegenerationBase()
        {
            WillRegenerationBaseText.text =
                $"{RoundValue(WillRegenerationBase)}{_perSecondName} {_baseName}";
        }
        private void UpdateText_WillRegenerationFromIntellect()
        {
            WillRegenerationFromIntellectText.text =
                $"+{RoundValue(WillRegenerationFromIntellect)}{_perSecondName} {_fromIntellectName}";
        }
        private void UpdateText_WillRegenerationFromArmorSet()
        {
            WillRegenerationFromArmorSetText.text =
                $"+{RoundValue(WillRegenerationFromArmorSet)}{_perSecondName} {_fromArmorSetName}";
        }
        private void UpdateText_WillRegenerationFromPerk()
        {
            WillRegenerationFromPerkText.text = $"+{PercentValueFormatting(WillRegenerationFromPerk)}% {_fromPerkName} (+{AdditiveSumm() + _perSecondName})";

            float AdditiveSumm()
            {
                float WillRegeneration =
                    (WillRegenerationBase +
                    WillRegenerationFromIntellect +
                    WillRegenerationFromArmorSet) *
                    WillRegenerationFromPerk;

                return RoundValue(WillRegeneration);
            }
        }
        #endregion

        #region Damage
        private void UpdateText_Damage()
        {
            DamageText.text = $"Урон: {RoundValue(Damage)}";
        }
        private void UpdateText_DamageBase()
        {
            DamageBaseText.text =
                $"{RoundValue(DamageBase)} {_baseName}";
        }
        private void UpdateText_DamageFromLevel()
        {
            DamageFromLevelText.text =
                $"+{RoundValue(DamageFromLevel)} {_fromLevelName}";
        }
        private void UpdateText_DamageFromWeapon()
        {
            DamageFromWeaponText.text =
                $"+{RoundValue(DamageFromWeapon)} {_fromWeaponName}";
        }
        private void UpdateText_DamageFromStrength()
        {
            DamageFromStrengthText.text =
                $"+{RoundValue(DamageFromStrength)} {_fromStrengthName}";
        }
        private void UpdateText_DamageFromArmorSet()
        {
            DamageFromArmorSetText.text =
                $"+{RoundValue(DamageFromArmorSet)} {_fromArmorSetName}";
        }
        private void UpdateText_DamageFromPerk()
        {
            DamageFromPerkText.text = $"+{PercentValueFormatting(DamageFromPerk)}% {_fromPerkName} (+{AdditiveSumm()})";

            float AdditiveSumm()
            {
                float Damage =
                    (DamageBase +
                    DamageFromLevel +
                    DamageFromWeapon +
                    DamageFromStrength +
                    DamageFromArmorSet) *
                    DamageFromPerk;

                return RoundValue(Damage);
            }
        }
        #endregion

        #region Armor
        private void UpdateText_Armor()
        {
            ArmorText.text = $"Броня: {RoundValue(Armor)}";
        }
        private void UpdateText_ArmorBase()
        {
            ArmorBaseText.text =
                $"{RoundValue(ArmorBase)} {_baseName}";
        }
        private void UpdateText_ArmorFromArmorSet()
        {
            ArmorFromArmorSetText.text =
                $"+{RoundValue(ArmorFromArmorSet)} {_fromArmorSetName}";
        }
        private void UpdateText_ArmorFromWeapon()
        {
            ArmorFromWeaponText.text =
                $"+{RoundValue(ArmorFromWeapon)} {_fromWeaponName}";
        }
        private void UpdateText_ArmorFromDexterity()
        {
            ArmorFromDexterityText.text =
                $"+{RoundValue(ArmorFromDexterity)} {_fromDexterityName}";
        }
        private void UpdateText_ArmorFromPerk()
        {
            ArmorFromPerkText.text = $"+{PercentValueFormatting(ArmorFromPerk)}% {_fromPerkName} (+{AdditiveSumm()})";

            float AdditiveSumm()
            {
                float Armor =
                    (ArmorBase +
                    ArmorFromArmorSet +
                    ArmorFromWeapon +
                    ArmorFromDexterity) *
                    ArmorFromPerk;

                return RoundValue(Armor);
            }
        }
        #endregion

        #region Evasion
        private void UpdateText_Evasion()
        {
            EvasionText.text = $"Уклонение: {RoundValue(Evasion * 100)}%";
        }
        private void UpdateText_EvasionBase()
        {
            EvasionBaseText.text =
                $"{RoundValue(EvasionBase * 100)}% {_baseName}";
        }
        private void UpdateText_EvasionFromDexterity()
        {
            EvasionFromDexterityText.text =
                $"+{RoundValue(EvasionFromDexterity * 100)}% {_fromDexterityName}";
        }
        private void UpdateText_EvasionFromArmorSet()
        {
            EvasionFromArmorSetText.text =
                $"+{RoundValue(EvasionFromArmorSet * 100)}% {_fromArmorSetName}";
        }
        private void UpdateText_EvasionFromWeapon()
        {
            EvasionFromWeaponText.text =
                $"+{RoundValue(EvasionFromWeapon * 100)}% {_fromWeaponName}";
        }
        private void UpdateText_EvasionFromPerk()
        {
            EvasionFromPerkText.text = $"+{PercentValueFormatting(EvasionFromPerk * 100)}% {_fromPerkName} (+{AdditiveSumm() * 100}%)";

            float AdditiveSumm()
            {
                float Evasion =
                    (EvasionBase +
                    EvasionFromArmorSet +
                    EvasionFromWeapon +
                    EvasionFromDexterity) *
                    EvasionFromPerk;

                return RoundValue(Evasion);
            }
        }
        #endregion

        #region Strength
        private void UpdateText_Strength()
        {
            StrengthText.text = $"  Силушка: {Math.Round(Strength)}";
        }
        private void UpdateText_StrengthBase()
        {
            StrengthBaseText.text =
                    $"{RoundValue(StrengthBase)} {_baseName}";
        }
        private void UpdateText_StrengthFromLevel()
        {
            StrengthFromLevelText.text =
                    $"+{RoundValue(StrengthFromLevel)} {_fromLevelName}";
        }
        private void UpdateText_StrengthFromPumpingPoints()
        {
            StrengthFromPumpingPointsText.text =
                $"+{RoundValue(StrengthFromPumpingPoints)} {_fromPumpingPointsName}"; ;
        }
        private void UpdateText_StrengthFromArmorSet()
        {
            StrengthFromArmorSetText.text =
                    $"+{RoundValue(StrengthFromArmorSet)} {_fromArmorSetName}";
        }
        private void UpdateText_StrengthFromWeapon()
        {
            StrengthFromWeaponText.text =
                    $"+{RoundValue(StrengthFromWeapon)} {_fromWeaponName}";
        }
        private void UpdateText_StrengthFromPerk()
        {
            StrengthFromPerkText.text = $"+{PercentValueFormatting(StrengthFromPerk)}% {_fromPerkName} (+{AdditiveSumm()})";

            float AdditiveSumm()
            {
                float Strength =
                    (StrengthBase +
                    StrengthFromArmorSet +
                    StrengthFromLevel +
                    StrengthFromPumpingPoints +
                    StrengthFromWeapon) *
                    StrengthFromPerk;

                return RoundValue(Strength);
            }
        }
        #endregion

        #region Dexterity
        private void UpdateText_Dexterity()
        {
            DexterityText.text = $"  Проворство: {Math.Round(Dexterity)}";
        }
        private void UpdateText_DexterityBase()
        {
            DexterityBaseText.text =
                $"{RoundValue(DexterityBase)} {_baseName}";
        }
        private void UpdateText_DexterityFromArmorSet()
        {
            DexterityFromArmorSetText.text =
                 $"+{RoundValue(DexterityFromArmorSet)} {_fromArmorSetName}";
        }
        private void UpdateText_DexterityFromWeapon()
        {
            DexterityFromWeaponText.text =
                 $"+{RoundValue(DexterityFromWeapon)} {_fromWeaponName}";
        }
        private void UpdateText_DexterityFromLevel()
        {
            DexterityFromLevelText.text =
                 $"+{RoundValue(DexterityFromLevel)} {_fromLevelName}";
        }
        private void UpdateText_DexterityFromPumpingPoints()
        {
            DexterityFromPumpingPointsText.text = 
                $"+{RoundValue(DexterityFromPumpingPoints)} {_fromPumpingPointsName}"; ;
        }
        private void UpdateText_DexterityFromPerk()
        {
            DexterityFromPerkText.text = $"+{PercentValueFormatting(DexterityFromPerk)}% {_fromPerkName} (+{AdditiveSumm()})";

            float AdditiveSumm()
            {
                float Dexterity =
                    (DexterityBase +
                    DexterityFromLevel +
                    DexterityFromArmorSet +
                    DexterityFromPumpingPoints +
                    DexterityFromWeapon) *
                    DexterityFromPerk;

                return RoundValue(Dexterity);
            }
        }
        #endregion

        #region Intellect
        private void UpdateText_Intellect()
        {
            IntellectText.text = $"  Разум: {Math.Round(Intellect)}";
        }
        private void UpdateText_IntellectBase()
        {
            IntellectBaseText.text =
                    $"{RoundValue(IntellectBase)} {_baseName}";
        }
        private void UpdateText_IntellectFromLevel()
        {
            IntellectFromLevelText.text =
                    $"+{RoundValue(IntellectFromLevel)} {_fromLevelName}";
        }
        private void UpdateText_IntellectFromPumpingPoints()
        {
            IntellectFromPumpingPointsText.text =
                    $"+{RoundValue(IntellectFromPumpingPoints)} {_fromPumpingPointsName}";
        }
        private void UpdateText_IntellectFromArmorSet()
        {
            IntellectFromArmorSetText.text =
                    $"+{RoundValue(IntellectFromArmorSet)} {_fromArmorSetName}";
        }
        private void UpdateText_IntellectFromPerk()
        {
            IntellectFromPerkText.text = $"+{PercentValueFormatting(IntellectFromPerk)}% {_fromPerkName} (+{AdditiveSumm()})";

            float AdditiveSumm()
            {
                float Intellect =
                    (IntellectBase +
                    IntellectFromArmorSet +
                    IntellectFromPumpingPoints +
                    IntellectFromLevel) *
                    IntellectFromPerk;

                return RoundValue(Intellect);
            }
        }
        #endregion

        #endregion

        #region Formatting
        private float PercentValueFormatting(float value)
        {
            return (float)Math.Round((value * 100), 2);
        }

        private float RoundValue(float curr_var)
        {
            float value = (float)Math.Round(curr_var, 2);
            return value;
        }
        #endregion

        #region UnionMethods
        public void CalcAllCharacteristics()
        {
            CalcSet_Strength();
            CalcSet_Dexterity();
            CalcSet_Intellect();

            CalcSet_MaxHealth();
            CalcSet_HealthRegeneration();
            CalcSet_MaxStamina();
            CalcSet_StaminaRegeneration();
            CalcSet_MaxWill();
            CalcSet_WillRegeneration();
            CalcSet_Evasion();
            CalcSet_Damage();
            CalcSet_Armor();
        }
        #endregion

        #region Other
        public void SetArrows()
        {
            StatButtonsImages[0] = MaxHealth_ButtonImage;
            StatButtonsImages[1] = HealthRegeneration_ButtonImage;
            StatButtonsImages[2] = MaxStamina_ButtonImage;
            StatButtonsImages[3] = StaminaRegeneration_ButtonImage;
            StatButtonsImages[4] = MaxWill_ButtonImage;
            StatButtonsImages[5] = WillRegeneration_ButtonImage;
            StatButtonsImages[6] = Damage_ButtonImage;
            StatButtonsImages[7] = Armor_ButtonImage;
            StatButtonsImages[8] = Evasion_ButtonImage;
            StatButtonsImages[9] = Strength_ButtonImage;
            StatButtonsImages[10] = Dexterity_ButtonImage;
            StatButtonsImages[11] = Intellect_ButtonImage;

            StatButtonsBackgrounds[0] = ButtonBackground_MaxHealth;
            StatButtonsBackgrounds[1] = ButtonBackground_HealthRegeneration;
            StatButtonsBackgrounds[2] = ButtonBackground_MaxStamina;
            StatButtonsBackgrounds[3] = ButtonBackground_StaminaRegeneration;
            StatButtonsBackgrounds[4] = ButtonBackground_MaxWill;
            StatButtonsBackgrounds[5] = ButtonBackground_WillRegeneration;
            StatButtonsBackgrounds[6] = ButtonBackground_Damage;
            StatButtonsBackgrounds[7] = ButtonBackground_Armor;
            StatButtonsBackgrounds[8] = ButtonBackground_Evasion;
            StatButtonsBackgrounds[9] = ButtonBackground_Strength;
            StatButtonsBackgrounds[10] = ButtonBackground_Dexterity;
            StatButtonsBackgrounds[11] = ButtonBackground_Intellect;

            ArrStatsName[0] = MaxHealthText;
            ArrStatsName[1] = HealthRegenerationText;
            ArrStatsName[2] = MaxStaminaText;
            ArrStatsName[3] = StaminaRegenerationText;
            ArrStatsName[4] = MaxWillText;
            ArrStatsName[5] = WillRegenerationText;
            ArrStatsName[6] = DamageText;
            ArrStatsName[7] = ArmorText;
            ArrStatsName[8] = EvasionText;
            ArrStatsName[9] = StrengthText;
            ArrStatsName[10] = DexterityText;
            ArrStatsName[11] = IntellectText;
        }

        public void SetSets()
        {
            StatSetsBuffer[0] = MaxHealthSet;
            StatSetsBuffer[1] = HealthRegenerationSet;
            StatSetsBuffer[2] = MaxStaminaSet;
            StatSetsBuffer[3] = StaminaRegenerationSet;
            StatSetsBuffer[4] = MaxWillSet;
            StatSetsBuffer[5] = WillRegenerationSet;
            StatSetsBuffer[6] = DamageSet;
            StatSetsBuffer[7] = ArmorSet;
            StatSetsBuffer[8] = EvasionSet;
            StatSetsBuffer[9] = StrengthSet;
            StatSetsBuffer[10] = DexteritySet;
            StatSetsBuffer[11] = IntellectSet;
        }

        public void SetExtendedStatsInArrays()
        {
            ExtendedStats_MaxHealth[0] = MaxHealthBaseText.gameObject;
            ExtendedStats_MaxHealth[1] = MaxHealthFromLevelText.gameObject;
            ExtendedStats_MaxHealth[2] = MaxHealthFromStrengthText.gameObject;
            ExtendedStats_MaxHealth[3] = MaxHealthFromArmorSetText.gameObject;
            ExtendedStats_MaxHealth[4] = MaxHealthFromWeaponText.gameObject;
            ExtendedStats_MaxHealth[5] = MaxHealthFromPerkText.gameObject;

            ExtendedStats_HealthRegeneration[0] = HealthRegenerationBaseText.gameObject;
            ExtendedStats_HealthRegeneration[1] = HealthRegenerationFromStrengthText.gameObject;
            ExtendedStats_HealthRegeneration[2] = HealthRegenerationFromArmorSetText.gameObject;
            ExtendedStats_HealthRegeneration[3] = HealthRegenerationFromWeaponText.gameObject;
            ExtendedStats_HealthRegeneration[4] = HealthRegenerationFromPerkText.gameObject;

            ExtendedStats_MaxStamina[0] = MaxStaminaBaseText.gameObject;
            ExtendedStats_MaxStamina[1] = MaxStaminaFromLevelText.gameObject;
            ExtendedStats_MaxStamina[2] = MaxStaminaFromStrengthText.gameObject;
            ExtendedStats_MaxStamina[3] = MaxStaminaFromDexterityText.gameObject;
            ExtendedStats_MaxStamina[4] = MaxStaminaFromArmorSetText.gameObject;
            ExtendedStats_MaxStamina[5] = MaxStaminaFromWeaponText.gameObject;
            ExtendedStats_MaxStamina[6] = MaxStaminaFromPerkText.gameObject;

            ExtendedStats_StaminaRegeneration[0] = StaminaRegenerationBaseText.gameObject;
            ExtendedStats_StaminaRegeneration[1] = StaminaRegenerationFromStrengthText.gameObject;
            ExtendedStats_StaminaRegeneration[2] = StaminaRegenerationFromDexterityText.gameObject;
            ExtendedStats_StaminaRegeneration[3] = StaminaRegenerationFromArmorSetText.gameObject;
            ExtendedStats_StaminaRegeneration[4] = StaminaRegenerationFromWeaponText.gameObject;
            ExtendedStats_StaminaRegeneration[5] = StaminaRegenerationFromPerkText.gameObject;

            ExtendedStats_MaxWill[0] = MaxWillBaseText.gameObject;
            ExtendedStats_MaxWill[1] = MaxWillFromLevelText.gameObject;
            ExtendedStats_MaxWill[2] = MaxWillFromIntellectText.gameObject;
            ExtendedStats_MaxWill[3] = MaxWillFromArmorSetText.gameObject;
            ExtendedStats_MaxWill[4] = MaxWillFromPerkText.gameObject;

            ExtendedStats_WillRegeneration[0] = WillRegenerationBaseText.gameObject;
            ExtendedStats_WillRegeneration[1] = WillRegenerationFromIntellectText.gameObject;
            ExtendedStats_WillRegeneration[2] = WillRegenerationFromArmorSetText.gameObject;
            ExtendedStats_WillRegeneration[3] = WillRegenerationFromPerkText.gameObject;

            ExtendedStats_Damage[0] = DamageBaseText.gameObject;
            ExtendedStats_Damage[1] = DamageFromLevelText.gameObject;
            ExtendedStats_Damage[2] = DamageFromWeaponText.gameObject;
            ExtendedStats_Damage[3] = DamageFromStrengthText.gameObject;
            ExtendedStats_Damage[4] = DamageFromArmorSetText.gameObject;
            ExtendedStats_Damage[5] = DamageFromPerkText.gameObject;

            ExtendedStats_Armor[0] = ArmorBaseText.gameObject;
            ExtendedStats_Armor[1] = ArmorFromArmorSetText.gameObject;
            ExtendedStats_Armor[2] = ArmorFromWeaponText.gameObject;
            ExtendedStats_Armor[3] = ArmorFromDexterityText.gameObject;
            ExtendedStats_Armor[4] = ArmorFromPerkText.gameObject;

            ExtendedStats_Evasion[0] = EvasionBaseText.gameObject;
            ExtendedStats_Evasion[1] = EvasionFromDexterityText.gameObject;
            ExtendedStats_Evasion[2] = EvasionFromArmorSetText.gameObject;
            ExtendedStats_Evasion[3] = EvasionFromWeaponText.gameObject;
            ExtendedStats_Evasion[4] = EvasionFromPerkText.gameObject;

            ExtendedStats_Strength[0] = StrengthBaseText.gameObject;
            ExtendedStats_Strength[1] = StrengthFromLevelText.gameObject;
            ExtendedStats_Strength[2] = StrengthFromPumpingPointsText.gameObject;
            ExtendedStats_Strength[3] = StrengthFromArmorSetText.gameObject;
            ExtendedStats_Strength[4] = StrengthFromWeaponText.gameObject;
            ExtendedStats_Strength[5] = StrengthFromPerkText.gameObject;

            ExtendedStats_Dexterity[0] = DexterityBaseText.gameObject;
            ExtendedStats_Dexterity[1] = DexterityFromLevelText.gameObject;
            ExtendedStats_Dexterity[2] = DexterityFromPumpingPointsText.gameObject;
            ExtendedStats_Dexterity[3] = DexterityFromArmorSetText.gameObject;
            ExtendedStats_Dexterity[4] = DexterityFromWeaponText.gameObject;
            ExtendedStats_Dexterity[5] = DexterityFromPerkText.gameObject;

            ExtendedStats_Intellect[0] = IntellectBaseText.gameObject;
            ExtendedStats_Intellect[1] = IntellectFromLevelText.gameObject;
            ExtendedStats_Intellect[2] = IntellectFromPumpingPointsText.gameObject;
            ExtendedStats_Intellect[3] = IntellectFromArmorSetText.gameObject;
            ExtendedStats_Intellect[4] = IntellectFromPerkText.gameObject;

            //for (int i = 0; i < ExtendedStats_MaxHealth.Length; i++)
            //    ExtendedStatsBuffer[0, i] = ExtendedStats_MaxHealth[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[1, i] = ExtendedStats_HealthRegeneration[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[2, i] = ExtendedStats_MaxStamina[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[3, i] = ExtendedStats_StaminaRegeneration[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[4, i] = ExtendedStats_MaxWill[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[5, i] = ExtendedStats_WillRegeneration[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[6, i] = ExtendedStats_Damage[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[7, i] = ExtendedStats_Armor[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[8, i] = ExtendedStats_Evasion[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[9, i] = ExtendedStats_Strength[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[10, i] = ExtendedStats_Dexterity[i];
            //for (int i = 0; i < ExtendedStats_HealthRegeneration.Length; i++)
            //    ExtendedStatsBuffer[11, i] = ExtendedStats_Intellect[i];
        }

        public void ResetArmorStats()
        {
            StrengthFromArmorSet = 0;
            DexterityFromArmorSet = 0;
            IntellectFromArmorSet = 0;

            MaxHealthFromArmorSet = 0;
            HealthRegenerationFromArmorSet = 0;
            MaxStaminaFromArmorSet = 0;
            StaminaRegenerationFromArmorSet = 0;
            MaxWillFromArmorSet = 0;
            WillRegenerationFromArmorSet = 0;
            ArmorFromArmorSet = 0;
            DamageFromArmorSet = 0;
            EvasionFromArmorSet = 0;
        }

        public void ResetWeaponStats()
        {
            StrengthFromWeapon = 0;
            DexterityFromWeapon = 0;

            MaxHealthFromWeapon = 0;
            HealthRegenerationFromWeapon = 0;
            MaxStaminaFromWeapon = 0;
            StaminaRegenerationFromWeapon = 0;
            DamageFromWeapon = 0;
            EvasionFromWeapon = 0;
            ArmorFromWeapon = 0;
            CritDamageFromWeapon = 0;
            CritChanceFromWeapon = 0;
            CostStaminaToAttackFromWeapon = 0;
        }

        public void ResetOtherStats()
        {
            StrengthFromLevel = 0;
            StrengthFromPumpingPoints = 0;
            DexterityFromLevel = 0;
            DexterityFromPumpingPoints = 0;
            IntellectFromLevel = 0;
            IntellectFromPumpingPoints = 0;

            MaxHealthFromLevel = 0;
            MaxStaminaFromLevel = 0;
            MaxWillFromLevel = 0;
            DamageFromLevel = 0;

            PumpingPoints = 0;
            SwitchPumpingPointsState(false);
        }

        public void RedactHeroCardToUnlockState()
        {
            LevelText.transform.parent.gameObject.SetActive(true);
            SelectHeroButton.interactable = true;
            BuyHeroButton.gameObject.SetActive(false);
            PriceNarrationText.gameObject.SetActive(false);
            LockImage.gameObject.SetActive(false);
            SelectHeroButton.image.color = new Color(1, 1, 1, 1);
        }

        public void RedactHeroCardToLockState()
        {
            LevelText.transform.parent.gameObject.SetActive(false);
            SelectHeroButton.interactable = false;
            BuyHeroButton.gameObject.SetActive(true);
            PriceNarrationText.gameObject.SetActive(true);
            LockImage.gameObject.SetActive(true);
            SelectHeroButton.image.color = new Color(1, 1, 1, 0.75f);
        }

        public void SetActualStocksToMAX()
        {
            ActualHealth = MaxHealth;
            ActualStamina = MaxStamina;
            ActualWill = MaxWill;
        }

        private void CheckHeroLevelUp()
        {
            if (ActualEXP >= EXPForNextLevel)
                HeroLevelUp();
        }

        private void HeroLevelUp()
        {
            Level++;
            _actualEXP -= EXPForNextLevel;
            EXPForNextLevel += EXPForNextLevel * _multiplierPercentForNextLevel;
            PumpingPoints += AdditivePumpingPoints;
            SwitchPumpingPointsState(true);
            StrengthFromLevel += _additiveStrengthFromLevel;
            DexterityFromLevel += _additiveDexterityFromLevel;
            IntellectFromLevel += _additiveIntellectFromLevel;
            MaxHealthFromLevel += _additiveMaxHealthFromLevel;
            MaxStaminaFromLevel += _additiveMaxStaminaFromLevel;
            MaxWillFromLevel += _additiveMaxWillFromLevel;
            DamageFromLevel += _additiveDamageFromLevel;
        }

        private void SwitchPumpingPointsState(bool state)
        {
            PumpingPointsText.gameObject.SetActive(state);
            StrengthPumpingPointButton.gameObject.SetActive(state);
            DexterityPumpingPointButton.gameObject.SetActive(state);
            IntellectPumpingPointButton.gameObject.SetActive(state);
        }
        #endregion

        #region HeroDeadEvents
        public IEnumerator DeadCountingDown()
        {
            float time;
            CheckAndChangeHeroDeadDisplay();

            if (RemainingTimeToResurrationHero <= 0)
            {
                time = TimeToResurration;
                RemainingTimeToResurrationHero = time;
            }
            else time = RemainingTimeToResurrationHero;

            while (time > 0 && !IsAlive)
            {
                TimeToResurractionText.text = $"{Math.Round(time)} сек.";
                yield return null;
                time -= Time.deltaTime;
                RemainingTimeToResurrationHero = time;
            }

            RemainingTimeToResurrationHero = 0;
            HeroIsResurrected();
        }

        public void HeroIsResurrected()
        {
            if (!IsAlive)
            {
                IsAlive = true;         
                SetActualStocksToMAX();
                CheckAndChangeHeroDeadDisplay();
            }
        }

        private void CheckAndChangeHeroDeadDisplay()
        {
            if (IsAlive)
            {
                SwitchActivateTimer(false);
                ChangeHeroColor(White);
                SwitchDeadImage(false);
            }
            else
            {
                SwitchActivateTimer(true);
                ChangeHeroColor(Red);
                SwitchDeadImage(true);
            }
        }

        private void ChangeHeroColor(Color color)
        { SelectHeroButton.image.color = color; }

        private void SwitchActivateTimer(bool state)
        { TimeToResurractionText.gameObject.SetActive(state); }

        private void SwitchDeadImage(bool state)
        { DeadImage.gameObject.SetActive(state); }
        #endregion
    }

    void Awake()
    {
        InitializeFields();
        FindComponents();

        for (int i = 0; i < HeroCount; i++)
        {
            hero[i].SetSets();
            hero[i].SetArrows();
            hero[i].SetExtendedStatsInArrays();
        }
    }

    private void Start()
    {
        if (Game.FirstLaunchScene[(int)Game.BuildIndex.Russians_vs_Lizards])
        {
            SetStartValuesFromHeroes();
            CurrentHero = hero[0];
        }

        SetConstantValuesForHeroes();

        for (int i = 0; i < HeroCount; i++)
        {
            hero[i].CalcAllCharacteristics();
            hero[i].UpdateAllText();
            DisplayHeroCostText(i);

            if (hero[i].Bought)
            {
                Heroes.hero[i].RedactHeroCardToUnlockState();
                HeroesMethods.CheckPumpingPoints(i);
            }
            else
            {
                hero[i].SetActualStocksToMAX();
            }

            if (hero[i].RemainingTimeToResurrationHero > 0)
            {
                hero[i].IsAlive = false;
                StartCoroutine(hero[i].DeadCountingDown());
            }
            else hero[i].IsAlive = true;
        }

        ChooseHero(CurrentHeroIndex);

        if (!CurrentHero.IsAlive)
            BattleHero.HeroIsDead();
    }

    #region Displaying
    public void UpdateAllHeroPriceValues()
    {
        for (int i = 0; i < HeroCount; i++)
            if (Heroes.hero[i].Bought == false)
                Heroes.DisplayHeroCostText(i);
    }

    private void DisplayHeroCostText(int heroIndex)
    {
        string message_1 = $"\r\n{ValuesRounding.FormattingValue("", "", Facilities.FaithCurrency)}/{ValuesRounding.FormattingValue("", "", hero[heroIndex].PriceHero)} веры и";
        string message_2 = $"\r\n{Achievements_R_vs_L.AccumulatedKills}/{hero[heroIndex].NeedLizards} убитых ящеров";

        if (Facilities.FaithCurrency >= hero[heroIndex].PriceHero)
        {
            message_1 = $"\r\n<color=green>{ValuesRounding.FormattingValue("", "", Facilities.FaithCurrency)}/{ValuesRounding.FormattingValue("", "", hero[heroIndex].PriceHero)}</color> веры и";
        }
        if (Achievements_R_vs_L.AccumulatedKills >= hero[heroIndex].NeedLizards)
        {
            message_2 = $"\r\n<color=green>{Achievements_R_vs_L.AccumulatedKills}/{hero[heroIndex].NeedLizards}</color> убитых ящеров";
        }

        hero[heroIndex].PriceNarrationText.text = $"Требуется:{message_1}{message_2}";
    }
    #endregion

    #region Initialize
    private void InitializeFields()
    {
        hero = new Hero[HeroCount];

        for (int i = 0; i < HeroCount; i++)
        {
            hero[i] = new()
            {
                Index = i
            };
        }
    }

    private void FindComponents()
    {
        for (int i = 0; i < hero.Length; i++)
        {
            RectTransform[] rectComponents = HeroObject[i].GetComponentsInChildren<RectTransform>(includeInactive: true);
            foreach (RectTransform component in rectComponents)
            {
                switch (component.name)
                {                    
                    case ("Body"):
                        hero[i].CharacteristicsBody = component;
                        break;
                    case ("MaxHealth_Set"):
                        hero[i].MaxHealthSet = component;
                        break;
                    case ("HealthRegeneration_Set"):
                        hero[i].HealthRegenerationSet = component;
                        break;
                    case ("MaxStamina_Set"):
                        hero[i].MaxStaminaSet = component;
                        break;
                    case ("StaminaRegeneration_Set"):
                        hero[i].StaminaRegenerationSet = component;
                        break;
                    case ("MaxWill_Set"):
                        hero[i].MaxWillSet = component;
                        break;
                    case ("WillRegeneration_Set"):
                        hero[i].WillRegenerationSet = component;
                        break;
                    case ("Damage_Set"):
                        hero[i].DamageSet = component;
                        break;
                    case ("Armor_Set"):
                        hero[i].ArmorSet = component;
                        break;
                    case ("Evasion_Set"):
                        hero[i].EvasionSet = component;
                        break;
                    case ("Strength_Set"):
                        hero[i].StrengthSet = component;
                        break;
                    case ("Dexterity_Set"):
                        hero[i].DexteritySet = component;
                        break;
                    case ("Intellect_Set"):
                        hero[i].IntellectSet = component;
                        break;
                    default: break;
                }
            }

            Slider[] sliderComponents = HeroObject[i].GetComponentsInChildren<Slider>(includeInactive: true);
            foreach (Slider component in sliderComponents)
            {
                switch (component.name)
                {
                    case ("ExperienceBar"):
                        hero[i].ExperienceBar = component;
                        break;
                    case ("HealthBar"):
                        hero[i].HealthBar = component;
                        break;
                    case ("StaminaBar"):
                        hero[i].StaminaBar = component;
                        break;
                    case ("WillBar"):
                        hero[i].WillBar = component;
                        break;
                    default: break;
                }
            }

            TextMeshProUGUI[] textComponent = HeroObject[i].GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
            foreach (TextMeshProUGUI component in textComponent)
            {
                switch (component.name)
                {
                    #region MaxHealth
                    case ("MaxHealth_name"):
                        hero[i].MaxHealthText = component;
                        break;
                    case ("MaxHealth_Base"):
                        hero[i].MaxHealthBaseText = component;
                        break;
                    case ("MaxHealth_FromLevel"):
                        hero[i].MaxHealthFromLevelText = component;
                        break;
                    case ("MaxHealth_FromStrength"):
                        hero[i].MaxHealthFromStrengthText = component;
                        break;
                    case ("MaxHealth_FromArmorSet"):
                        hero[i].MaxHealthFromArmorSetText = component;
                        break;
                    case ("MaxHealth_FromWeapon"):
                        hero[i].MaxHealthFromWeaponText = component;
                        break;
                    case ("MaxHealth_FromPerk"):
                        hero[i].MaxHealthFromPerkText = component;
                        break;
                    #endregion

                    #region HealthRegeneration
                    case ("HealthRegeneration_name"):
                        hero[i].HealthRegenerationText = component;
                        break;
                    case ("HealthRegeneration_Base"):
                        hero[i].HealthRegenerationBaseText = component;
                        break;
                    case ("HealthRegeneration_FromStrength"):
                        hero[i].HealthRegenerationFromStrengthText = component;
                        break;
                    case ("HealthRegeneration_FromArmorSet"):
                        hero[i].HealthRegenerationFromArmorSetText = component;
                        break;
                    case ("HealthRegeneration_FromWeapon"):
                        hero[i].HealthRegenerationFromWeaponText = component;
                        break;
                    case ("HealthRegeneration_FromPerk"):
                        hero[i].HealthRegenerationFromPerkText = component;
                        break;
                    #endregion

                    #region MaxStamina
                    case ("MaxStamina_name"):
                        hero[i].MaxStaminaText = component;
                        break;
                    case ("MaxStamina_Base"):
                        hero[i].MaxStaminaBaseText = component;
                        break;
                    case ("MaxStamina_FromLevel"):
                        hero[i].MaxStaminaFromLevelText = component;
                        break;
                    case ("MaxStamina_FromStrength"):
                        hero[i].MaxStaminaFromStrengthText = component;
                        break;
                    case ("MaxStamina_FromDexterity"):
                        hero[i].MaxStaminaFromDexterityText = component;
                        break;
                    case ("MaxStamina_FromArmorSet"):
                        hero[i].MaxStaminaFromArmorSetText = component;
                        break;
                    case ("MaxStamina_FromWeapon"):
                        hero[i].MaxStaminaFromWeaponText = component;
                        break;
                    case ("MaxStamina_FromPerk"):
                        hero[i].MaxStaminaFromPerkText = component;
                        break;
                    #endregion

                    #region StaminaRegeneration
                    case ("StaminaRegeneration_name"):
                        hero[i].StaminaRegenerationText = component;
                        break;
                    case ("StaminaRegeneration_Base"):
                        hero[i].StaminaRegenerationBaseText = component;
                        break;
                    case ("StaminaRegeneration_FromStrength"):
                        hero[i].StaminaRegenerationFromStrengthText = component;
                        break;
                    case ("StaminaRegeneration_FromDexterity"):
                        hero[i].StaminaRegenerationFromDexterityText = component;
                        break;
                    case ("StaminaRegeneration_FromArmorSet"):
                        hero[i].StaminaRegenerationFromArmorSetText = component;
                        break;
                    case ("StaminaRegeneration_FromWeapon"):
                        hero[i].StaminaRegenerationFromWeaponText = component;
                        break;
                    case ("StaminaRegeneration_FromPerk"):
                        hero[i].StaminaRegenerationFromPerkText = component;
                        break;
                    #endregion

                    #region MaxWill
                    case ("MaxWill_name"):
                        hero[i].MaxWillText = component;
                        break;
                    case ("MaxWill_Base"):
                        hero[i].MaxWillBaseText = component;
                        break;
                    case ("MaxWill_FromLevel"):
                        hero[i].MaxWillFromLevelText = component;
                        break;
                    case ("MaxWill_FromIntellect"):
                        hero[i].MaxWillFromIntellectText = component;
                        break;
                    case ("MaxWill_FromArmorSet"):
                        hero[i].MaxWillFromArmorSetText = component;
                        break;
                    case ("MaxWill_FromPerk"):
                        hero[i].MaxWillFromPerkText = component;
                        break;
                    #endregion

                    #region WillRegeneration
                    case ("WillRegeneration_name"):
                        hero[i].WillRegenerationText = component;
                        break;
                    case ("WillRegeneration_Base"):
                        hero[i].WillRegenerationBaseText = component;
                        break;
                    case ("WillRegeneration_FromIntellect"):
                        hero[i].WillRegenerationFromIntellectText = component;
                        break;
                    case ("WillRegeneration_FromArmorSet"):
                        hero[i].WillRegenerationFromArmorSetText = component;
                        break;
                    case ("WillRegeneration_FromPerk"):
                        hero[i].WillRegenerationFromPerkText = component;
                        break;
                        #endregion

                    #region Damage
                    case ("Damage_name"):
                        hero[i].DamageText = component;
                        break;
                    case ("Damage_Base"):
                        hero[i].DamageBaseText = component;
                        break;
                    case ("Damage_FromLevel"):
                        hero[i].DamageFromLevelText = component;
                        break;
                    case ("Damage_FromWeapon"):
                        hero[i].DamageFromWeaponText = component;
                        break;
                    case ("Damage_FromStrength"):
                        hero[i].DamageFromStrengthText = component;
                        break;
                    case ("Damage_FromArmorSet"):
                        hero[i].DamageFromArmorSetText = component;
                        break;
                    case ("Damage_FromPerk"):
                        hero[i].DamageFromPerkText = component;
                        break;
                    #endregion

                    #region Armor
                    case ("Armor_name"):
                        hero[i].ArmorText = component;
                        break;
                    case ("Armor_Base"):
                        hero[i].ArmorBaseText = component;
                        break;
                    case ("Armor_FromArmorSet"):
                        hero[i].ArmorFromArmorSetText = component;
                        break;
                    case ("Armor_FromWeapon"):
                        hero[i].ArmorFromWeaponText = component;
                        break;
                    case ("Armor_FromDexterity"):
                        hero[i].ArmorFromDexterityText = component;
                        break;
                    case ("Armor_FromPerk"):
                        hero[i].ArmorFromPerkText = component;
                        break;
                    #endregion

                    #region Evasion
                    case ("Evasion_name"):
                        hero[i].EvasionText = component;
                        break;
                    case ("Evasion_Base"):
                        hero[i].EvasionBaseText = component;
                        break;
                    case ("Evasion_FromDexterity"):
                        hero[i].EvasionFromDexterityText = component;
                        break;
                    case ("Evasion_FromArmorSet"):
                        hero[i].EvasionFromArmorSetText = component;
                        break;
                    case ("Evasion_FromWeapon"):
                        hero[i].EvasionFromWeaponText = component;
                        break;
                    case ("Evasion_FromPerk"):
                        hero[i].EvasionFromPerkText = component;
                        break;
                    #endregion

                    #region Strength
                    case ("Strength_name"):
                        hero[i].StrengthText = component;
                        break;
                    case ("Strength_Base"):
                        hero[i].StrengthBaseText = component;
                        break;
                    case ("Strength_FromLevel"):
                        hero[i].StrengthFromLevelText = component;
                        break; 
                    case ("Strength_FromPumpingPoints"):
                        hero[i].StrengthFromPumpingPointsText = component;
                        break;
                    case ("Strength_FromArmorSet"):
                        hero[i].StrengthFromArmorSetText = component;
                        break;
                    case ("Strength_FromWeapon"):
                        hero[i].StrengthFromWeaponText = component;
                        break;
                    case ("Strength_FromPerk"):
                        hero[i].StrengthFromPerkText = component;
                        break;
                    #endregion

                    #region Dexterity
                    case ("Dexterity_name"):
                        hero[i].DexterityText = component;
                        break;
                    case ("Dexterity_Base"):
                        hero[i].DexterityBaseText = component;
                        break;
                    case ("Dexterity_FromLevel"):
                        hero[i].DexterityFromLevelText = component;
                        break;
                    case ("Dexterity_FromPumpingPoints"):
                        hero[i].DexterityFromPumpingPointsText = component;
                        break;
                    case ("Dexterity_FromArmorSet"):
                        hero[i].DexterityFromArmorSetText = component;
                        break;
                    case ("Dexterity_FromWeapon"):
                        hero[i].DexterityFromWeaponText = component;
                        break;
                    case ("Dexterity_FromPerk"):
                        hero[i].DexterityFromPerkText = component;
                        break;
                    #endregion

                    #region Intellect
                    case ("Intellect_name"):
                        hero[i].IntellectText = component;
                        break;
                    case ("Intellect_Base"):
                        hero[i].IntellectBaseText = component;
                        break;
                    case ("Intellect_FromLevel"):
                        hero[i].IntellectFromLevelText = component;
                        break;
                    case ("Intellect_FromPumpingPoints"):
                        hero[i].IntellectFromPumpingPointsText = component;
                        break;
                    case ("Intellect_FromArmorSet"):
                        hero[i].IntellectFromArmorSetText = component;
                        break;
                    case ("Intellect_FromPerk"):
                        hero[i].IntellectFromPerkText = component;
                        break;

                    default: break;
                        #endregion
                }
            }

            TextMeshProUGUI[] textComponents = HeroObject[i].GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
            foreach (TextMeshProUGUI component in textComponents)
            {
                switch (component.name)
                {
                    case ("HeroName"):
                        hero[i].NameText = component;
                        break;
                    case ("ExperienceValue"):
                        hero[i].ExperienceText = component;
                        break;
                    case ("ExperiencePercentValue"):
                        hero[i].ExperiencePercentText = component;
                        break;
                    case ("HealthValue"):
                        hero[i].HealthText = component;
                        break;
                    case ("HealthPercentValue"):
                        hero[i].HealthPercentText = component;
                        break;
                    case ("HealthRegenerationValue"):
                        hero[i].HealthRegenerationTextForSlider = component;
                        break;
                    case ("StaminaValue"):
                        hero[i].StaminaText = component; 
                        break;
                    case ("StaminaPercentValue"):
                        hero[i].StaminaPercentText = component;
                        break;
                    case ("StaminaRegenerationValue"):
                        hero[i].StaminaRegenerationTextForSlider = component;
                        break;
                    case ("WillValue"):
                        hero[i].WillText = component;
                        break;
                    case ("WillPercentValue"):
                        hero[i].WillPercentText = component;
                        break;
                    case ("WillRegenerationValue"):
                        hero[i].WillRegenerationTextForSlider = component;
                        break;
                    case ("HeroLevel"):
                        hero[i].LevelText = component;
                        break;
                    case ("PriceNarration"):
                        hero[i].PriceNarrationText = component;
                        break;
                    case ("HeroPumpingPoints"):
                        hero[i].PumpingPointsText = component;
                        break;
                    case ("TimeToResurraction"):
                        hero[i].TimeToResurractionText = component;
                        break;
                }
            }

            Button[] buttonComponents = HeroObject[i].GetComponentsInChildren<Button>(includeInactive: true);
            foreach (Button component in buttonComponents)
            {
                switch (component.name)
                {
                    case ("BuyHero_Button"):
                        hero[i].BuyHeroButton = component;
                        break;
                    case ("SelectHero_Button"):
                        hero[i].SelectHeroButton = component;
                        break;
                    case ("ChangeSizeWindow_Button"):
                        hero[i].WindowSwipeButton = component;
                        break;
                    case ("AddStrengthButton"):
                        hero[i].StrengthPumpingPointButton = component;
                        break;
                    case ("AddDexterityButton"):
                        hero[i].DexterityPumpingPointButton = component;
                        break;
                    case ("AddIntellectButton"):
                        hero[i].IntellectPumpingPointButton = component;
                        break;
                }
            }

            Image[] imageComponents = HeroObject[i].GetComponentsInChildren<Image>(includeInactive: true);
            foreach (Image component in imageComponents)
            {
                switch (component.name)
                {
                    case ("Lock"):
                        hero[i].LockImage = component;
                        break;
                    case ("Background_HeroWindow"):
                        hero[i].HeroBackgroundHighLight = component;
                        break;
                    case ("Сharacteristics"):
                        hero[i].СharacteristicsWindow = component;
                        break;
                    case ("Arrow"):
                        hero[i].WindowArrow = component;
                        break;
                    case ("MaxHealth_Arrow"):
                        hero[i].MaxHealth_ButtonImage = component;
                        hero[i].ButtonBackground_MaxHealth = FindStatButton(component.gameObject);
                        break;
                    case ("MaxStamina_Arrow"):
                        hero[i].MaxStamina_ButtonImage = component;
                        hero[i].ButtonBackground_MaxStamina = FindStatButton(component.gameObject);
                        break;
                    case ("MaxWill_Arrow"):
                        hero[i].MaxWill_ButtonImage = component;
                        hero[i].ButtonBackground_MaxWill = FindStatButton(component.gameObject);
                        break;
                    case ("HealthRegeneration_Arrow"):
                        hero[i].HealthRegeneration_ButtonImage = component;
                        hero[i].ButtonBackground_HealthRegeneration = FindStatButton(component.gameObject);
                        break;
                    case ("StaminaRegeneration_Arrow"):
                        hero[i].StaminaRegeneration_ButtonImage = component;
                        hero[i].ButtonBackground_StaminaRegeneration = FindStatButton(component.gameObject);
                        break;
                    case ("WillRegeneration_Arrow"):
                        hero[i].WillRegeneration_ButtonImage = component;
                        hero[i].ButtonBackground_WillRegeneration = FindStatButton(component.gameObject);
                        break;
                    case ("Damage_Arrow"):
                        hero[i].Damage_ButtonImage = component;
                        hero[i].ButtonBackground_Damage = FindStatButton(component.gameObject);
                        break;
                    case ("Armor_Arrow"):
                        hero[i].Armor_ButtonImage = component;
                        hero[i].ButtonBackground_Armor = FindStatButton(component.gameObject);
                        break;
                    case ("Evasion_Arrow"):
                        hero[i].Evasion_ButtonImage = component;
                        hero[i].ButtonBackground_Evasion = FindStatButton(component.gameObject);
                        break;
                    case ("Strength_Arrow"):
                        hero[i].Strength_ButtonImage = component;
                        hero[i].ButtonBackground_Strength = FindStatButton(component.gameObject);
                        break;
                    case ("Dexterity_Arrow"):
                        hero[i].Dexterity_ButtonImage = component;
                        hero[i].ButtonBackground_Dexterity = FindStatButton(component.gameObject);
                        break; 
                    case ("Intellect_Arrow"):
                        hero[i].Intellect_ButtonImage = component;
                        hero[i].ButtonBackground_Intellect = FindStatButton(component.gameObject);
                        break;
                    case ("DeadImage"):
                        hero[i].DeadImage = component;
                        break;
                }
            }

            static Image FindStatButton(GameObject image_component)
            {
                Image[] arr = image_component.GetComponentsInParent<Image>(includeInactive: true);
                foreach (Image curr in arr)
                {
                    switch (curr.name)
                    {
                        case ("DiscloseMoreValues_Button"):
                            return curr.GetComponent<Image>();
                    }
                }
                return null;
            }
        }
    }

    private void SetConstantValuesForHeroes()
    {
        int k;

        k = 0;
        hero[k].CostStaminaToAttackBase = 5;
        hero[k].TimeToResurration = 60;
        hero[k].PriceHero = 0;
        hero[k].NeedLizards = 0;
        hero[k].NameText.text = "Третьяк Викторович";

        k = 1;
        hero[k].CostStaminaToAttackBase = 6;
        hero[k].TimeToResurration = 150;
        hero[k].PriceHero = 6000;
        hero[k].NeedLizards = 25;
        hero[k].NameText.text = "Ярополк Скорострельный";

        k = 2;
        hero[k].CostStaminaToAttackBase = 8;
        hero[k].TimeToResurration = 300;
        hero[k].PriceHero = 12000;
        hero[k].NeedLizards = 60;
        hero[k].NameText.text = "Доброслав Никитич";

        k = 3;
        hero[k].CostStaminaToAttackBase = 10;
        hero[k].TimeToResurration = 480;
        hero[k].PriceHero = 23000;
        hero[k].NeedLizards = 110;
        hero[k].NameText.text = "Берислав Широчайший";

        k = 4;
        hero[k].CostStaminaToAttackBase = 15;
        hero[k].TimeToResurration = 600;
        hero[k].PriceHero = 50000;
        hero[k].NeedLizards = 190;
        hero[k].NameText.text = "Радогор Гигачадович";
    }

    public void SetStartValuesFromHeroes()
    {
        #region Hero_1
        int k = 0;
        hero[k].MaxHealthBase = 100;
        hero[k].MaxStaminaBase = 25;
        hero[k].MaxWillBase = 15;
        hero[k].DamageBase = 30;
        hero[k].ArmorBase = 5;
        hero[k].EvasionBase = 0.01f;
        hero[k].StrengthBase = 25;
        hero[k].DexterityBase = 15;
        hero[k].IntellectBase = 10;
        hero[k].EXPForNextLevel = 1500;
        hero[k].CritChanceBase = 0.02f;
        hero[k].CritDamageBase = 1.1f;

        hero[k].HealthRegenerationBase = 0.5f;
        hero[k].StaminaRegenerationBase = 0.35f;
        hero[k].WillRegenerationBase = 0.1f;

        hero[k].Bought = true;
        #endregion

        #region Hero_2
        k = 1;
        hero[k].MaxHealthBase = 200;
        hero[k].MaxStaminaBase = 50;
        hero[k].MaxWillBase = 30;
        hero[k].DamageBase = 8;
        hero[k].ArmorBase = 10;
        hero[k].EvasionBase = 0.02f;
        hero[k].StrengthBase = 50;
        hero[k].DexterityBase = 30;
        hero[k].IntellectBase = 50;
        hero[k].EXPForNextLevel = 2000;
        hero[k].CritChanceBase = 0.04f;
        hero[k].CritDamageBase = 1.25f;

        hero[k].HealthRegenerationBase = 0.8f;
        hero[k].StaminaRegenerationBase = 0.5f;
        hero[k].WillRegenerationBase = 0.2f;

        hero[k].Bought = false;
        #endregion

        #region Hero_3
        k = 2;
        hero[k].MaxHealthBase = 500;
        hero[k].MaxStaminaBase = 100;
        hero[k].MaxWillBase = 50;
        hero[k].DamageBase = 25;
        hero[k].ArmorBase = 20;
        hero[k].EvasionBase = 0.05f;
        hero[k].StrengthBase = 85;
        hero[k].DexterityBase = 75;
        hero[k].IntellectBase = 60;
        hero[k].EXPForNextLevel = 3000;
        hero[k].CritChanceBase = 0.06f;
        hero[k].CritDamageBase = 1.45f;

        hero[k].HealthRegenerationBase = 1.9f;
        hero[k].StaminaRegenerationBase = 0.8f;
        hero[k].WillRegenerationBase = 0.7f;

        hero[k].Bought = false;
        #endregion

        #region Hero_4
        k = 3;
        hero[k].MaxHealthBase = 1000;
        hero[k].MaxStaminaBase = 145;
        hero[k].MaxWillBase = 135;
        hero[k].DamageBase = 50;
        hero[k].ArmorBase = 30;
        hero[k].EvasionBase = 0.08f;
        hero[k].StrengthBase = 140;
        hero[k].DexterityBase = 130;
        hero[k].IntellectBase = 110;
        hero[k].EXPForNextLevel = 4000;
        hero[k].CritChanceBase = 0.08f;
        hero[k].CritDamageBase = 1.65f;

        hero[k].HealthRegenerationBase = 3.5f;
        hero[k].StaminaRegenerationBase = 2.35f;
        hero[k].WillRegenerationBase = 1.1f;

        hero[k].Bought = false;
        #endregion

        #region Hero_5
        k = 4;
        hero[k].MaxHealthBase = 2000;
        hero[k].MaxStaminaBase = 235;
        hero[k].MaxWillBase = 185;
        hero[k].DamageBase = 100;
        hero[k].ArmorBase = 45;
        hero[k].EvasionBase = 0.1f;
        hero[k].StrengthBase = 255;
        hero[k].DexterityBase = 260;
        hero[k].IntellectBase = 240;
        hero[k].EXPForNextLevel = 5000;
        hero[k].CritChanceBase = 0.1f;
        hero[k].CritDamageBase = 1.9f;

        hero[k].HealthRegenerationBase = 5.5f;
        hero[k].StaminaRegenerationBase = 4.35f;
        hero[k].WillRegenerationBase = 2.8f;

        hero[k].Bought = false;
        #endregion

        for (int i = 0; i < HeroCount; i++)
        {
            hero[i].Level = 0;
            hero[i].ActualEXP = 0;
            hero[i].IsAlive = true;
            hero[i].SetActualStocksToMAX();
        }
    }
    #endregion

    #region ButtonEvenets
    public void SwitchHeroesWindow()
    {
        if (ChooseHero_Flag == false)
        { ChooseHero_Flag = !ChooseHero_Flag; }

        HeroesWindow.SetActive(ChooseHero_Flag);
        WeaponWindow.SetActive(false);
        ArmorWindow.SetActive(false);
    }

    public void SwitchArmorWindow()
    {
        if (ChooseArmor_Flag == false)
        { ChooseArmor_Flag = !ChooseArmor_Flag; }

        ArmorWindow.SetActive(ChooseArmor_Flag);
        WeaponWindow.SetActive(false);
        HeroesWindow.SetActive(false);
    }

    public void SwitchWeaponWindow()
    {
        if (ChooseWeapon_Flag == false)
        { ChooseWeapon_Flag = !ChooseWeapon_Flag; }

        WeaponWindow.SetActive(ChooseWeapon_Flag);
        ArmorWindow.SetActive(false);
        HeroesWindow.SetActive(false);
    }
    #endregion

    #region DelegateMethods
    public void BuyHero(int heroIndex)
    {
        if (Facilities.CheckFaithCurrencyAmount(hero[heroIndex].PriceHero) &&
            Achievements_R_vs_L.AccumulatedKills >= hero[heroIndex].NeedLizards)
        {
            PlayAudioEffect();
            ChooseHero(heroIndex);

            Facilities.FaithCurrencyPay(Heroes.hero[heroIndex].PriceHero);
            Heroes.hero[heroIndex].Bought = true;
            Heroes.hero[heroIndex].RedactHeroCardToUnlockState();

            if (Achievements_R_vs_L.HeroIsUnlocked[heroIndex] == false)
            {
                Achievements_R_vs_L.HeroIsUnlocked[heroIndex] = true;
                Achievements_R_vs_L.AccumulateHeroes();
            }
        }

        void PlayAudioEffect()
        {
            AudioEffects.PlayOneShotEffect(Heroes.PrayerAfterPurchase);
            switch (heroIndex)
            {
                case 1:
                    AudioEffects.PlayOneShotEffect(Heroes.AudioMessage_hero_2);
                    break;
                case 2:
                    AudioEffects.PlayOneShotEffect(Heroes.AudioMessage_hero_3);
                    break;
                case 3:
                    AudioEffects.PlayOneShotEffect(Heroes.AudioMessage_hero_4);
                    break;
                case 4:
                    AudioEffects.PlayOneShotEffect(Heroes.AudioMessage_hero_5);
                    break;
            }
        }
    }

    public void ChooseHero(int hero_index)
    {
        if (hero[hero_index].IsAlive || CurrentHeroIndex == hero_index)
        {
            if (Battle.BattleIsActive == false)
            {
                if (CurrentHero != null)
                {
                    ResetColor();
                    Heroes.CurrentHero.ResetWeaponStats();
                    Heroes.CurrentHero.ResetArmorStats();
                    Heroes.CurrentHero.UpdateAllText();
                }

                SelectHero();
                SetColor();
                SetImage();
                SetName();
                BattleHero.ComplexUpdateEXP();
                BattleHero.CheckAndChangeHeroDeadDisplay();

                Weapons.SetWeaponStatsToHero();

                if (ArmorMark_1.Selected)
                    ArmorMark_1.SetCharacteriscticsValues();
                else if (ArmorMark_2.Selected)
                    ArmorMark_2.SetCharacteriscticsValues();
                else if (ArmorMark_3.Selected)
                    ArmorMark_3.SetCharacteriscticsValues();

                ListOfEffects.DestroyBuffEffects();
                Items.TimerText.gameObject.SetActive(false);

                if (Items.Timer[CurrentHero.Index] > 0)
                {
                    StartCoroutine(Items.BuffAndCountDown());
                }

                if (ListOfEffects.CachedBuffTime_PrayerToTheGods[hero_index] != 0)
                {
                    float buffTime = ListOfEffects.CachedBuffTime_PrayerToTheGods[hero_index];

                    ListOfEffects.CreateBuffEffect((int)BuffEnum.Damage, buffTime);
                    ListOfEffects.CreateBuffEffect((int)BuffEnum.Health, buffTime);
                    ListOfEffects.CreateBuffEffect((int)BuffEnum.Armor, buffTime);
                    StartCoroutine(ResetStats());

                    IEnumerator ResetStats()
                    {
                        yield return new WaitForSeconds(buffTime);
                        ListOfEffects.CachedBuffTime_PrayerToTheGods[hero_index] = 0;
                        Heroes.hero[hero_index].DamageBase -= ListOfEffects.CachedAdditiveStats_PrayerToTheGods[hero_index, 0];
                        Heroes.hero[hero_index].MaxHealthBase -= ListOfEffects.CachedAdditiveStats_PrayerToTheGods[hero_index, 1];
                        Heroes.hero[hero_index].ArmorBase -= ListOfEffects.CachedAdditiveStats_PrayerToTheGods[hero_index, 2];
                    }
                }

                if (ListOfEffects.CachedBuffTime_GeneralFee[hero_index] != 0)
                {
                    float buffTime = ListOfEffects.CachedBuffTime_GeneralFee[hero_index];

                    ListOfEffects.CreateBuffEffect((int)BuffEnum.Attributes, buffTime);
                    StartCoroutine(ResetStats());

                    IEnumerator ResetStats()
                    {
                        yield return new WaitForSeconds(buffTime);
                        ListOfEffects.CachedBuffTime_GeneralFee[hero_index] = 0;
                        Heroes.hero[hero_index].StrengthBase -= ListOfEffects.CachedAdditiveStats_GeneralFee[hero_index, 0];
                        Heroes.hero[hero_index].DexterityBase -= ListOfEffects.CachedAdditiveStats_GeneralFee[hero_index, 1];
                        Heroes.hero[hero_index].IntellectBase -= ListOfEffects.CachedAdditiveStats_GeneralFee[hero_index, 2];
                    }
                }
            }            
        }

        void ResetColor()
        {
            CurrentHero.HeroBackgroundHighLight.color = HighlightColor_Deselect;
        }

        void SelectHero()
        {
            CurrentHero = hero[hero_index];
            CurrentHeroIndex = hero_index;
        }

        void SetColor()
        {
            CurrentHero.HeroBackgroundHighLight.color = HighlightColor_Select;
        }

        void SetImage()
        {
            BattleHero.GetHeroImage().sprite = CurrentHero.SelectHeroButton.image.sprite;
        }

        void SetName()
        {
            BattleHero.HeroNameText.text = CurrentHero.NameText.text;
        }
    }

    public void AttackEnemy()
    {
        if (CurrentHero.ActualStamina >= CurrentHero.CostStaminaToAttack)
        {
            float damageFromMaxHealth = 0;

            if (Weapons.CurrentWeapon != null)
            {
                float bleedingChance = UnityEngine.Random.Range(0f, 1f);
                float stunChance = UnityEngine.Random.Range(0f, 1f);

                if (Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.ChanceToBleeding] > bleedingChance)
                {
                    ListOfEffects.CreateDebuffEffect((int)ListOfEffects.DebuffEnum.Bleeding,
                        Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.BleedingDuration]);

                    StartCoroutine(Battle.PeriodicDamage(Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.BleedingDamage],
                        Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.BleedingDuration]));
                }

                else if (Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.ChanceToStun] > stunChance)
                {
                    if (EnemiesSystem.enemy.ReceivedStun <= 0)
                    {
                        ListOfEffects.CreateDebuffEffect((int)ListOfEffects.DebuffEnum.Stun,
                            Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.StunDuration]);
                    }
                    else
                    {
                        SuperimprosedEffects effect = ListOfEffects.GetDebuffBuffer()[(int)ListOfEffects.DebuffEnum.Stun].GetComponent<SuperimprosedEffects>();
                        effect.Duration = Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.StunDuration];
                    }

                    EnemiesSystem.enemy.ReceivedStun = Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.StunDuration];
                }

                else if (Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.PercentFromEnemyMaxHealthToDamage] > 0)
                {
                    damageFromMaxHealth = EnemiesSystem.enemy.MaxHealth *
                        Weapons.CurrentWeapon.StatsValue[(int)Weapons.StatsIndex.PercentFromEnemyMaxHealthToDamage];
                }
            }

            CurrentHero.ActualStamina -= CurrentHero.CostStaminaToAttack;
            Battle.ProbabilityOfDifferentVersionsOfAttack((CurrentHero.Damage + damageFromMaxHealth), (int)Battle.EntityType.Hero);
        }
        else BattleHero.StaminaSliderAnimator.SetTrigger("Active");
    }
    #endregion

    #region QualityItems
    public void SetQuality(Image headerImage, TextMeshProUGUI qualityText, int summUpgradeCount, int maxUpgradeCountForCurrentWeapon)
    {
        string qualityName = "";
        Color qualityColor = Color.white;
        float summ = ((1 + ((float)summUpgradeCount / maxUpgradeCountForCurrentWeapon)) - 1);

        if (summ >= 0 && summ < 0.20f)
        {
            qualityName = "обычное";
            qualityColor = QualityColor[(int)QualityColorEnum.Average];
        }
        else if (summ >= 0.2f && summ < 0.45f)
        {
            qualityName = "необычное";
            qualityColor = QualityColor[(int)QualityColorEnum.Unusual];
        }
        else if (summ >= 0.45f && summ < 0.7f)
        {
            qualityName = "редкое";
            qualityColor = QualityColor[(int)QualityColorEnum.Rare];
        }
        else if (summ >= 0.7f && summ < 0.95f)
        {
            qualityName = "эпическое";
            qualityColor = QualityColor[(int)QualityColorEnum.Epic];
        }
        else if (summ >= 0.95f)
        {
            qualityName = "легендарное";
            qualityColor = QualityColor[(int)QualityColorEnum.Legendary];
        }

        headerImage.color = qualityColor;
        qualityText.color = qualityColor;
        qualityText.text = $"Качество: {qualityName}";
    }
    #endregion
}