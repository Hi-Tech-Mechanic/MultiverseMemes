using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using Random = UnityEngine.Random;

public class BattleHero : DataStructure
{
    [SerializeField] private Image _heroImage;
    [SerializeField] private Image _deadImage;
    [SerializeField] private GameObject _heroObject;
    [SerializeField] private TextMeshProUGUI _timeToResurrationText;
    [SerializeField] private Animator _animator;
    public GameObject[] CrossObject;

    [Header("Статы богатыря")]
    [Space(10)]
    [SerializeField] private GameObject DamageIcon;
    [SerializeField] private GameObject ArmorIcon;
    [SerializeField] private GameObject EvasionIcon;
    [SerializeField] private GameObject CritChanceIcon;
    [SerializeField] private GameObject CritDamageIcon;

    public Animator StaminaSliderAnimator;
    public Animator WillSliderAnimator;
    public Animator[] SkillsAnimator;
    [Tooltip("Получение урона")] public AudioClip[] GetDamageClip;
    [Tooltip("Нанесение урона")] public AudioClip[] MakeDamageClip;
    [Tooltip("Насмешка")] public AudioClip[] MockeryClip;
    [Tooltip("Звук смерти")] public AudioClip[] DeadClip;

    [NonSerialized] public Transform HeroTransform;
    private const int _averageGetDamageCount = 3;
    private const int _averageGetHigthDamageCount = 1;

    private Slider _healthBar;
    private Slider _staminaBar;
    private Slider _willBar;
    private Slider _experienceBar;

    [NonSerialized] public TextMeshProUGUI HeroNameText;
    private TextMeshProUGUI _healthValue;
    private TextMeshProUGUI _healthPercentValue;
    private TextMeshProUGUI _healthRegenerationValue;
    private TextMeshProUGUI _staminaValue;
    private TextMeshProUGUI _staminaPercentValue;
    private TextMeshProUGUI _staminaRegenerationValue;
    private TextMeshProUGUI _willPercentValue;
    private TextMeshProUGUI _willValue;
    private TextMeshProUGUI _willRegenerationValue;
    private TextMeshProUGUI _experiencePercentValueText;
    private TextMeshProUGUI _experienceValueText;
    private TextMeshProUGUI _levelText;

    private TextMeshProUGUI _damageText;
    private TextMeshProUGUI _armorText;
    private TextMeshProUGUI _evasionText;
    private TextMeshProUGUI _critChanceText;
    private TextMeshProUGUI _critDamageText;

    private void Awake()
    {
        HeroTransform = _heroImage.transform;
        FindComponents();
    }

    private void Update()
    {
        if (Heroes.CurrentHero.IsAlive)
        {
            ComplexUpdateHealth();
            ComplexUpdateStamina();
            ComplexUpdateWill();
            UpdateCharacteristicsText();
        }
    }

    private void FindComponents()
    {
        RectTransform[] HeroComponents = _heroObject.GetComponentsInChildren<RectTransform>(includeInactive: true);
        foreach (RectTransform component in HeroComponents)
        {
            switch (component.name)
            {
                case ("HealthBar"):
                    _healthBar = component.GetComponent<Slider>();
                    break;
                case ("StaminaBar"):
                    _staminaBar = component.GetComponent<Slider>();
                    break;
                case ("WillBar"):
                    _willBar = component.GetComponent<Slider>();
                    break;
                case ("ExperienceBar"):
                    _experienceBar = component.GetComponent<Slider>();
                    break;

                case ("HealthValue"):
                    _healthValue = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("HealthPercentValue"):
                    _healthPercentValue = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("HealthRegenerationValue"):
                    _healthRegenerationValue = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("StaminaValue"):
                    _staminaValue = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("StaminaPercentValue"):
                    _staminaPercentValue = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("StaminaRegenerationValue"):
                    _staminaRegenerationValue = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("WillValue"):
                    _willValue = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("WillPercentValue"):
                    _willPercentValue = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("WillRegenerationValue"):
                    _willRegenerationValue = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("ExperienceValue"):
                    _experienceValueText = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("ExperiencePercentValue"):
                    _experiencePercentValueText = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("HeroLevel"):
                    _levelText = component.GetComponent<TextMeshProUGUI>();
                    break;
                case ("HeroName"):
                    HeroNameText = component.GetComponent<TextMeshProUGUI>();
                    break;                    
                default: break;
            }
        }

        _damageText = DamageIcon.GetComponentInChildren<TextMeshProUGUI>();
        _armorText = ArmorIcon.GetComponentInChildren<TextMeshProUGUI>();
        _evasionText = EvasionIcon.GetComponentInChildren<TextMeshProUGUI>();
        _critChanceText = CritChanceIcon.GetComponentInChildren<TextMeshProUGUI>();
        _critDamageText = CritDamageIcon.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void UpdateCharacteristicsText()
    {
        _damageText.text = $"{ValuesRounding.FormattingValue("", "", Heroes.CurrentHero.Damage)}";
        _armorText.text = $"{Math.Round(Heroes.CurrentHero.Armor, 1)}";
        _evasionText.text = $"{Math.Round(Heroes.CurrentHero.Evasion * 100, 2)}%";
        _critChanceText.text = $"{Math.Round(Heroes.CurrentHero.CritChance * 100, 2)}%";
        _critDamageText.text = $"{Math.Round(Heroes.CurrentHero.CritDamage * 100)}%";
    }

    public void UpdateAllInfo()
    {
        ComplexUpdateHealth();
        ComplexUpdateStamina();
        ComplexUpdateWill();
        ComplexUpdateEXP();
        UpdateCharacteristicsText();
    }

    public void ComplexUpdateHealth()
    {
        UpdateText_Health();
        UpdateSliderValue_Health();
        UpdateText_PercentHealth();
        HealthRegeneration();
        UpdateText_HealthRegeneration();

        void UpdateText_Health()
        {
            _healthValue.text = $"{ValuesRounding.FormattingValue("", "", Heroes.CurrentHero.ActualHealth)}/" +
                $"{ValuesRounding.FormattingValue("", "", Heroes.CurrentHero.MaxHealth)}";
        }

        void UpdateSliderValue_Health()
        {
            _healthBar.value = (Heroes.CurrentHero.ActualHealth / Heroes.CurrentHero.MaxHealth); 
        }

        void UpdateText_PercentHealth()
        {
            double expression = Math.Round(((Heroes.CurrentHero.ActualHealth / Heroes.CurrentHero.MaxHealth) * 100), 1);
            _healthPercentValue.text = $"({expression}%)";
        }

        void UpdateText_HealthRegeneration()
        {
            _healthRegenerationValue.text = $"{ValuesRounding.UltraAccuracyFormattingValue("", "", Heroes.CurrentHero.HealthRegeneration)}/сек.";
        }

        void HealthRegeneration()
        {
            if (Heroes.CurrentHero.ActualHealth < Heroes.CurrentHero.MaxHealth)
                Heroes.CurrentHero.ActualHealth += Heroes.CurrentHero.HealthRegeneration / (1 / Time.deltaTime);

            else if (Heroes.CurrentHero.ActualHealth > Heroes.CurrentHero.MaxHealth)
                Heroes.CurrentHero.ActualHealth = Heroes.CurrentHero.MaxHealth;
        }
    }

    public void ComplexUpdateStamina()
    {
        UpdateText_Stamina();
        UpdateSliderValue_Stamina();
        UpdateText_PercentStamina();
        StaminaRegeneration();
        UpdateText_StaminaRegeneration();

        void UpdateText_Stamina()
        {
            _staminaValue.text = $"{ValuesRounding.FormattingValue("", "", Heroes.CurrentHero.ActualStamina)}/" +
                $"{ValuesRounding.FormattingValue("", "", Heroes.CurrentHero.MaxStamina)}";
        }

        void UpdateSliderValue_Stamina()
        { _staminaBar.value = (Heroes.CurrentHero.ActualStamina / Heroes.CurrentHero.MaxStamina); }

        void UpdateText_PercentStamina()
        {
            double expression = Math.Round(((Heroes.CurrentHero.ActualStamina / Heroes.CurrentHero.MaxStamina) * 100), 1);
            _staminaPercentValue.text = $"({expression}%)";
        }

        void UpdateText_StaminaRegeneration()
        {
            _staminaRegenerationValue.text = $"{ValuesRounding.UltraAccuracyFormattingValue("", "", Heroes.CurrentHero.StaminaRegeneration)}/сек.";
        }

        void StaminaRegeneration()
        {
            if (Heroes.CurrentHero.ActualStamina < Heroes.CurrentHero.MaxStamina)
                Heroes.CurrentHero.ActualStamina += Heroes.CurrentHero.StaminaRegeneration / (1 / Time.deltaTime);

            else if (Heroes.CurrentHero.ActualStamina > Heroes.CurrentHero.MaxStamina)
                Heroes.CurrentHero.ActualStamina = Heroes.CurrentHero.MaxStamina;
        }
    }

    public void ComplexUpdateWill()
    {
        UpdateText_Will();
        UpdateSliderValue_Will();
        UpdateText_PercentWill();
        WillRegeneration();
        UpdateText_WillRegeneration();

        void UpdateText_Will()
        {
            _willValue.text = $"{ValuesRounding.FormattingValue("", "", Heroes.CurrentHero.ActualWill)}/" +
                $"{ValuesRounding.FormattingValue("", "", Heroes.CurrentHero.MaxWill)}";
        }

        void UpdateSliderValue_Will()
        { 
            _willBar.value = (Heroes.CurrentHero.ActualWill / Heroes.CurrentHero.MaxWill);
        }

        void UpdateText_PercentWill()
        {
            double expression = Math.Round(((Heroes.CurrentHero.ActualWill / Heroes.CurrentHero.MaxWill) * 100), 1);
            _willPercentValue.text = $"({expression}%)";
        }

        void UpdateText_WillRegeneration()
        {
            _willRegenerationValue.text = $"{ValuesRounding.UltraAccuracyFormattingValue("", "", Heroes.CurrentHero.WillRegeneration)}/сек.";
        }

        void WillRegeneration()
        {
            if (Heroes.CurrentHero.ActualWill < Heroes.CurrentHero.MaxWill)
                Heroes.CurrentHero.ActualWill += Heroes.CurrentHero.WillRegeneration / (1 / Time.deltaTime);

            else if (Heroes.CurrentHero.ActualWill > Heroes.CurrentHero.MaxWill)
                Heroes.CurrentHero.ActualWill = Heroes.CurrentHero.MaxWill;
        }
    }

    public void ComplexUpdateEXP()
    {
        UpdateText_EXP();
        UpdateSliderValue_EXP();
        UpdateText_PercentEXP();
        UpdateLevelText();

        void UpdateText_EXP()
        {
            _experienceValueText.text = $"{ValuesRounding.FormattingValue("", "", Heroes.CurrentHero.ActualEXP)}/" +
                $"{ValuesRounding.FormattingValue("", "", Heroes.CurrentHero.EXPForNextLevel)}";
        }

        void UpdateSliderValue_EXP()
        {
            _experienceBar.value = (Heroes.Heroes.CurrentHero.ActualEXP / Heroes.CurrentHero.EXPForNextLevel); 
        }

        void UpdateText_PercentEXP()
        {
            double expression = Math.Round(((Heroes.Heroes.CurrentHero.ActualEXP / Heroes.CurrentHero.EXPForNextLevel) * 100), 1);
            _experiencePercentValueText.text = $"({expression}%)";
        }

        void UpdateLevelText()
        {
            _levelText.text = $"Уровень: {Heroes.CurrentHero.Level}";
        }
    }

    public void HeroIsDead()
    {
        Heroes.CurrentHero.IsAlive = false;

        if (Heroes.CurrentHero.RemainingTimeToResurrationHero == 0)
        { 
            Battle.CurrentStage--;
            AudioEffects.PlayEntitiesAudioEffects(EnemiesSystem.MockeryClip, "PlayOneShot");
            AudioEffects.PlayEntitiesAudioEffects(DeadClip, "PlayOneShot");
            DeadQuestion.OpenQuestionWindow();
        }

        CheckAndChangeHeroDeadDisplay();
        StartCoroutine(Heroes.CurrentHero.DeadCountingDown());
        StartCoroutine(DeadCountingDown());
        StartCoroutine(AnimateDeadImage());
        Battle.SwitchBattleMode("Deactive");
        DumpStocks();

        IEnumerator DeadCountingDown()
        {
            while (Heroes.CurrentHero.RemainingTimeToResurrationHero > 0)
            {
                _timeToResurrationText.text = $"{Math.Round(Heroes.CurrentHero.RemainingTimeToResurrationHero)} сек.";
                yield return new WaitForSeconds(1);
            }
            HeroIsResurrected();
        }

        IEnumerator AnimateDeadImage()
        {
            float time = 0.5f;
            float timer = 0;
            Vector2 scale = new (5, 5);
            Vector2 unscale = new (1, 1);

            while (timer < time)
            {
                timer += Time.deltaTime;
                _deadImage.rectTransform.localScale = Vector2.Lerp(scale, unscale, timer / time);
                yield return null;
            }
            _deadImage.rectTransform.localScale = unscale;
        }

        void DumpStocks()
        {
            Heroes.CurrentHero.ActualHealth = 0;
            Heroes.CurrentHero.ActualStamina = 0;
            Heroes.CurrentHero.ActualWill = 0;

            ComplexUpdateHealth();
            ComplexUpdateStamina();
            ComplexUpdateWill();
        }
    }

    public void HeroIsResurrected()
    {
        CheckAndChangeHeroDeadDisplay();
        ComplexUpdateHealth();
        ComplexUpdateStamina();
        ComplexUpdateWill();
        DeadQuestion.CloseQuestionWindow();
    }

    public void GetDamageAnimation(float damage)
    {
        int anim_idex;

        if (damage > (Heroes.CurrentHero.MaxHealth / 2))
        {
            for (int i = 0; i < _averageGetHigthDamageCount; i++)
                _animator.ResetTrigger($"GetHigthDamage_{i}");
            anim_idex = Random.Range(0, _averageGetHigthDamageCount);
            _animator.SetTrigger($"GetHigthDamage_{anim_idex}");
        }
        else
        {
            for (int i = 0; i < _averageGetDamageCount; i++)
                _animator.ResetTrigger($"GetDamage_{i}");
            anim_idex = Random.Range(0, _averageGetDamageCount);
            _animator.SetTrigger($"GetDamage_{anim_idex}");
        }
    }

    public void CheckAndChangeHeroDeadDisplay()
    {
        if (Heroes.CurrentHero.IsAlive)
        {
            SwitchActivateTimer(false);
            ChangeHeroColor(Heroes.White);
            SwitchDeadImage(false);
        }
        else
        {
            SwitchActivateTimer(true);
            ChangeHeroColor(Heroes.Red);
            SwitchDeadImage(true);
        }
    }

    public Image GetHeroImage()
    {
        return _heroImage;
    }

    private void ChangeHeroColor(Color color)
    { 
        _heroImage.color = color;
    }

    private void SwitchActivateTimer(bool state)
    { 
        _timeToResurrationText.gameObject.SetActive(state);
    }

    private void SwitchDeadImage(bool state)
    {
        _deadImage.gameObject.SetActive(state);
    }
}