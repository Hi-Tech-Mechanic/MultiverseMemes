using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectBackground : MonoBehaviour
{
    [NonSerialized] public int BackgroundIndex;
    private Image _theBackground;
    private Image _selectedField;
    private Image _backgroundimage;

    public void Init()
    {
        if (GameObject.Find("/SceneTemplate Variant/Canvas/SafeArea/TheBackground") != null)
            _theBackground = GameObject.Find("/SceneTemplate Variant/Canvas/SafeArea/TheBackground").GetComponent<Image>();
        else _theBackground = GameObject.Find("/Canvas/SafeArea/TheBackground").GetComponent<Image>();

        Image[] components = gameObject.GetComponentsInChildren<Image>(includeInactive: true);
        foreach (Image component in components)
        {
            switch (component.name) 
            {
                case("SelectedField"):
                    _selectedField = component;
                    break;
                case ("Target Image"):
                    _backgroundimage = component;
                    break;
            }
        }
    }

    public void Select()
    {
        _theBackground.sprite = _backgroundimage.sprite;
        _selectedField.gameObject.SetActive(true);
        SwitchBackground.CurrentImageIndex[Game.CurrentScene] = BackgroundIndex;
    }

    public void HideSelected()
    {
        _selectedField.gameObject.SetActive(false);
    }
}
