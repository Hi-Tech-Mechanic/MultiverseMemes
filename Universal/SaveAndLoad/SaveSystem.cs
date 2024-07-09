using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static string Path_1 => Application.persistentDataPath + "/data_1.fun";
    public static string Path_2 => Application.persistentDataPath + "/data_2.fun";

    public static Action OnSaved;

    public static void SavePlayerData_1(MoneyMenu moneyMenu, Upgrades upgrades, GlobalUpgrades globalUpgrades,
        MemeShop memeShop, MusicOptions musicOptions, AudioEffectsOptions audioEffectsOptions)
    {
        BinaryFormatter formatter = new();
        FileStream stream = new (Path_1, FileMode.Create);

        PlayerData_1 data = new (moneyMenu, upgrades, globalUpgrades, memeShop, musicOptions, audioEffectsOptions);

        formatter.Serialize(stream, data);
        SaveAndLoad.IsSavingData_1 = true;
        stream.Close();

        if (stream.CanWrite == false)
        {
            Debug.Log("SavePlayerData_1 Complete");
            SaveAndLoad.IsSavingData_1 = false;
            OnSaved?.Invoke();
        }
    }

    public static PlayerData_1 LoadPlayer_1()
    {
        BinaryFormatter formatter = new ();
        FileStream stream = new (Path_1, FileMode.Open);

        PlayerData_1 data = formatter.Deserialize(stream) as PlayerData_1;

        stream.Close();
        return data;
    }

    public static void SavePlayerData_2(Heroes heroes, ArmorMark_1 mark_1, ArmorMark_2 mark_2, ArmorMark_3 mark_3, Weapons weapons,
        PerkTree perkTree, Skills skills, Facilities facilities, Russians_vs_Lizards.Achievements achievements, Battle battle, Items items, ListOfEffects listOfEffects)
    {
        BinaryFormatter formatter = new();
        FileStream stream = new(Path_2, FileMode.Create);

        PlayerData_2 data = new(heroes, mark_1, mark_2, mark_3, weapons, perkTree, skills, facilities, achievements, battle, items, listOfEffects);

        formatter.Serialize(stream, data);
        SaveAndLoad.IsSavingData_2 = true;
        stream.Close();

        if (stream.CanWrite == false)
        {
            Debug.Log("SavePlayerData_2 Complete");
            SaveAndLoad.IsSavingData_2 = false;
            OnSaved?.Invoke();
        }
    }

    public static PlayerData_2 LoadPlayer_2()
    {
        BinaryFormatter formatter = new();
        FileStream stream = new(Path_2, FileMode.Open);

        PlayerData_2 data = formatter.Deserialize(stream) as PlayerData_2;

        stream.Close();
        return data;
    }
}