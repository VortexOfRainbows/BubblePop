using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Experimental.AI;

public static class PlayerData
{
    public static int PlayerDeaths;
    public static float SFXVolume = 1;
    public static float MusicVolume = 1;
    public static bool PauseDuringPowerSelect = true;
    public static bool BriefDescriptionsByDefault = true;
    public static void SaveSettings()
    {
        SaveBool("PauseDuringPowerSelect", PauseDuringPowerSelect);
        SaveBool("BriefDescriptionsByDefault", BriefDescriptionsByDefault);
    }
    public static void SaveSound()
    {
        SaveFloat("SFX", SFXVolume);
        SaveFloat("Music", MusicVolume);
    }
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll(); //Reset all save data persistently for the purposes of the playtest
        LoadAll();
        SaveAll();
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
        PauseDuringPowerSelect = GetBool("PauseDuringPowerSelect", true);
        BriefDescriptionsByDefault = GetBool("BriefDescriptionsByDefault", true);
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
    public static void SaveBool(string tag, bool value)
    {
        PlayerPrefs.SetInt(tag, value ? 1 : 0);
    }
    public static bool GetBool(string tag, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(tag, defaultValue ? 1 : 0) == 1;
    }
    public static void SaveString(string tag, string value)
    {
        PlayerPrefs.SetString(tag, value);
    }
    public static string GetString(string tag)
    {
        return PlayerPrefs.GetString(tag, string.Empty);
    }
    public static void SaveTierList(TierList list)
    {
        string CSV = "";
        for(int i = 0; i < list.Categories.Length; ++i)
        {
            char tier = TierList.TierNames[i];
            CSV += tier;
            CSV += ":";
            TierCategory cat = list.Categories[i];
            List<CompendiumPowerUpElement> l = Compendium.GetCPUEChildren(cat.Grid.transform, out int c);
            for(int j = 0; j < l.Count; ++j)
            {
                CompendiumPowerUpElement cpue = l[j];
                if(cpue != null)
                {
                    CSV += cpue.PowerID;
                    CSV += ",";
                }
            }
        }
        //Output should look something like this:
        /*
         *S:1,2,3,4,5,
         *A:6,7,8,9,10,
         *B:11,12,13,14,15,
         *etc.
         */
        Debug.Log(CSV);
        SaveString("TierList", CSV);
    }
    public static void LoadTierList(TierList list)
    {
        string CSV = GetString("TierList");
        Debug.Log(CSV);
        if (CSV.Length <= 0)
            return;
        TierList.ReadingFromSave = true;
        string[] words = CSV.Split(',', ':');
        int CurrentCategory = -1;
        for(int i = 0; i < words.Length; ++i)
        {
            string word = words[i];
            if(TierList.TierNames.Contains(word)) //SABCDE
            {
                ++CurrentCategory;
            }
            else //Nums
            {
                list.SetSelectedCategory(CurrentCategory);
                int PowerID = int.Parse(word);
                if(PowerID >= 0)
                    list.PlacePower(PowerID, false);
            }
        }
        TierList.ReadingFromSave = false;
    }
}