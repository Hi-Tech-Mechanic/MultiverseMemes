using UnityEngine;
using UnityEngine.UI;

public class SwitchWeaponSkins : MonoBehaviour
{
    public Sprite[] Skins;
    public Image _currentImage;
    private int _index = 0;

    public void NextSkin()
    {
        _index++;
        if (_index == Skins.Length)
        { _index = 0; }
        _currentImage.sprite = Skins[_index];
    }

    public void PreviousSkin()
    {
        _index--;
        if (_index < 0)
        { _index = Skins.Length - 1; }
        _currentImage.sprite = Skins[_index];
    }
}