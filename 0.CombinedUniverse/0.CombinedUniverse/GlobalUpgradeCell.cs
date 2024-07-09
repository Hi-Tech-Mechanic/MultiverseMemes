using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalUpgradeCell : DataStructure
{
    [NonSerialized] public GameObject MAXMessageText;
    [NonSerialized] public GameObject ArrowIcon;
    [NonSerialized] public Button UpgradeButton;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _purchasedCountText;
    private TextMeshProUGUI _additiveValueText;
    private TextMeshProUGUI _currentValueText;
    private TextMeshProUGUI _targetValueText;
    private TextMeshProUGUI _priceText;
    private Image _cellIcon;

    public void FindComponents()
    {
        RectTransform[] Elements = gameObject.GetComponentsInChildren<RectTransform>(includeInactive: true);
        foreach (RectTransform element in Elements)
        {
            switch (element.name)
            {
                case ("Name"):
                    _nameText = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("PurchasedCount"):
                    _purchasedCountText = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("AdditiveValue"):
                    _additiveValueText = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("CurrentValue"):
                    _currentValueText = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("TargetValue"):
                    _targetValueText = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("Price"):
                    _priceText = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("GlobalUpgradeButton"):
                    UpgradeButton = element.GetComponent<Button>();
                    break;
                case ("Picture"):
                    _cellIcon = element.GetComponent<Image>();
                    break;
                case ("Arrow"):
                    ArrowIcon = element.gameObject;
                    break;
                case ("Text_maximum"):
                    MAXMessageText = element.gameObject;
                    break;
            }
        }
    }

    public TextMeshProUGUI GetCurrentValueTMP()
    { return _currentValueText; }

    public GameObject GetTargetValueObj()
    { return _targetValueText.gameObject; }

    public GameObject GetAdditiveValueObj()
    { return _additiveValueText.gameObject; }

    public void SetCellIcon(Sprite sprite) 
    { _cellIcon.sprite = sprite; }

    public void DisplayName(string message)
    { _nameText.text = message; }

    public void DisplayPurchasedCount(string message)
    { _purchasedCountText.text = message; }

    public void DisplayAdditiveValue(string message)
    { _additiveValueText.text = message; }

    public void DisplayCurrentValue(string message)
    { _currentValueText.text = message; }

    public void DisplayTargetValue(string message)
    { _targetValueText.text = message; }

    public void DisplayPrice(string message)
    { _priceText.text = message; }
}