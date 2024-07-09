using TMPro;
using UnityEngine;
using YG;

public class PlayerAuthentication : MonoBehaviour
{
    [SerializeField] private GameObject _authWindow;
    [SerializeField] private GameObject _moreInfoButton;
    [SerializeField] private TextMeshProUGUI _authStateInfo;

    private void OnEnable()
    {
       YandexGame.GetDataEvent += Init;
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= Init;
    }

    private void Start()
    {
        if (YandexGame.SDKEnabled)
        {
            Init();
        }
    }

    public void Init()
    {
        if (YandexGame.auth)
        {
            _authStateInfo.alignment = TextAlignmentOptions.Midline;
            _authStateInfo.text = "Авторизация:\n<color=green>выполнена</color>";
            _moreInfoButton.gameObject.SetActive(false);

#if UNITY_EDITOR
            Debug.Log("<color=green>Auth COMPLETE!</color>");
#endif
        }
        else
        {
            _authStateInfo.alignment = TextAlignmentOptions.Top;
            _authStateInfo.text = "Авторизация:\n<color=red>не выполнена</color>";
            _moreInfoButton.gameObject.SetActive(true);

#if UNITY_EDITOR
            Debug.Log("<color=red>Auth NON COMPLETE!</color>");
#endif
        }
    }
}