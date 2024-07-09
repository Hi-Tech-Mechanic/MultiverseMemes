using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Upgrades : DataStructure
{
    [Header ("======CLICK======")]
    [SerializeField] private Button _clickMenuButton;
    [SerializeField] private GameObject _clickWindow;
    [SerializeField] private GameObject _clickUpgradesBuffer;
    [SerializeField] private Sprite[] _clickUpgradeSprites;
    public int ClickCellsCount => _clickUpgradeSprites.Length;

    [Header("======IDLE======")]
    [SerializeField] private Button _idleMenuButton;
    [SerializeField] private GameObject _idleWindow;
    [SerializeField] private GameObject _idleUpgradesBuffer;
    [SerializeField] private Sprite[] _idleUpgradeSprites;
    public int IdleCellsCount => _idleUpgradeSprites.Length;

    public int[,] AccumulatedPurchasedClickUpgrades
    { 
        get { return _accumulatedPurchasedClickUpgrades; }
        set { _accumulatedPurchasedClickUpgrades = value; }
    }    
    [NonSerialized] private int[,] _accumulatedPurchasedClickUpgrades = new int[Game.ScenesCount, Game.MaxCellCountInGame];

    public int[,] AccumulatedPurchasedIdleUpgrades 
    { 
        get { return _accumulatedPurchasedIdleUpgrades; }
        set { _accumulatedPurchasedIdleUpgrades = value; }
    }
    [NonSerialized] private int[,] _accumulatedPurchasedIdleUpgrades = new int[Game.ScenesCount, Game.MaxCellCountInGame];

    private const float _incomeMultiplicationFromNewLevel = 4f;
    private const float _clickIncomeFactor = 2f;
    private const float _initialIncome = 0.2f;
    private const float _initialPrice = 10f;
    private const float _nextUpgradeIncomeMultiplication = 3f;
    private const float _nextUpgradePriceMultiplication = 3.1f;
    private static readonly int[] _purchasedAmountToNextLevel = { 10, 25, 50, 100, 150, 200, 250, 300, 350, 400, 450, 500 };

    [Header("Ìåíþ óëó÷øåíèé")]
    [SerializeField] private GameObject _mainAttentionImage;
    [SerializeField] private Color _passiveHeaderButtonColor;
    [SerializeField] private Color _activeHeaderButtonColor;
    [SerializeField] private Color _passiveUpgradeButtonColor;
    [SerializeField] private Color _activeUpgradeButtonColor;

    [Header("Êíîïêè ìíîæåñòâåííîé ïîêóïêè")]
    [SerializeField] private Button[] _multiplePurchaseButtons;
    [SerializeField] private Color _selectedMultiplePurchaseButton;
    [SerializeField] private Color _deselectedMultiplePurchaseButton;

    private static int _currentScene => Game.CurrentScene;
    public static bool _maxBuyAmountFactor = false;
    private bool _canBuyClickUpgrade;
    private bool _canBuyIdleUpgrade;
    private const int _maxUpgradeAmount = 500;

    public enum Type
    {
        Click = 0,
        Idle = 1
    }

    public class SampleUpgrade
    {
        public Image[] UpgradeImage;
        public Button[] UpgradeButton;
        public TextMeshProUGUI[] UpgradeName;
        public TextMeshProUGUI[] AdditiveIncomeText;
        public TextMeshProUGUI[] UpgradePriceText;
        public TextMeshProUGUI[] SummIncomeText;
        private GameObject[] _cells;
        private GameObject[] _maxAmountMessage;
        private TextMeshProUGUI[] _maxAmountText;
        private TextMeshProUGUI[] _nextLevelAmountText;
        private TextMeshProUGUI[] _currentBuyAmountText;
        private Slider[] _maxCountProgressBar;
        private Slider[] _nextLevelProgressBar;
        public float[,] Income = new float[Game.ScenesCount, Game.MaxCellCountInGame];
        public float[,] CachedIncome = new float[Game.ScenesCount, Game.MaxCellCountInGame];
        public float[,] SummIncome = new float[Game.ScenesCount, Game.MaxCellCountInGame];
        public float[,] Prices = new float[Game.ScenesCount, Game.MaxCellCountInGame];
        public float[,] CachedPrice = new float[Game.ScenesCount, Game.MaxCellCountInGame];
        public int[,] ReceivedLevels = new int[Game.ScenesCount, Game.MaxCellCountInGame];
        public int[,] CachedReceivedLevels = new int[Game.ScenesCount, Game.MaxCellCountInGame];
        public int[,] PurchasedUpgradeAmount = new int[Game.ScenesCount, Game.MaxCellCountInGame];
        public int[,] CurrentBuyAmount = new int[Game.ScenesCount, Game.MaxCellCountInGame];
        private int cellCount;

        public void InitializeCell(GameObject upradesBuffer)
        {
            cellCount = upradesBuffer.transform.childCount;

            _cells = new GameObject[cellCount];
            UpgradeImage = new Image[cellCount];
            UpgradeButton = new Button[cellCount];
            _maxAmountMessage = new GameObject[cellCount];
            UpgradeName = new TextMeshProUGUI[cellCount];
            AdditiveIncomeText = new TextMeshProUGUI[cellCount];
            SummIncomeText = new TextMeshProUGUI[cellCount];
            UpgradePriceText = new TextMeshProUGUI[cellCount];
            _maxAmountText = new TextMeshProUGUI[cellCount];
            _nextLevelAmountText = new TextMeshProUGUI[cellCount];
            _currentBuyAmountText = new TextMeshProUGUI[cellCount];
            _maxCountProgressBar = new Slider[cellCount];
            _nextLevelProgressBar = new Slider[cellCount];

            //for (int i = 0; i < cellCount; i++)
            //{
            //    for (int j = 0; j < Game.ScenesCount; j++)
            //    {
            //        CachedPrice[j, i] = Price[j, i];
            //        CachedIncome[j, i] = Income[j, i];
            //        CachedReceivedLevels[j, i] = ReceivedLevels[j, i];
            //        CurrentBuyAmount[j, i] = 1;
            //    }
            //}

            int k = 0;
            foreach (Transform transform in upradesBuffer.transform)
                _cells[k++] = transform.gameObject;

            int index = 0;
            foreach (GameObject cell in _cells)
            {
                RectTransform[] components = cell.GetComponentsInChildren<RectTransform>(includeInactive: true);
                foreach (RectTransform component in components)
                {
                    switch (component.name)
                    {
                        case ("Cell Image"):
                            UpgradeImage[index] = component.GetComponent<Image>();
                            break;
                        case ("Button"):
                            UpgradeButton[index] = component.GetComponent<Button>();
                            break;
                        case ("Text_maximum"):
                            _maxAmountMessage[index] = component.gameObject;
                            break;
                        case ("Name"):
                            UpgradeName[index] = component.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("AdditiveValue"):
                            AdditiveIncomeText[index] = component.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("General_Income"):
                            SummIncomeText[index] = component.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("Price"):
                            UpgradePriceText[index] = component.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("MaxCountText"):
                            _maxAmountText[index] = component.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("CountToNextLevelText"):
                            _nextLevelAmountText[index] = component.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("CurrentBuyAmount"):
                            _currentBuyAmountText[index] = component.GetComponent<TextMeshProUGUI>();
                            break;
                        case ("MaxCount Slider"):
                            _maxCountProgressBar[index] = component.GetComponent<Slider>();
                            break;
                        case ("CountToNextLevel Slider"):
                            _nextLevelProgressBar[index] = component.GetComponent<Slider>();
                            break;
                        default: break;
                    }
                }
                index++;
            }
        }

        public void CalcStartIncomeAndPrices(bool isClickUpgrade)
        {
            int subtract;
            int startCell;

            if (Game.MaxCellCountInGame > YandexGame.savesData.MaxCellCountInGame && YandexGame.savesData.MaxCellCountInGame != 0)
            {
                startCell = YandexGame.savesData.MaxCellCountInGame - 1;
            }
            else
            {
                startCell = 0;
            }

            for (int scene = 0; scene < Game.ScenesCount; scene++)
            {
                if (scene == (int)Game.BuildIndex.Paranormal_Universe
                    || scene == (int)Game.BuildIndex.Papich_Universe)
                {
                    subtract = 1;
                }
                else subtract = 0;

                for (int cellIndex = startCell; cellIndex < Game.MaxCellCountInGame; cellIndex++)
                {
                    if (cellIndex == 0)
                    {
                        if (isClickUpgrade)
                        {
                            Income[scene, cellIndex] = _initialIncome * _clickIncomeFactor;
                        }
                        else
                        {
                            Income[scene, cellIndex] = _initialIncome;
                        }

                        Prices[scene, cellIndex] = _initialPrice;
                    }
                    else
                    {
                        Income[scene, cellIndex] = Income[scene, cellIndex - 1] * (_nextUpgradeIncomeMultiplication - subtract);
                        Prices[scene, cellIndex] = Prices[scene, cellIndex - 1] * (_nextUpgradePriceMultiplication - subtract);
                    }
                }
            }
        }

        public void CalculatePurchaseAmount(int cellIndex, float upgradeCostMultiplier)
        {
            int purchasedAmount;
            int buyAmount;
            int receivedLevels;
            float cachedPrice;
            float cachedIncome;
            float priceSumm = 0;
            float incomeSumm = 0;

            purchasedAmount = PurchasedUpgradeAmount[_currentScene, cellIndex];
            buyAmount = CurrentBuyAmount[_currentScene, cellIndex];
            receivedLevels = ReceivedLevels[_currentScene, cellIndex];
            cachedPrice = Prices[_currentScene, cellIndex];
            cachedIncome = Income[_currentScene, cellIndex];

            if (buyAmount == 1)
            {
                buyAmount = 0;
                priceSumm = cachedPrice;
                incomeSumm += cachedIncome;
            }

            for (int i = 0; i < buyAmount; i++)
            {
                if (purchasedAmount < _maxUpgradeAmount)
                {
                    if (purchasedAmount >= _purchasedAmountToNextLevel[receivedLevels])
                    {
                        cachedIncome *= _incomeMultiplicationFromNewLevel;
                        receivedLevels++;
                    }

                    cachedPrice *= upgradeCostMultiplier;
                    priceSumm += cachedPrice;
                    incomeSumm += cachedIncome;
                    purchasedAmount++;
                }
                else break;
            }

            CachedIncome[_currentScene, cellIndex] = incomeSumm;
            CachedPrice[_currentScene, cellIndex] = priceSumm;
            CachedReceivedLevels[_currentScene, cellIndex] = receivedLevels;
            //if (cellIndex == 0)
            //{
            //    Debug.Log($"receivedLevels_1 = {receivedLevels}");
            //}
        }

        public void CalculateMaximumPurchaseAmount(int cellIndex, float moneyCapital, float upgradeCostMultiplier)
        {
            int purchasedAmount;
            int receivedLevels;
            float cachedPrice;
            float secondCachedPrice;
            float cachedIncome;
            float secondCachedIncome;
            float summPrice;
            float summIncome = 0;

            purchasedAmount = PurchasedUpgradeAmount[_currentScene, cellIndex];
            receivedLevels = ReceivedLevels[_currentScene, cellIndex];
            summPrice = Prices[_currentScene, cellIndex];
            cachedPrice = CachedPrice[_currentScene, cellIndex];
            cachedIncome = Income[_currentScene, cellIndex];

            for (; summPrice <= moneyCapital;)
            {
                if (purchasedAmount < _maxUpgradeAmount)
                {
                    if (purchasedAmount >= _purchasedAmountToNextLevel[receivedLevels])
                    {
                        cachedIncome *= _incomeMultiplicationFromNewLevel;
                        receivedLevels++;
                    }

                    secondCachedPrice = cachedPrice;
                    secondCachedIncome = cachedIncome;
                    cachedPrice *= upgradeCostMultiplier;
                    summPrice += cachedPrice;
                    summIncome += cachedIncome;
                    CurrentBuyAmount[_currentScene, cellIndex]++;
                    purchasedAmount++;

                    if (summPrice > moneyCapital)
                    {
                        purchasedAmount--;

                        summPrice -= cachedPrice; 
                        summIncome -= cachedIncome - secondCachedIncome;

                        if (purchasedAmount < _purchasedAmountToNextLevel[receivedLevels] && receivedLevels > 0)
                        {
                            cachedIncome /= _incomeMultiplicationFromNewLevel;
                            receivedLevels--;
                        }
                        break;
                    }
                }
                else break;
            }

            CachedPrice[_currentScene, cellIndex] = summPrice;
            CachedIncome[_currentScene, cellIndex] = summIncome;
            CachedReceivedLevels[_currentScene, cellIndex] = receivedLevels;
            //if (cellIndex == 0)
            //    Debug.Log($"receivedLevels_2 = {receivedLevels}");
        }

        public void UpdateAllCellValues(int cellIndex)
        {
            DisplayCellAdditivesIncome(cellIndex);
            DisplayUpgradePrice(cellIndex);
            DisplayCurrentBuyAmount(cellIndex);
            DisplayNextLevelAmount(cellIndex);
            DisplayMaxAmount(cellIndex);
        }

        public void DisplayCellAdditivesIncome(int cellIndex)
        {
            DisplayIncome(cellIndex);
            DisplaySummIncome(cellIndex);
        }

        public void CheckBuyAmount(int cellIndex)
        {
            if ((CurrentBuyAmount[_currentScene, cellIndex] + PurchasedUpgradeAmount[_currentScene, cellIndex]) > _maxUpgradeAmount)
            {
                CurrentBuyAmount[_currentScene, cellIndex] = _maxUpgradeAmount - PurchasedUpgradeAmount[_currentScene, cellIndex];
            }
        }

        public void AddPrice(int cellIndex, float value)
        { 
            Prices[_currentScene, cellIndex] += value;
            CachePrice(cellIndex);
            DisplayUpgradePrice(cellIndex);
        }

        public void AddSummIncome(int cellIndex, float income)
        {
            SummIncome[_currentScene, cellIndex] += income;
            DisplaySummIncome(cellIndex);
        }

        public void AddPurchasedUpgradeAmount(int cellIndex, int purchasedAmount)
        {
            PurchasedUpgradeAmount[_currentScene, cellIndex] += purchasedAmount;
            DisplayNextLevelAmount(cellIndex);
            DisplayMaxAmount(cellIndex);
            CheckLevelUp(cellIndex);
            CheckMaxPurchaseAmount(cellIndex);
        }

        public void CachePrice(int cellIndex)
        { CachedPrice[_currentScene, cellIndex] = Prices[_currentScene, cellIndex]; }

        public void CacheIncome(int cellIndex)
        { CachedIncome[_currentScene, cellIndex] = Income[_currentScene, cellIndex]; }

        public void DisplayCurrentBuyAmount(int cellIndex)
        { _currentBuyAmountText[cellIndex].text = $"+{CurrentBuyAmount[_currentScene, cellIndex]}"; }

        public void DisplayUpgradePrice(int cellIndex)
        { UpgradePriceText[cellIndex].text = ValuesRounding.FormattingValue("", "$", CachedPrice[_currentScene, cellIndex]); }

        private void DisplayIncome(int cellIndex)
        { 
            if (cellIndex < cellCount)
                AdditiveIncomeText[cellIndex].text = ValuesRounding.FormattingValue("+", "$", CachedIncome[_currentScene, cellIndex]); 
        }

        private void DisplaySummIncome(int cellIndex)
        {
            if (cellIndex < cellCount)
                SummIncomeText[cellIndex].text = ValuesRounding.FormattingValue("+", "$", SummIncome[_currentScene, cellIndex]); 
        }

        public void DisplayNextLevelAmount(int cellIndex)
        {
            _nextLevelAmountText[cellIndex].text = $"{PurchasedUpgradeAmount[_currentScene, cellIndex]}" +
                $"/{_purchasedAmountToNextLevel[ReceivedLevels[_currentScene, cellIndex]]}";
            UpdateNextLevelProgressBar(cellIndex);

            void UpdateNextLevelProgressBar(int cellIndex)
            { 
                _nextLevelProgressBar[cellIndex].value = (float)PurchasedUpgradeAmount[_currentScene, cellIndex]
                    / _purchasedAmountToNextLevel[ReceivedLevels[_currentScene, cellIndex]]; 
            }
        }

        private void DisplayMaxAmount(int cellIndex)
        {
            _maxAmountText[cellIndex].text = $"{PurchasedUpgradeAmount[_currentScene, cellIndex]}" +
                $"/{_maxUpgradeAmount}";
            UpdateMaxProgressBar(cellIndex);

            void UpdateMaxProgressBar(int cellIndex)
            {
                _maxCountProgressBar[cellIndex].value = (float)PurchasedUpgradeAmount[_currentScene, cellIndex] /
                    _maxUpgradeAmount; 
            }
        }

        public void CheckMaxPurchaseAmount(int cellIndex)
        {
            if (PurchasedUpgradeAmount[_currentScene, cellIndex] == _maxUpgradeAmount)
            {
                HideUpgradeButton();
                SetActiveMaxAmountMessage();
            }

            void HideUpgradeButton() { UpgradeButton[cellIndex].gameObject.SetActive(false); }
            void SetActiveMaxAmountMessage() { _maxAmountMessage[cellIndex].SetActive(true); }
        }

        private void CheckLevelUp(int cellIndex)
        {
            while (PurchasedUpgradeAmount[_currentScene, cellIndex] >= _purchasedAmountToNextLevel[ReceivedLevels[_currentScene, cellIndex]]) //&& PurchasedUpgradeAmount[_currentScene, cellIndex] != _maxUpgradeAmount
            {
                CheckMethod();
                if (PurchasedUpgradeAmount[_currentScene, cellIndex] <= _purchasedAmountToNextLevel[ReceivedLevels[_currentScene, cellIndex]])
                {
                    if (PurchasedUpgradeAmount[_currentScene, cellIndex] != _maxUpgradeAmount)
                        CheckMethod();
                    break;
                }
            }

            void CheckMethod()
            {
                if (ReceivedLevels[_currentScene, cellIndex] < _purchasedAmountToNextLevel.Length - 1)
                {
                    ReceivedLevels[_currentScene, cellIndex]++;
                    Income[_currentScene, cellIndex] *= _incomeMultiplicationFromNewLevel;
                    DisplayNextLevelAmount(cellIndex);
                    DisplayCellAdditivesIncome(cellIndex);
                }
            }
        }
    }

    public class Click : SampleUpgrade { }
    public Click _Click = new();

    public class Idle : SampleUpgrade { }
    public Idle _Idle = new();

    private void Awake()
    {
        if (Game.CurrentScene != (int)Game.BuildIndex.Russians_vs_Lizards)
        {
            _Click.InitializeCell(_clickUpgradesBuffer);
            _Idle.InitializeCell(_idleUpgradesBuffer);
        }
    }

    public void Init() //Start
    {
        if (Game.FirstLaunchGame || Game.MaxCellCountInGame > YandexGame.savesData.MaxCellCountInGame)
        {
            _Click.CalcStartIncomeAndPrices(true);
            _Idle.CalcStartIncomeAndPrices(false);
        }

        if (Game.CurrentScene != (int)Game.BuildIndex.Russians_vs_Lizards)
        {
            for (int i = 0; i < _clickUpgradesBuffer.transform.childCount; i++)
            {
                _Click.CachedPrice[_currentScene, i] = _Click.Prices[_currentScene, i];
                _Click.CachedIncome[_currentScene, i] = _Click.Income[_currentScene, i];
                _Click.CachedReceivedLevels[_currentScene, i] = _Click.ReceivedLevels[_currentScene, i];
                _Click.CurrentBuyAmount[_currentScene, i] = 1;
            }

            for (int i = 0; i < _idleUpgradesBuffer.transform.childCount; i++)
            {
                _Idle.CachedPrice[_currentScene, i] = _Idle.Prices[_currentScene, i];
                _Idle.CachedIncome[_currentScene, i] = _Idle.Income[_currentScene, i];
                _Idle.CachedReceivedLevels[_currentScene, i] = _Idle.ReceivedLevels[_currentScene, i];
                _Idle.CurrentBuyAmount[_currentScene, i] = 1;
            }

            StartUpdateCells();
            CheckPurchasingPower();
            SecondInit();

            void StartUpdateCells()
            {
                SetHeaderButtonColor();

                for (int i = 0; i < ClickCellsCount; i++)
                {
                    SetClickCellImageAndName(i);
                    _Click.UpdateAllCellValues(i);
                    _Click.CheckMaxPurchaseAmount(i);
                }
                for (int i = 0; i < IdleCellsCount; i++)
                {
                    SetIdleCellImageAndName(i);
                    _Idle.UpdateAllCellValues(i);
                    _Idle.CheckMaxPurchaseAmount(i);
                }

                void SetClickCellImageAndName(int i)
                {
                    _Click.UpgradeImage[i].sprite = _clickUpgradeSprites[i];
                    _Click.UpgradeName[i].text = _clickUpgradeSprites[i].name;
                }

                void SetIdleCellImageAndName(int i)
                {
                    _Idle.UpgradeImage[i].sprite = _idleUpgradeSprites[i];
                    _Idle.UpgradeName[i].text = _idleUpgradeSprites[i].name;
                }

                void SetHeaderButtonColor()
                {
                    _clickMenuButton.image.color = _passiveHeaderButtonColor;
                    _idleMenuButton.image.color = _passiveHeaderButtonColor;
                }
            }

            void SecondInit()
            {
                _multiplePurchaseButtons[0].image.color = _selectedMultiplePurchaseButton;
            }
        }
    } 

    #region SwitchButtonsColor
    public void SwitchMultiplePurchaseButtonColor(int i)
    {
        foreach (Button button in _multiplePurchaseButtons)
            button.image.color = _deselectedMultiplePurchaseButton;

        _multiplePurchaseButtons[i].image.color = _selectedMultiplePurchaseButton;
    }

    #region Click
    private void SetClickHeaderColorToActive()
    { _clickMenuButton.image.color = _activeHeaderButtonColor; }

    private void SetClickHeaderColorToPassive()
    { _clickMenuButton.image.color = _passiveHeaderButtonColor; }

    private void ClickUpgradeButtonActive(int i)
    { _Click.UpgradeButton[i].image.color = _activeUpgradeButtonColor; }

    private void ClickUpgradeButtonPassive(int i)
    { _Click.UpgradeButton[i].image.color = _passiveUpgradeButtonColor; }
    #endregion

    #region Idle
    private void SetIdleHeaderColorToActive()
    { _idleMenuButton.image.color = _activeHeaderButtonColor; }

    private void SetIdleHeaderColorToPassive()
    { _idleMenuButton.image.color = _passiveHeaderButtonColor; }

    public void IdleUpgradeButtonActive(int i)
    { _Idle.UpgradeButton[i].image.color = _activeUpgradeButtonColor; }

    public void IdleUpgradeButtonPassive(int i)
    { _Idle.UpgradeButton[i].image.color = _passiveUpgradeButtonColor; }
    #endregion
    #endregion

    public void CheckPurchasingPower()
    {
        int counterClick = 0;
        int counterIdle = 0;
        int counterMain = 0;

        for (int j = 0; j < ClickCellsCount; j++) 
        {
            if (MoneyMenu.ÑheckTheAmountOfMoney(_Click.CachedPrice[_currentScene, j]))
            {
                _canBuyClickUpgrade = true;
                _Click.UpgradeButton[j].interactable = true;
                ClickUpgradeButtonActive(j);
                counterClick++;
                counterMain++;
            }
            else
            {
                _Click.UpgradeButton[j].interactable = false;
                ClickUpgradeButtonPassive(j);
            }
        }

        for (int j = 0; j < IdleCellsCount; j++)
        {
            if (MoneyMenu.ÑheckTheAmountOfMoney(_Idle.CachedPrice[_currentScene, j]))
            {
                _canBuyIdleUpgrade = true;
                _Idle.UpgradeButton[j].interactable = true;
                IdleUpgradeButtonActive(j);
                counterIdle++;
                counterMain++;
            }
            else
            {
                _Idle.UpgradeButton[j].interactable = false;
                IdleUpgradeButtonPassive(j);
            }
        }

        if (counterClick == 0)
            _canBuyClickUpgrade = false;
        if (counterIdle == 0)
            _canBuyIdleUpgrade = false;
        if (counterMain == 0)
            _mainAttentionImage.SetActive(false);
        else _mainAttentionImage.SetActive(true);

        ChangeHeaderButtonColor();

        void ChangeHeaderButtonColor()
        {
            if (_canBuyClickUpgrade)
                SetClickHeaderColorToActive();
            else SetClickHeaderColorToPassive();

            if (_canBuyIdleUpgrade)
                SetIdleHeaderColorToActive();
            else SetIdleHeaderColorToPassive();
        }
    }

    #region UpgradesSamples
    public void SampleUpgradeClick(int cellIndex)
    {
        if (MoneyMenu.ÑheckTheAmountOfMoney(_Click.CachedPrice[_currentScene, cellIndex]) &&
            _Click.PurchasedUpgradeAmount[_currentScene, cellIndex] < _maxUpgradeAmount)
        {
            AudioEffects.PlayUpgradeEffect_1();
            _Click.CheckBuyAmount(cellIndex);
            int buyAmount = _Click.CurrentBuyAmount[_currentScene, cellIndex];
            float incomeSumm = _Click.CachedIncome[_currentScene, cellIndex];
            float additiveCachedPrice = (_Click.CachedPrice[_currentScene, cellIndex] * (GlobalUpgrades.UpgradeCostMultiplier - 1));

            MoneyMenu.DecreaseMoneyCapital(_Click.CachedPrice[_currentScene, cellIndex]);
            MoneyMenu.IncreaseÑlickIncome(incomeSumm);
            _Click.AddSummIncome(cellIndex, incomeSumm);
            _Click.AddPrice(cellIndex, additiveCachedPrice);
            _Click.AddPurchasedUpgradeAmount(cellIndex, buyAmount);
            _accumulatedPurchasedClickUpgrades[_currentScene, cellIndex] += buyAmount;
            Instantiate(FloatPrefabs.Outlier, _Click.UpgradePriceText[cellIndex].transform).GetComponent<Outlier>().DesignateOutlier("Up", "+", "$", additiveCachedPrice, Color.white);
            Instantiate(FloatPrefabs.Outlier, _Click.SummIncomeText[cellIndex].transform).GetComponent<Outlier>().DesignateOutlier("Up", "+", "$", incomeSumm, Color.green);

            if (_maxBuyAmountFactor == true) CalculateSomeUpgrades(0);
            else CalculateSomeUpgrades(buyAmount);
        }
    }

    public void SampleUpgradeIdle(int cellIndex)
    {
        if (MoneyMenu.ÑheckTheAmountOfMoney(_Idle.Prices[_currentScene, cellIndex]) &&
            _Idle.PurchasedUpgradeAmount[_currentScene, cellIndex] < _maxUpgradeAmount)
        {
            AudioEffects.PlayUpgradeEffect_1();
            _Idle.CheckBuyAmount(cellIndex);
            int buyAmount = _Idle.CurrentBuyAmount[_currentScene, cellIndex];
            float incomeSumm = _Idle.CachedIncome[_currentScene, cellIndex];
            float additiveCachedPrice = (_Idle.CachedPrice[_currentScene, cellIndex] * (GlobalUpgrades.UpgradeCostMultiplier - 1));

            MoneyMenu.DecreaseMoneyCapital(_Idle.CachedPrice[_currentScene, cellIndex]);
            MoneyMenu.IncreaseTickIncome(incomeSumm);
            _Idle.AddSummIncome(cellIndex, incomeSumm);
            _Idle.AddPrice(cellIndex, additiveCachedPrice);
            _Idle.AddPurchasedUpgradeAmount(cellIndex, buyAmount);
            _accumulatedPurchasedIdleUpgrades[_currentScene, cellIndex] += buyAmount;
            Instantiate(FloatPrefabs.Outlier, _Idle.UpgradePriceText[cellIndex].transform).GetComponent<Outlier>().DesignateOutlier("Up", "+", "$", additiveCachedPrice, Color.white);
            Instantiate(FloatPrefabs.Outlier, _Idle.SummIncomeText[cellIndex].transform).GetComponent<Outlier>().DesignateOutlier("Up", "+", "$", incomeSumm, Color.green);

            if (_maxBuyAmountFactor == true) CalculateSomeUpgrades(0);
            else CalculateSomeUpgrades(buyAmount);
        }
    }
    #endregion

    #region CalculateSomeUpgrades
    public void CalculateSomeUpgrades(int factor)
    {
        for (int cellIndex = 0; cellIndex < ClickCellsCount; cellIndex++)
        {
            _Click.CurrentBuyAmount[_currentScene, cellIndex] = factor;
            _Click.CacheIncome(cellIndex);
            _Click.CachePrice(cellIndex);
        }
        for (int cellIndex = 0; cellIndex < IdleCellsCount; cellIndex++)
        {
            _Idle.CurrentBuyAmount[_currentScene, cellIndex] = factor;
            _Idle.CacheIncome(cellIndex);
            _Idle.CachePrice(cellIndex);
        }

        if (factor == 0)
        {
            _maxBuyAmountFactor = true;

            for (int cellIndex = 0; cellIndex < ClickCellsCount; cellIndex++)
                _Click.CalculateMaximumPurchaseAmount(cellIndex, MoneyMenu.GetMoneyCapital()[_currentScene], GlobalUpgrades.UpgradeCostMultiplier);
            for (int cellIndex = 0; cellIndex < IdleCellsCount; cellIndex++)
                _Idle.CalculateMaximumPurchaseAmount(cellIndex, MoneyMenu.GetMoneyCapital()[_currentScene], GlobalUpgrades.UpgradeCostMultiplier);
        }
        else
        {
            _maxBuyAmountFactor = false;

            for (int cellIndex = 0; cellIndex < ClickCellsCount; cellIndex++)
                _Click.CalculatePurchaseAmount(cellIndex, GlobalUpgrades.UpgradeCostMultiplier);
            for (int cellIndex = 0; cellIndex < IdleCellsCount; cellIndex++)
                _Idle.CalculatePurchaseAmount(cellIndex, GlobalUpgrades.UpgradeCostMultiplier);
        }

        for (int cellIndex = 0; cellIndex < ClickCellsCount; cellIndex++)
        {
            _Click.DisplayUpgradePrice(cellIndex);
            _Click.DisplayCellAdditivesIncome(cellIndex);
            _Click.DisplayCurrentBuyAmount(cellIndex);
        }
        for (int cellIndex = 0; cellIndex < IdleCellsCount; cellIndex++)
        {
            _Idle.DisplayUpgradePrice(cellIndex);
            _Idle.DisplayCellAdditivesIncome(cellIndex);
            _Idle.DisplayCurrentBuyAmount(cellIndex);
        }

        CheckPurchasingPower();
    }
    #endregion
}