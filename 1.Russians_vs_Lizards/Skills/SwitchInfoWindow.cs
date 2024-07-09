using System;
using UnityEngine;

public class SwitchInfoWindow : DataStructure
{
    [SerializeField] private GameObject _informationWindow;
    [NonSerialized] private Animator _animator;
    [NonSerialized] private bool _active = false;

    private void Awake()
    { _animator = _informationWindow.GetComponent<Animator>(); }

    public void SwitchInformationWindow()
    {
        _active = !_active;
        if (_active)
        {
            _informationWindow.SetActive(_active);
            _animator.SetTrigger("Open");
        }
        else _animator.SetTrigger("Close");
    }
}