using UnityEngine;
using UnityEngine.UI;

public class ExpandedWindowController : MonoBehaviour
{
    [Header("Крупное окно выбора заднего фона")]
    [SerializeField] private Animator _animatorExpandedWindow;
    [SerializeField] private Image _arrowImage;
    private bool _expandedWindow = false;

    private void OnEnable()
    {
        _animatorExpandedWindow.SetBool("IsActive", _expandedWindow);
    }

    public void SwitchExpandedWindow()
    {
        Vector3 angles;
        _expandedWindow = !_expandedWindow;

        if (_expandedWindow)
        {
            angles = _arrowImage.transform.rotation.eulerAngles;
            angles.z = 0;
            _animatorExpandedWindow.SetTrigger("Active");
            _animatorExpandedWindow.SetBool("IsActive", _expandedWindow);
        }
        else
        {
            angles = _arrowImage.transform.rotation.eulerAngles;
            angles.z = 180;
            _animatorExpandedWindow.SetTrigger("Close");
            _animatorExpandedWindow.SetBool("IsActive", _expandedWindow);
        }

        _arrowImage.transform.rotation = Quaternion.Euler(angles);
    }
}
