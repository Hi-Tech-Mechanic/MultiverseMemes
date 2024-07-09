using UnityEngine;

public class PrayerToTheGods : DataStructure
{
    [SerializeField] private GameObject[] _stats;

    public void Awake()
    {
        Skills._PrayerToTheGods.Init(_stats);
    }

    private void Start()
    {
        Skills._PrayerToTheGods.Start();
    }

    public void AddPercentToStats()
    {
        Skills._PrayerToTheGods.AddPercentToStats();
    }

    public void AddDuration()
    {
        Skills._PrayerToTheGods.AddDuration();
    }

    public void DecreaseReload()
    {
        Skills._PrayerToTheGods.DecreaseReload();
    }
}
