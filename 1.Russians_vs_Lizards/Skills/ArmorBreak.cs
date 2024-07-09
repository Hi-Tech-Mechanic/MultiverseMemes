using UnityEngine;

public class ArmorBreak : DataStructure
{
    [SerializeField] private GameObject[] _stats;

    public void Awake()
    {
        Skills._ArmorBreak.Init(_stats);
    }

    private void Start()
    {
        Skills._ArmorBreak.Start();
    }

    public void AddDamage()
    {
        Skills._ArmorBreak.AddDamage();
    }

    public void DecreaseArmor()
    {
        Skills._ArmorBreak.DecreaseArmor();
    }

    public void AddDuration()
    {
        Skills._ArmorBreak.AddDuration();
    }

    public void DecreaseReload()
    {
        Skills._ArmorBreak.DecreaseReload();
    }
}
