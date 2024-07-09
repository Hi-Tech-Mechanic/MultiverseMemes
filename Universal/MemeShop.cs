using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MemeShop : DataStructure
{
    public const int MaxClipLength = 28;
    [NonSerialized] public int[] SelectedClipCount = new int[Game.ScenesCount];
    [NonSerialized] public bool[,] ClipIsAvailable = new bool[Game.ScenesCount, MaxClipLength];
    [NonSerialized] public bool[,] ClipIsSelected = new bool[Game.ScenesCount, MaxClipLength];
    [NonSerialized] public int[] PurchasedMemeClips = new int[Game.ScenesCount];

    [Header("CurrentSceneFields")]
    [Space(5)]
    [SerializeField] private AudioClip[] _effectsClips;
    [SerializeField] private GameObject _audioCellsBuffer;
    [SerializeField] private TextMeshProUGUI _selectedCountText;

    [Header("General")]
    [Space(5)]
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _deselectedColor;

    private GameObject[] _audioCells;
    private GameObject[] _lock;
    private GameObject[] _selectedMessage;
    private Button[] _buyButton;
    private Button[] _clipButton;
    private TextMeshProUGUI[] _priceText;
    private int[] _prices;
    private int _effectsCount;

    private void Awake()
    {
        if (Game.CurrentScene != (int)Game.BuildIndex.Russians_vs_Lizards &&
            Game.CurrentScene != (int)Game.BuildIndex.CombinedUniverse)
        {
            _effectsCount = _effectsClips.Length;
            _audioCells = new GameObject[_effectsCount];
            _lock = new GameObject[_effectsCount];
            _selectedMessage = new GameObject[_effectsCount];
            _clipButton = new Button[_effectsCount];
            _buyButton = new Button[_effectsCount];
            _priceText = new TextMeshProUGUI[_effectsCount];
            _prices = new int[_effectsCount];

            for (int i = 0; i < _effectsCount; i++)
            {
                if (i > _audioCellsBuffer.transform.childCount - 1)
                {
                    _audioCells[i] = Instantiate(_cellPrefab, _audioCellsBuffer.transform);
                }
                else _audioCells[i] = _audioCellsBuffer.transform.GetChild(i).gameObject;
            }

            if (_audioCellsBuffer.transform.childCount > _effectsCount)
            {
                for (int i = _audioCellsBuffer.transform.childCount; i > _effectsCount; i--)
                {
                    Destroy(_audioCellsBuffer.transform.GetChild(i - 1).gameObject);
                }
            }


            //if (YandexGame.SDKEnabled)
            //{
            //    Init();
            //}
        }        
    }

    public void Init() //Start
    {
        if (Game.CurrentScene != (int)Game.BuildIndex.Russians_vs_Lizards &&
            Game.CurrentScene != (int)Game.BuildIndex.CombinedUniverse)
        {
            FindAndSetClipInfo();
            SetPrices();
            CheckSavedData();
            DisplaySelectedCountText();
        }
    }

    #region ButtonEvent
    public void BuyMemeClip(int clipIndex)
    {
        if (MoneyMenu.GetMemeCoins() >= _prices[clipIndex])
        {
            AudioEffects.PlayPurchaseEffect();
            AccumulateMemeClip();
            ClipIsAvailable[Game.CurrentScene, clipIndex] = true;
            MoneyMenu.SpendMemeCoins(_prices[clipIndex]);
            TemplateUnlock(clipIndex);

            if (SelectedClipCount[Game.CurrentScene] < GlobalUpgrades.MemeClipCount)
            {
                SelectAudioEffect(clipIndex);
            }
        }
    }

    public void SelectAudioEffect(int clipIndex)
    {
        AudioEffects.PlayButtonClickEffect();

        if (ClipIsSelected[Game.CurrentScene, clipIndex] == true)
        {
            if (SelectedClipCount[Game.CurrentScene] > 0)
            {
                ClipIsSelected[Game.CurrentScene, clipIndex] = false; 
                SelectedClipCount[Game.CurrentScene]--;
                AudioEffects.AviableMemeClips.Remove(_effectsClips[clipIndex]);
                DisplaySelectedCountText();
                _clipButton[clipIndex].image.color = _deselectedColor;
                _selectedMessage[clipIndex].SetActive(false);
            }
        }
        else
        {
            if (SelectedClipCount[Game.CurrentScene] < GlobalUpgrades.MemeClipCount)
            {
                ClipIsSelected[Game.CurrentScene, clipIndex] = true;
                SelectedClipCount[Game.CurrentScene]++;
                DisplaySelectedCountText();
                TemplateSelect(clipIndex);
            }
        }
    }
    #endregion

    public void AccumulateMemeClip()
    {
        PurchasedMemeClips[Game.CurrentScene]++;
    }

    private void TemplateUnlock(int clipIndex)
    {
        _lock[clipIndex].SetActive(false);
        _buyButton[clipIndex].gameObject.SetActive(false);
        _clipButton[clipIndex].interactable = true;
        _clipButton[clipIndex].image.color = _deselectedColor;
    }

    private void TemplateSelect(int clipIndex)
    {
        AudioEffects.AviableMemeClips.Add(_effectsClips[clipIndex]);
        _clipButton[clipIndex].image.color = _selectedColor;
        _selectedMessage[clipIndex].SetActive(true);
    }

    private void CheckSavedData()
    {
        if (Game.CurrentScene == (int)Game.BuildIndex.CombinedUniverse)
        {
            for (int scene = 0; scene < Game.ScenesCount; scene++)
            {
                for (int clipIndex = 0; clipIndex < MaxClipLength; clipIndex++)
                {
                    if (ClipIsAvailable[scene, clipIndex] == true)
                    {
                        TemplateUnlock(clipIndex);

                        if (ClipIsSelected[scene, clipIndex] == true)
                        {
                            TemplateSelect(clipIndex);
                        }
                    }
                }
            }
        }
        else
        {
            for (int clipIndex = 0; clipIndex < MaxClipLength; clipIndex++)
            {
                if (ClipIsAvailable[Game.CurrentScene, clipIndex] == true)
                {
                    TemplateUnlock(clipIndex);

                    if (ClipIsSelected[Game.CurrentScene, clipIndex] == true)
                    {
                        TemplateSelect(clipIndex);
                    }
                }
            }
        }
    }

    private void DisplaySelectedCountText()
    {
        _selectedCountText.text = $"Выбрано {SelectedClipCount[Game.CurrentScene]}/{GlobalUpgrades.MemeClipCount}";
    }

    private void SetPrices()
    {
        for (int i = 0; i < _effectsCount; i++)
        {
            _prices[i] = (1 + i) * 10;
            _priceText[i].text = _prices[i].ToString();
        }
    }

    private void FindAndSetClipInfo()
    {
        for (int i = 0; i < _effectsCount; i++)
        {
            _clipButton[i] = _audioCells[i].GetComponent<Button>();

            RectTransform[] components = _audioCells[i].GetComponentsInChildren<RectTransform>(includeInactive: true);
            FindAndSet(components, i);
        }

        void FindAndSet(RectTransform[] components, int cellIndex)
        {
            foreach (RectTransform component in components)
            {
                switch (component.name)
                {
                    case ("Lock"):
                        _lock[cellIndex] = component.gameObject;
                        break;
                    case ("SelectedMessage"):
                        _selectedMessage[cellIndex] = component.gameObject;
                        break;
                    case ("BuyButton"):
                        _buyButton[cellIndex] = component.GetComponent<Button>();
                        break;
                    case ("Cost"):
                        _priceText[cellIndex] = component.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("TrackNumber"):
                        component.GetComponent<TextMeshProUGUI>().text = $"{cellIndex + 1}.";
                        break;
                    case ("Name&Author"):
                        component.GetComponent<TextMeshProUGUI>().text = $"{_effectsClips[cellIndex].name}";
                        break;
                    case ("EffectLength"):
                        component.GetComponent<TextMeshProUGUI>().text = $"{Math.Round(_effectsClips[cellIndex].length, 3)} сек.";
                        break;
                }
            }
        }
    }
}
