using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class CaseMenu : DataStructure
{
    [SerializeField] private Transform _triggerTransform;

    [SerializeField] private TextMeshProUGUI _memeCoinsText;
    [SerializeField] private TextMeshProUGUI _nextCaseTimeText;
    [SerializeField] private TextMeshProUGUI _openedCasesCountText;
    [SerializeField] private TextMeshProUGUI _CasesLeftText;

    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _everydayRewardWindow;
    [SerializeField] private GameObject _attentionObject;

    [SerializeField] private Animator _windowAnimator;
    [SerializeField] private Animator _sliderAnimator;

    [SerializeField] private AudioClip _caseGetEffect;
    [SerializeField] private AudioClip _caseScrollEffect;

    [SerializeField] private Button _scrollingButton;
    [SerializeField] private Button _ADButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Scrollbar _contentScrollBar;

    [SerializeField] private GameObject[] _infoCells;
    [SerializeField] private CurrentCaseData[] _casesData;
    [SerializeField] private Color[] _qualityColor;

    [NonSerialized] public bool EverydayCaseIsTaken;

    public int OpenedCases
    {
        get { return _openedCases; }
        set { _openedCases = value; DisplayOpenedCases(); }
    }
    private int _openedCases;

    public int CasesCount
    {
        get { return _casesCount; }
        set { _casesCount = value; DisplayCasesCount(); CheckCasesCount(); }
    }
    private int _casesCount = 0;

    public int WatchedVideosInCurrentDay
    {
        get { return _watchedVideosInCurrentDay; }
        set { _watchedVideosInCurrentDay = value; DisplayADButtonText(); }
    }
    private int _watchedVideosInCurrentDay;

    private const int _dayStockVideos = 3;

    private const float _timeToScroll = 8f;
    private readonly int[] _memeCoinsRewards = { 50, 100, 200, 400, 800, 1500, 2500, 5000 };
    private readonly float[] _percentProbability = { 31f, 27f, 17f, 12f, 6.5f, 3.5f, 2f, 1f };

    private void Awake()
    {
        int counter = 0;

        foreach (GameObject cell in _infoCells)
        {
            TextMeshProUGUI[] textArr = cell.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in textArr)
            {
                switch (text.name)
                {
                    case ("DropPercent"):
                        text.text = $"{_percentProbability[counter]}%";
                        break;
                    case ("DropCount"):
                        text.text = $"{ValuesRounding.FormattingValue("", "", _memeCoinsRewards[counter])}";
                        break;
                }
            }

            counter++;
        }
    }

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += ADReward;
        Game.NewDayEvent += NewDayEntry;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= ADReward;
        Game.NewDayEvent -= NewDayEntry;
    }

    public void Init()
    {
        if (Game.FirstLaunchGame || EverydayCaseIsTaken == false)
            NewDayEntry();

        CheckWatchedVideosInCurrentDay();
        DisplayADButtonText();
    }

    public void StartScrolling()
    {
        if (_casesCount > 0)
        {
            float probability;
            int caseIndex;

            OpenedCases++;
            CasesCount--;

            AudioEffects.PlayOneShotEffect(_caseGetEffect);
            AudioEffects.PlayOneShotEffect(_caseScrollEffect);

            foreach (CurrentCaseData _case in _casesData)
            {
                SetCaseData();

                void SetCaseData()
                {
                    probability = UnityEngine.Random.Range(0f, 100f);
                    caseIndex = UnityEngine.Random.Range(0, _memeCoinsRewards.Length);

                    if (probability < _percentProbability[caseIndex])
                    {
                        _case.MemeCoinsCount = _memeCoinsRewards[caseIndex];
                        _case.QualityColor = _qualityColor[caseIndex];
                        _case.UpdateItemInfo();
                    }
                    else
                    {
                        SetCaseData();
                    }
                }
            }

            StartCoroutine(MoveItems());
        }

        IEnumerator MoveItems()
        {
            _ADButton.interactable = false;
            _exitButton.interactable = false;
            _scrollingButton.interactable = false;

            //_sliderAnimator.SetTrigger("Active");
            float timer = 0;
            float speed;

            while (timer < _timeToScroll)
            {
                yield return null;
                timer += Time.unscaledDeltaTime;
                speed = 2 / (1 + (timer / _timeToScroll));
                _contentScrollBar.value = (timer / _timeToScroll * speed) * 0.95f;
            }

            RaycastHit2D hitInfo = Physics2D.Raycast(_triggerTransform.position, _triggerTransform.up);
            if (hitInfo)
            {
                int value = hitInfo.transform.GetComponent<CurrentCaseData>().MemeCoinsCount;
                GetMoneyAnimation.CreateAndAddCoins(value, "CaseMenu");
            }

            _exitButton.interactable = true;
            CheckWatchedVideosInCurrentDay();
            CheckCasesCount();
        }
    }

    public void OpenWindow()
    {
        _background.gameObject.SetActive(true);
        _windowAnimator.SetTrigger("Open");
        UpdateWindowInfo();
    }

    public void CloseWindow()
    {
        if (_background.activeInHierarchy)
        {
            StartCoroutine(Coroutine());
        }

        IEnumerator Coroutine()
        {
            StopCountingDown();
            _windowAnimator.SetTrigger("Close");
            yield return new WaitForSeconds(0.4f);
            _background.gameObject.SetActive(false);
        }
    }

    public void GetCase()
    {
        CasesCount++;
        EverydayCaseIsTaken = true;
        AudioEffects.PlayOneShotEffect(_caseGetEffect);

        if (_attentionObject.activeInHierarchy)
            _attentionObject.SetActive(false);
    }

    public void GetCaseAD()
    {
        if (CheckWatchedVideosInCurrentDay())
        {
            YandexGame.RewVideoShow((int)Game.RewardIndex.CaseReward);
        }
    }

    private void ADReward(int index)
    {
        if (index== (int) Game.RewardIndex.CaseReward)
        {
            WatchedVideosInCurrentDay++;
            CheckWatchedVideosInCurrentDay();
            GetCase();
        }
    }

    private void UpdateWindowInfo()
    {
        _memeCoinsText.text = $"{ValuesRounding.FormattingValue("", "", MoneyMenu.GetMemeCoins())}";
        DisplayOpenedCases();
        DisplayCasesCount();

        StartCoroutine(TimeCountingDown());
    }

    private void StopCountingDown()
    {
        StopCoroutine(TimeCountingDown());
    }

    private IEnumerator TimeCountingDown()
    {
        while (Game.GetSecondsToNextDay() >= 0)
        {
            _nextCaseTimeText.text = $"До следующего кейса: {ValuesRounding.GetFormattedLongTime(Game.GetSecondsToNextDay())}";
            Game.CheckVisitingDays();
            yield return new WaitForSeconds(1);
        }
    }

    private void CheckCasesCount()
    {
        if (_casesCount > 0)
            _scrollingButton.interactable = true;
        else _scrollingButton.interactable = false;
    }

    private bool CheckWatchedVideosInCurrentDay()
    {
        if (_watchedVideosInCurrentDay < _dayStockVideos)
        {
            _ADButton.interactable = true;
            return true;
        }
        else
        {
            _ADButton.interactable = false;
            return false;
        }
    }

    private void NewDayEntry()
    {
        WatchedVideosInCurrentDay = 0;
        _everydayRewardWindow.SetActive(true);
        _attentionObject.SetActive(true);
    }

    private void DisplayADButtonText() => _ADButton.GetComponentInChildren<TextMeshProUGUI>().text = $"+1 кейс {_watchedVideosInCurrentDay}/{_dayStockVideos}";

    private void DisplayOpenedCases() => _openedCasesCountText.text = $"Кейсов открыто: {_openedCases}";
    private void DisplayCasesCount() => _CasesLeftText.text = $"Осталось: {_casesCount}";
}
