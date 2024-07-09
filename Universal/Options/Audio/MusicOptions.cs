using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class MusicOptions : DataStructure
{
    [NonSerialized] public int SelectedMusicIndex;
    [NonSerialized] public bool MusicIsMute = false;
    public float CurrentMusicTime
    {
        get { return _musicPlayer.time; }
        set { _currentMusicTime = value; }
    }
    private float _currentMusicTime;

    public float MusicVolume
    {
        get { return _musicPlayer.volume; } 
        set 
        {
            _musicPlayer.volume = value;
            _musicVolumeSlider.GetComponent<SlidersOption>().Start();
            _musicVolumeSlider.value = value;
        }
    }
    public float MusicPitch
    {
        get { return _musicPlayer.pitch; }
        set 
        { 
            _musicPlayer.pitch = value;
            _musicPitchSlider.GetComponent<SlidersOption>().Start();
            _musicPitchSlider.value = value;
        }
    }

    [Header("Иконки состояния музыки")]
    [Space(10)]
    [SerializeField] private Image _targetImage;
    [SerializeField] private Sprite _on;
    [SerializeField] private Sprite _off;

    [Header("Общее")]
    [Space(10)]
    [SerializeField] private AudioSource _musicPlayer;
    [SerializeField] private AudioClip[] _music;
    [SerializeField] private GameObject[] _trackCells;
    [SerializeField] private GameObject _playButton;
    [SerializeField] private GameObject _pauseButton;
    [SerializeField] private TextMeshProUGUI _currentPlayingTimeText;
    [SerializeField] private TextMeshProUGUI _currentTrackNameText;
    [SerializeField] private TextMeshProUGUI _musicLengthText;
    [SerializeField] private Slider _selectedTrackSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _musicPitchSlider;
    [SerializeField] private Color _selectedColor;

    private AudioClip[] _musics;
    private TextMeshProUGUI[] _tracksNames;
    private Image[] _tracksImages;
    private Animator _playButtonAnimator;
    private Animator _pauseButtonAnimator;
    private IEnumerator _changeSliderCoroutine;
    private Color _standardColor;
    private string[] _names;
    private static bool _cameFromRussiansScene = false;
    private bool _windowIsOn = false;

    private void Awake()
    {
        _musics = _music;
        _tracksImages = new Image[_musics.Length];
        _tracksNames = new TextMeshProUGUI[_musics.Length];
        _names = new string[_musics.Length];
        _standardColor = _trackCells[0].GetComponent<Image>().color;
        _playButtonAnimator = _playButton.GetComponent<Animator>();
        _pauseButtonAnimator = _pauseButton.GetComponent<Animator>();

        for (int i = 0; i < _musics.Length; i++)
        {
            _names[i] = _musics[i].name;
            _tracksImages[i] = _trackCells[i].GetComponent<Image>();
            _tracksNames[i] = _trackCells[i].GetComponentInChildren<TextMeshProUGUI>();
            _tracksNames[i].text = $"{_names[i]} ({ValuesRounding.GetFormattedTime(_musics[i].length)})";
        }
    }

    public void Init() //Start
    {
        if (Game.CurrentScene == (int)Game.BuildIndex.Russians_vs_Lizards)
        {
            if (SelectedMusicIndex >= 4)
            {
                SelectedMusicIndex -= 4; //количество не фонков
                _cameFromRussiansScene = true;
                _musicPlayer.clip = _musics[SelectedMusicIndex];
            }

            if (Learning.RussiansLessonIsFinished == true)
            {
                if (MusicIsMute == false)
                    PlayMusic();
            }
        }
        else
        {
            if (_cameFromRussiansScene)
            {
                _cameFromRussiansScene = false;
                SelectedMusicIndex += 4;
            }

            if (MusicIsMute == false)
                PlayMusic();
        }

        for (int i = 0; i < _musics.Length; i++)
        {
            SetColorForSelectedTrack(i);
        }

        CheckMusicState();
        UpdatePlayingInfo();
    } 

    #region ButtonsEvents
    public void SwitchStateWindowActivity()
    {
        _windowIsOn = !_windowIsOn;
    }

    public void SwitchMusicEffectsActivity()
    {
        AudioEffects.PlayButtonClickEffect();
        MusicIsMute = !MusicIsMute;
        _currentMusicTime = _musicPlayer.time;
        CheckMusicState();
    }

    private void CheckMusicState()
    {
        if (MusicIsMute)
        {
            _targetImage.sprite = _off;
            _musicPlayer.mute = true;
        }
        else
        {
            _targetImage.sprite = _on;
            _musicPlayer.mute = false;
        }
    }

    public void SelectNextMusic()
    {
        AudioEffects.PlayButtonClickEffect();
        ResetMusicPlayerTime();
        SetColorForUnselectedTrack(SelectedMusicIndex);
        SelectedMusicIndex++;
        PlayMusic();
        SetColorForSelectedTrack(SelectedMusicIndex);
        UpdatePlayingInfo();
    }

    public void SelectPreviousMusic()
    {
        AudioEffects.PlayButtonClickEffect();
        ResetMusicPlayerTime();
        SetColorForUnselectedTrack(SelectedMusicIndex);
        SelectedMusicIndex--;
        PlayMusic();
        SetColorForSelectedTrack(SelectedMusicIndex);
        UpdatePlayingInfo();
    }

    public void PauseMusic()
    {
        AudioEffects.PlayButtonClickEffect();
        MusicIsMute = false;
        _musicPlayer.Pause();
        _currentMusicTime = _musicPlayer.time;
        StartCoroutine(Animate());

        IEnumerator Animate()
        {
            _pauseButtonAnimator.SetTrigger("NonActive");
            yield return new WaitForSeconds(0.1f);
            _pauseButton.SetActive(false);
            _playButton.SetActive(true);
            _playButtonAnimator.SetTrigger("Active");
        }
    }

    public void UnPauseMusic()
    {
        AudioEffects.PlayButtonClickEffect();
        MusicIsMute = true;
        PlayMusic();
        UpdatePlayingInfo();
        StartCoroutine(Animate());

        IEnumerator Animate()
        {
            _playButtonAnimator.SetTrigger("NonActive");
            yield return new WaitForSeconds(0.1f);
            _playButton.SetActive(false);
            _pauseButton.SetActive(true);
            _pauseButtonAnimator.SetTrigger("Active");

        }
    }

    public void ChangePlayingTimeSlider()
    {
        if (_windowIsOn && Input.anyKeyDown) 
        {
            if (_selectedTrackSlider.value != 1)
            {
                _musicPlayer.time = _selectedTrackSlider.value * _musics[SelectedMusicIndex].length;
                _currentMusicTime = _musicPlayer.time;
            }
        }
    }

    public void ChangeMusicVolume()
    { 
        _musicPlayer.volume = _musicVolumeSlider.value;
    }

    public void ChangeMusicPitch()
    { 
        _musicPlayer.pitch = _musicPitchSlider.value; 
    }
    #endregion

    #region Getters
    public AudioSource GetMusicPlayer()
    { return _musicPlayer; }

    public AudioClip[] GetMusicBuffer()
    { return _music; }
    #endregion

    private void PlayMusic()
    {
        if (_changeSliderCoroutine != null)
            StopCoroutine(_changeSliderCoroutine);

        if (SelectedMusicIndex >= _music.Length)
        {
            SelectedMusicIndex = 0;
        }
        else if (SelectedMusicIndex < 0)
        {
            SelectedMusicIndex = _music.Length - 1;
        }

        _musicPlayer.clip = _music[SelectedMusicIndex];
        _musicPlayer.pitch = _musicPitchSlider.value;
        _musicPlayer.time = _currentMusicTime;
        _musicPlayer.Play();

        _changeSliderCoroutine = ChangeCurrentTrackSlider();
        StartCoroutine(_changeSliderCoroutine);
    }

    private IEnumerator ChangeCurrentTrackSlider()
    {
        int cachedMusicIndex = SelectedMusicIndex;
        UpdateSelectedTrackSlider(cachedMusicIndex);

        while (_selectedTrackSlider.value != 1)
        {
            DisplayMusicCurrentPlayingTime();
            UpdateSelectedTrackSlider(cachedMusicIndex);
            yield return null;
        }
        SelectNextMusic();
    }

    private void UpdatePlayingInfo()
    {
        DisplayCurrentMusicName();
        DisplayCurrentMusicLength();
        DisplayMusicCurrentPlayingTime();
        UpdateSelectedTrackSlider(SelectedMusicIndex);
    }

    private void SetColorForSelectedTrack(int i)
    {
        if (i == SelectedMusicIndex)
            _tracksImages[i].color = _selectedColor;
    }

    private void SetColorForUnselectedTrack(int i)
    {
        if (i == SelectedMusicIndex)
            _tracksImages[i].color = _standardColor;
    }

    private void DisplayCurrentMusicName()
    { 
        if (_musicPlayer.clip != null)
            _currentTrackNameText.text = _musicPlayer.clip.name; 
    }

    private void DisplayCurrentMusicLength()
    { 
        _musicLengthText.text = ValuesRounding.GetFormattedTime(_music[SelectedMusicIndex].length); 
    }

    private void DisplayMusicCurrentPlayingTime()
    {
        _currentPlayingTimeText.text = $"{ValuesRounding.GetFormattedTime(_musicPlayer.time)}";
    }

    private void UpdateSelectedTrackSlider(int musicIndex)
    {
        _selectedTrackSlider.value = _musicPlayer.time / _musics[musicIndex].length;
    }

    private void ResetMusicPlayerTime()
    { 
        _musicPlayer.time = 0;
        _currentMusicTime = 0;
    }
}