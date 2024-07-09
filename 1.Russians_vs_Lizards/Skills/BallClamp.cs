using UnityEngine;

public class BallClamp : DataStructure
{
    [SerializeField] private GameObject[] _stats;

    public void Awake()
    {
        Skills._BallClamp.Init(_stats);
    }

    private void Start()
    {
        Skills._BallClamp.Start();
    }

    public void AddDamage()
    {
        Skills._BallClamp.AddDamage();
    }

    public void AddStunDuration()
    {
        Skills._BallClamp.AddStunDuration();
    }

    public void DecreaseReload()
    {
        Skills._BallClamp.DecreaseReload();
    }
}