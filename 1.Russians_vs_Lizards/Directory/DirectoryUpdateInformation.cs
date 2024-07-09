using TMPro;
using UnityEngine;
using System;

public class DirectoryUpdateInformation : DataStructure
{
    [SerializeField] private TextMeshProUGUI Strength;
    [SerializeField] private TextMeshProUGUI Dexterity;
    [SerializeField] private TextMeshProUGUI Intellect;

    public void UpdateInfo()
    {
        Strength.text = $"За единицу силы дается {Heroes.AdditiveMaxHealthFromStrength} здоровья, {Math.Round(Heroes.AdditiveHealthRegenerationFromStrength, 2)} её регенерации, " +
            $"{Heroes.AdditiveDamageFromStrength} урона, {Heroes.AdditiveMaxStaminaFromStrength} запаса выносливости, {Math.Round(Heroes.AdditiveStaminaRegenerationFromStrength, 2)} её регенерации.";

        Dexterity.text = $"За единицу ловкости даётся {Math.Round(Heroes.AdditiveArmorFromDexterity, 3)} брони, {Heroes.AdditiveMaxStaminaFromDexterity} к запасу выносливости, " +
            $"{Math.Round(Heroes.AdditiveStaminaRegenerationFromDexterity, 2)} её регенерации";

        Intellect.text = $"За единицу интеллекта даётся {Heroes.AdditiveMaxWillFromIntellect} к запасу воли, {Math.Round(Heroes.AdditiveWillRegenerationFromIntellect, 2)} её регенерации";
    }
}
