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
        Strength.text = $"�� ������� ���� ������ {Heroes.AdditiveMaxHealthFromStrength} ��������, {Math.Round(Heroes.AdditiveHealthRegenerationFromStrength, 2)} � �����������, " +
            $"{Heroes.AdditiveDamageFromStrength} �����, {Heroes.AdditiveMaxStaminaFromStrength} ������ ������������, {Math.Round(Heroes.AdditiveStaminaRegenerationFromStrength, 2)} � �����������.";

        Dexterity.text = $"�� ������� �������� ����� {Math.Round(Heroes.AdditiveArmorFromDexterity, 3)} �����, {Heroes.AdditiveMaxStaminaFromDexterity} � ������ ������������, " +
            $"{Math.Round(Heroes.AdditiveStaminaRegenerationFromDexterity, 2)} � �����������";

        Intellect.text = $"�� ������� ���������� ����� {Heroes.AdditiveMaxWillFromIntellect} � ������ ����, {Math.Round(Heroes.AdditiveWillRegenerationFromIntellect, 2)} � �����������";
    }
}
