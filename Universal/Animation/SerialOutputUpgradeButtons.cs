using System.Collections;
using UnityEngine;

public class SerialOutputUpgradeButtons : DataStructure
{
    private GameObject[] ClickUpgradesBuffer;
    private GameObject[] IdleUpgradesBuffer;
    private Animator[] _animatorsBuffer_Click;
    private Animator[] _animatorsBuffer_Idle;
    private bool _activeClick = false;
    private bool _activeIdle = false;

    private void Start()
    {
        //ClickUpgradesBuffer = _Upgrades.GetClickCells();
        //IdleUpgradesBuffer = _Upgrades.GetIdleCells();
        //_animatorsBuffer_Click = new Animator[ClickUpgradesBuffer.Length];
        //_animatorsBuffer_Idle = new Animator[IdleUpgradesBuffer.Length];

        //for (int i = 0; i < ClickUpgradesBuffer.Length; i++)
        //    _animatorsBuffer_Click[i] = ClickUpgradesBuffer[i].GetComponent<Animator>();
        //for (int i = 0; i < IdleUpgradesBuffer.Length; i++)
        //    _animatorsBuffer_Idle[i] = IdleUpgradesBuffer[i].GetComponent<Animator>();
    }

    public void DisplayButtons()
    {
        if (_activeIdle == true)
            DisplayButtons_2();
        if (_activeIdle == false) 
        {
            for (int i = 0; i < ClickUpgradesBuffer.Length; i++)
                ClickUpgradesBuffer[i].SetActive(true);
            _activeClick = !_activeClick;
            StartCoroutine(active_animation());
        }

        IEnumerator active_animation()
        {
            for (int i = 0; i < ClickUpgradesBuffer.Length; i++)
            {
                _animatorsBuffer_Click[i].SetBool("SetButton", _activeClick);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    public void DisplayButtons_2()
    {
        if (_activeClick == true)
            DisplayButtons();
        if (_activeClick == false)
        {
            for (int i = 0; i < IdleUpgradesBuffer.Length; i++)
                IdleUpgradesBuffer[i].SetActive(true);
            _activeIdle = !_activeIdle;
            StartCoroutine(active_animation());
        }

        IEnumerator active_animation()
        {
            for (int i = 0; i < IdleUpgradesBuffer.Length; i++)
            {
                _animatorsBuffer_Idle[i].SetBool("SetButton", _activeIdle);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}