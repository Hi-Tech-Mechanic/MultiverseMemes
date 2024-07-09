using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartLearning : DataStructure
{
    public static bool StartLessonIsComplete = false;

    [SerializeField] private GameObject _lessonWindow;
    [SerializeField] private GameObject _narratorWindow;
    [SerializeField] private GameObject _nextMessageButton;
    [SerializeField] private TextMeshProUGUI _textMessage;
    [SerializeField] private GameObject _globalUpgradesButton;
    [SerializeField] private GameObject _indexArrow_1;
    [SerializeField] private GameObject _indexArrow_2;
    [SerializeField] private GameObject _multiverseButton;
    [SerializeField] private Animator _optionsAnimator;
    [SerializeField] private Animator _calendarAnimator;
    [SerializeField] private AudioSource _narratorAudioSource;
    [SerializeField] private AudioClip[] _audioClipBuffer;

    private const int _messageCount = 10;
    private const float _timeDivider = 0.5f;
    private const float _standardVolume = 0.5f;

    private readonly string[] _messageBuffer = new string[_messageCount];
    private int _currentMessage;
    private float _timeToNextSymbol;

    private void Init()
    {
        _lessonWindow.gameObject.SetActive(true);

        _messageBuffer[0] = "Здравствуйте достопочтенный человек, вы попали в немного абсурдный, но забавный мир, разбитый на параллельные вселенные мемов.";

        _messageBuffer[1] = "Здесь предстоит покупать и прокачивать разнообразные мемы, выкупать звуковые приколы, достигать множества достижений, да и просто наслаждаться кринжем.";

        _messageBuffer[2] = "Улучшения действуют локально на выбранную ветку реальности, однако в текущей - 'комбинированной' имеются глобальные улучшения они влияют на все остальные миры.";

        _messageBuffer[3] = "Зайдите и посмотрите на глобальные улучшения. Для их покупок потребуются мем коины - общепризнанная валюта во всех этих вселенных. У вас уже есть немного.";

        _messageBuffer[4] = "Хочу отметить что в игре реализованы гибкие настройки, можно выбирать разный фон, музыку, изменять тональность всех звуков и их режимы, что позволит создавать уникальные ситуации на сцене.";

        _messageBuffer[5] = "Не забывайте про календарь и мем-кейсы, чтобы каждый день получать подарки от доброго создателя.";

        _messageBuffer[6] = "Нажмите на портал, чтобы открыть меню выбора вселенных.";

        _messageBuffer[7] = "Рекомендую ознакомится с миром русов против ящеров, он кардинально отличается от всех остальных.";

        _messageBuffer[8] = "Если у вас есть идеи что добавить в игру или что улучшить в ней, то оставьте отзыв. Возможно именно ваше предложение попадёт в следующее обновление.";

        _messageBuffer[9] = "Я думаю дальше разберетесь сами. А мне пора идти на работу в Красти Краб и слушать тупые разговоры Спанч Боба.";
    }

    public void ActivateStartLearning()
    {
        if (StartLessonIsComplete == false)
        {
            Init();
            StartMessage();
        }
        else
        {
            Destroy(_lessonWindow);
        }
    }

    private void StartMessage()
    {
        _timeToNextSymbol = GetReadingSpeed(2);
        CheckMessageID(_currentMessage);
        PlayAudioClip();
        StartCoroutine(AddNextSymbol());
    }

    #region ButtonEvent
    public void NextMessage()
    {
        StopAllCoroutines();
        SetMusicVolumeToStandard();
        _currentMessage++;

        if (_currentMessage < _messageCount)
        {
            _timeToNextSymbol = GetReadingSpeed(0);
            CheckMessageID(_currentMessage);
            CheckEndLearning();
            ResetTextField();
            PlayAudioClip();
            StartCoroutine(AddNextSymbol());
        }
    }

    public void Step4_Event()
    {
        SwipeHeroWindowAxisOffset_Y(-300);
        _indexArrow_1.SetActive(false);
        _globalUpgradesButton.SetActive(false);
    }

    public void Step7_Event()
    {
        SwipeHeroWindowAxisOffset_Y(-300);
        _indexArrow_2.SetActive(false);
        NextMessage();
    }
    #endregion

    private IEnumerator AddNextSymbol()
    {
        int _savedMessageID = _currentMessage;

        if (_currentMessage <= _messageCount)
        {
            foreach (char symbol in _messageBuffer[_currentMessage])
            {
                _textMessage.text += symbol;
                yield return new WaitForSeconds(_timeToNextSymbol);
                if (_currentMessage != _savedMessageID)
                { break; }
            }
        }
    }

    private void SwipeHeroWindowAxisOffset_Y(float value)
    {
        Vector2 tmp_position;

        tmp_position = _narratorWindow.GetComponent<RectTransform>().anchoredPosition;
        tmp_position.y = value;
        _narratorWindow.GetComponent<RectTransform>().anchoredPosition = tmp_position;
    }

    private void EndLesson()
    {
        StartLessonIsComplete = true;
        _narratorAudioSource.Stop();
        Destroy(_lessonWindow);
    }

    private void CheckEndLearning()
    {
        if ((_messageCount - 1) == _currentMessage)
        {
            _nextMessageButton.GetComponentInChildren<TextMeshProUGUI>().text = "Завершить";
            Button tmp_button = _nextMessageButton.GetComponent<Button>();
            tmp_button.onClick.RemoveAllListeners();
            tmp_button.onClick.AddListener(EndLesson);
        }
    }

    private void SetMusicVolumeToStandard()
    {
        Music.GetMusicPlayer().volume = _standardVolume;
    }

    private void PlayAudioClip()
    {
        _narratorAudioSource.clip = _audioClipBuffer[_currentMessage];
        _narratorAudioSource.Play();
    }

    private void ResetTextField()
    { _textMessage.text = ""; }

    private float GetReadingSpeed(float adjustment)
    {
        float value = (_audioClipBuffer[_currentMessage].length - 0.1f - adjustment) / _messageBuffer[_currentMessage].Length / 1.5f;
        return value;
    }

    private void CheckMessageID(int curr_mes)
    {
        switch (curr_mes)
        {
            case (0):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                StartCoroutine(HideAndActiveNextMessageButton(_audioClipBuffer[_currentMessage].length * _timeDivider));
                break;
            case (1):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                StartCoroutine(HideAndActiveNextMessageButton(_audioClipBuffer[_currentMessage].length * _timeDivider));
                break;
            case (2):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                StartCoroutine(HideAndActiveNextMessageButton(_audioClipBuffer[_currentMessage].length * _timeDivider));
                break;
            case (3):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                StartCoroutine(HideAndActiveNextMessageButton(_audioClipBuffer[_currentMessage].length));
                _globalUpgradesButton.SetActive(true);
                _indexArrow_1.SetActive(true);
                break;
            case (4):
                SwipeHeroWindowAxisOffset_Y(-1570);
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                StartCoroutine(HideAndActiveNextMessageButton(_audioClipBuffer[_currentMessage].length * _timeDivider));

                if (_globalUpgradesButton.activeInHierarchy)
                {
                    _indexArrow_1.SetActive(false);
                    _globalUpgradesButton.SetActive(false);
                }
                else
                {
                    GlobalUpgrades.SequentialDisclosureUpgradeElements("Close");
                }

                _optionsAnimator.gameObject.SetActive(true);
                _optionsAnimator.SetTrigger("Active");
                break;
            case (5):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                StartCoroutine(HideAndActiveNextMessageButton(_audioClipBuffer[_currentMessage].length * _timeDivider));
                _optionsAnimator.SetTrigger("Close");
                StartCoroutine(OpenCalendar());

                IEnumerator OpenCalendar() 
                {
                    yield return new WaitForSeconds(0.4f);
                    _calendarAnimator.gameObject.SetActive(true);
                    _calendarAnimator.SetTrigger("Open");
                }
                break;
            case (6):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                _nextMessageButton.SetActive(false);
                SwipeHeroWindowAxisOffset_Y(-950);
                _calendarAnimator.SetTrigger("Close");
                _indexArrow_2.SetActive(true);
                _multiverseButton.gameObject.SetActive(true);
                break;
            case (7):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                StartCoroutine(HideAndActiveNextMessageButton(_audioClipBuffer[_currentMessage].length * _timeDivider));
                break;
            case (8):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                StartCoroutine(HideAndActiveNextMessageButton(_audioClipBuffer[_currentMessage].length * _timeDivider));
                break;
            case (9):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                StartCoroutine(HideAndActiveNextMessageButton(_audioClipBuffer[_currentMessage].length * _timeDivider));
                break;

            default: break;
        }
    }

    private IEnumerator RedactMusicVolume(float time)
    {
        Music.GetMusicPlayer().volume = 0.2f;
        yield return new WaitForSeconds(time);
        SetMusicVolumeToStandard();
    }

    private IEnumerator HideAndActiveNextMessageButton(float time)
    {
        _nextMessageButton.SetActive(false);
        yield return new WaitForSeconds(time);
        _nextMessageButton.SetActive(true);
    }
}
