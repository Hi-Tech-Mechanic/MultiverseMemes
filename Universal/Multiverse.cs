using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Multiverse : DataStructure
{
    #region Serialized
    [SerializeField] private GameObject _multiverseWindow;
    [SerializeField] private GameObject _multiverseButton;
    [SerializeField] private GameObject _multiverseBackground;
    [SerializeField] private GameObject _nonClickBackground;
    [SerializeField] private GameObject[] Universes;
    [SerializeField] private Animator _warpAnimator;
    [SerializeField] private Animator _blackoutAnimator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _cosmoEffect;
    [SerializeField] private AudioClip _warpBegin;
    [SerializeField] private AudioClip _warpEnd;
    #endregion

    [NonSerialized] public static float[] CurrentUniverseProgress = new float[_universesCount];
    private Animator _multiverseWindowAnimator;
    private readonly GameObject[] _selectedStateMessage = new GameObject[_universesCount];
    private readonly TextMeshProUGUI[] _progressPercentText = new TextMeshProUGUI[_universesCount];
    private readonly Button[] _universesButtons = new Button[_universesCount];
    private readonly Image[] _imagesUniverses = new Image[_universesCount];
    private readonly Image[] _progressBarFillImage = new Image[_universesCount];
    private readonly Slider[] _progressBar = new Slider[_universesCount];
    private const int _universesCount = 9;
    private const int _overclockedSpeed = 20;
    private const float _selectedAlpha = 0.8f;
    private const int _minHSV_H = 5;
    private const int _maxHSV_H = 125;
    private const float HSV_S = 100;
    private const float HSV_V = 77;
    private int _speedMultiplier = 1;
    private bool _chooseIsActive = false;

    public void Init() //Start
    {
        _multiverseWindowAnimator = _multiverseWindow.GetComponent<Animator>();

        FindComponents();
        TemplateUniverseSelect(Game.CurrentScene);
        UpdateUniverseProgressValue();

        _warpAnimator.SetTrigger("End");
        PlayAudioClip(_warpEnd);
    }

    private void Update()
    {
        RotateObject();
    }

    private void UpdateUniverseProgressValue()
    {
        float currentColor;
        float percent;

        for (int universeIndex = 0; universeIndex < _universesCount; universeIndex++)
        {
            if (Game.CurrentScene == universeIndex)
            {
                int counter = 0;
                int achievementsLength = 0;

                switch (Game.CurrentScene)
                {
                    case 0:
                        achievementsLength = Achievements_CombinedUniverse.GetAchievementsScripts().Length;
                        foreach (AchievementUnit achievementUnit in Achievements_CombinedUniverse.GetAchievementsScripts())
                        {
                            if (achievementUnit.RewardIsReady || achievementUnit.Claimed)
                            {                                
                                counter++;
                            }
                        }
                        break;
                    case 1:
                        achievementsLength = Achievements_R_vs_L.GetAchievementsScripts().Length;
                        foreach (AchievementUnit achievementUnit in Achievements_R_vs_L.GetAchievementsScripts())
                        {
                            if (achievementUnit.RewardIsReady || achievementUnit.Claimed)
                            {
                                counter++;
                            }
                        }
                        break;
                    case 2:
                        achievementsLength = Achievements_GenaBukinUniverse.GetAchievementsScripts().Length;
                        foreach (AchievementUnit achievementUnit in Achievements_GenaBukinUniverse.GetAchievementsScripts())
                        {
                            if (achievementUnit.RewardIsReady || achievementUnit.Claimed)
                            {
                                counter++;
                            }
                        }
                        break;
                    case 3:
                        achievementsLength = Achievements_GenaGorinUniverse.GetAchievementsScripts().Length;
                        foreach (AchievementUnit achievementUnit in Achievements_GenaGorinUniverse.GetAchievementsScripts())
                        {
                            if (achievementUnit.RewardIsReady || achievementUnit.Claimed)
                            {
                                counter++;
                            }
                        }
                        break;
                    case 4:
                        achievementsLength = Achievements_PapichUniverse.GetAchievementsScripts().Length;
                        foreach (AchievementUnit achievementUnit in Achievements_PapichUniverse.GetAchievementsScripts())
                        {
                            if (achievementUnit.RewardIsReady || achievementUnit.Claimed)
                            {
                                counter++;
                            }
                        }
                        break;
                    case 5:
                        achievementsLength = Achievements_UniverUniverse.GetAchievementsScripts().Length;
                        foreach (AchievementUnit achievementUnit in Achievements_UniverUniverse.GetAchievementsScripts())
                        {
                            if (achievementUnit.RewardIsReady || achievementUnit.Claimed)
                            {
                                counter++;
                            }
                        }
                        break;
                    case 6:
                        achievementsLength = Achievements_ParanormalUniverse.GetAchievementsScripts().Length;
                        foreach (AchievementUnit achievementUnit in Achievements_ParanormalUniverse.GetAchievementsScripts())
                        {
                            if (achievementUnit.RewardIsReady || achievementUnit.Claimed)
                            {
                                counter++;
                            }
                        }
                        break;
                    case 7:
                        achievementsLength = Achievements_RockUniverse.GetAchievementsScripts().Length;
                        foreach (AchievementUnit achievementUnit in Achievements_RockUniverse.GetAchievementsScripts())
                        {
                            if (achievementUnit.RewardIsReady || achievementUnit.Claimed)
                            {
                                counter++;
                            }
                        }
                        break;
                    case 8:
                        achievementsLength = Achievements_ShrekUniverse.GetAchievementsScripts().Length;
                        foreach (AchievementUnit achievementUnit in Achievements_ShrekUniverse.GetAchievementsScripts())
                        {
                            if (achievementUnit.RewardIsReady || achievementUnit.Claimed)
                            {
                                counter++;
                            }
                        }
                        break;
                }

                percent = (float)counter / achievementsLength;
                currentColor = percent * _maxHSV_H;

                if (currentColor <= 0)
                {
                    currentColor = _minHSV_H;
                }

                CurrentUniverseProgress[universeIndex] = percent;
                _progressBarFillImage[universeIndex].color = Color.HSVToRGB(currentColor / 360, HSV_S / 100, HSV_V / 100);
                _progressPercentText[universeIndex].text = $"{Math.Round(CurrentUniverseProgress[universeIndex] * 100, 2)}%";
                _progressBar[universeIndex].value = CurrentUniverseProgress[universeIndex];
            }
            else
            {
                currentColor = CurrentUniverseProgress[universeIndex] * _maxHSV_H;

                _progressBarFillImage[universeIndex].color = Color.HSVToRGB(currentColor / 360, HSV_S / 100, HSV_V / 100);
                _progressPercentText[universeIndex].text = $"{Math.Round(CurrentUniverseProgress[universeIndex] * 100, 2)}%";
                _progressBar[universeIndex].value = CurrentUniverseProgress[universeIndex];
            }

            switch (Game.CurrentScene)
            {
                case 0:
                    Achievements_CombinedUniverse.UpdateAllInfo();
                    break;
                case 1:
                    Achievements_R_vs_L.UpdateAllInfo();
                    break;
                case 2:
                    Achievements_GenaBukinUniverse.UpdateAllInfo();
                    break;
                case 3:
                    Achievements_GenaGorinUniverse.UpdateAllInfo();
                    break;
                case 4:
                    Achievements_PapichUniverse.UpdateAllInfo();
                    break;
                case 5:
                    Achievements_UniverUniverse.UpdateAllInfo();
                    break;
                case 6:
                    Achievements_ParanormalUniverse.UpdateAllInfo();
                    break;
                case 7:
                    Achievements_RockUniverse.UpdateAllInfo();
                    break;
                case 8:
                    Achievements_ShrekUniverse.UpdateAllInfo();
                    break;
            }
        }
    }

    public void SwithMultiverseWindow()
    {
        _chooseIsActive = !_chooseIsActive;
        _nonClickBackground.SetActive(_chooseIsActive);

        if (_chooseIsActive)
        {
            _multiverseWindow.SetActive(_chooseIsActive);
            _speedMultiplier = _overclockedSpeed;
            _cosmoEffect.Play();
            _multiverseWindowAnimator.SetTrigger("Active");
            UpdateUniverseProgressValue();
        }
        else
        {
            _speedMultiplier = 1;
            _cosmoEffect.Stop();
            _multiverseWindowAnimator.SetTrigger("Close");
        }
    }

    public void SwithDisplayMiltiverse()
    {        
        if (Battle.BattleIsActive)
            _chooseIsActive = false;            
        else _chooseIsActive = true;

        _multiverseButton.SetActive(_chooseIsActive);
        _multiverseBackground.SetActive(_chooseIsActive);
    }

    public void CloseMultiverseWindow()
    {
        _chooseIsActive = false;
        _multiverseWindow.SetActive(_chooseIsActive);
        _nonClickBackground.SetActive(_chooseIsActive);

        _speedMultiplier = 1;
    }

    public void ChooseUniverse(int universe_index)
    {
        SwithMultiverseWindow();
        SaveAndLoad.SavePlayerData();

        TemplateUniverseDeselect();
        TemplateUniverseSelect(universe_index);

        StartCoroutine(Warp());

        IEnumerator Warp()
        {
            _warpAnimator.SetTrigger("Begin");
            _blackoutAnimator.SetTrigger("Begin");
            PlayAudioClip(_warpBegin);
            yield return new WaitForSeconds(2);

            switch (universe_index)
            {
                case 0:
                    SceneManager.LoadSceneAsync("0.Combined_Universe");
                    break;
                case 1:
                    SceneManager.LoadSceneAsync("1.Russians_vs_Lizards");
                    break;
                case 2:
                    SceneManager.LoadSceneAsync("2.GenaBukin_Universe");
                    break;
                case 3:
                    SceneManager.LoadSceneAsync("3.GenaGorin_Universe");
                    break;
                case 4:
                    SceneManager.LoadSceneAsync("4.Papich_Universe");
                    break;
                case 5:
                    SceneManager.LoadSceneAsync("5.Univer_Universe");
                    break;
                case 6:
                    SceneManager.LoadSceneAsync("6.Paranormal_Universe");
                    break;
                case 7:
                    SceneManager.LoadSceneAsync("7.Rock_Universe");
                    break;
                case 8:
                    SceneManager.LoadSceneAsync("8.Shrek_Universe");
                    break;
            }
        } 
    }

    private void TemplateUniverseSelect(int universe_index)
    {
        Color new_alpha;

        _universesButtons[universe_index].interactable = false;
        _selectedStateMessage[universe_index].SetActive(true);

        new_alpha = _imagesUniverses[universe_index].color;
        new_alpha.a = _selectedAlpha;
        _imagesUniverses[universe_index].color = new_alpha;
    }

    private void TemplateUniverseDeselect()
    {
        Color new_alpha;

        _universesButtons[Game.CurrentScene].interactable = true;
        _selectedStateMessage[Game.CurrentScene].SetActive(false);

        new_alpha = _imagesUniverses[Game.CurrentScene].color;
        new_alpha.a = 1;
        _imagesUniverses[Game.CurrentScene].color = new_alpha;

    }

    private void FindComponents()
    {
        int i = 0;
        foreach (GameObject universe in Universes)
        {
            foreach (RectTransform component in universe.GetComponentsInChildren<RectTransform>(includeInactive: true))
            {
                switch (component.name)
                {
                    case ("Background/Button"):
                        _universesButtons[i] = component.GetComponent<Button>();
                        break;
                    case ("ImageUniverse"):
                        _imagesUniverses[i] = component.GetComponent<Image>();
                        break;
                    case ("Fill"):
                        _progressBarFillImage[i] = component.GetComponent<Image>();
                        break;
                    case ("State"):
                        _selectedStateMessage[i] = component.gameObject;
                        break;
                    case ("ProgressPercent"):
                        _progressPercentText[i] = component.GetComponent<TextMeshProUGUI>();
                        break;
                    case ("ProgressBar"):
                        _progressBar[i] = component.GetComponent<Slider>();
                        break;
                    default: break;
                }
            }
            i++;
        }
    }

    private void RotateObject()
    {
        Vector3 angles = _multiverseButton.transform.rotation.eulerAngles;
        angles.z -= Time.deltaTime * (20 * _speedMultiplier);
        _multiverseButton.transform.rotation = Quaternion.Euler(angles);
    }

    private void PlayAudioClip(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    //private void ChangeDeltaSize()
    //{
    //    Vector3 size = _multiverseBackgroundRect.localScale;

    //    if (_inrease == true)
    //    {
    //        if (size.x >= min_deltaSize && size.x < max_deltaSize)
    //            size.x += Time.deltaTime * (0.1f);
    //        else if (size.x > max_deltaSize) 
    //        {
    //            size.x = max_deltaSize;
    //            _inrease = false;
    //            _decrease = true;
    //        }
    //    }

    //    if (_decrease == true)
    //    {
    //        if (size.x > min_deltaSize && size.x <= max_deltaSize)
    //            size.x -= Time.deltaTime * (0.1f);
    //        else if (size.x < min_deltaSize) 
    //        {
    //            size.x = min_deltaSize;
    //            _inrease = true;
    //            _decrease = false;
    //        }
    //    }

    //    _multiverseBackgroundRect.localScale = new Vector3(size.x, size.x, 0);
    //}

    //private void CreatingRainbowColors()
    //{
    //    if (R == 1 && B == 0)
    //    {
    //        G += _colorValue;
    //        if (G > 1) { G = 1; }
    //    }
    //    if (R <= 1 && R != 0 && G == 1)
    //    {
    //        R -= _colorValue;
    //        if (R < 0) { R = 0; }
    //    }
    //    //
    //    if (R == 0 && G == 1)
    //    {
    //        B += _colorValue;
    //        if (B > 1) { B = 1; }
    //    }
    //    if (B == 1 && G <= 1 && G != 0)
    //    {
    //        G -= _colorValue;
    //        if (G < 0) { G = 0; }
    //    }
    //    //
    //    if (B == 1 && G == 0)
    //    {
    //        R += _colorValue;
    //        if (R > 1) { R = 1; }
    //    }
    //    if (B <= 1 && B != 0 && R == 1)
    //    {
    //        B -= _colorValue;
    //        if (B < 0) { B = 0;}
    //    }

    //    _buttonImage.color = new Color(R, G, B, 1);
    //}
}