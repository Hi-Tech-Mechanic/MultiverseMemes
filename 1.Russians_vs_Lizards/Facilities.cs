using TMPro;
using UnityEngine;

public class Facilities : DataStructure
{
    #region SerializeField
    public float FaithCurrency
    {
        get { return _faithCurrency; }
        set { _faithCurrency = value; UpdateFaithCurrencyText(); Heroes.UpdateAllHeroPriceValues(); }
    }
    [SerializeField] private float _faithCurrency;

    public float AncestralPower
    {
        get { return _ancestralPower; }
        set { _ancestralPower = value; UpdateAncestralPowerText(); }
    }
    [SerializeField] private float _ancestralPower;

    [SerializeField] private TextMeshProUGUI _faithCurrencyText;
    [SerializeField] private TextMeshProUGUI _ancestralPowerText;

    public float FaithMultiplier 
    { 
        get { return _faithMultiplier; }
        set { _faithMultiplier = value; } 
    }
    private float _faithMultiplier = 1f;
    #endregion

    private void Start()
    {
        if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
        {
            UpdateFaithCurrencyText();
            UpdateAncestralPowerText();
        }
    }

    public void FaithCurrencyPay(float cost)
    {
        FaithCurrency -= cost;
    }

    public void AncestralPowerPay(float cost)
    {
        AncestralPower -= cost;
    }

    public void UpdateFaithCurrencyText()
    { _faithCurrencyText.text = $"{ValuesRounding.ExtendedAccuracyFormattingValue("", "", FaithCurrency)}"; }

    public void UpdateAncestralPowerText()
    { _ancestralPowerText.text = $"{ValuesRounding.ExtendedAccuracyFormattingValue("", "", AncestralPower)}"; }

    public static void DisplayCostText(TextMeshProUGUI text, float value, string postfix)
    { text.text = $"{ValuesRounding.ExtendedAccuracyFormattingValue("", "", value)} {postfix}"; }

    public void AddFaithCurrency(float value)
    {
        FaithCurrency += value;
        Achievements_R_vs_L.AccumulateFaith(value);
    }

    public void AddAncestralPower(float value) 
    {
        AncestralPower += value;
        AudioEffects.PlayGetCoinEffect();
        Achievements_R_vs_L.AccumulateAncestralPower(value);
    }

    public bool CheckFaithCurrencyAmount(float cost)
    {
        return (FaithCurrency >= cost);
    }

    public bool CheckAncestralPowerAmount(float cost)
    {
        return (AncestralPower >= cost);
    }
}
