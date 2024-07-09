using System;
using UnityEngine;

public class FloatPrefabs : MonoBehaviour
{
    public GameObject Outlier;
    public GameObject InfoOutlier;
    [SerializeField] private GameObject _clickParent;
    [SerializeField] private GameObject _standardClick;
    [SerializeField] private GameObject _critClick;
    [NonSerialized] public ClickObject[] clickTextPool = new ClickObject[40];
    [NonSerialized] public CritClickObject[] CritClickTextPool = new CritClickObject[35];
    [NonSerialized] public int StandardTextNumber;
    [NonSerialized] public int CritTextNumber;

    private void Awake()
    { CacheClickTextPool(); }

    private void CacheClickTextPool()
    {
        for (int j = 0; j < clickTextPool.Length; j++)
            clickTextPool[j] = Instantiate(_standardClick, _clickParent.transform).GetComponent<ClickObject>();
        for (int j = 0; j < CritClickTextPool.Length; j++)
            CritClickTextPool[j] = Instantiate(_critClick, _clickParent.transform).GetComponent<CritClickObject>();
    }
}
