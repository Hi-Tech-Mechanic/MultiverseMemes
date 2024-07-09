using UnityEngine;

public class Disclaimer : MonoBehaviour
{
    public static bool DisclaimerIsWathced = false;
    [SerializeField] private GameObject _window;

    public void Init()
    {
        if (!DisclaimerIsWathced)
        {
            _window.SetActive(true);
        }
    }

    public void AcceptDisclimer()
    {
        DisclaimerIsWathced = true;
    }
}
