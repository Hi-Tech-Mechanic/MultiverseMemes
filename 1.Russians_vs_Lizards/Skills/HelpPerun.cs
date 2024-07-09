using UnityEngine;

public class HelpPerun : DataStructure
{
    [SerializeField] private GameObject[] _stats;

    public void Awake()
    {
        Skills._HelpPerun.Init(_stats);
    }

    private void Start()
    {
        Skills._HelpPerun.Start();
    }

    public void AddLightningCount()
    {
        Skills._HelpPerun.AddLightningCount();
    }

    public void DecreaseReload()
    {
        Skills._HelpPerun.DecreaseReload();
    }
}
