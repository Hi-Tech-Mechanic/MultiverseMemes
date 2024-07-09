using DG.Tweening;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GetMoneyAnimation : DataStructure
{
    public GameObject PrefabsParent;

    [Header("MemeCoins")]
    [Space(5)]
    [SerializeField] private GameObject _memeCoinsAchievementTabTarget;
    [SerializeField] private GameObject _memeCoinsCalendarTabTarget;
    [SerializeField] private GameObject _memeCoinsCaseMenuTabTarget;
    [SerializeField] private GameObject _memeCoinPrefab;

    private Vector2 _memeCoinAchievementTabPosition;
    private Vector2 _memeCoinCalendarTabPosition;
    private Vector2 _memeCoinCaseMenuTabPosition;

    [Header("Other")]
    [Space(5)]
    public GameObject FaithCoinPrefab;
    public GameObject AncestralPowerCoinPrefab;

    [SerializeField] private GameObject _faithTabTarget;
    [SerializeField] private GameObject _ancestralPowerTabTarget;

    private readonly GameObject[] _createdMemeCoins = new GameObject[_maxCoinsInScene];
    private readonly GameObject[] _createdFaithCoins = new GameObject[_maxCoinsInScene];
    private readonly GameObject[] _createdAncestralPowerCoins = new GameObject[_maxCoinsInScene];

    private Vector2 _faithTabPostion;
    private Vector2 _ancestralPowerTabPosition;
    private const int _createFaithCoinsCount = 8;
    private const int _createAncestralPowerCoinsCount = 10;

    private const int _maxCoinsInScene = 10;
    private const int _createMemeCoinsCount = 8;
    private const float _timeToMove = 0.15f;
    private const float _timeToCreate = 0.05f;
    private const int _minRange = -300;
    private const int _maxRange = 300;
    private readonly Vector2 _baseSize = new(1, 1);
    private readonly Vector2 _targetSize = new(0.5f, 0.5f);

    private void Start()
    {
        if (_faithTabTarget != null)
        {
            _faithTabPostion = _faithTabTarget.transform.position;
        }
        if (_ancestralPowerTabTarget != null)
        {
            _ancestralPowerTabPosition = _ancestralPowerTabTarget.transform.position;
        }

        if (_memeCoinsAchievementTabTarget != null)
        {
            _memeCoinAchievementTabPosition = _memeCoinsAchievementTabTarget.transform.position;
        }
        if (_memeCoinsCalendarTabTarget != null)
        {
            _memeCoinCalendarTabPosition = _memeCoinsCalendarTabTarget.transform.position;
        }
        if (_memeCoinsCaseMenuTabTarget != null)
        {
            _memeCoinCaseMenuTabPosition = _memeCoinsCaseMenuTabTarget.transform.position;
        }
    }

    //public void CreateFaithCoins(float get_value)
    //{
    //    float x;
    //    float y;
    //    Vector2 vector;

    //    StartCoroutine(CreatePrefabs());

    //    IEnumerator CreatePrefabs()
    //    {
    //        for (int i = 0; i < _createFaithCoinsCount; i++)
    //        {
    //            if (_createdFaithCoins[i] == null)
    //            {
    //                x = Random.Range(_minRange, _maxRange);
    //                y = Random.Range(_minRange, _maxRange);
    //                vector = new(x, y);

    //                AudioEffects.PlayCreateCoinEffect();
    //                _createdFaithCoins[i] = Instantiate(FaithCoinPrefab, PrefabsParent.transform);
    //                _createdFaithCoins[i].transform.localPosition = vector;
    //            }

    //            yield return new WaitForSeconds(_timeToCreate);
    //        }

    //        StartCoroutine(MoveToTarget());
    //    }

    //    IEnumerator MoveToTarget()
    //    {
    //        for (int i = 0; i < _createFaithCoinsCount; i++)
    //        {
    //            if (_createdFaithCoins[i] != null)
    //            {
    //                float timer = 0;
    //                Vector2 base_position = _createdFaithCoins[i].transform.position;

    //                while (timer < _timeToMove)
    //                {
    //                    if (_createdFaithCoins[i] != null)
    //                    {
    //                        _createdFaithCoins[i].transform.position = Vector2.Lerp(base_position, _faithTabPostion, timer / _timeToMove);
    //                        _createdFaithCoins[i].transform.transform.localScale = Vector2.Lerp(_baseSize, _targetSize, timer / _timeToMove);
    //                    }

    //                    timer += Time.smoothDeltaTime;
    //                    yield return null;
    //                }

    //                if (_createdFaithCoins[i] != null)
    //                    _createdFaithCoins[i].transform.position = _faithTabPostion;

    //                DestroyPrefabs(i);
    //            }
    //            AddMoneyForCoin();
    //        }
    //    }

    //    void DestroyPrefabs(int index)
    //    {
    //        if (_createdFaithCoins[index] != null)
    //        {
    //            AudioEffects.PlayGetCoinEffect();
    //            Destroy(_createdFaithCoins[index]);
    //        }
    //    }

    //    void AddMoneyForCoin()
    //    {
    //        Facilities.AddFaithCurrency(get_value / _createFaithCoinsCount);
    //    }
    //}

    //public void CreateAncestralPowerCoins(float get_value)
    //{
    //    float x;
    //    float y;
    //    int min_range = -500;
    //    int max_range = 500;
    //    Vector2 vector;

    //    StartCoroutine(CreatePrefabs());

    //    IEnumerator CreatePrefabs()
    //    {
    //        for (int i = 0; i < _createAncestralPowerCoinsCount; i++)
    //        {
    //            if (_createdAncestralPowerCoins[i] == null)
    //            {
    //                x = Random.Range(min_range, max_range);
    //                y = Random.Range(min_range, max_range);
    //                vector = new(x, y);

    //                AudioEffects.PlayCreateCoinEffect();
    //                _createdAncestralPowerCoins[i] = Instantiate(AncestralPowerCoinPrefab, PrefabsParent.transform);
    //                _createdAncestralPowerCoins[i].transform.localPosition = vector;
    //            }

    //            yield return new WaitForSeconds(_timeToCreate);
    //        }

    //        StartCoroutine(MoveToTarget());
    //    }

    //    IEnumerator MoveToTarget()
    //    {
    //        for (int i = 0; i < _createAncestralPowerCoinsCount; i++)
    //        {
    //            if (_createdAncestralPowerCoins[i] != null)
    //            {
    //                float timer = 0;
    //                Vector2 base_position = _createdAncestralPowerCoins[i].transform.position;

    //                while (timer < _timeToMove)
    //                {
    //                    if (_createdAncestralPowerCoins[i] != null)
    //                    {
    //                        _createdAncestralPowerCoins[i].transform.position = Vector2.Lerp(base_position, _ancestralPowerTabPosition, timer / _timeToMove);
    //                        _createdAncestralPowerCoins[i].transform.transform.localScale = Vector2.Lerp(_baseSize, _targetSize, timer / _timeToMove);
    //                    }

    //                    timer += Time.smoothDeltaTime;
    //                    yield return null;
    //                }

    //                if (_createdAncestralPowerCoins[i] != null)
    //                    _createdAncestralPowerCoins[i].transform.position = _faithTabPostion;

    //                DestroyPrefabs(i);
    //            }
    //            AddMoneyForCoin();
    //        }
    //    }

    //    void DestroyPrefabs(int index)
    //    {
    //        if (_createdAncestralPowerCoins[index] != null)
    //        {
    //            AudioEffects.PlayGetCoinEffect();
    //            Destroy(_createdAncestralPowerCoins[index]);
    //        }
    //    }

    //    void AddMoneyForCoin()
    //    {
    //        Facilities.AddAncestralPower(get_value / _createAncestralPowerCoinsCount);
    //    }
    //}

    public void CreateAndAddCoins(float get_value, string type)
    {
        int currentCoinsAmount = _createMemeCoinsCount;
        float x;
        float y;
        Vector2 vector;
        Vector2 target_vector = Vector2.zero;
        GameObject coinPrefab = _memeCoinPrefab;
        Tween[] tween_move = new Tween[_maxCoinsInScene];
        Tween[] tween_size = new Tween[_maxCoinsInScene];

        switch (type)
        {
            case "CaseMenu":
                target_vector = _memeCoinCaseMenuTabPosition;
                break;
            case "Achievement":
                target_vector = _memeCoinAchievementTabPosition;
                break;
            case "Calendar":
                target_vector = _memeCoinCalendarTabPosition;
                break;
            case "FaithCoin":
                target_vector = _faithTabPostion;
                coinPrefab = FaithCoinPrefab;
                currentCoinsAmount = _createFaithCoinsCount;
                break;
            case "AncestralPower":
                target_vector = _ancestralPowerTabPosition;
                coinPrefab = AncestralPowerCoinPrefab;
                currentCoinsAmount = _createAncestralPowerCoinsCount;
                break;
        }

        StartCoroutine(CreatePrefabs());

        IEnumerator CreatePrefabs()
        {
            for (int i = 0; i < currentCoinsAmount; i++)
            {
                //DOTween.KillAll();

                if (_createdMemeCoins[i] == null)
                {
                    tween_move[i]?.Kill();
                    tween_size[i]?.Kill();

                    x = Random.Range(_minRange, _maxRange);
                    y = Random.Range(_minRange, _maxRange);
                    vector = new(x, y);

                    AudioEffects.PlayCreateCoinEffect();
                    _createdMemeCoins[i] = Instantiate(coinPrefab, PrefabsParent.transform);
                    _createdMemeCoins[i].transform.localPosition = vector;

                    yield return new WaitForSeconds(_timeToCreate);
                }
            }

            for (int i = 0; i < currentCoinsAmount; i++)
            {
                StartCoroutine(MoveToTarget(i));
                yield return new WaitForSeconds(_timeToCreate);
            }
        }

        IEnumerator MoveToTarget(int i)
        {
            if (_createdMemeCoins[i] != null)
            {
                tween_move[i] = _createdMemeCoins[i].transform.DOMove(target_vector, duration: _timeToMove);
                tween_size[i] = _createdMemeCoins[i].transform.DOScale(_targetSize, duration: _timeToMove);
                yield return new WaitForSeconds(_timeToMove);
                DestroyPrefabs(i);
            }

            AddMoneyForCoin();
        }

        void DestroyPrefabs(int index)
        {
            if (_createdMemeCoins[index] != null)
            {
                AudioEffects.PlayGetCoinEffect();
                tween_move[index]?.Kill();
                tween_size[index]?.Kill();
                Destroy(_createdMemeCoins[index]);
            }
        }

        void AddMoneyForCoin()
        {
            if (type == "FaithCoin")
                Facilities.AddFaithCurrency(get_value / currentCoinsAmount);
            else if (type == "AncestralPower")
                Facilities.AddAncestralPower(get_value / currentCoinsAmount);
            else 
                MoneyMenu.AddMemeCoins(get_value / currentCoinsAmount);
        }
    }
}