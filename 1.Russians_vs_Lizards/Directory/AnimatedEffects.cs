using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class AnimatedEffects : MonoBehaviour
{
    private Image _filledArea;
    private TextMeshProUGUI _remainingTime;

    private void Awake()
    {
        _filledArea = gameObject.GetComponent<Image>();
        _remainingTime = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    { StartCoroutine(LoopAnimateEffects()); }

    private IEnumerator LoopAnimateEffects()
    {
        float _timer = 0;
        float _duration = Random.Range(1.5f, 8);

        while (_timer < _duration)
        {
            _remainingTime.text = $"{(int)(_duration - _timer)} c.";
            _filledArea.fillAmount = 1 - (_timer / _duration);
            _timer+= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(LoopAnimateEffects());
    }
}
