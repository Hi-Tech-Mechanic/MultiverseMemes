using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SlavsArsenalAnimations : HeroCardAnimations
{
    [SerializeField] private Color _standardColor;
    [SerializeField] private Color _activeColor;
    [NonSerialized] private Image ChooseHeroImage;
    [NonSerialized] private Image ChooseArmorImage;
    [NonSerialized] private Image ChooseWeaponImage;

    private void Start()
    {
        ChooseHeroImage = Heroes.ChooseHeroButton.GetComponent<Image>();
        ChooseArmorImage = Heroes.ChooseArmorButton.GetComponent<Image>();
        ChooseWeaponImage = Heroes.ChooseWeaponButton.GetComponent<Image>();
    }

    public void SoftTransformMenuButtons(int i)
    {
        RedactComponentTransform("close", ChooseHeroImage);
        RedactComponentTransform("close", ChooseArmorImage);
        RedactComponentTransform("close", ChooseWeaponImage);

        if (i == 0)
            RedactComponentTransform("open", ChooseHeroImage);
        else if (i == 1)
            RedactComponentTransform("open", ChooseArmorImage);
        else if (i == 2)
            RedactComponentTransform("open", ChooseWeaponImage);

        void RedactComponentTransform(string state, Image current_button)
        {
            float collapse_y = 1f;
            float expand_y = -14.5f;
            float expand_height = 110;
            float collapse_height = 80;

            StartCoroutine(MoveButton());

            IEnumerator MoveButton()
            {
                float timer = 0;
                float time = 0.2f;
                float current_x = current_button.rectTransform.localPosition.x;
                float current_y = current_button.rectTransform.localPosition.y;
                float current_x_size = current_button.rectTransform.sizeDelta.x;
                float current_y_size = current_button.rectTransform.sizeDelta.y;
                Vector2 base_position = new (current_x, current_y);
                Vector2 base_size = new (current_x_size, current_y_size);

                if (state == "open")
                {
                    current_y = expand_y;
                }
                else current_y = collapse_y;

                if (state == "open")
                {
                    current_y_size = expand_height;
                }
                else current_y_size = collapse_height;

                Vector2 target_position = new (current_x, current_y);
                Vector2 target_size = new (current_x_size, current_y_size);

                while (timer < time)
                {
                    current_button.rectTransform.localPosition = Vector2.Lerp(base_position, target_position, timer / time);
                    current_button.rectTransform.sizeDelta = Vector2.Lerp(base_size, target_size, timer / time);

                    if (state == "open")
                    {
                        current_button.color = Color.Lerp(_standardColor, _activeColor, timer / time);
                    }
                    else { current_button.color = _standardColor; }

                    yield return null;
                    timer += Time.deltaTime;
                }

                current_button.rectTransform.localPosition = new (current_x, current_y);
                current_button.rectTransform.sizeDelta = new (current_x_size, current_y_size);
            }
        }
    }
}