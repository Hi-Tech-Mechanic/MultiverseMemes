using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClickObject : DataStructure
{
    private bool _move;
    private Vector2 _randomVector;
    private TextMeshProUGUI _standardScoreText;

    private void Awake()
    { _standardScoreText = gameObject.GetComponent<TextMeshProUGUI>(); }

    private void Update()
    {
        if (!_move) return;
            gameObject.transform.Translate(_randomVector * Time.deltaTime);
    }

    public void StartMotion(float scoreIncrease)
    {
        gameObject.transform.localPosition = Vector2.zero;
        _standardScoreText.text = ValuesRounding.FormattingValue("+", "$", scoreIncrease);
        _randomVector = new Vector2 (Random.Range(-250,250), Random.Range(-250,250));
        _move = true;
        GetComponent<Animation>().Play("ClickTextFade");
    }

    public void StopMotion()
    { _move = false; }
}