using TMPro;
using UnityEngine;

public class Shop : DataStructure
{
    [SerializeField] private GameObject _ancestralPowerContent;
    [SerializeField] private GameObject _faithContent;
    [SerializeField] private TextMeshProUGUI _baikalWaterCostText;
    [SerializeField] private TextMeshProUGUI[] _potionsCostText;
    [SerializeField] private TextMeshProUGUI _memeCoinsText;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioClip _purchaseEffect;

    private const int _currencyItemCount = 8;
    private readonly int[] _ancestralPowerAdditive = { 5, 10, 25, 50, 100, 250, 500, 1000 };
    private readonly int[] _faithAdditive = { 500, 1000, 2500, 5000, 10000, 25000, 50000, 100000 };
    private readonly int[] _currencyCost = { 10, 20, 50, 100, 200, 500, 1000, 2000 };
    private const int _baikalWaterCost = 50;
    private const int _potionsCost = 15;
    private bool _windowIsOpen = false;

    private readonly TextMeshProUGUI[] _ancestralPowerItemCost = new TextMeshProUGUI[_currencyItemCount];
    private readonly TextMeshProUGUI[] _ancestralPowerItemValue = new TextMeshProUGUI[_currencyItemCount];
    private readonly TextMeshProUGUI[] _faithItemCost = new TextMeshProUGUI[_currencyItemCount];
    private readonly TextMeshProUGUI[] _faithItemValue = new TextMeshProUGUI[_currencyItemCount];

    private void Awake()
    {
        FindComponents();
    }

    private void Start()
    {
        DisplayAllInfo();
    }

    private void OnEnable()
    {
        DisplayMemeCoins();
    }

    public void CloseShop()
    {
        _windowIsOpen = false;
        _animator.SetTrigger("Close");
    }

    public void SwitchActiveShop()
    {
        _windowIsOpen = gameObject.activeInHierarchy;

        if (!_windowIsOpen)
        {
            gameObject.SetActive(true);
            _animator.SetTrigger("Open");
        }
        else _animator.SetTrigger("Close");

        _windowIsOpen = !_windowIsOpen;
    }

    public void BuyAncestralPowerItem(int itemIndex)
    {
        if (TemplateCheckAndPurchase(itemIndex))
        {
            GetMoneyAnimation.CreateAndAddCoins(_ancestralPowerAdditive[itemIndex], "AncestralPower");
        }
    }

    public void BuyFaithItem(int itemIndex)
    {
        if (TemplateCheckAndPurchase(itemIndex))
        {
            GetMoneyAnimation.CreateAndAddCoins(_faithAdditive[itemIndex], "FaithCoin");
        }
    }

    public void BuyBaikalWater()
    {
        if (MoneyMenu.GetMemeCoins() >= _baikalWaterCost)
        {
            AudioEffects.PlayOneShotEffect(_purchaseEffect);
            MoneyMenu.SpendMemeCoins(_baikalWaterCost);
            DisplayMemeCoins();
            Items._BaikalWater.Count++;
        }
    }

    public void BuyPotion(int potionIndex)
    {
        if (MoneyMenu.GetMemeCoins() >= _potionsCost)
        {
            AudioEffects.PlayOneShotEffect(_purchaseEffect);
            MoneyMenu.SpendMemeCoins(_potionsCost);
            DisplayMemeCoins();

            if (potionIndex == (int)Items.PotionsEnum.HealthPotion)
            {
                Items.HealthPotion.Count++;
            }
            else if (potionIndex == (int)Items.PotionsEnum.StaminaPotion)
            {
                Items.StaminaPotion.Count++;
            }
            else if (potionIndex == (int)Items.PotionsEnum.WillPotion)
            {
                Items.WillPotion.Count++;
            }
        }
    }

    private bool TemplateCheckAndPurchase(int itemIndex)
    {
        if (MoneyMenu.GetMemeCoins() >= _currencyCost[itemIndex])
        {
            MoneyMenu.SpendMemeCoins(_currencyCost[itemIndex]);
            DisplayMemeCoins();
            return true;
        }
        else return false;
    }

    private void FindComponents()
    {
        _animator = gameObject.GetComponent<Animator>();
        TextMeshProUGUI[] components;

        int i = 0;

        components = _ancestralPowerContent.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI component in components)
        {
            switch (component.name) 
            {
                case ("GetValue"):
                    _ancestralPowerItemValue[i] = component;
                    break;                
                case ("Cost"):
                    _ancestralPowerItemCost[i] = component;
                    i++;
                    break;
            }
        }

        i = 0;

        components = _faithContent.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI component in components)
        {
            switch (component.name)
            {
                case ("GetValue"):
                    _faithItemValue[i] = component;
                    break;
                case ("Cost"):
                    _faithItemCost[i] = component;
                    i++;
                    break;
            }
        }
    }

    private void DisplayAllInfo()
    {
        for (int i = 0; i < _currencyItemCount; i++)
        {
            _faithItemCost[i].text = $"{ValuesRounding.FormattingValue("", "", _currencyCost[i])}";
            _ancestralPowerItemCost[i].text = $"{ValuesRounding.FormattingValue("", "", _currencyCost[i])}";
            _faithItemValue[i].text = $"{ValuesRounding.FormattingValue("", "", _faithAdditive[i])}";
            _ancestralPowerItemValue[i].text = $"{ValuesRounding.FormattingValue("", "", _ancestralPowerAdditive[i])}";

            _baikalWaterCostText.text = $"{ValuesRounding.FormattingValue("", "", _baikalWaterCost)}";
            _potionsCostText[0].text = $"{ValuesRounding.FormattingValue("", "", _potionsCost)}";
            _potionsCostText[1].text = $"{ValuesRounding.FormattingValue("", "", _potionsCost)}";
            _potionsCostText[2].text = $"{ValuesRounding.FormattingValue("", "", _potionsCost)}";
        }

        DisplayMemeCoins();
    }

    private void DisplayMemeCoins() 
    { _memeCoinsText.text = $"{ValuesRounding.UltraAccuracyFormattingValue("", "", MoneyMenu.GetMemeCoins())}"; }
}
