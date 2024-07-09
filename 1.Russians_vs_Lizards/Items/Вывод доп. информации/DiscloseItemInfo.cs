using System.Collections;
using TMPro;
using UnityEngine;

public class DiscloseItemInfo : MonoBehaviour
{
    public TextMeshProUGUI InfoText;
    public GameObject InfoWindow;
    private string _savedText;
    private float _timeToNextSymbol = 0.01f;
    private bool _closed = true;

    private void Start()
    {
        _savedText = InfoText.text;
    }

    private void DiscloseInformation()
    {
        InfoText.text = "";
        InfoWindow.SetActive(_closed);

        StartCoroutine(WriteMessage());
    }

    private IEnumerator WriteMessage()
    {
        foreach (char symbol in _savedText)
        {
            InfoText.text += symbol;
            yield return new WaitForSeconds(_timeToNextSymbol);
            if (_closed == true)
                break;
        }
    }

    public void SwitchInfoDisplay()
    {
        if (_closed == true) 
        { DiscloseInformation(); }
        else 
        InfoWindow.SetActive(_closed);

        _closed = !_closed;
    }
}
