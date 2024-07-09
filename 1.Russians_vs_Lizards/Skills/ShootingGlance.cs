using UnityEngine;

public class ShootingGlance : DataStructure
{
    [SerializeField] private GameObject[] _stats;

    public void Awake()
    {
        Skills._ShootingGlance.Init(_stats);
    }

    private void Start()
    {
        Skills._ShootingGlance.Start();
    }

    public void AddDamage()
    {
        Skills._ShootingGlance.AddDamage();
    }

    public void AddPeriodicDamage()
    {
        Skills._ShootingGlance.AddPeriodicDamage();
    }

    public void AddDuration()
    {
        Skills._ShootingGlance.AddDuration();
    }

    public void DecreaseReload()
    {
        Skills._ShootingGlance.DecreaseReload();
    }
}
