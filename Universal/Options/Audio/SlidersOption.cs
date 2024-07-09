using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlidersOption : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private Slider _slider;
    private float _volumeDefaultValue = 0.5f;
    private int _pitchDefaultValue = 1;

    public void Start()
    {
        _slider = gameObject.GetComponent<Slider>();
        Display();
    }

    public void OnChanged()
    { Display(); }

    public void SetDefaultPitchValue()
    { _slider.value = _pitchDefaultValue; }

    public void SetDefaultVolumeValue()
    { _slider.value = _volumeDefaultValue; }

    private void Display()
    { _text.text = $"{(int)(_slider.value * 100)}%"; }
}