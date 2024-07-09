using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentCaseData : MonoBehaviour
{
    [NonSerialized] public int MemeCoinsCount;
    [NonSerialized] public Color QualityColor;

    private Image _image;
    private TextMeshProUGUI _rewardText;

    private void Awake()
    {
        _image = gameObject.GetComponent<Image>();
        _rewardText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateItemInfo()
    {
        _image.color = QualityColor;
        _rewardText.text = $"{ValuesRounding.FormattingValue("", "", MemeCoinsCount)}";
    }
}
