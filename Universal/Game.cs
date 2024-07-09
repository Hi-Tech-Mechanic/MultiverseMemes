using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class Game : DataStructure
{
    public static Transform WorldCanvasTranform => GameObject.FindWithTag("Canvas").transform;
    public static int CurrentScene => SceneManager.GetActiveScene().buildIndex;
    public static int ScenesCount => Enum.GetNames(typeof(BuildIndex)).Length;

    public static int MaxCellCountInGame = 31;
    public static int MaxAchievementsCountInGame = 516;

    public static bool[] FirstLaunchScene = { true, true, true, true, true, true, true, true, true };
    public static bool FirstLaunchGame = true;

    public static long[] LastPlayingTime = new long[ScenesCount];
    public static long LastLaunchGameTime;
    public static long CurrentTime;

    public static float AccumulatedTime;
    public static float AccumulatedClicks;
    public static int VisitingDays;
    public static int CountWatchedAD;

    public static Action NewDayEvent;

    public enum BuildIndex
    {
        CombinedUniverse = 0,
        Russians_vs_Lizards = 1,
        GenaBukin_Universe = 2,
        GenaGorin_Universe = 3,
        Papich_Universe = 4,
        Univer_Universe = 5,
        Paranormal_Universe = 6,
        Rock_Universe = 7,
        Shrek_Universe = 8
    }

    public enum RewardIndex 
    { 
        ClickBuff = 0,
        IdleBuff = 1,
        OfflineReward = 2,
        AddBaikalWater = 3,
        PrestigeReward = 4,
        CaseReward = 5
    }

    private void Awake()
    {
        UpdateCurrentTime();

        if (SaveAndLoad.CurrentSaveMode == (int)SaveAndLoad.SaveMode.BinarySave)
        {
            SaveAndLoad.CheckBinarySave();
            Init();
        }

        if (YandexGame.SDKEnabled)
        {
            SaveAndLoad.CheckSaveYG();
        }
    }

    private void OnEnable()
    {
        YandexGame.GetDataEvent += SaveAndLoad.CheckSaveYG;

        //if (YandexGame.savesData.FirstLaunchGame)
        //    YandexGame.Instance.infoYG.playerInfoSimulation.authorized = false; //todo delete
        //else 
        //{
        //    Debug.Log("auth TRUE");
        //    YandexGame.Instance.infoYG.playerInfoSimulation.authorized = true; //todo delete
        //}
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= SaveAndLoad.CheckSaveYG;
    }

    public static void Init() //Start
    {
        CheckVisitingDays();
    }

    private void Update()
    {
        AccumulateTime();
        SaveAndLoad.AutoSave();
    }

    public static long[] GetLastPlayingTime()
    {
        UpdateLastPlayingTime();
        return LastPlayingTime;
    }

    public static int GetSecondsToNextDay()
    {
        long targetTime = LastLaunchGameTime;
        long currentTime = DateTime.Now.Ticks;

        TimeSpan elapsedTargetSpan = new(targetTime);
        TimeSpan elapsedCurrentSpan = new(currentTime);

        return (int)((elapsedTargetSpan.TotalSeconds + 86400) - elapsedCurrentSpan.TotalSeconds);
    }

    public static void AddOrUpdateMainLidearboard()
    {
        YandexGame.NewLeaderboardScores("PlayTime", (int)(AccumulatedTime * 60));
        Debug.Log("Leaderboard 'PlayTime' updated");
    }

    public static void CheckVisitingDays()
    {
        long residual = LastLaunchGameTime;
        TimeSpan elapsedSpan = new(residual);

        if (DateTime.Now.Ticks >= (elapsedSpan.Ticks + ((long)86400 * 10000000)))
        {
            LastLaunchGameTime = DateTime.Now.Ticks;
            VisitingDays++;
            NewDayEvent?.Invoke();
            Debug.Log("NEW DAY");
        }
    }

    private void UpdateCurrentTime()
    { CurrentTime = DateTime.Now.Ticks; }

    private static void UpdateLastPlayingTime()
    { LastPlayingTime[CurrentScene] = DateTime.Now.Ticks; }

    public static void AccumulateTime()
    { AccumulatedTime += Time.deltaTime / 60; }

    public static void AccumulateClicks()
    { AccumulatedClicks++; }

    public static void AccumulateWatchedAD()
    { CountWatchedAD++; }
}