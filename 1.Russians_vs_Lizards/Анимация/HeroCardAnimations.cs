using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeroCardAnimations : DataStructure
{
    private const int _sizeStatElement_Y = 40;
    private const int _statClosePosition_X = -5;
    private const int _statOpenPosition_X = 50;

    private const float _timeToResizeWindow = 0.7f;
    private const float _timeToCloseStat = 0.125f;
    private const float _timeToMoveStatSet = 0.125f;
    private const float _timeToRotateArrow = 0.25f;
    private const float _timeToSwipeArrowsAndStats = 0.06f;

    private bool _disclosureIsTrue = false;
    private bool _characteristicsWindowCoroutineIsComplete = true;
    private bool _statSetMoveIsComplete = false;
    private bool _allAnimationIsComplete = false;
    private readonly Vector2[,] _cachedMainStatsLocalPosition = new Vector2[Heroes.HeroCount, Heroes.StatsCount];
    private readonly Vector2[] _cachedBodySize = new Vector2[Heroes.HeroCount];

    private Animator _animator;

    private void Awake()
    {
        if (gameObject.name == "ChangeSizeWindow_Button")
        {
            _animator = gameObject.GetComponentInParent<Animator>();
        }
    }

    private void OnEnable()
    {
        if (gameObject.name == "ChangeSizeWindow_Button")
            _animator.SetBool("IsOpen", _disclosureIsTrue);
    }

    private void Start()
    {
        for (int heroIndex = 0; heroIndex < Heroes.HeroCount; heroIndex++)
        {
            _cachedBodySize[heroIndex] = Heroes.hero[heroIndex].CharacteristicsBody.sizeDelta;

            for (int statIndex = 0; statIndex < Heroes.StatsCount; statIndex++)
            {
                _cachedMainStatsLocalPosition[heroIndex, statIndex] = Heroes.hero[heroIndex].StatSetsBuffer[statIndex].localPosition;
                Heroes.hero[heroIndex].StatButtons[statIndex] = Heroes.hero[heroIndex].StatButtonsBackgrounds[statIndex].gameObject.GetComponent<Button>();
            }  
        }
    }

    #region ButtonEvent
    public void CloseCharacteristicElements()
    {
        for (int i = 0; i < Heroes.HeroCount; i++)
        {
            Heroes.hero[i].WindowSwipeButton.interactable = true;

            TemplateCloseCharacteristicElements(i);
        }
    }
    #endregion

    public void SwitchWidthÑharacteristicsWindow(int hero_index)
    {
        if (_characteristicsWindowCoroutineIsComplete == true)
        {
            AudioEffects.PlayExtendedList();

            if (_disclosureIsTrue == false)
            {
                _animator.SetTrigger("Open");
                SwipeStatsPosition(hero_index);
                StartCoroutine(WaitOpenOrClose(true));
                ReverseWindowArrow();
            }
            else
            {
                TemplateCloseCharacteristicElements(hero_index);
                _animator.SetTrigger("Close");
                SwipeStatsPosition(hero_index);
                StartCoroutine(WaitOpenOrClose(false));
                ReverseWindowArrow();
            }
        }

        IEnumerator WaitOpenOrClose(bool ToOpen)
        {
            _characteristicsWindowCoroutineIsComplete = false;
            Button window_arrow = gameObject.GetComponent<Button>();
            window_arrow.interactable = false;
            Heroes.ChooseHeroButton.interactable = false;
            Heroes.ChooseArmorButton.interactable = false;
            Heroes.ChooseWeaponButton.interactable = false;
            Heroes.ExitButton.interactable = false;

            if (ToOpen)
            {
                _animator.SetBool("IsOpen", false);

                yield return new WaitForSeconds(_timeToResizeWindow);

                _animator.SetBool("IsOpen", true);
            }
            else
            {
                _animator.SetBool("IsOpen", true);

                yield return new WaitForSeconds(_timeToResizeWindow);

                _animator.SetBool("IsOpen", false);
            }

            window_arrow.interactable = true;
            _disclosureIsTrue = !_disclosureIsTrue;
            _characteristicsWindowCoroutineIsComplete = true;
            Heroes.ChooseHeroButton.interactable = true;
            Heroes.ChooseArmorButton.interactable = true;
            Heroes.ChooseWeaponButton.interactable = true;
            Heroes.ExitButton.interactable = true;
        }

        void ReverseWindowArrow()
        {
            if (_disclosureIsTrue == false)
            { Heroes.hero[hero_index].WindowArrow.transform.localRotation = Quaternion.Euler(0, 0, 0); }
            else { Heroes.hero[hero_index].WindowArrow.transform.localRotation = Quaternion.Euler(0, 0, 180); }
        }
    }

    public void DiscloseMoreValues(int heroIndex)
    {
        bool arrowCoroutineIsComplete = false;
        _statSetMoveIsComplete = true;

        Image[] arrowComponents = gameObject.GetComponentsInChildren<Image>(includeInactive:true);
        foreach (Image Arrow in arrowComponents)
        {
            switch (Arrow.name)
            {
                case ("MaxHealth_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].MaxHealth_ButtonImage);
                    IndicateExtendedStats_MaxHealth();
                    break;
                case ("MaxStamina_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].MaxStamina_ButtonImage);
                    IndicateStats_MaxStamina();
                    break;
                case ("MaxWill_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].MaxWill_ButtonImage);
                    IndicateStats_MaxWill();
                    break;
                case ("HealthRegeneration_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].HealthRegeneration_ButtonImage);
                    IndicateStats_HealthRegeneration();
                    break;
                case ("StaminaRegeneration_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].StaminaRegeneration_ButtonImage);
                    IndicateStats_StaminaRegeneration();
                    break;
                case ("WillRegeneration_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].WillRegeneration_ButtonImage);
                    IndicateStats_WillRegeneration();
                    break;
                case ("Damage_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].Damage_ButtonImage);
                    IndicateStats_Damage();
                    break;
                case ("Armor_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].Armor_ButtonImage);
                    IndicateStats_Armor();
                    break;
                case ("Evasion_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].Evasion_ButtonImage);
                    IndicateStats_Evasion();
                    break;
                case ("Strength_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].Strength_ButtonImage);
                    IndicateStats_Strength();
                    break;
                case ("Dexterity_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].Dexterity_ButtonImage);
                    IndicateStats_Dexterity();
                    break;
                case ("Intellect_Arrow"):
                    ChangeCurrentArrow(Heroes.hero[heroIndex].Intellect_ButtonImage);
                    IndicateStats_Intellect();
                    break;
            }
        }

        #region Collection Stats Methods
        void IndicateExtendedStats_MaxHealth()
        {
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_MaxHealth));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_MaxHealth.Length, Heroes.hero[heroIndex].MaxHealthSet);
        }

        void IndicateStats_HealthRegeneration()
        {          
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_HealthRegeneration));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_HealthRegeneration.Length, Heroes.hero[heroIndex].HealthRegenerationSet);
        }

        void IndicateStats_MaxStamina()
        {       
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_MaxStamina));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_MaxStamina.Length, Heroes.hero[heroIndex].MaxStaminaSet);
        }

        void IndicateStats_StaminaRegeneration()
        {          
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_StaminaRegeneration));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_StaminaRegeneration.Length, Heroes.hero[heroIndex].StaminaRegenerationSet);
        }

        void IndicateStats_MaxWill()
        {            
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_MaxWill));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_MaxWill.Length, Heroes.hero[heroIndex].MaxWillSet);
        }

        void IndicateStats_WillRegeneration()
        {           
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_WillRegeneration));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_WillRegeneration.Length, Heroes.hero[heroIndex].WillRegenerationSet);
        }

        void IndicateStats_Damage()
        { 
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_Damage));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_Damage.Length, Heroes.hero[heroIndex].DamageSet);
        }

        void IndicateStats_Armor()
        {    
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_Armor));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_Armor.Length, Heroes.hero[heroIndex].ArmorSet);
        }

        void IndicateStats_Evasion()
        {
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_Evasion));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_Evasion.Length, Heroes.hero[heroIndex].EvasionSet);
        }

        void IndicateStats_Strength()
        {    
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_Strength));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_Strength.Length, Heroes.hero[heroIndex].StrengthSet);
        }

        void IndicateStats_Dexterity()
        {
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_Dexterity));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_Dexterity.Length, Heroes.hero[heroIndex].DexteritySet);
        }

        void IndicateStats_Intellect()
        {        
            StartCoroutine(ShowStatsSequentially(Heroes.hero[heroIndex].ExtendedStats_Intellect));
            MoveStatSet_AxisY(Heroes.hero[heroIndex].ExtendedStats_Intellect.Length, Heroes.hero[heroIndex].IntellectSet);
        }
        #endregion

        void ChangeCurrentArrow(Image current_arrow)
        {
            if (current_arrow.transform.localRotation != Heroes.StandardArrowRotation)
            { StartCoroutine(SoftRotateArrow(Heroes.StandardArrowRotation)); }
            else
            { StartCoroutine(SoftRotateArrow(Heroes.DiscloseArrowRotation)); }

            IEnumerator SoftRotateArrow(Quaternion target_rotation)
            {
                float Timer = 0;
                arrowCoroutineIsComplete = false;
                Quaternion base_rotation = current_arrow.transform.localRotation;

                while (Timer < _timeToRotateArrow)
                {
                    current_arrow.transform.localRotation = 
                        Quaternion.Lerp(base_rotation, target_rotation, Timer / _timeToRotateArrow);
                    yield return null;
                    Timer += Time.deltaTime;
                }

                current_arrow.transform.localRotation = target_rotation;
                arrowCoroutineIsComplete = true;
            }
        }

        IEnumerator ShowStatsSequentially(GameObject[] stats)
        {
            SwitchInteractableArrows(heroIndex, false);
            _statSetMoveIsComplete = false;
            _disclosureIsTrue = !_disclosureIsTrue;

            if (stats[0].activeSelf == false)
            {
                _disclosureIsTrue = true;
                for (int i = 0; i < stats.Length; i++)
                {
                    stats[i].SetActive(true);
                    AudioEffects.PlayStatsDisclosure();
                    yield return new WaitForSeconds(_timeToCloseStat);
                }
            }
            else
            {
                _disclosureIsTrue = false;
                for (int i = stats.Length - 1; i >= 0; i--)
                {
                    stats[i].SetActive(false);
                    AudioEffects.PlayStatsDisclosure();
                    yield return new WaitForSeconds(_timeToCloseStat);
                }
            }
            _statSetMoveIsComplete = true;
            if (arrowCoroutineIsComplete == true && _statSetMoveIsComplete == true)
            {
                SwitchInteractableArrows(heroIndex, true);
            }
        }

        void MoveStatSet_AxisY(int stats_length, RectTransform current_element)
        {
            int iter_start = 0;
            float time = _timeToMoveStatSet * stats_length;
            float calculatedValue_Y = _sizeStatElement_Y * stats_length;
            ChangeBodyHeight();

            switch (current_element.name)
            {
                case ("MaxHealth_Set"):
                    iter_start = 0;
                    break;
                case ("HealthRegeneration_Set"):
                    iter_start = 1;
                    break;
                case ("MaxStamina_Set"):
                    iter_start = 2;
                    break;
                case ("StaminaRegeneration_Set"):
                    iter_start = 3;
                    break;
                case ("MaxWill_Set"):
                    iter_start = 4;
                    break;
                case ("WillRegeneration_Set"):
                    iter_start = 5;
                    break;
                case ("Damage_Set"):
                    iter_start = 6;
                    break;
                case ("Armor_Set"):
                    iter_start = 7;
                    break;
                case ("Evasion_Set"):
                    iter_start = 8;
                    break;
                case ("Strength_Set"):
                    iter_start = 9;
                    break;
                case ("Dexterity_Set"):
                    iter_start = 10;
                    break;
                case ("Intellect_Set"):
                    iter_start = 11;
                    break;
            }

            for (int i = iter_start + 1; i < Heroes.StatsCount; i++)
            {
                StartCoroutine(SoftMoveStatSet(i));
            }

            IEnumerator SoftMoveStatSet(int statIndex)
            {
                float summ_Y = 0;
                float timer = 0;
                Vector2 current_element_position = Heroes.hero[heroIndex].StatSetsBuffer[statIndex].transform.localPosition;
                float current_X = current_element_position.x;
                float current_Y = current_element_position.y;

                if (_disclosureIsTrue == true)
                    summ_Y = current_Y - calculatedValue_Y;
                else summ_Y = current_Y + calculatedValue_Y;

                Vector2 base_pozition = new (current_X, current_Y);
                Vector2 target_pozition = new (current_X, summ_Y);

                while (timer < time)
                {
                    Heroes.hero[heroIndex].StatSetsBuffer[statIndex].transform.localPosition = Vector2.Lerp(base_pozition, target_pozition, timer / time);
                    yield return null;
                    timer += Time.deltaTime;
                }

                Heroes.hero[heroIndex].StatSetsBuffer[statIndex].transform.localPosition = target_pozition;
            }

            void ChangeBodyHeight()
            {
                Vector2 bodySize = Heroes.hero[heroIndex].CharacteristicsBody.sizeDelta;
                if (_disclosureIsTrue == true) bodySize.y += calculatedValue_Y;
                else bodySize.y -= calculatedValue_Y;
                Heroes.hero[heroIndex].CharacteristicsBody.sizeDelta = bodySize;
            }
        }
    }

    private void SwipeStatsPosition(int hero_index)
    {
        float y_position;
        float x_size;
        float y_size;
        int x_close = _statClosePosition_X;
        int x_open = _statOpenPosition_X;
        Vector2 close_scale = new(0, 0);
        Vector2 open_scale = new(1, 1);
        Vector2 close_position;
        Vector2 open_position;
        Vector2 const_size;

        StartCoroutine(SwipeArrowsAndStatNames(_timeToSwipeArrowsAndStats));

        IEnumerator SwipeArrowsAndStatNames(float time)
        {
            float timer = 0;
            _allAnimationIsComplete = false;
            SwitchInteractableArrows(hero_index, false);

            if (Heroes.hero[hero_index].StatButtonsBackgrounds[0].gameObject.activeInHierarchy == false)
            {
                for (int k = 0; k < Heroes.StatsCount; k++)
                {
                    AudioEffects.PlayExtendedList();
                    y_position = Heroes.hero[hero_index].ArrStatsName[k].rectTransform.localPosition.y;
                    x_size = Heroes.hero[hero_index].ArrStatsName[k].rectTransform.sizeDelta.x;
                    y_size = Heroes.hero[hero_index].ArrStatsName[k].rectTransform.sizeDelta.y;

                    close_position = new(x_close, y_position);
                    open_position = new(x_open, y_position);
                    const_size = new(x_size, y_size);

                    Heroes.hero[hero_index].StatButtonsBackgrounds[k].rectTransform.localScale = close_scale;
                    Heroes.hero[hero_index].StatButtonsBackgrounds[k].gameObject.SetActive(true);
                    while (timer < time)
                    {
                        Heroes.hero[hero_index].ArrStatsName[k].rectTransform.localPosition =
                            Vector2.Lerp(close_position, open_position, timer / time);
                        Heroes.hero[hero_index].StatButtonsBackgrounds[k].rectTransform.localScale =
                            Vector2.Lerp(close_scale, open_scale, timer / time);
                        timer += Time.deltaTime;
                        yield return null;
                    }

                    if (k == Heroes.StatsCount - 1)
                        _allAnimationIsComplete = true;
                    if (_allAnimationIsComplete)
                        SwitchInteractableArrows(hero_index, true);
                    Heroes.hero[hero_index].ArrStatsName[k].rectTransform.sizeDelta = const_size;
                    Heroes.hero[hero_index].ArrStatsName[k].rectTransform.localPosition = open_position;
                    Heroes.hero[hero_index].StatButtonsBackgrounds[k].rectTransform.localScale = open_scale;
                    timer = 0;
                }
            }
            else
            {
                for (int k = Heroes.StatsCount - 1; k >= 0; k--)
                {
                    AudioEffects.PlayExtendedList();
                    y_position = Heroes.hero[hero_index].ArrStatsName[k].rectTransform.localPosition.y;
                    x_size = Heroes.hero[hero_index].ArrStatsName[k].rectTransform.sizeDelta.x;
                    y_size = Heroes.hero[hero_index].ArrStatsName[k].rectTransform.sizeDelta.y;

                    close_position = new(-x_close, y_position);
                    open_position = new(x_open, y_position);
                    const_size = new(x_size, y_size);

                    while (timer < time)
                    {
                        Heroes.hero[hero_index].ArrStatsName[k].rectTransform.localPosition =
                            Vector2.Lerp(open_position, close_position, timer / time);
                        Heroes.hero[hero_index].StatButtonsBackgrounds[k].rectTransform.localScale =
                            Vector2.Lerp(open_scale, close_scale, timer / time);
                        timer += Time.deltaTime;
                        yield return null;
                    }

                    if(k == 0)
                        _allAnimationIsComplete = true;
                    if (_allAnimationIsComplete)
                        SwitchInteractableArrows(hero_index, true);
                    Heroes.hero[hero_index].ArrStatsName[k].rectTransform.sizeDelta = const_size;
                    Heroes.hero[hero_index].ArrStatsName[k].rectTransform.localPosition = close_position;
                    Heroes.hero[hero_index].StatButtonsBackgrounds[k].rectTransform.localScale = close_scale;
                    Heroes.hero[hero_index].StatButtonsBackgrounds[k].gameObject.SetActive(false);
                    timer = 0;
                }
            }
        }
    }

    private void TemplateCloseCharacteristicElements(int heroIndex)
    {
        Heroes.hero[heroIndex].CharacteristicsBody.sizeDelta = _cachedBodySize[heroIndex];

        for (int j = 0; j < Heroes.StatsCount; j++)
        {
            Heroes.hero[heroIndex].StatSetsBuffer[j].transform.localPosition = _cachedMainStatsLocalPosition[heroIndex, j];
            Heroes.hero[heroIndex].StatButtonsImages[j].transform.localRotation = Heroes.StandardArrowRotation;
            Heroes.hero[heroIndex].StatButtons[j].interactable = true;
        }

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_MaxHealth.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_MaxHealth[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_HealthRegeneration.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_HealthRegeneration[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_MaxStamina.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_MaxStamina[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_StaminaRegeneration.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_StaminaRegeneration[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_MaxWill.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_MaxWill[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_WillRegeneration.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_WillRegeneration[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_Damage.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_Damage[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_Armor.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_Armor[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_Evasion.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_Evasion[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_Strength.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_Strength[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_Dexterity.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_Dexterity[j].SetActive(false);

        for (int j = 0; j < Heroes.hero[heroIndex].ExtendedStats_Intellect.Length; j++)
            Heroes.hero[heroIndex].ExtendedStats_Intellect[j].SetActive(false);
    }

    private void SwitchInteractableArrows(int heroIndex, bool state)
    {
        Heroes.hero[heroIndex].WindowSwipeButton.interactable = state;

        for (int i = 0; i < Heroes.StatsCount; i++)
        {
            Heroes.hero[heroIndex].StatButtonsBackgrounds[i].GetComponent<Button>().interactable = state;
        }
    }
}