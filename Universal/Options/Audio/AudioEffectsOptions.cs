using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Random = UnityEngine.Random;

public class AudioEffectsOptions : MonoBehaviour
{
    [NonSerialized] public bool AudioEffectsIsMute = false;
    [NonSerialized] public bool IdleAudioEffectsIsOn = true;
    [NonSerialized] public bool AudioEffectsIsLayering = true;

    public float AudioEffectsVolume
    {
        get { return _audioEffectPlayer.volume; }
        set
        {
            _audioEffectPlayer.volume = value;
            _audioEffectsVolumeSlider.GetComponent<SlidersOption>().Start();
            _audioEffectsVolumeSlider.value = value;
        }
    }
    public float AudioEffectsPitch
    {
        get { return _audioEffectPlayer.pitch; }
        set
        {
            _audioEffectPlayer.pitch = value;
            _audioEffectsPitchSlider.GetComponent<SlidersOption>().Start();
            _audioEffectsPitchSlider.value = value;
        }
    }

    [Header("Иконки состояния звуковых эффектов")]
    [Space(10)]
    [SerializeField] private Image _targetImage;
    [SerializeField] private Sprite _on;
    [SerializeField] private Sprite _off;

    [Header("Общее")]
    [Space(10)]
    [Tooltip("Проигрыватель звуковых эффектов")]
    [SerializeField] private AudioSource _audioEffectPlayer;
    [SerializeField] private AudioClip _averageButtonClip;
    [SerializeField] private AudioClip _createCoins;
    [SerializeField] private AudioClip _getCoins;
    [SerializeField] private AudioClip _upgradeEffect_1;
    [SerializeField] private AudioClip _purchaseEffect;
    [SerializeField] private Slider _audioEffectsVolumeSlider;
    [SerializeField] private Slider _audioEffectsPitchSlider;
    [SerializeField] private Toggle _idleAudioEffectsToggle;
    [SerializeField] private Toggle _AudioEffectsLayeringToggle;

    [Header("Русы против ящеров")]
    [Space(10)]
    [Tooltip("Проигрыватель звуковых эффектов существа")][SerializeField] private AudioSource _statementsPlayer;
    [Tooltip("Проигрыватель звуковых эффектов по одному разу")][SerializeField] private AudioSource OnePlayer;
    [Tooltip("Звук улучшения")][SerializeField] private AudioClip _upgradeEffect_2;
    [Tooltip("Раскрывающееся окно характеристик")][SerializeField] private AudioClip _extendedList;
    [Tooltip("Раскрытие одного стата")][SerializeField] private AudioClip _statsDisclosure;
    [Tooltip("Выбор предмета")][SerializeField] private AudioClip _selectItem;
    [Tooltip("Звуки наковальни")][SerializeField] private AudioClip[] _soundsOfAnAnvil;

    [Tooltip("Буфер звуков")][NonSerialized] public List<AudioClip> AviableMemeClips = new(1);

    //private void Awake()
    //{
    //    if (YandexGame.SDKEnabled)
    //    {
    //        Init();
    //    }
    //}

    public void Init() //Start
    {
        CheckAudioEffectsState();
        CheckToggles();
    }

    #region PlayingAudio
    public void PlayMemeClip()
    {
        if (AviableMemeClips.Count != 0)
        {
            int rnd = Random.Range(0, AviableMemeClips.Count);
            AudioClip clip = AviableMemeClips[rnd];

            _audioEffectPlayer.pitch = _audioEffectsPitchSlider.value;

            if (AudioEffectsIsLayering)
            {
                _audioEffectPlayer.PlayOneShot(clip);
            }
            else
            {
                if (_audioEffectPlayer.isPlaying == false)
                {
                    _audioEffectPlayer.PlayOneShot(clip);
                }
            }
        }
    }

    public void PlayButtonClickEffect()
    { PlayOneShotEffect(_averageButtonClip); }

    public void PlayUpgradeEffect_1()
    { PlayOneShotEffect(_upgradeEffect_1); }

    public void PlayUpgradeEffect_2()
    { PlayOneShotEffect(_upgradeEffect_2); }

    public void PlayPurchaseEffect()
    { PlayOneShotEffect(_purchaseEffect); }

    public void PlaySharpeningEffect()
    {
        int rnd = Random.Range(0, _soundsOfAnAnvil.Length);
        PlayOneShotEffect(_soundsOfAnAnvil[rnd]);
    }

    public void PlayCreateCoinEffect()
    { PlayOneShotEffect(_createCoins); }

    public void PlayGetCoinEffect()
    { PlayOneShotEffect(_getCoins); }

    public void PlayStatsDisclosure()
    { PlayOneShotEffect(_statsDisclosure); }

    public void PlayExtendedList()
    { PlayOneShotEffect(_extendedList); }

    public void PlayOneShotEffect(AudioClip clip)
    { _audioEffectPlayer.PlayOneShot(clip); }

    public void PlayArsenalItemSelect()
    {
        OnePlayer.clip = _selectItem;
        if (OnePlayer.isPlaying != true)
            OnePlayer.Play();
    }

    public void PlayEntitiesAudioEffects(AudioClip[] clips_array, string type)
    {
        int random_clip = Random.Range(0, clips_array.Length);

        if (type == "Play")
        {
            if (!_statementsPlayer.isPlaying)
            {
                _statementsPlayer.clip = clips_array[random_clip];
                _statementsPlayer.Play();
            }
        }
        else if (type == "PlayOneShot")
            PlayOneShotEffect(clips_array[random_clip]);
    }
    #endregion

    #region ButtonEvents
    public void SwitchAudioEffectsLayering()
    {
        PlayButtonClickEffect();
        AudioEffectsIsLayering = !AudioEffectsIsLayering;
    }

    public void SwitchAudioEffectsActivity()
    {
        PlayButtonClickEffect();
        AudioEffectsIsMute = !AudioEffectsIsMute;
        CheckAudioEffectsState();
    }

    public void SwitchIdleAudioEffectActivity()
    {
        PlayButtonClickEffect();
        IdleAudioEffectsIsOn = !IdleAudioEffectsIsOn;
    }

    public void ChangeAudioEffectVolume()
    { 
        _audioEffectPlayer.volume = _audioEffectsVolumeSlider.value;

        if (_statementsPlayer != null && OnePlayer != null)
        {
            _statementsPlayer.volume = _audioEffectsVolumeSlider.value;
            OnePlayer.volume = _audioEffectsVolumeSlider.value;
        }
    }

    public void ChangeAudioEffecPitch()
    { 
        _audioEffectPlayer.pitch = _audioEffectsPitchSlider.value; 

        if (_statementsPlayer != null && OnePlayer != null)
        {
            _statementsPlayer.pitch = _audioEffectsPitchSlider.value;
            OnePlayer.pitch = _audioEffectsPitchSlider.value;
        }
    }
    #endregion

    #region Getters
    public AudioSource GetAudioEffectPlayer()
    { return _audioEffectPlayer; }
    #endregion

    private void CheckAudioEffectsState()
    {
        if (AudioEffectsIsMute)
        {
            _targetImage.sprite = _off;
            GetAudioEffectPlayer().mute = true;
        }
        else
        {
            _targetImage.sprite = _on;
            GetAudioEffectPlayer().mute = false;
        }
    }

    private void CheckToggles()
    {
        bool cachedState = IdleAudioEffectsIsOn;
        bool cachedState_2 = AudioEffectsIsLayering;

        _idleAudioEffectsToggle.isOn = cachedState;
        IdleAudioEffectsIsOn = cachedState;

        _AudioEffectsLayeringToggle.isOn = cachedState_2;
        AudioEffectsIsLayering = cachedState_2;
    }
}