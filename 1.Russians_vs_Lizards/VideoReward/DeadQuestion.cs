using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using YG;

public class DeadQuestion : DataStructure
{
    [SerializeField] private GameObject _questionWindow;
    [SerializeField] private GameObject _videoImage;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private TextMeshProUGUI _itemCount;

    private void OnEnable()
    {
        CheckWaterCount();
        YandexGame.RewardVideoEvent += ADReward;
    }

    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= ADReward;
    }

    public void OpenQuestionWindow()
    { _questionWindow.SetActive(true); }

    public void CloseQuestionWindow()
    { _questionWindow.SetActive(false); }

    public void AcceptResurrected()
    {
        if (!Heroes.CurrentHero.IsAlive)
        {
            if (Items._BaikalWater.Count > 0)
            {
                Items.DrinkBaikalWater();
                UpdateItemCount();
                CloseQuestionWindow();
            }
        }
    }

    private void WathVideoForBaikalWater()
    {
        YandexGame.RewVideoShow((int)Game.RewardIndex.AddBaikalWater);
    }

    private void ADReward(int i)
    {
        if (i == (int)Game.RewardIndex.AddBaikalWater)
        {
            Game.AccumulateWatchedAD();

            Items._BaikalWater.Count++;
            Items.DrinkBaikalWater();
            CloseQuestionWindow();

            SaveAndLoad.SavePlayerData();
        }
    }

    private void CheckWaterCount()
    {
        UpdateItemCount();

        if (Items._BaikalWater.Count == 0)
        {
            _videoImage.SetActive(true);
            _acceptButton.onClick.AddListener(WathVideoForBaikalWater);
        }
        else 
        {
            _videoImage.SetActive(false);
            _acceptButton.onClick.RemoveAllListeners();
        }
    }

    private void UpdateItemCount()
    { _itemCount.text = $"{Items._BaikalWater.Count}"; }
}