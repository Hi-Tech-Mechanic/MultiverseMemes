using DG.Tweening;
using UnityEngine;

public class DOTweenAnimations : MonoBehaviour
{
    public void ButtonClickEffect()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), duration: 0.1f))
            .Append(transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), duration: 0.1f))
            .Append(transform.DOScale(new Vector3(1, 1, 1), duration: 0.05f));
    }

    public void ButtonClickLowEffect()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), duration: 0.1f))
            .Append(transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), duration: 0.1f))
            .Append(transform.DOScale(new Vector3(1, 1, 1), duration: 0.05f));
    }
}
