using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Learning : DataStructure
{
    public TextMeshProUGUI TextHero;
    public TextMeshProUGUI TextGrandfather;
    public AudioSource NarratorAudioSource;
    public GameObject HeroWindow;
    public GameObject Grandfather;
    public GameObject NextMessageButton;
    public GameObject NonClickableBackground;
    public GameObject ArsenalButton;
    public GameObject ParentWindow;
    public GameObject IndexArrow_0;
    public GameObject IndexArrow_1;
    public GameObject SlavsArsenalMenu;
    public GameObject SkillsMenu;
    public GameObject PerkMenu;
    public GameObject BattleButton;
    [SerializeField] private Button _finalMessageButton;
    [SerializeField] private AudioClip[] _audioClipBuffer;
    [SerializeField] private AudioClip _failureAudioClip;
    [SerializeField] private AudioClip _finalGameMessageFromHero;
    [SerializeField] private GameObject _nonClickableImage;

    private TextMeshProUGUI _textMessage;
    private readonly string[] _messageBuffer = new string[15];
    private string _failureMessage;
    private string _finalGameMessage;
    private float _timeToNextSymbol; // = 0.075f;
    private int _currentMessage;
    private int _messageCount;
    private GameObject _createdArrow;
    private const float _standardVolume = 0.5f;
    public static bool RussiansLessonIsFinished = false;

    private void Awake()
    {
        _failureMessage = "Как так то? Не подводи, на первый раз я восстановлю тебе все запасы водой Байкальской.";

        _finalGameMessage = "Вот это да, уважаю! Ты сразил финального босса ящеров и освободил наши земли, настоящий Древнерус! Ты прошел эту реальность, если хочешь - можешь и дальше продолжать всех уничтожать.";

        _messageBuffer[0] = "Здравствуй, раз ты решил посетить эту ветку реальности - " +
            "я расскажу тебе её предысторию. Давным давно, на древней Руси жили русичи православные. " +
            "Они были добрыми, смелыми, сильными. Но однажды их постигло ящерское иго, " +
            "которое уничтожило поселение древних славян. Они сражались как герои, однако сил" +
            " хитрых ящеров было гораздо больше. Вам мой друг предстоит восстановить былую мощь" +
            " славян, и отомстить этим ящерам.";

        _messageBuffer[1] = "Первым шагам тебя обучит Доброслав Никитич, знакомься.";

        _messageBuffer[2] = "Здравствуй новичок. Первое что тебе нужно знать это то, что " +
            "великорусы просто так не сдаются. Даже после ужасных травм при помощи силы предков, " +
            "чисто на морально волевых качествах мы двигаемся и развиваемся дальше.";

        _messageBuffer[3] = "Заходи в славянский арсенал.";

        _messageBuffer[4] = "Тут находятся уже доступные и еще не призванные богатыри, у каждого есть свои" +
            " характеристики и особенности. Их стоит снабдить комплектами брони и оружием, которые можно" +
            " улучшать при помощи нашей веры.";

        _messageBuffer[5] = "Перейдем в выбор способностей, я открою сам. Здесь предложены на выбор наши " +
            "способности, передающиеся из поколения в поколение. Их можно покупать, развивать, комбинировать" +
            " между собой.";

        _messageBuffer[6] = "Когда уничтожать ящеров станет слишком тяжело - можно сбросить прогресс. Не переживай, " +
            "в замен ты получишь силу предков. За них в древе навыков улучшаются аспекты для всех богатырей сразу, и они остаются навсегда.";

        _messageBuffer[7] = "При помощи этих улучшений ты можешь стать интеллектуалом, который уничтожает противников своими способностями. " +
            "Либо ловкачом который уворачивается от большинства атак, или развивать силушку богатырскую сокрушая противников руками. " +
            "Или вовсе быть универсалом.";

        _messageBuffer[8] = "Наконец перейдем на поле битвы. Заходи в бой!";

        _messageBuffer[9] = "Тут всегда играет фонк, это база, без которой Русы жить не могут. Это повышает боевой дух, да и просто по кайфу.";

        _messageBuffer[10] = "Первый уродец на подходе. Приготовься сражаться. Обрати внимание что у каждого существа отображены его характеристики. " +
            "И то что твоя атака стоит выносливости.";

        _messageBuffer[11] = "Бей!";

        _messageBuffer[12] = "Красавчик, так ему! Поступай так с каждым, потому что ящеры не церемонятся атакуя нас.";

        _messageBuffer[13] = "Если ты погибнешь, либо сбежишь с поля боя, богатырь окажется недоступен." +
            " Он восстановится сам через некоторое время, однако есть возможность оживить его с помощи водицы Байкальской." +
            " Но лучше выкладывайся в битве по полной, как наши предки.";

        _messageBuffer[14] = "Дальше я думаю разберёшься сам, уничтожь ящеров и восстанови мир на древней Руси, удачи!";

        _messageCount = _messageBuffer.Length - 1;
    }

    private void Start()
    {
        if (RussiansLessonIsFinished == false)
        {
            AudioListener.pause = false;
            Music.GetMusicPlayer().mute = false;
            Music.MusicIsMute = false;
            AudioEffects.GetAudioEffectPlayer().mute = false;
            AudioEffects.AudioEffectsIsMute = false;
            Music.MusicPitch = 1;
            Music.MusicVolume = _standardVolume;
            AudioEffects.AudioEffectsPitch = 1;
            AudioEffects.AudioEffectsVolume = _standardVolume;
            Music.SelectedMusicIndex = 0;

            if (Heroes.CurrentHero.IsAlive == false)
            {
                Heroes.CurrentHero.HeroIsResurrected();
                BattleHero.HeroIsResurrected();
            }

            StartMessage();
        }
    }

    private float GetReadingSpeed(float adjustment)
    {
        float value = (_audioClipBuffer[_currentMessage].length - 0.1f - adjustment) / _messageBuffer[_currentMessage].Length / 1.5f; // добвлен делитель 1.5 для мобильных устройств, так как текст слишком медленно прочитывается
        return value;
    }

    private void StartMessage()
    {
        _timeToNextSymbol = GetReadingSpeed(2);
        CheckMessageID(_currentMessage);
        PlayAudioClip();
        StartCoroutine(AddNextSymbol());
        _nonClickableImage.SetActive(true);
    }    

    public void NextMessage()
    {
        StopAllCoroutines();
        SetMusicVolumeToStandard();

        if (_currentMessage < _messageCount)
        {
            _currentMessage++;
            _timeToNextSymbol = GetReadingSpeed(0);
            CheckMessageID(_currentMessage);
            CheckEndLearning();
            ResetTextField();
            PlayAudioClip();
            StartCoroutine(AddNextSymbol());
        }
    }

    public void PlayFinalMessage()
    {
        AudioListener.pause = false;
        float cachedMusicVolume = Music.MusicVolume;
        Music.MusicVolume = 0.2f;

        HeroWindow.SetActive(true);
        NonClickableBackground.SetActive(true);

        _finalMessageButton.gameObject.SetActive(true);
        _finalMessageButton.onClick.AddListener(End);

        _timeToNextSymbol = (_finalGameMessageFromHero.length - 0.1f) / _finalGameMessage.Length / 1.5f;
        _textMessage = TextHero;
        ResetTextField();
        NarratorAudioSource.clip = _finalGameMessageFromHero;
        NarratorAudioSource.Play();
        StartCoroutine(AddNextSymbol());

        void End()
        {
            NarratorAudioSource.Stop();
            HeroWindow.SetActive(false);
            NonClickableBackground.SetActive(false);
            Music.MusicVolume = cachedMusicVolume;
        }

        IEnumerator AddNextSymbol()
        {
            foreach (char symbol in _finalGameMessage)
            {
                _textMessage.text += symbol;
                yield return new WaitForSeconds(_timeToNextSymbol);
            }
        }
    }

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

    private void CheckMessageID(int curr_mes)
    {
        if (curr_mes <= 1)
        {
            _textMessage = TextGrandfather;
            HeroWindow.SetActive(false);

            if (Grandfather.activeInHierarchy == false)
            { Grandfather.SetActive(true); }
        }
        else
        {
            _textMessage = TextHero;
            Grandfather.SetActive(false);

            if (HeroWindow.activeInHierarchy == false)
            { HeroWindow.SetActive(true); }
        }

        switch (curr_mes)
        {
            case (3):
                SlavsArsenalButtonActivate();
                NextMessageButton.SetActive(false);
                IndexArrow_0.SetActive(true);
                //IndexArrowInstantiate();
                //ExpansionAndСontractionObject(_createdArrow);
                break;
            case (4):
                HideArrow(0);
                NextMessageButton.SetActive(true);
                SwipeHeroWindowAxisOffset_Y(-500);
                break;
            case (5):              
                StartCoroutine(CloseAndOpenWindow_1());

                IEnumerator CloseAndOpenWindow_1()
                {
                    yield return new WaitForSeconds(2);
                    AudioEffects.PlayButtonClickEffect();
                    SlavsArsenalMenu.SetActive(false);
                    SkillsMenu.SetActive(false);
                    yield return new WaitForSeconds(1);
                    AudioEffects.PlayButtonClickEffect();
                    SkillsMenu.SetActive(true);
                    SwipeHeroWindowAxisOffset_Y(-840);
                }
                break;
            case (6):
                SwipeHeroWindowAxisOffset_Y(360);
                StartCoroutine(CloseAndOpenWindow_2());

                IEnumerator CloseAndOpenWindow_2()
                {
                    yield return new WaitForSeconds(2);
                    AudioEffects.PlayButtonClickEffect();
                    SkillsMenu.SetActive(false);
                    SlavsArsenalMenu.SetActive(false);
                    yield return new WaitForSeconds(1);
                    AudioEffects.PlayButtonClickEffect();                    
                    PerkMenu.SetActive(true);
                }
                break;
            case (8):
                BattleButton.SetActive(true);
                IndexArrow_1.SetActive(true);
                NextMessageButton.SetActive(false);
                StartCoroutine(CloseAndOpenWindow_3());
                SwipeHeroWindowAxisOffset_Y(-840);

                //var tmp_rotation = _createdArrow.transform.rotation.eulerAngles;
                //tmp_rotation.z = -340;
                //tmp_position = _createdArrow.transform.localPosition;
                //tmp_position.y = 545;
                //tmp_position.x = 47;
                //_createdArrow.transform.localRotation = Quaternion.Euler(tmp_rotation);
                //_createdArrow.transform.localPosition = tmp_position;
                //ExpansionAndСontractionObject(_createdArrow);

                IEnumerator CloseAndOpenWindow_3()
                {
                    yield return new WaitForSeconds(1);
                    AudioEffects.PlayButtonClickEffect();
                    SlavsArsenalMenu.SetActive(false);
                    PerkMenu.SetActive(false);
                    SkillsMenu.SetActive(false);
                }                
                break;
            case (9):
                SetMusicVolumeToStandard();
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                Music.SelectNextMusic();
                NextMessageButton.SetActive(true);
                HideArrow(1);
                SwipeHeroWindowAxisOffset_Y(200);
                break;
            case (10):
                SwipeHeroWindowAxisOffset_Y(-900);
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                break;
            case (11):
                NextMessageButton.SetActive(false);
                StartCoroutine(CloseAndOpenWindow_4());
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));

                IEnumerator CloseAndOpenWindow_4()
                {
                    yield return new WaitForSeconds(1.5f);
                    NonClickableBackground.SetActive(false);
                    HeroWindow.SetActive(false);
                    StagesMenu.GoToNextStage();

                    while (Battle.BattleIsActive)
                    {
                        yield return null;
                    }

                    NonClickableBackground.SetActive(true);

                    if (Heroes.CurrentHero.IsAlive == false)
                    {
                        HeroWindow.SetActive(true);
                        NarratorAudioSource.clip = _failureAudioClip;
                        NarratorAudioSource.Play();
                        _timeToNextSymbol = 0.075f;
                        StopAllCoroutines();
                        ResetTextField();
                        StartCoroutine(RedactMusicVolume(_failureAudioClip.length - 0.1f));
                        StartCoroutine(coroutine());

                        IEnumerator coroutine()
                        {
                            foreach (char symbol in _failureMessage)
                            {
                                _textMessage.text += symbol;
                                yield return new WaitForSeconds(_timeToNextSymbol);
                            }

                            Heroes.CurrentHero.HeroIsResurrected();
                            BattleHero.HeroIsResurrected();
                            StartCoroutine(CloseAndOpenWindow_4());
                        }
                    }
                    else
                    {
                        NextMessage();
                        NextMessageButton.SetActive(true);
                        HeroWindow.SetActive(true);
                    }
                }
                break;
            case (12):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                break;
            case (13):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                break;
            case (14):
                StartCoroutine(RedactMusicVolume(_audioClipBuffer[_currentMessage].length - 0.1f));
                break;

            default: break;
        }

        IEnumerator RedactMusicVolume(float time)
        {
            Music.GetMusicPlayer().volume = 0.07f;
            yield return new WaitForSeconds(time);
            SetMusicVolumeToStandard();
        }
    }

    public void SetMusicVolumeToStandard()
    {
        Music.GetMusicPlayer().volume = _standardVolume;
    }

    public void HideArrow(int index)
    {
        if (index == 0)
        {
            IndexArrow_0.SetActive(false);
        }
        else if (index == 1)
        {
            IndexArrow_1.SetActive(false);
        }
    }

    private void PlayAudioClip()
    {
        NarratorAudioSource.clip = _audioClipBuffer[_currentMessage];
        NarratorAudioSource.Play();
    }

    private void ResetTextField()
    { _textMessage.text = ""; }

    private void SlavsArsenalButtonActivate()
    { ArsenalButton.SetActive(true); }

    private void IndexArrowInstantiate()
    { 
        _createdArrow = Instantiate(IndexArrow_1, ParentWindow.transform);
        _createdArrow.name = "Arrow";
    }

    private void ExpansionAndСontractionObject(GameObject var)
    {
        Vector3 min_scale = var.transform.localScale;
        Vector3 max_scale = new(1.3f, 1.3f, 1.3f);
        float time = 1;

        StartCoroutine(ExpandSize());

        IEnumerator ExpandSize()
        {
            float timer = 0;

            while (timer < time)
            {
                var.transform.localScale = Vector3.Lerp(min_scale, max_scale, timer / time);
                timer += Time.deltaTime;
                yield return null;
            }
            StopCoroutine(ExpandSize());
            StartCoroutine(ContractionSize());
        }

        IEnumerator ContractionSize()
        {
            float timer = 0;

            while (timer < time)
            {
                var.transform.localScale = Vector3.Lerp(max_scale, min_scale, timer / time);
                timer += Time.deltaTime;
                yield return null;
            }
            StopCoroutine(ContractionSize());
            StartCoroutine(ExpandSize());
        }
    }

    private void CheckEndLearning()
    {
        if (_messageCount == _currentMessage)
        {
            NextMessageButton.GetComponentInChildren<TextMeshProUGUI>().text = "Завершить";
            Button tmp_button = NextMessageButton.GetComponent<Button>();
            tmp_button.onClick.RemoveAllListeners();
            tmp_button.onClick.AddListener(EndLesson);
        }
    }

    private void EndLesson()
    {
        HeroWindow.SetActive(false);
        Grandfather.SetActive(false);
        NarratorAudioSource.Stop();
        NonClickableBackground.SetActive(false);
        RussiansLessonIsFinished = true;
    }

    private void SwipeHeroWindowAxisOffset_Y(float value)
    {
        Vector2 tmp_position;

        tmp_position = HeroWindow.transform.localPosition;
        tmp_position.y = value;
        HeroWindow.transform.localPosition = tmp_position;
    }
}