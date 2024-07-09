using UnityEngine;

public class GeneralFee : DataStructure
{
    [SerializeField] private GameObject[] _stats;

    public void Awake()
    {
        Skills._GeneralFee.Init(_stats);
    }

    private void Start()
    {
        Skills._GeneralFee.Start();
    }

    public void AddPercentToStats()
    {
        Skills._GeneralFee.AddPercentToStats();
    }

    public void AddDuration()
    {
        Skills._GeneralFee.AddDuration();
    }

    public void DecreaseReload()
    {
        Skills._GeneralFee.DecreaseReload();
    }
}
