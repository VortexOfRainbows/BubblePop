using UnityEngine;
using UnityEngine.Experimental.AI;

public static class PlayerData
{
    public static int PlayerDeaths;
    public static float SFXVolume = 1;
    public static float MusicVolume = 1;
    public static void SaveSound()
    {
        SaveFloat("SFX", SFXVolume);
        SaveFloat("Music", MusicVolume);
    }
    public static void SaveAll()
    {
        SaveInt("Deaths", PlayerDeaths);
        SaveSound();
        UnlockCondition.SaveAllData();
        PowerUp.SaveAllData();
        CoinManager.Save();
    }
    public static void LoadAll()
    {
        PlayerDeaths = GetInt("Deaths");
        SFXVolume = GetFloat("SFX", 1);
        MusicVolume = GetFloat("Music", 1);
        UnlockCondition.LoadAllData();
        PowerUp.LoadAllData();
        CoinManager.Load();
    }
    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
    }
    public static void SaveInt(string tag, int value)
    {
        PlayerPrefs.SetInt(tag, value);
    }
    public static int GetInt(string tag, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(tag, defaultValue);
    }
    public static void SaveFloat(string tag, float value)
    {
        PlayerPrefs.SetFloat(tag, value);
    }
    public static float GetFloat(string tag, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(tag, defaultValue);
    }
}