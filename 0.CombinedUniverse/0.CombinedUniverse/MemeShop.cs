using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace CombinedUniverse
{
    public class MemeShop : DataStructure
    {
        [NonSerialized] public static bool[,] ClipIsSelected = new bool[Game.ScenesCount, global::MemeShop.MaxClipLength];

        [Header("CurrentSceneFields")]
        [Space(5)]
        [SerializeField] private TextMeshProUGUI _selectedCountText;
        [SerializeField] private GameObject[] _cellsBuffers;
        [SerializeField] private AudioClip[] _otherClips;
        [SerializeField] private AudioClip[] _genaBukinClips;
        [SerializeField] private AudioClip[] _genaGorinClips;
        [SerializeField] private AudioClip[] _papichClips;
        [SerializeField] private AudioClip[] _univerClips;
        [SerializeField] private AudioClip[] _paranormalClips;
        [SerializeField] private AudioClip[] _rockClips;
        [SerializeField] private AudioClip[] _shrekClips;

        [Header("General")]
        [Space(5)]
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private Color _selectedColor;

        private TextMeshProUGUI[] _priceText;
        private Button[] _buyButton;
        private GameObject[] _audioCells;
        private GameObject[] _lock;
        private GameObject[,] _lockMessage = new GameObject[Game.ScenesCount, global::MemeShop.MaxClipLength];
        private GameObject[,] _selectedMessage = new GameObject[Game.ScenesCount, global::MemeShop.MaxClipLength];
        private GameObject[,] _allOtherAudioCells = new GameObject[Game.ScenesCount, global::MemeShop.MaxClipLength];
        private Button[,] _clipButton = new Button[Game.ScenesCount, global::MemeShop.MaxClipLength];
        private int[] _prices;
        private int _clipCount;
        private const int _startCountFreeClips = 3;

        private void Awake()
        {
            if (Game.CurrentScene == (int)Game.BuildIndex.CombinedUniverse)
            {
                _clipCount = _otherClips.Length;
                _prices = new int[_clipCount];
                _priceText = new TextMeshProUGUI[_clipCount];
                _buyButton = new Button[_clipCount];
                _audioCells = new GameObject[_clipCount];
                _lock = new GameObject[_clipCount];

                for (int i = 0; i < _clipCount; i++)
                {
                    if (i > _cellsBuffers[0].transform.childCount - 1)
                    {
                        _audioCells[i] = Instantiate(_cellPrefab, _cellsBuffers[0].transform);
                    }
                    else _audioCells[i] = _cellsBuffers[0].transform.GetChild(i).gameObject;
                }

                if (_cellsBuffers[0].transform.childCount > _clipCount)
                {
                    for (int i = _cellsBuffers[0].transform.childCount; i > _clipCount; i--)
                    {
                        Destroy(_cellsBuffers[0].transform.GetChild(i - 1).gameObject);
                    }
                }

                int bufferIndex = 0;

                foreach (GameObject buffer in _cellsBuffers)
                {
                    for (int j = 0; j < buffer.transform.childCount; j++)
                    {
                        if (buffer.transform.GetChild(j).gameObject != null)
                            _allOtherAudioCells[bufferIndex, j] = buffer.transform.GetChild(j).gameObject;
                    }

                    bufferIndex++;
                }


                //if (YandexGame.SDKEnabled)
                //{
                //    Init();
                //}
            }
        }

        public void Init() //Start
        {
            int currentScene = (int)Game.BuildIndex.CombinedUniverse;

            if (Game.CurrentScene == currentScene)
            {
                FindAndSetClipInfo();
                SetPrices();
                CheckSavedData();
                DisplaySelectedCountText();
            }

            if (Game.FirstLaunchScene[currentScene])
            {
                for (int i = 0; i < _startCountFreeClips; i++)
                {
                    BuyMemeClip(i);
                }
            }
        }

        #region ButtonEvent
        public void UpdateInfo()
        {
            DisplaySelectedCountText();
        }

        public void BuyMemeClip(int clipIndex)
        {
            if (MoneyMenu.GetMemeCoins() >= _prices[clipIndex])
            {
                if (clipIndex != 0 && clipIndex != 1 && clipIndex != 2)
                {
                    AudioEffects.PlayPurchaseEffect();
                }

                MemeShop.AccumulateMemeClip();
                MemeShop.ClipIsAvailable[0, clipIndex] = true;
                MoneyMenu.SpendMemeCoins(_prices[clipIndex]);
                TemplateUnlock(0, clipIndex);

                if (MemeShop.SelectedClipCount[0] < GlobalUpgrades.MemeClipCount)
                {
                    SelectAudioEffect(clipIndex);
                }
            }
        }

        public void SelectAudioEffect(int clipIndex)
        {
            int currentScene = (int)Game.BuildIndex.CombinedUniverse;
            int scene = clipIndex / 100;
            int index = clipIndex % 100;

            AudioEffects.PlayButtonClickEffect();

            if (ClipIsSelected[scene, index] == true)
            {
                if (MemeShop.SelectedClipCount[currentScene] > 0)
                {
                    MemeShop.SelectedClipCount[currentScene]--;
                    ClipIsSelected[scene, index] = false;
                    AudioEffects.AviableMemeClips.Remove(GetAudioClip(scene, index));
                    _clipButton[scene, index].image.color = Color.white;
                    _selectedMessage[scene, index].SetActive(false);
                    DisplaySelectedCountText();
                }
            }
            else
            {
                if (MemeShop.SelectedClipCount[currentScene] < GlobalUpgrades.MemeClipCount)
                {
                    MemeShop.SelectedClipCount[currentScene]++;
                    ClipIsSelected[scene, index] = true;
                    DisplaySelectedCountText();
                    TemplateSelect(scene, index);
                }
            }
        }
        #endregion

        private void TemplateUnlock(int sceneIndex, int clipIndex)
        {
            if (sceneIndex == (int)Game.BuildIndex.CombinedUniverse)
            {
                _lock[clipIndex].SetActive(false);
                _buyButton[clipIndex].gameObject.SetActive(false);
            }

            if (_lockMessage[sceneIndex, clipIndex] != null)
            {
                _lockMessage[sceneIndex, clipIndex].SetActive(false);
            }

            _clipButton[sceneIndex, clipIndex].interactable = true;
            _clipButton[sceneIndex, clipIndex].image.color = Color.white;
        }

        private void TemplateSelect(int sceneIndex, int clipIndex)
        {
            AudioEffects.AviableMemeClips.Add(GetAudioClip(sceneIndex, clipIndex));
            _clipButton[sceneIndex, clipIndex].image.color = _selectedColor;
            _selectedMessage[sceneIndex, clipIndex].SetActive(true);
        }

        private void CheckSavedData()
        {
            for (int scene = 0; scene < Game.ScenesCount; scene++)
            {
                for (int clipIndex = 0; clipIndex < global::MemeShop.MaxClipLength; clipIndex++)
                {
                    if (MemeShop.ClipIsAvailable[scene, clipIndex] == true)
                    {
                        TemplateUnlock(scene, clipIndex);

                        if (ClipIsSelected[scene, clipIndex] == true)
                        {
                            TemplateSelect(scene, clipIndex);
                        }
                    }
                }
            }
        }

        private void DisplaySelectedCountText()
        {
            _selectedCountText.text = $"Выбрано {MemeShop.SelectedClipCount[(int)Game.BuildIndex.CombinedUniverse]}/{GlobalUpgrades.MemeClipCount}";
        }

        private void SetPrices()
        {
            for (int i = _startCountFreeClips; i < _clipCount; i++)
            {
                _prices[i] = (1 + i - _startCountFreeClips) * 10;
                _priceText[i].text = _prices[i].ToString();
            }
        }

        private void FindAndSetClipInfo()
        {
            for (int i = 0; i < _clipCount; i++)
            {
                _clipButton[0, i] = _audioCells[i].GetComponent<Button>();

                RectTransform[] components = _audioCells[i].GetComponentsInChildren<RectTransform>(includeInactive: true);
                FindAndSet(components, 0, i);
            }

            for (int bufferIndex = 0; bufferIndex < Game.ScenesCount; bufferIndex++)
            {
                int childCount = _cellsBuffers[bufferIndex].transform.childCount;

                for (int cellIndex = 0; cellIndex < childCount; cellIndex++)
                {
                    if (_allOtherAudioCells[bufferIndex, cellIndex] != null)
                    {
                        _clipButton[bufferIndex, cellIndex] = _allOtherAudioCells[bufferIndex, cellIndex].GetComponent<Button>();

                        RectTransform[] components = _allOtherAudioCells[bufferIndex, cellIndex].GetComponentsInChildren<RectTransform>(includeInactive: true);
                        FindAndSet(components, bufferIndex, cellIndex);
                    }
                }
            }

            void FindAndSet(RectTransform[] components, int sceneIndex, int cellIndex)
            {
                foreach (RectTransform component in components)
                {
                    switch (component.name)
                    {
                        case ("Lock"):
                            _lock[cellIndex] = component.gameObject;
                            break;
                        case ("SelectedMessage"):
                            _selectedMessage[sceneIndex, cellIndex] = component.gameObject;
                            break;
                        case ("LockMessage"):
                            _lockMessage[sceneIndex, cellIndex] = component.gameObject;
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
                            component.GetComponent<TextMeshProUGUI>().text = $"{GetAudioClip(sceneIndex, cellIndex).name}";
                            break;
                        case ("EffectLength"):
                            component.GetComponent<TextMeshProUGUI>().text = $"{Math.Round(GetAudioClip(sceneIndex, cellIndex).length, 3)} сек.";
                            break;
                    }
                }
            }
        }

        private AudioClip GetAudioClip(int bufferIndex, int clipIndex)
        {
            AudioClip clip;

            switch (bufferIndex)
            {
                case (int)Game.BuildIndex.GenaBukin_Universe:
                    clip = _genaBukinClips[clipIndex];
                    break;
                case (int)Game.BuildIndex.GenaGorin_Universe:
                    clip = _genaGorinClips[clipIndex];
                    break;
                case (int)Game.BuildIndex.Papich_Universe:
                    clip = _papichClips[clipIndex];
                    break;
                case (int)Game.BuildIndex.Univer_Universe:
                    clip = _univerClips[clipIndex];
                    break;
                case (int)Game.BuildIndex.Paranormal_Universe:
                    clip = _paranormalClips[clipIndex];
                    break;
                case (int)Game.BuildIndex.Rock_Universe:
                    clip = _rockClips[clipIndex];
                    break;
                case (int)Game.BuildIndex.Shrek_Universe:
                    clip = _shrekClips[clipIndex];
                    break;
                default:
                    clip = _otherClips[clipIndex];
                    break;
            }

            return clip;
        }
    }
}