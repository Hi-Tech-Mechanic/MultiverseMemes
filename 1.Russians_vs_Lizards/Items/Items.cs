using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static ListOfEffects;

public class Items : DataStructure
{
    public TextMeshProUGUI TimerText;
    [SerializeField] private TextMeshProUGUI _baikalWaterCountText;
    [SerializeField] private TextMeshProUGUI[] _potionCountText;
    [SerializeField] private AudioClip[] _drinkEffects;
    [NonSerialized] public float[,] AdditiveStats = new float[Heroes.HeroCount, 3];
    [NonSerialized] public float[] Timer = new float[Heroes.HeroCount];

    public enum PotionsEnum
    {
        HealthPotion,
        StaminaPotion,
        WillPotion
    }

    public class BaikalWater
    {
        public const float BuffDuration = 30;
        public const float AdditivePercent = 0.3f;
        public int Count = 3;
    }
    public BaikalWater _BaikalWater = new();

    public class Potions
    {
        public int Count = 5;
        public const float RecoveryPercent = 0.25f;
    }
    public Potions HealthPotion = new();
    public Potions StaminaPotion = new();
    public Potions WillPotion = new();

    private void OnEnable()
    {
        DisplayBaikalWaterCount();
        DisplayAllPotionCount();
    }

    public void DrinkPotion(int potionIndex)
    {
        if (Heroes.CurrentHero.IsAlive)
        {
            if (potionIndex == (int)PotionsEnum.HealthPotion)
            {
                if (HealthPotion.Count > 0)
                {
                    if (Heroes.CurrentHero.ActualHealth != Heroes.CurrentHero.MaxHealth)
                    {
                        int rnd = UnityEngine.Random.Range(0, _drinkEffects.Length);
                        AudioEffects.PlayOneShotEffect(_drinkEffects[rnd]);

                        Items.HealthPotion.Count--;
                        _potionCountText[(int)PotionsEnum.HealthPotion].text = $"{HealthPotion.Count}";
                        Heroes.CurrentHero.ActualHealth += Heroes.CurrentHero.MaxHealth * Potions.RecoveryPercent;
                    }
                }
            }
            else if (potionIndex == (int)PotionsEnum.StaminaPotion)
            {
                if (StaminaPotion.Count > 0)
                {
                    if (Heroes.CurrentHero.ActualStamina != Heroes.CurrentHero.MaxStamina)
                    {
                        int rnd = UnityEngine.Random.Range(0, _drinkEffects.Length);
                        AudioEffects.PlayOneShotEffect(_drinkEffects[rnd]);

                        Items.StaminaPotion.Count--;
                        _potionCountText[(int)PotionsEnum.StaminaPotion].text = $"{StaminaPotion.Count}";
                        Heroes.CurrentHero.ActualStamina += Heroes.CurrentHero.MaxStamina * Potions.RecoveryPercent;
                    }
                }
            }
            else if (potionIndex == (int)PotionsEnum.WillPotion)
            {
                if (WillPotion.Count > 0)
                {
                    if (Heroes.CurrentHero.ActualWill != Heroes.CurrentHero.MaxWill)
                    {
                        int rnd = UnityEngine.Random.Range(0, _drinkEffects.Length);
                        AudioEffects.PlayOneShotEffect(_drinkEffects[rnd]);

                        Items.WillPotion.Count--;
                        _potionCountText[(int)PotionsEnum.WillPotion].text = $"{WillPotion.Count}";
                        Heroes.CurrentHero.ActualWill += Heroes.CurrentHero.MaxWill * Potions.RecoveryPercent;
                    }
                }
            }
        }
    }

    public void DrinkBaikalWater()
    {
        if (!TimerText.gameObject.activeInHierarchy || !Heroes.CurrentHero.IsAlive)
        {
            if (_BaikalWater.Count > 0)
            {
                int rnd = UnityEngine.Random.Range(0, _drinkEffects.Length);
                AudioEffects.PlayOneShotEffect(_drinkEffects[rnd]);

                _BaikalWater.Count--;
                DisplayBaikalWaterCount();
                StopAllCoroutines();
                Timer[Heroes.CurrentHero.Index] = BaikalWater.BuffDuration;
                StartCoroutine(BuffAndCountDown());

                if (!Heroes.CurrentHero.IsAlive)
                {
                    Heroes.CurrentHero.HeroIsResurrected();
                    BattleHero.HeroIsResurrected();
                }
            }
        }

        else DeadQuestion.AcceptResurrected();
    }

    public IEnumerator BuffAndCountDown()
    {
        int heroIndex = Heroes.CurrentHero.Index;
         
        if (Timer[heroIndex] == 0)
        {
            Timer[heroIndex] = BaikalWater.BuffDuration;
        }
        else
        {
            ListOfEffects.DestroySelectedBuffEffect((int)BuffEnum.WaterAttributes);
        }

        if (AdditiveStats[heroIndex, 0] == 0)
        {
            AdditiveStats[heroIndex, 0] = Heroes.CurrentHero.Strength * BaikalWater.AdditivePercent;
            AdditiveStats[heroIndex, 1] = Heroes.CurrentHero.Dexterity * BaikalWater.AdditivePercent;
            AdditiveStats[heroIndex, 2] = Heroes.CurrentHero.Intellect * BaikalWater.AdditivePercent;
            Heroes.CurrentHero.StrengthBase += AdditiveStats[heroIndex, 0];
            Heroes.CurrentHero.DexterityBase += AdditiveStats[heroIndex, 1];
            Heroes.CurrentHero.IntellectBase += AdditiveStats[heroIndex, 2];
        }

        TimerText.gameObject.SetActive(true);
        ListOfEffects.CreateBuffEffect((int)BuffEnum.WaterAttributes, Timer[heroIndex]);

        while (Timer[heroIndex] > 0 && heroIndex == Heroes.CurrentHero.Index)
        {
            TimerText.text = $"{(int)Timer[heroIndex]} сек.";
            Timer[heroIndex] -= Time.deltaTime;
            yield return null;
        }

        if (Timer[heroIndex] > 0)
            yield return new WaitForSeconds(Timer[heroIndex]);
        Timer[heroIndex] = 0;

        if (heroIndex == Heroes.CurrentHero.Index)
            TimerText.gameObject.SetActive(false);

        ResetReceivedStats(heroIndex);
    }

    private void ResetReceivedStats(int heroIndex)
    {
        Heroes.hero[heroIndex].StrengthBase -= AdditiveStats[heroIndex, 0];
        Heroes.hero[heroIndex].DexterityBase -= AdditiveStats[heroIndex, 1];
        Heroes.hero[heroIndex].IntellectBase -= AdditiveStats[heroIndex, 2];
        AdditiveStats[heroIndex, 0] = 0;
        AdditiveStats[heroIndex, 1] = 0;
        AdditiveStats[heroIndex, 2] = 0;
    }

    private void DisplayBaikalWaterCount()
    { _baikalWaterCountText.text = $"{_BaikalWater.Count}"; }

    private void DisplayAllPotionCount()
    {
        _potionCountText[(int)PotionsEnum.HealthPotion].text = $"{HealthPotion.Count}";
        _potionCountText[(int)PotionsEnum.StaminaPotion].text = $"{StaminaPotion.Count}";
        _potionCountText[(int)PotionsEnum.WillPotion].text = $"{WillPotion.Count}";
    }
}