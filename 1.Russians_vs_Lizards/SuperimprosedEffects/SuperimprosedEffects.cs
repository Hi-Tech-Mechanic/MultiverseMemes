using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuperimprosedEffects : DataStructure
{
    [NonSerialized] public float Duration;
    [NonSerialized] public string EffectType;
    private Image _filledArea;
    private TextMeshProUGUI _remainingTime;
    private bool _initIsComplete = false;

    private void OnEnable()
    {
        if (_initIsComplete == false)
        {
            Start();
            _initIsComplete = true;
        }

        if (Duration > 0)
        {
            StartCoroutine(Countdown());
        }
    }

    private void Start()
    {
        if (_initIsComplete == false)
        {
            _filledArea = gameObject.GetComponent<Image>();
            _remainingTime = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        }
        else
        {
            StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown()
    {
        float duration = Duration;

        while (Duration > 0)
        {
            Duration -= Time.deltaTime;
            _filledArea.fillAmount = 1 + ((Duration / duration) - 1);
            _remainingTime.text = $"{Math.Round(Duration), 1} c.";
            yield return null;
        }
        Destroy(gameObject);

        switch (EffectType)
        {
            case "Buff":
                ListOfEffects.DecreaseCreatedBuffCount();
                break;
            case "Debuff":
                ListOfEffects.DecreaseCreatedDebuffCount();
                break;
        }
    }
}
