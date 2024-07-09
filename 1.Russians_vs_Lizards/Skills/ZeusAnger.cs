using UnityEngine;

public class ZeusAnger : DataStructure
{
    [SerializeField] private GameObject[] _stats;

    public void Awake()
    {
        Skills._ZeusAnger.Init(_stats);
    }

    private void Start()
    {
        Skills._ZeusAnger.Start();
    }

    public void AddPercentDamage()
    {
        Skills._ZeusAnger.AddPercentDamage();
    }

    public void AddLightningCount()
    {
        Skills._ZeusAnger.AddLightningCount();
    }

    public void DecreaseTimeBetweenAttacks()
    {
        Skills._ZeusAnger.DecreaseTimeBetweenAttacks();
    }

    public void DecreaseReload()
    {
        Skills._ZeusAnger.DecreaseReload();
    }
}
