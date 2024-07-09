using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CritClickObject : DataStructure
{
    private bool _move;
    private Vector2 _randomVector;
    private TextMeshProUGUI _scoreText;

    private void Awake()
    { _scoreText = gameObject.GetComponent<TextMeshProUGUI>(); }

    private void Update()
    {
        if (!_move) return;
            gameObject.transform.Translate(_randomVector * Time.deltaTime);
    }

    public void StartMotionCrit(float scoreIncrease)
    {
        gameObject.transform.localPosition = Vector2.zero;
        _scoreText.text = ValuesRounding.FormattingValue("+", "$", scoreIncrease);
        _randomVector = new Vector2 (Random.Range(-250,250), Random.Range(-250,250));
        _move = true;
        GetComponent<Animation>().Play("CritClickTextFade");
    }

    public void StopMotionCrit()
    { _move = false; }
}