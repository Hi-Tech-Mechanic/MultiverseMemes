using TMPro;
using UnityEngine;

public class StagesMenu : DataStructure
{
    [SerializeField] private TextMeshProUGUI _stageText;
    [SerializeField] private GameObject _selectNextStageButton;
    [SerializeField] private GameObject _selectPreviousStageButton;
    [SerializeField] private GameObject _prestigeAttentionSign;

    private void OnEnable()
    {
        CheckStagesState();
        UpdateStageText();
    }

    #region ButtonEvents
    public void SelectPreviousStage()
    {
        if (Battle.CurrentStage != 0)
        {
            Battle.CurrentStage--;
            UpdateStageText();
            CheckStagesState();
        }
        else _selectPreviousStageButton.SetActive(false);
    }

    public void SelectNextStage()
    {
        Battle.CurrentStage++;
        UpdateStageText();
        CheckStagesState();
    }

    public void GoToNextStage()
    {
        if (Heroes.CurrentHero.IsAlive)
        {
            Battle.CurrentStage++;
            UpdateStageText();

            if (Battle.CurrentStage >= 10 + (Battle.ResetCount * 5))
            {
                ActivePrestigeAttentionSign();
            }

            Battle.SwitchBattleMode("Active");
            EnemiesSystem.CreateEnemy();

            if (Battle.CurrentStageIsBossStage())
                Battle.StartBossBattle();
            Battle.CheckEnemiesInStage();
            CheckStagesState();
            CheckPlayingMusic();
        }
    }
    #endregion

    private void UpdateStageText()
    { _stageText.text = $"Поляна {Battle.CurrentStage}"; }

    private void CheckStagesState()
    {
        if (Battle.MaxOpenStage == 0)
        {
            _selectNextStageButton.SetActive(false);
            _selectPreviousStageButton.SetActive(false);
        }
        else if (Battle.CurrentStage == 0 && Battle.MaxOpenStage > 0)
        {
            _selectNextStageButton.SetActive(true);
            _selectPreviousStageButton.SetActive(false);
        }
        else if (Battle.CurrentStage != 0 && Battle.CurrentStage >= Battle.MaxOpenStage)
        {
            _selectNextStageButton.SetActive(false);
            _selectPreviousStageButton.SetActive(true);
        }
        else if (Battle.CurrentStage != 0 && Battle.CurrentStage < Battle.MaxOpenStage)
        {
            _selectNextStageButton.SetActive(true);
            _selectPreviousStageButton.SetActive(true);
        }
    }

    private void CheckPlayingMusic()
    {
        if (Music.GetMusicPlayer().isPlaying == false)
        {
            Music.SelectNextMusic();
        }
    }

    private void ActivePrestigeAttentionSign()
    {
        _prestigeAttentionSign.SetActive(true);
    }

    public void DisablePrestigeAttentionSign()
    {
        _prestigeAttentionSign.SetActive(false);
    }
}