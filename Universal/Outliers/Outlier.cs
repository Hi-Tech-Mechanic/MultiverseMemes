using TMPro;
using UnityEngine;

public class Outlier : MonoBehaviour
{
    private Vector2 _targetPosition;
    private TextMeshProUGUI _valueText;
    private float _animateTime = 0.3f;

    private void Update()
    {
        gameObject.transform.Translate(_targetPosition * (Time.unscaledDeltaTime * (1 + (1 / _animateTime))));
    }

    public void DesignateOutlier(string direction, string prefix, string postfix, float value, Color color)
    {
        Init();

        if (direction == "Up")
            _targetPosition.y += 50;
        else _targetPosition.y -= 50;

        _valueText.color = color;
        _valueText.text = ValuesRounding.FormattingValue(prefix, postfix, value);
    }

    public void DesignateInfoOutlier(string direction, string message, Color color)
    {
        _animateTime = 2;
        _valueText = GetComponentInChildren<TextMeshProUGUI>();

        if (direction == "Up")
            _targetPosition.y += 50;
        else _targetPosition.y -= 50;

        _valueText.color = color;
        _valueText.text = message;
    }

    public void DestroyObject()
    { Destroy(gameObject); }

    private void Init()
    {
        _valueText = GetComponent<TextMeshProUGUI>();
        gameObject.transform.localPosition = Vector2.zero;
        _targetPosition = transform.localPosition;
    }
}
