using UnityEngine;

public class BlackSauna : DataStructure
{
    [SerializeField] private GameObject[] _stats;

    public void Awake()
    {
        Skills._BlackSauna.Init(_stats);
    }

    private void Start()
    {
        Skills._BlackSauna.Start();
    }

    public void AddPeriodicDamage()
    {
        Skills._BlackSauna.AddPeriodicDamage();
    }

    public void DecreaseHealthRegeneration()
    {
        Skills._BlackSauna.DecreaseHealthRegeneration();
    }

    public void AddDuration()
    {
        Skills._BlackSauna.AddDuration();
    }

    public void DecreaseReload()
    {
        Skills._BlackSauna.DecreaseReload();
    }
}
