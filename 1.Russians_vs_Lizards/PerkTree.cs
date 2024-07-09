using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Heroes;

public class PerkTree : DataStructure
{
    public static readonly int PerkCount = 15;

    [SerializeField] private GameObject PerkWindow;
    [SerializeField] private Animator InfoWindow;
    [SerializeField] private TextMeshProUGUI PerkNameText;
    [SerializeField] private TextMeshProUGUI PerkLevelText;
    [SerializeField] private TextMeshProUGUI PerkNarrationText;
    [SerializeField] private TextMeshProUGUI ValueGainedText;
    [SerializeField] private TextMeshProUGUI PerkCostText;
    [SerializeField] private Button UpgradeButton;
    [SerializeField] private Image PerkImageInInformationWindow;
    [SerializeField] private Image PerkBackgroundInInformationWindow;
    [SerializeField] private Color GreenPrice;
    [SerializeField] private Color YellowPrice;
    [SerializeField] private Color GreyPrice;
    [SerializeField] private Color StrengthCompoundColor;
    [SerializeField] private Color DexterityCompoundColor;
    [SerializeField] private Color IntellectCompoundColor;
    [SerializeField] private Color StrengthBackgroundColor;
    [SerializeField] private Color DexterityBackgroundColor;
    [SerializeField] private Color IntellectBackgroundColor;
    [SerializeField] private Color StrengthImageColor;
    [SerializeField] private Color DexterityImageColor;
    [SerializeField] private Color IntellectImageColor;
    [SerializeField] private Sprite Strength_Sprite;
    [SerializeField] private Sprite MaxHealthFromStrength_Sprite;
    [SerializeField] private Sprite HealthRegenerationFromStrength_Sprite;
    [SerializeField] private Sprite Dexterity_Sprite;
    [SerializeField] private Sprite MaxStaminaFromDexterity_Sprite;
    [SerializeField] private Sprite StaminaRegenerationFromDexterity_Sprite;
    [SerializeField] private Sprite Intellect_Sprite;
    [SerializeField] private Sprite MaxWillFromIntellect_Sprite;
    [SerializeField] private Sprite WillRegenerationFromIntellect_Sprite;

    [SerializeField] private Image[] Strength_CompoundLayer_1;
    [SerializeField] private Image[] Dexterity_CompoundLayer_1;
    [SerializeField] private Image[] Intellect_CompoundLayer_1;

    public enum Stat
    {
        Strength = 0,
        Dexterity = 1,
        Intellect = 2
    }

    private class PerkMessages
    {
        public readonly string BaseStrength = "Ќавык силушки, добавка перва€";
        public readonly string ArithmeticStrength = "Ќавык силушки, добавка втора€";
        public readonly string GeometricStrength = "Ќавык силушки, добавка треть€";
        public readonly string HealthRegenerationFromStrength = "Ќавык регенераци€ здоровь€";
        public readonly string MaxHealthFromStrength = "Ќавык максимального здоровь€";

        public readonly string BaseDexterity = "Ќавык проворства, добавка перва€";
        public readonly string ArithmeticDexterity = "Ќавык проворства, добавка втора€";
        public readonly string GeometricDexterity = "Ќавык проворства, добавка треть€";
        public readonly string StaminaRegenerationFromDexterity = "Ќавык регенерации выносливости";
        public readonly string MaxStaminaFromDexterity = "Ќавык максимальной выносливости";

        public readonly string BaseIntellect = "Ќавык разума, добавка перва€";
        public readonly string ArithmeticIntellect = "Ќавык разума, добавка втора€";
        public readonly string GeometricIntellect = "Ќавык разума, добавка треть€";
        public readonly string WillRegenerationFromIntellect = "Ќавык регенерации воли";
        public readonly string MaxWillFromIntellect = "Ќавык максимальной воли";

        public readonly string ToBaseStrength = "к базовой силушке";
        public readonly string ToBaseDexterity = "к базовому проворству";
        public readonly string ToBaseIntellect = "к базовому разуму";

        public readonly string ToStrength = "к силушке";
        public readonly string ToDexterity = "к проворству";
        public readonly string ToIntellect = "к разуму";

        public readonly string ToHealthRegenerationFromStrength = "к регенерации здоровь€ за единицу силушки";
        public readonly string ToStaminaRegenerationFromDexteiry = "к регенерации выносливости за единицу проворства";
        public readonly string ToWillRegenerationFromIntellect = "к регенерации воли за единицу разума";

        public readonly string ToMaxHealthFromStrength = "к максимальному здоровью за единицу силушки";
        public readonly string ToMaxStaminaFromDexterity = "к максимальной выносливости за единицу проворства";
        public readonly string ToMaxWillFromIntellect = "к максимальной воле за единицу разума";

        public readonly string Arithmetic = "(арифметически)";
        public readonly string Geometric = "(геометрически)";
    }
    readonly PerkMessages _perkMessage = new();

    #region NonSerialized

    public class PerkTemplate : DataStructure
    {
        public float Additive;
        public float Summ;
        public int MaxUpgradeCount = 15;
        public int CurrentUpgradeIndex
        {
            get { return _currentUpgradeIndex; }
            set { _currentUpgradeIndex = value; CheckMaxPurchase();  UpdateText_PurchasedPerkValue(); }
        }
        protected int _currentUpgradeIndex;

        public float[] Cost = { 20, 40, 65, 100, 140, 185, 240, 300, 370, 450, 540, 650, 760, 880, 1000 };
        public Text PurchasedPerkValueText;
        public Image MaxStateElement;
        public Image PerkBackground;
        public Image PerkImage;
        public Button PerkButton;
        public Image Compound;

        public void Start()
        {
            if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
                UpdateText_PurchasedPerkValue();
        }

        public void FindComponents(Component parentComponent)
        {
            Component[] Components;

            Components = parentComponent.GetComponentsInChildren<RectTransform>(includeInactive: true);
            foreach (Component component in Components)
            {
                switch (component.name)
                {
                    case ("PurchasedPerkValue"):
                        PurchasedPerkValueText = component.GetComponent<Text>();
                        break;
                    case ("MaxState"):
                        MaxStateElement = component.GetComponent<Image>();
                        break;
                    case ("Perk_Background"):
                        PerkBackground = component.GetComponent<Image>();
                        break;
                    case ("PerkImage"):
                        PerkImage = component.GetComponent<Image>();
                        break;
                    case ("Perk_Button"):
                        PerkButton = component.GetComponent<Button>();
                        break;
                    case ("Compound"):
                        Compound = component.GetComponent<Image>();
                        break;
                }
            }
        }

        public virtual void IncreaseMethod(int stat_index)
        {
            AudioEffects.PlayUpgradeEffect_2();
            Facilities.AncestralPowerPay(Cost[CurrentUpgradeIndex]);
            Add(stat_index);
            CurrentUpgradeIndex++;
            Achievements_R_vs_L.AccumulatePerks();

            for (int i = 0; i < Heroes.HeroCount; i++)
            {
                if (Heroes.hero[i].Bought == false)
                {
                    Heroes.hero[i].SetActualStocksToMAX();
                }
            }
        }

        public virtual void ChangePerkBackgroundColor()
        {
            if (PerkButton.interactable == true)
            {
                if (CurrentUpgradeIndex == MaxUpgradeCount)
                {
                    PerkBackground.color = PerkTree.GreyPrice;
                    return;
                }

                if (CurrentUpgradeIndex != MaxUpgradeCount)
                {
                    if (Cost[CurrentUpgradeIndex] < (Facilities.AncestralPower / 2))
                        PerkBackground.color = PerkTree.GreenPrice;
                    else if (Cost[CurrentUpgradeIndex] > (Facilities.AncestralPower / 2) && Cost[CurrentUpgradeIndex] < Facilities.AncestralPower)
                        PerkBackground.color = PerkTree.YellowPrice;
                    else if (Cost[CurrentUpgradeIndex] > Facilities.AncestralPower)
                        PerkBackground.color = PerkTree.GreyPrice;
                }
            }
        }

        protected virtual void Add(int stat_index)
        {
            switch (stat_index)
            {
                case ((int)Stat.Strength):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].StrengthBase += Additive;
                    break;
                case ((int)Stat.Dexterity):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].DexterityBase += Additive;
                    break;
                case ((int)Stat.Intellect):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].IntellectBase += Additive;
                    break;
            }
        }

        protected virtual void UpdateText_PurchasedPerkValue()
        {
            PurchasedPerkValueText.text = $"{CurrentUpgradeIndex}/{MaxUpgradeCount}";
        }

        protected virtual void CheckMaxPurchase()
        {
            if (CurrentUpgradeIndex == MaxUpgradeCount)
                MaxStateElement.gameObject.SetActive(true);
        }

        public void CheckPerkImages()
        {
            if (PerkButton.interactable == true)
                SwitchAlpha();

            void SwitchAlpha()
            {
                Color alpha_change;

                alpha_change = PerkImage.color;
                alpha_change.a = 1;
                PerkImage.color = alpha_change;
            }
        }

        public virtual void TemplateUpdateMethod(string status, TextMeshProUGUI level,
            TextMeshProUGUI valueGained, TextMeshProUGUI costText)
        {
            level.text = $"”ровень {CurrentUpgradeIndex}/{MaxUpgradeCount}";

            if (status == "percent") valueGained.text = $"+{Math.Round(Summ * 100, 2)}%";
            else valueGained.text = $"+{PerkTree.RoundValue(Summ)}";

            if (CurrentUpgradeIndex != MaxUpgradeCount) Facilities.DisplayCostText(costText, Cost[CurrentUpgradeIndex], "силы предков");
            else costText.text = $"ћакс. уровень";
        }

        public virtual void UpdateNarrationText(TextMeshProUGUI Narration, string postfix)
        {
            Narration.text = $"+{PerkTree.RoundValue(Additive)} {postfix}";
        }
    }

    public class BaseAdditional : PerkTemplate
    {
        public new int[] Additive = { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 };

        public override void IncreaseMethod(int stat_index)
        {
            if (CurrentUpgradeIndex < MaxUpgradeCount && Facilities.CheckAncestralPowerAmount(Cost[CurrentUpgradeIndex]))
            {
                Summ += Additive[CurrentUpgradeIndex];
                base.IncreaseMethod(stat_index);
            }
        }

        public override void UpdateNarrationText(TextMeshProUGUI Narration, string postfix)
        {
            float tmp;

            if (CurrentUpgradeIndex == MaxUpgradeCount) tmp = PerkTree.RoundValue(Additive[CurrentUpgradeIndex - 1]);
            else tmp = PerkTree.RoundValue(Additive[CurrentUpgradeIndex]);
            Narration.text = $"+{tmp} {postfix}";
        }

        public override void TemplateUpdateMethod(string status, TextMeshProUGUI level,
            TextMeshProUGUI valueGained, TextMeshProUGUI costText)
        {
            level.text = $"”ровень {CurrentUpgradeIndex}/{MaxUpgradeCount}";

            if (status == "percent") valueGained.text = $"+{Math.Round(Summ * 100, 2)}%";
            else valueGained.text = $"+{PerkTree.RoundValue(Summ)}";

            if (CurrentUpgradeIndex != MaxUpgradeCount) Facilities.DisplayCostText(costText, Cost[CurrentUpgradeIndex], "силы предков");
            else costText.text = $"ћакс. уровень";
        }
        protected override void Add(int stat_index)
        {
            switch (stat_index)
            {
                case ((int)Stat.Strength):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].StrengthBase += Additive[CurrentUpgradeIndex];
                    break;
                case ((int)Stat.Dexterity):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].DexterityBase += Additive[CurrentUpgradeIndex];
                    break;
                case ((int)Stat.Intellect):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].IntellectBase += Additive[CurrentUpgradeIndex];
                    break;
            }
        }
    }
    public readonly BaseAdditional BaseStrengthAdditional = new() { MaxUpgradeCount = 12 };
    public readonly BaseAdditional BaseDexterityAdditional = new() { MaxUpgradeCount = 12 };
    public readonly BaseAdditional BaseIntellectAdditional = new() { MaxUpgradeCount = 12 };

    public class ArithmeticAdditional : PerkTemplate
    {
        public override void IncreaseMethod(int stat_index)
        {
            if (CurrentUpgradeIndex < MaxUpgradeCount && Facilities.CheckAncestralPowerAmount(Cost[CurrentUpgradeIndex]))
            {
                Summ += Additive;
                base.IncreaseMethod(stat_index);
            }
        }

        public override void UpdateNarrationText(TextMeshProUGUI Narration, string postfix)
        {
            Narration.text = $"+{PerkTree.RoundValue(Additive * 100)}% {postfix}";
        }

        protected override void Add(int stat_index)
        {
            switch (stat_index)
            {
                case ((int)Stat.Strength):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].StrengthFromPerk += Additive;
                    break;
                case ((int)Stat.Dexterity):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].DexterityFromPerk += Additive;
                    break;
                case ((int)Stat.Intellect):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].IntellectFromPerk += Additive;
                    break;
            }
        }
    }
    public readonly ArithmeticAdditional ArithmeticStrengthAdditional = new() { Additive = 0.10f };
    public readonly ArithmeticAdditional ArithmeticDexterityAdditional = new() { Additive = 0.10f };
    public readonly ArithmeticAdditional ArithmeticIntellectAdditional = new() { Additive = 0.10f };

    public class GeometricAdditional : PerkTemplate
    {
        public override void IncreaseMethod(int stat_index)
        {
            if (CurrentUpgradeIndex < MaxUpgradeCount && Facilities.CheckAncestralPowerAmount(Cost[CurrentUpgradeIndex]))
            {
                Summ += Additive + (Summ * Additive);
                base.IncreaseMethod(stat_index);
            }
        }

        public override void UpdateNarrationText(TextMeshProUGUI Narration, string postfix)
        {
            Narration.text = $"+{PerkTree.RoundValue(Additive * 100)}% {postfix}";
        }

        protected override void Add(int stat_index)
        {
            switch (stat_index)
            {
                case ((int)Stat.Strength):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].StrengthFromPerk += Additive + (Summ * Additive);
                    break;
                case ((int)Stat.Dexterity):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].StrengthFromPerk += Additive + (Summ * Additive);
                    break;
                case ((int)Stat.Intellect):
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].StrengthFromPerk += Additive + (Summ * Additive);
                    break;
            }
        }
    }
    public readonly GeometricAdditional GeometricStrengthAdditional = new() { Additive = 0.075f };
    public readonly GeometricAdditional GeometricDexterityAdditional = new() { Additive = 0.075f };
    public readonly GeometricAdditional GeometricIntellectAdditional = new() { Additive = 0.075f };

    public class MaxStockFromStat : PerkTemplate
    {
        public override void IncreaseMethod(int stat_index)
        {
            if (CurrentUpgradeIndex < MaxUpgradeCount && Facilities.CheckAncestralPowerAmount(Cost[CurrentUpgradeIndex]))
            {
                Summ += Additive;
                base.IncreaseMethod(stat_index);
            }
        }

        public override void UpdateNarrationText(TextMeshProUGUI Narration, string postfix)
        {
            Narration.text = $"+{Math.Round(Additive, 2)} {postfix}";
        }

        protected override void Add(int stat_index)
        {
            switch (stat_index)
            {
                case ((int)Stat.Strength):
                    AdditiveMaxHealthFromStrength += Additive;
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].CalcSet_Strength();
                    break;
                case ((int)Stat.Dexterity):
                    AdditiveMaxStaminaFromDexterity += Additive;
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].CalcSet_Dexterity();
                    break;
                case ((int)Stat.Intellect):
                    AdditiveMaxWillFromIntellect += Additive;
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].CalcSet_Intellect();
                    break;
            }
        }
    }
    public readonly MaxStockFromStat MaxHealthFromStrength = new() { Additive = 1f };
    public readonly MaxStockFromStat MaxStaminaFromDexterity = new() { Additive = 1f };
    public readonly MaxStockFromStat MaxWillFromIntellect = new() { Additive = 1f };

    public class RegenerationFromStat : PerkTemplate
    {
        internal readonly static float[] NewCost = { 40, 80, 130, 200, 280, 370, 480, 600, 740, 900, 1080, 1300, 1520, 1760, 2000 };

        public override void IncreaseMethod(int stat_index)
        {
            if (CurrentUpgradeIndex < MaxUpgradeCount && Facilities.CheckAncestralPowerAmount(Cost[CurrentUpgradeIndex]))
            {
                Summ += Additive;
                base.IncreaseMethod(stat_index);
            }
        }

        public override void UpdateNarrationText(TextMeshProUGUI Narration, string postfix)
        {
            Narration.text = $"+{Math.Round(Additive, 2)} {postfix}";
        }

        protected override void Add(int stat_index)
        {
            switch (stat_index)
            {
                case ((int)Stat.Strength):
                    AdditiveHealthRegenerationFromStrength += Additive;
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].CalcSet_Strength();
                    break;
                case ((int)Stat.Dexterity):
                    AdditiveStaminaRegenerationFromDexterity += Additive;
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].CalcSet_Dexterity();
                    break;
                case ((int)Stat.Intellect):
                    AdditiveWillRegenerationFromIntellect += Additive;
                    for (int i = 0; i < HeroCount; i++)
                        Heroes.hero[i].CalcSet_Intellect();
                    break;
            }
        }

    }
    public readonly RegenerationFromStat HealthRegenerationFromStrength = new() { Additive = 0.04f, MaxUpgradeCount = 12, Cost = RegenerationFromStat.NewCost };
    public readonly RegenerationFromStat StaminaRegenerationFromDexterity = new() { Additive = 0.04f, MaxUpgradeCount = 12, Cost = RegenerationFromStat.NewCost };
    public readonly RegenerationFromStat WillRegenerationFromIntellect = new() { Additive = 0.04f, MaxUpgradeCount = 12, Cost = RegenerationFromStat.NewCost };
    #endregion

    private void Awake()
    {
        if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
            FindAllPerks();
    }

    private void Start()
    {
        if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
        {
            if (BaseStrengthAdditional.CurrentUpgradeIndex > 0)
                ActiveSet_0();
            if (ArithmeticStrengthAdditional.CurrentUpgradeIndex > 0)
                ActiveSet_1();
            if (MaxHealthFromStrength.CurrentUpgradeIndex > 0)
                ActiveSet_2();

            if (BaseDexterityAdditional.CurrentUpgradeIndex > 0)
                ActiveSet_3();
            if (ArithmeticDexterityAdditional.CurrentUpgradeIndex > 0)
                ActiveSet_4();
            if (MaxStaminaFromDexterity.CurrentUpgradeIndex > 0)
                ActiveSet_5();

            if (BaseIntellectAdditional.CurrentUpgradeIndex > 0)
                ActiveSet_6();
            if (ArithmeticIntellectAdditional.CurrentUpgradeIndex > 0)
                ActiveSet_7();
            if (MaxWillFromIntellect.CurrentUpgradeIndex > 0)
                ActiveSet_8();

            #region UpdateAllText
            BaseStrengthAdditional.Start();
            BaseDexterityAdditional.Start();
            BaseIntellectAdditional.Start();

            ArithmeticStrengthAdditional.Start();
            ArithmeticDexterityAdditional.Start();
            ArithmeticIntellectAdditional.Start();

            GeometricStrengthAdditional.Start();
            GeometricDexterityAdditional.Start();
            GeometricIntellectAdditional.Start();

            MaxHealthFromStrength.Start();
            MaxStaminaFromDexterity.Start();
            MaxWillFromIntellect.Start();

            HealthRegenerationFromStrength.Start();
            StaminaRegenerationFromDexterity.Start();
            WillRegenerationFromIntellect.Start();
            #endregion

            CheckAllPerkImages();
            SwitchAllPerkBackgroundsColor();
        }
    }

    private void FindAllPerks()
    {
        Component[] Components = PerkWindow.GetComponentsInChildren<RectTransform>(includeInactive: true);
        foreach (Component perk in Components)
        {
            switch (perk.name)
            {
                #region Strength
                case ("Perk_Strength_1"):
                    BaseStrengthAdditional.FindComponents(perk);
                    break;
                case ("Perk_Strength_2"):
                    ArithmeticStrengthAdditional.FindComponents(perk);
                    break;
                case ("Perk_Strength_3"):
                    GeometricStrengthAdditional.FindComponents(perk);
                    break;
                case ("Perk_HealthFromStrength"):
                    MaxHealthFromStrength.FindComponents(perk);
                    break;
                case ("Perk_HealthRegenerationFromStrength"):
                    HealthRegenerationFromStrength.FindComponents(perk);
                    break;
                #endregion

                #region Dexterity
                case ("Perk_Dexterity_1"):
                    BaseDexterityAdditional.FindComponents(perk);
                    break;
                case ("Perk_Dexterity_2"):
                    ArithmeticDexterityAdditional.FindComponents(perk);
                    break;
                case ("Perk_Dexterity_3"):
                    GeometricDexterityAdditional.FindComponents(perk);
                    break;
                case ("Perk_StaminaFromDexterity"):
                    MaxStaminaFromDexterity.FindComponents(perk);
                    break;
                case ("Perk_StaminaRegenerationFromDexterity"):
                    StaminaRegenerationFromDexterity.FindComponents(perk);
                    break;
                #endregion

                #region Will
                case ("Perk_Will_1"):
                    BaseIntellectAdditional.FindComponents(perk);
                    break;
                case ("Perk_Will_2"):
                    ArithmeticIntellectAdditional.FindComponents(perk);
                    break;
                case ("Perk_Will_3"):
                    GeometricIntellectAdditional.FindComponents(perk);
                    break;
                case ("Perk_WillFromIntellect"):
                    MaxWillFromIntellect.FindComponents(perk);
                    break;
                case ("Perk_WillRegenerationFromIntellect"):
                    WillRegenerationFromIntellect.FindComponents(perk);
                    break;
                    #endregion
            }
        }
    }

    private void CheckAllPerkImages()
    {
        BaseStrengthAdditional.CheckPerkImages();
        BaseDexterityAdditional.CheckPerkImages();
        BaseIntellectAdditional.CheckPerkImages();

        ArithmeticStrengthAdditional.CheckPerkImages();
        ArithmeticDexterityAdditional.CheckPerkImages();
        ArithmeticIntellectAdditional.CheckPerkImages();

        GeometricStrengthAdditional.CheckPerkImages();
        GeometricDexterityAdditional.CheckPerkImages();
        GeometricIntellectAdditional.CheckPerkImages();

        MaxHealthFromStrength.CheckPerkImages();
        MaxStaminaFromDexterity.CheckPerkImages();
        MaxWillFromIntellect.CheckPerkImages();

        HealthRegenerationFromStrength.CheckPerkImages();
        StaminaRegenerationFromDexterity.CheckPerkImages();
        WillRegenerationFromIntellect.CheckPerkImages();
    }

    #region PerkButtonActions
    private float RoundValue(float var)
    {
        float summ = (float)Math.Round(var, 2);
        return summ;
    }

    public void UpdateInformationWindow(string perk_name)
    {
        _adoptThePerkImage();
        RemoveAllListenersOnUpgradeButton();
        SwitchAllPerkBackgroundsColor();

        switch (perk_name)
        {
            #region StrengthBrench
            case ("Strength_1"):
                UpdatePerkCard_Strength_1();
                UpgradeButton.onClick.AddListener(BaseIncreaseInStrength);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_Strength_1);
                break;
            case ("Strength_2"):
                UpdatePerkCard_Strength_2();
                UpgradeButton.onClick.AddListener(ArithmeticIncreaseInStrength);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_Strength_2);
                break;
            case ("Strength_3"):
                UpdatePerkCard_Strength_3();
                UpgradeButton.onClick.AddListener(GeometricIncreaseInStrength);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_Strength_3);
                break;
            case ("MaxHealthFromStrength"):
                UpdatePerkCard_MaxHealthFromStrength();
                UpgradeButton.onClick.AddListener(IncreaseMaxHealthFromStrength);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_MaxHealthFromStrength);
                break;
            case ("HealthRegenerationFromStrength"):
                UpdatePerkCard_HealthRegenerationFromStrength();
                UpgradeButton.onClick.AddListener(IncreaseHealthRegenerationFromStrength);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_HealthRegenerationFromStrength);
                break;
            #endregion

            #region DexterityBrench
            case ("Dexterity_1"):
                UpdatePerkCard_Dexterity_1();
                UpgradeButton.onClick.AddListener(BaseIncreaseInDexterity);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_Dexterity_1);
                break;
            case ("Dexterity_2"):
                UpdatePerkCard_Dexterity_2();
                UpgradeButton.onClick.AddListener(ArithmeticIncreaseInDexterity);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_Dexterity_2);
                break;
            case ("Dexterity_3"):
                UpdatePerkCard_Dexterity_3();
                UpgradeButton.onClick.AddListener(GeometricIncreaseInDexterity);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_Dexterity_3);
                break;
            case ("MaxStaminaFromDexterity"):
                UpdatePerkCard_MaxStaminaFromDexterity();
                UpgradeButton.onClick.AddListener(IncreaseMaxStaminaFromDexterity);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_MaxStaminaFromDexterity);
                break;
            case ("StaminaRegenerationFromDexterity"):
                UpdatePerkCard_StaminaRegenerationFromDexterity();
                UpgradeButton.onClick.AddListener(IncreaseStaminaRegenerationFromDexterity);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_StaminaRegenerationFromDexterity);
                break;
            #endregion

            #region IntellectBrench
            case ("Intellect_1"):
                UpdatePerkCard_Intellect_1();
                UpgradeButton.onClick.AddListener(BaseIncreaseInIntellect);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_Intellect_1);
                break;
            case ("Intellect_2"):
                UpdatePerkCard_Intellect_2();
                UpgradeButton.onClick.AddListener(ArithmeticIncreaseInIntellect);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_Intellect_2);
                break;
            case ("Intellect_3"):
                UpdatePerkCard_Intellect_3();
                UpgradeButton.onClick.AddListener(GeometricIncreaseInIntellect);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_Intellect_3);
                break;
            case ("MaxWillFromIntellect"):
                UpdatePerkCard_MaxWillFromIntellect();
                UpgradeButton.onClick.AddListener(IncreaseMaxWillFromIntellect);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_MaxWillFromIntellect);
                break;
            case ("WillRegenerationFromIntellect"):
                UpdatePerkCard_WillRegenerationFromIntellect();
                UpgradeButton.onClick.AddListener(IncreaseWillRegenerationFromIntellect);
                UpgradeButton.onClick.AddListener(UpdatePerkCard_WillRegenerationFromIntellect);
                break;
            #endregion

            default: break;
        }

        UpgradeButton.onClick.AddListener(SwitchAllPerkBackgroundsColor);
        UpgradeButton.onClick.AddListener(CheckAllPerkImages);

        void _adoptThePerkImage()
        {
            Image[] arr = gameObject.GetComponentsInChildren<Image>(includeInactive: true);
            foreach (Image required_image in arr)
            {
                switch (required_image.name)
                {
                    case ("PerkImage"):
                        PerkImageInInformationWindow.sprite = required_image.sprite;
                        break;
                }
            }
        }
    }

    private void RemoveAllListenersOnUpgradeButton()
    { UpgradeButton.onClick.RemoveAllListeners(); }

    #region UpdateTextInformationWindow
    private void SwitchAllPerkBackgroundsColor()
    {
        BaseStrengthAdditional.ChangePerkBackgroundColor();
        BaseDexterityAdditional.ChangePerkBackgroundColor();
        BaseIntellectAdditional.ChangePerkBackgroundColor();

        ArithmeticStrengthAdditional.ChangePerkBackgroundColor();
        ArithmeticDexterityAdditional.ChangePerkBackgroundColor();
        ArithmeticIntellectAdditional.ChangePerkBackgroundColor();

        GeometricStrengthAdditional.ChangePerkBackgroundColor();
        GeometricDexterityAdditional.ChangePerkBackgroundColor();
        GeometricIntellectAdditional.ChangePerkBackgroundColor();

        MaxHealthFromStrength.ChangePerkBackgroundColor();
        MaxStaminaFromDexterity.ChangePerkBackgroundColor();
        MaxWillFromIntellect.ChangePerkBackgroundColor();

        MaxHealthFromStrength.ChangePerkBackgroundColor();
        MaxStaminaFromDexterity.ChangePerkBackgroundColor();
        MaxWillFromIntellect.ChangePerkBackgroundColor();

        HealthRegenerationFromStrength.ChangePerkBackgroundColor();
        StaminaRegenerationFromDexterity.ChangePerkBackgroundColor();
        WillRegenerationFromIntellect.ChangePerkBackgroundColor();
    }

    #region 1
    private void StrengthTemplate()
    {
        if (InfoWindow.gameObject.activeInHierarchy == false)
        {
            InfoWindow.gameObject.SetActive(true);
            InfoWindow.SetTrigger("Open");
        }

        PerkImageInInformationWindow.color = StrengthImageColor;
        PerkBackgroundInInformationWindow.color = StrengthBackgroundColor;
        PerkNarrationText.color = StrengthImageColor;
        ValueGainedText.color = StrengthImageColor;
    }

    private void UpdatePerkCard_Strength_1()
    {
        StrengthTemplate();
        PerkImageInInformationWindow.sprite = Strength_Sprite;
        PerkNameText.text = _perkMessage.BaseStrength;
        BaseStrengthAdditional.UpdateNarrationText(PerkNarrationText, _perkMessage.ToBaseStrength);
        BaseStrengthAdditional.TemplateUpdateMethod("non_percent", PerkLevelText, ValueGainedText, PerkCostText);
        BaseStrengthAdditional.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_Strength_2()
    {
        StrengthTemplate();
        PerkImageInInformationWindow.sprite = Strength_Sprite;
        PerkNameText.text = _perkMessage.ArithmeticStrength;
        ArithmeticStrengthAdditional.UpdateNarrationText(PerkNarrationText, _perkMessage.Arithmetic);
        ArithmeticStrengthAdditional.TemplateUpdateMethod("percent", PerkLevelText, ValueGainedText, PerkCostText);
        ArithmeticStrengthAdditional.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_Strength_3()
    {
        StrengthTemplate();
        PerkImageInInformationWindow.sprite = Strength_Sprite;
        PerkNameText.text = _perkMessage.GeometricStrength;
        GeometricStrengthAdditional.UpdateNarrationText(PerkNarrationText, _perkMessage.Geometric);
        GeometricStrengthAdditional.TemplateUpdateMethod("percent", PerkLevelText, ValueGainedText, PerkCostText);
        GeometricStrengthAdditional.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_MaxHealthFromStrength()
    {
        StrengthTemplate();
        PerkImageInInformationWindow.sprite = MaxHealthFromStrength_Sprite;
        PerkNameText.text = _perkMessage.MaxHealthFromStrength;
        MaxHealthFromStrength.UpdateNarrationText(PerkNarrationText, _perkMessage.ToMaxHealthFromStrength);
        MaxHealthFromStrength.TemplateUpdateMethod("non_percent", PerkLevelText, ValueGainedText, PerkCostText);
        MaxHealthFromStrength.ChangePerkBackgroundColor();
    }
    private void UpdatePerkCard_HealthRegenerationFromStrength()
    {
        StrengthTemplate();
        PerkImageInInformationWindow.sprite = HealthRegenerationFromStrength_Sprite;
        PerkNameText.text = _perkMessage.HealthRegenerationFromStrength;
        HealthRegenerationFromStrength.UpdateNarrationText(PerkNarrationText, _perkMessage.ToHealthRegenerationFromStrength);
        HealthRegenerationFromStrength.TemplateUpdateMethod("non_percent", PerkLevelText, ValueGainedText, PerkCostText);
        HealthRegenerationFromStrength.ChangePerkBackgroundColor();
    }
    #endregion

    #region 2
    private void DexterityTemplate()
    {
        if (InfoWindow.gameObject.activeInHierarchy == false)
        {
            InfoWindow.gameObject.SetActive(true);
            InfoWindow.SetTrigger("Open");
        }

        PerkImageInInformationWindow.color = DexterityImageColor;
        PerkBackgroundInInformationWindow.color = DexterityBackgroundColor;
        PerkNarrationText.color = DexterityImageColor;
        ValueGainedText.color = DexterityImageColor;
    }

    private void UpdatePerkCard_Dexterity_1()
    {
        DexterityTemplate();
        PerkImageInInformationWindow.sprite = Dexterity_Sprite;
        PerkNameText.text = _perkMessage.BaseDexterity;
        BaseDexterityAdditional.UpdateNarrationText(PerkNarrationText, _perkMessage.ToBaseDexterity);
        BaseDexterityAdditional.TemplateUpdateMethod("non_percent", PerkLevelText, ValueGainedText, PerkCostText);
        BaseDexterityAdditional.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_Dexterity_2()
    {
        DexterityTemplate();
        PerkImageInInformationWindow.sprite = Dexterity_Sprite;
        PerkNameText.text = _perkMessage.ArithmeticDexterity;
        ArithmeticDexterityAdditional.UpdateNarrationText(PerkNarrationText, _perkMessage.Arithmetic);
        ArithmeticDexterityAdditional.TemplateUpdateMethod("percent", PerkLevelText, ValueGainedText, PerkCostText);
        ArithmeticDexterityAdditional.ChangePerkBackgroundColor();
    }
    private void UpdatePerkCard_Dexterity_3()
    {
        DexterityTemplate();
        PerkImageInInformationWindow.sprite = Dexterity_Sprite;
        PerkNameText.text = _perkMessage.GeometricDexterity;
        GeometricDexterityAdditional.UpdateNarrationText(PerkNarrationText, _perkMessage.Geometric);
        GeometricDexterityAdditional.TemplateUpdateMethod("percent", PerkLevelText, ValueGainedText, PerkCostText);
        GeometricDexterityAdditional.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_MaxStaminaFromDexterity()
    {
        DexterityTemplate();
        PerkImageInInformationWindow.sprite = MaxStaminaFromDexterity_Sprite;
        PerkNameText.text = _perkMessage.MaxStaminaFromDexterity;
        MaxStaminaFromDexterity.UpdateNarrationText(PerkNarrationText, _perkMessage.ToMaxStaminaFromDexterity);
        MaxStaminaFromDexterity.TemplateUpdateMethod("non_percent", PerkLevelText, ValueGainedText, PerkCostText);
        MaxStaminaFromDexterity.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_StaminaRegenerationFromDexterity()
    {
        DexterityTemplate();
        PerkImageInInformationWindow.sprite = StaminaRegenerationFromDexterity_Sprite;
        PerkNameText.text = _perkMessage.StaminaRegenerationFromDexterity;
        StaminaRegenerationFromDexterity.UpdateNarrationText(PerkNarrationText, _perkMessage.ToStaminaRegenerationFromDexteiry);
        StaminaRegenerationFromDexterity.TemplateUpdateMethod("non_percent", PerkLevelText, ValueGainedText, PerkCostText);
        StaminaRegenerationFromDexterity.ChangePerkBackgroundColor();
    }
    #endregion

    #region 3
    private void IntellectTemplate()
    {
        if (InfoWindow.gameObject.activeInHierarchy == false)
        {
            InfoWindow.gameObject.SetActive(true);
            InfoWindow.SetTrigger("Open");
        }

        PerkImageInInformationWindow.color = IntellectImageColor;
        PerkBackgroundInInformationWindow.color = IntellectBackgroundColor;
        PerkNarrationText.color = IntellectImageColor;
        ValueGainedText.color = IntellectImageColor;
    }

    private void UpdatePerkCard_Intellect_1()
    {
        IntellectTemplate();

        PerkImageInInformationWindow.sprite = Intellect_Sprite;
        PerkNameText.text = _perkMessage.BaseIntellect;
        BaseIntellectAdditional.UpdateNarrationText(PerkNarrationText, _perkMessage.ToBaseIntellect);
        BaseIntellectAdditional.TemplateUpdateMethod("non_percent", PerkLevelText, ValueGainedText, PerkCostText);
        BaseIntellectAdditional.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_Intellect_2()
    {
        IntellectTemplate();
        PerkImageInInformationWindow.sprite = Intellect_Sprite;
        PerkNameText.text = _perkMessage.ArithmeticIntellect;
        ArithmeticIntellectAdditional.UpdateNarrationText(PerkNarrationText, _perkMessage.Arithmetic);
        ArithmeticIntellectAdditional.TemplateUpdateMethod("percent", PerkLevelText, ValueGainedText, PerkCostText);
        ArithmeticIntellectAdditional.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_Intellect_3()
    {
        IntellectTemplate();
        PerkImageInInformationWindow.sprite = Intellect_Sprite;
        PerkNameText.text = _perkMessage.GeometricIntellect;
        GeometricIntellectAdditional.UpdateNarrationText(PerkNarrationText, _perkMessage.Geometric);
        GeometricIntellectAdditional.TemplateUpdateMethod("percent", PerkLevelText, ValueGainedText, PerkCostText);
        GeometricIntellectAdditional.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_MaxWillFromIntellect()
    {
        IntellectTemplate();
        PerkImageInInformationWindow.sprite = MaxWillFromIntellect_Sprite;
        PerkNameText.text = _perkMessage.MaxWillFromIntellect;
        MaxWillFromIntellect.UpdateNarrationText(PerkNarrationText, _perkMessage.ToMaxWillFromIntellect);
        MaxWillFromIntellect.TemplateUpdateMethod("non_percent", PerkLevelText, ValueGainedText, PerkCostText);
        MaxWillFromIntellect.ChangePerkBackgroundColor();
    }

    private void UpdatePerkCard_WillRegenerationFromIntellect()
    {
        IntellectTemplate();
        PerkImageInInformationWindow.sprite = WillRegenerationFromIntellect_Sprite;
        PerkNameText.text = _perkMessage.WillRegenerationFromIntellect;
        WillRegenerationFromIntellect.UpdateNarrationText(PerkNarrationText, _perkMessage.ToWillRegenerationFromIntellect);
        WillRegenerationFromIntellect.TemplateUpdateMethod("non_percent", PerkLevelText, ValueGainedText, PerkCostText);
        WillRegenerationFromIntellect.ChangePerkBackgroundColor();
    }

    #endregion

    #endregion

    #region StrengthBranch
    private void BaseIncreaseInStrength()
    {
        if (Facilities.CheckAncestralPowerAmount(BaseStrengthAdditional.Cost[BaseStrengthAdditional.CurrentUpgradeIndex]))
        {
            BaseStrengthAdditional.IncreaseMethod((int)Stat.Strength);
            ActiveSet_0();
        }
    }

    private void ArithmeticIncreaseInStrength()
    {
        if (Facilities.CheckAncestralPowerAmount(ArithmeticStrengthAdditional.Cost[ArithmeticStrengthAdditional.CurrentUpgradeIndex]))
        {
            ArithmeticStrengthAdditional.IncreaseMethod((int)Stat.Strength);
            ActiveSet_1();
        }
    }

    private void GeometricIncreaseInStrength()
    {
        if (Facilities.CheckAncestralPowerAmount(GeometricStrengthAdditional.Cost[GeometricStrengthAdditional.CurrentUpgradeIndex]))
        {
            GeometricStrengthAdditional.IncreaseMethod((int)Stat.Strength);
        }
    }

    private void IncreaseMaxHealthFromStrength()
    {
        if (Facilities.CheckAncestralPowerAmount(MaxHealthFromStrength.Cost[MaxHealthFromStrength.CurrentUpgradeIndex]))
        {
            MaxHealthFromStrength.IncreaseMethod((int)Stat.Strength);
            ActiveSet_2();
        }
    }

    private void IncreaseHealthRegenerationFromStrength()
    {
        if (Facilities.CheckAncestralPowerAmount(HealthRegenerationFromStrength.Cost[HealthRegenerationFromStrength.CurrentUpgradeIndex]))
        {
            HealthRegenerationFromStrength.IncreaseMethod((int)Stat.Strength);
        }
    }

    private void ActiveSet_0()
    {
        ArithmeticStrengthAdditional.PerkButton.interactable = true;
        MaxHealthFromStrength.PerkButton.interactable = true;
        for (int i = 0; i < Strength_CompoundLayer_1.Length; i++)
        {
            Strength_CompoundLayer_1[i].color = StrengthCompoundColor;
        }
    }

    private void ActiveSet_1()
    {
        GeometricStrengthAdditional.PerkButton.interactable = true;
        ArithmeticStrengthAdditional.Compound.color = StrengthCompoundColor;
    }

    private void ActiveSet_2()
    {
        HealthRegenerationFromStrength.PerkButton.interactable = true;
        MaxHealthFromStrength.Compound.color = StrengthCompoundColor;
    }
    #endregion

    #region AgilityBranch
    private void BaseIncreaseInDexterity()
    {
        if (Facilities.CheckAncestralPowerAmount(BaseDexterityAdditional.Cost[BaseDexterityAdditional.CurrentUpgradeIndex]))
        {
            BaseDexterityAdditional.IncreaseMethod((int)Stat.Dexterity);
            ActiveSet_3();
        }
    }

    private void ArithmeticIncreaseInDexterity()
    {
        if (Facilities.CheckAncestralPowerAmount(ArithmeticDexterityAdditional.Cost[ArithmeticDexterityAdditional.CurrentUpgradeIndex]))
        {
            ArithmeticDexterityAdditional.IncreaseMethod((int)Stat.Dexterity);
            ActiveSet_4();
        }
    }

    private void GeometricIncreaseInDexterity()
    {
        if (Facilities.CheckAncestralPowerAmount(GeometricDexterityAdditional.Cost[GeometricDexterityAdditional.CurrentUpgradeIndex]))
        {
            GeometricDexterityAdditional.IncreaseMethod((int)Stat.Dexterity);
        }
    }

    private void IncreaseMaxStaminaFromDexterity()
    {
        if (Facilities.CheckAncestralPowerAmount(MaxStaminaFromDexterity.Cost[MaxStaminaFromDexterity.CurrentUpgradeIndex]))
        {
            MaxStaminaFromDexterity.IncreaseMethod((int)Stat.Dexterity);
            ActiveSet_5();
        }
    }

    private void IncreaseStaminaRegenerationFromDexterity()
    {
        if (Facilities.CheckAncestralPowerAmount(StaminaRegenerationFromDexterity.Cost[StaminaRegenerationFromDexterity.CurrentUpgradeIndex]))
        {
            StaminaRegenerationFromDexterity.IncreaseMethod((int)Stat.Dexterity);
        }
    }

    private void ActiveSet_3()
    {
        ArithmeticDexterityAdditional.PerkButton.interactable = true;
        MaxStaminaFromDexterity.PerkButton.interactable = true;
        for (int i = 0; i < Dexterity_CompoundLayer_1.Length; i++)
        {
            Dexterity_CompoundLayer_1[i].color = DexterityCompoundColor;
        }
    }

    private void ActiveSet_4()
    {
        ArithmeticDexterityAdditional.Compound.color = DexterityCompoundColor;
        GeometricDexterityAdditional.PerkButton.interactable = true;
    }

    private void ActiveSet_5()
    {
        StaminaRegenerationFromDexterity.PerkButton.interactable = true;
        MaxStaminaFromDexterity.Compound.color = DexterityCompoundColor;
    }
    #endregion

    #region IntellectBranch
    private void BaseIncreaseInIntellect()
    {
        if (Facilities.CheckAncestralPowerAmount(BaseIntellectAdditional.Cost[BaseIntellectAdditional.CurrentUpgradeIndex]))
        {
            BaseIntellectAdditional.IncreaseMethod((int)Stat.Intellect);
            ActiveSet_6();
        }
    }

    private void ArithmeticIncreaseInIntellect()
    {
        if (Facilities.CheckAncestralPowerAmount(ArithmeticIntellectAdditional.Cost[ArithmeticIntellectAdditional.CurrentUpgradeIndex]))
        {
            ArithmeticIntellectAdditional.IncreaseMethod((int)Stat.Intellect);
            ActiveSet_7();
        }
    }

    private void GeometricIncreaseInIntellect()
    {
        if (Facilities.CheckAncestralPowerAmount(GeometricIntellectAdditional.Cost[GeometricIntellectAdditional.CurrentUpgradeIndex]))
        {
            GeometricIntellectAdditional.IncreaseMethod((int)Stat.Intellect);
        }
    }

    private void IncreaseMaxWillFromIntellect()
    {
        if (Facilities.CheckAncestralPowerAmount(MaxWillFromIntellect.Cost[MaxWillFromIntellect.CurrentUpgradeIndex]))
        {
            MaxWillFromIntellect.IncreaseMethod((int)Stat.Intellect);
            ActiveSet_8();
        }
    }

    private void IncreaseWillRegenerationFromIntellect()
    {
        if (Facilities.CheckAncestralPowerAmount(WillRegenerationFromIntellect.Cost[WillRegenerationFromIntellect.CurrentUpgradeIndex]))
        {
            WillRegenerationFromIntellect.IncreaseMethod((int)Stat.Intellect);
        }
    }

    private void ActiveSet_6()
    {
        ArithmeticIntellectAdditional.PerkButton.interactable = true;
        MaxWillFromIntellect.PerkButton.interactable = true;
        for (int i = 0; i < Intellect_CompoundLayer_1.Length; i++)
        {
            Intellect_CompoundLayer_1[i].color = IntellectCompoundColor;
        }
    }

    private void ActiveSet_7()
    {
        ArithmeticIntellectAdditional.Compound.color = IntellectCompoundColor;
        GeometricIntellectAdditional.PerkButton.interactable = true;
    }

    private void ActiveSet_8()
    {
        WillRegenerationFromIntellect.PerkButton.interactable = true;
        MaxWillFromIntellect.Compound.color = IntellectCompoundColor;
    }
    #endregion

    #endregion
}