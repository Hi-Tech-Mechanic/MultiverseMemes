using UnityEngine;
using UnityEngine.UI;
using YG;

public class MainAudio : DataStructure
{
    public static bool AudioIsOn = true;

    [Header("»конки состо€ни€ общего звука")]
    [Space(10)]
    [SerializeField] private Image _targetImage;
    [SerializeField] private Sprite _on;
    [SerializeField] private Sprite _off;

    //private void Awake()
    //{
    //    if (YandexGame.SDKEnabled)
    //    {
    //        Init();
    //    }
    //}

    public void Init()
    {
        CheckMusicState();
    }

    #region ButtonEvents
    public void SwitchMainAudioActivity()
    {
        AudioEffects.PlayButtonClickEffect();
        AudioIsOn = !AudioIsOn;
        CheckMusicState();
    }

    private void CheckMusicState()
    {
        if (!AudioIsOn)
        {
            _targetImage.sprite = _off;
            AudioListener.pause = true;
        }
        else
        {
            _targetImage.sprite = _on;
            AudioListener.pause = false;
        }
    }
    #endregion
}