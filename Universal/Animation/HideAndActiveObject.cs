using UnityEngine;

public class HideAndActiveObject : MonoBehaviour
{
    public void Hide()
    {
        if (gameObject.activeInHierarchy)
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (!gameObject.activeInHierarchy)
        gameObject.SetActive(true);
    }
}