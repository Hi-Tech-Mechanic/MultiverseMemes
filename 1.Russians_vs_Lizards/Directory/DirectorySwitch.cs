using UnityEngine;

public class DirectorySwitch : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private bool _windowIsOpen = false;

    public void SwitchState()
    {
        _windowIsOpen = gameObject.activeInHierarchy;

        if (!_windowIsOpen)
        {
            gameObject.SetActive(true);
            _animator.SetTrigger("Open");
        }
        else
        {
            _animator.SetTrigger("Close");
        }

        _windowIsOpen = !_windowIsOpen;
    }
}