using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class AchievementUnit : DataStructure
{
    [NonSerialized] public bool RewardIsReady = false;
    [NonSerialized] public bool Claimed = false;
    [NonSerialized] public int UnitIndex;
    [NonSerialized] public int RewardID;
    [NonSerialized] private GameObject _getRewardMenu;
    [NonSerialized] private GameObject _claimedVision;
    [NonSerialized] private Image _achievementImage;
    [NonSerialized] private TextMeshProUGUI Description;
    [NonSerialized] private TextMeshProUGUI _progressValue;
    [NonSerialized] private TextMeshProUGUI _rewardValue;
    [NonSerialized] private TextMeshProUGUI _progressPercent;
    [NonSerialized] private TextMeshProUGUI _rewardMultiplier;
    [NonSerialized] private Slider Progressbar;
    [NonSerialized] private Button AverageRewardButton;
    [NonSerialized] private Button ADRewardButton;
    [NonSerialized] private RectTransform _unitRect;
    [NonSerialized] private double _currentProgress;
    [NonSerialized] private double _targetProgress;
    [NonSerialized] private float _reward;
    [NonSerialized] private const float _expandedHeigth = 225;
    [NonSerialized] private Vector2 _defaultSize;
    [NonSerialized] private Vector2 _expandedSize;

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += ADReward;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= ADReward;
    }

    private void Start()
    {
        DisplayProgressInformation();
        DisplayReward();
        DisplayRewardMultiplier();

        if (Claimed)
        {
            TemplateReceivedReward();
        }
    }

    public void Initialize()
    {
        _unitRect = gameObject.GetComponent<RectTransform>();

        float width = _unitRect.sizeDelta.x;
        float heigth = _unitRect.sizeDelta.y;

        _defaultSize = new Vector2(width, heigth);
        _expandedSize = new Vector2(width, _expandedHeigth);

        FindComponents();
    }

    #region Getters|Setters
    public void SetCurrentProgress(float value)
    { 
        _currentProgress = value;
        CheckExecution();
        DisplayProgressInformation();
    }

    public void SetDescription(string message)
    { Description.text = message; }

    public void SetReward(float value)
    { _reward = value; }

    public void SetTargetProgress(double value)
    { _targetProgress = value; }

    public void SetAchievementImage(Sprite sprite)
    { _achievementImage.sprite = sprite; }

    public float GetProgress()
    { return (float)(_currentProgress / _targetProgress); }
    #endregion

    #region Displaying
    public void DisplayProgressInformation()
    {
        DisplayProgressbar();
        DisplayValue();
        DisplayProgressPercent();
    }

    private void DisplayValue()
    { _progressValue.text = ($"{ValuesRounding.FormattingValue("", "", _currentProgress)}" +
                             $"/{ValuesRounding.FormattingValue("", "", _targetProgress)}"); }

    private void DisplayProgressbar()
    { Progressbar.value = (float)(_currentProgress / _targetProgress); }

    private void DisplayProgressPercent() 
    { _progressPercent.text = $"{Math.Round((_currentProgress / _targetProgress * 100), 2)}%"; }

    private void DisplayReward() 
    { _rewardValue.text = ValuesRounding.FormattingValue("", "", _reward); }

    public void DisplayRewardMultiplier()
    { _rewardMultiplier.text = ValuesRounding.FormattingValue("Забрать ", "x", GlobalUpgrades.ADRewardMultiplier); }
    #endregion

    #region Rewards
    private void GetReward()
    {
        GetMoneyAnimation.CreateAndAddCoins(_reward, "Achievement");
        TemplateReceivedReward();
    }

    private void GetADReward()
    {
        YandexGame.RewVideoShow(RewardID);
    }

    private void ADReward(int rewardID)
    {
        if (rewardID == RewardID)
        {
            Game.AccumulateWatchedAD();

            GetMoneyAnimation.CreateAndAddCoins(_reward * GlobalUpgrades.ADRewardMultiplier, "Achievement");
            TemplateReceivedReward();

            StopAllCoroutines();
            StartCoroutine(ExpectToReceiveMemeCoins());

            IEnumerator ExpectToReceiveMemeCoins()
            {
                yield return new WaitForSeconds(2);
                SaveAndLoad.SavePlayerData();
            }
        }
    }

    private void TemplateReceivedReward()
    {
        Claimed = true;
        GlobalUpgrades.AchievementsRewardsIsClaimed[Game.CurrentScene, UnitIndex] = Claimed;
        RewardIsReady = false;
        _claimedVision.SetActive(true);
        _getRewardMenu.SetActive(false);
        _unitRect.sizeDelta = _defaultSize;
        gameObject.transform.SetAsLastSibling();
    }
    #endregion

    private void FindComponents()
    {
        RectTransform[] Elements = gameObject.GetComponentsInChildren<RectTransform>(includeInactive: true);
        foreach (RectTransform element in Elements)
        {
            switch (element.name)
            {
                case ("Value"):
                    _progressValue = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("RewardValue"):
                    _rewardValue = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("Description"):
                    Description = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("Progress Percent"):
                    _progressPercent = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("RewardMultiplier"):
                    _rewardMultiplier = element.GetComponent<TextMeshProUGUI>();
                    break;
                case ("Progressbar"):
                    Progressbar = element.GetComponent<Slider>();
                    break;
                case ("AverageReward"):
                    AverageRewardButton = element.GetComponent<Button>();
                    AverageRewardButton.onClick.AddListener(GetReward);
                    break;
                case ("ADReward"):
                    ADRewardButton = element.GetComponent<Button>();
                    ADRewardButton.onClick.AddListener(GetADReward);
                    break;
                case ("AchievementImage"):
                    _achievementImage = element.GetComponent<Image>();
                    break;
                case ("GetRewardMenu"):
                    _getRewardMenu = element.gameObject;
                    break;
                case ("ClaimedVision"):
                    _claimedVision = element.gameObject;
                    break;
            }
        }
    }

    private void CheckExecution()
    {
        if (_currentProgress >= _targetProgress && !RewardIsReady)
        {
            _currentProgress = _targetProgress;
            _getRewardMenu.SetActive(true);
            RewardIsReady = true;
            _unitRect.sizeDelta = _expandedSize;
            gameObject.transform.SetAsFirstSibling();
        }                
    }
}