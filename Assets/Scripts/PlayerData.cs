using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    private static readonly SaveData SaveData = SaveData.LoadFromJson();
    public static class MetaProgression
    {
        public static int AchievementStars { get; set; } = 0;
        public static int TotalAchievementStars { get; set; } = 0;
        public static int MeadowsStars { get; set; } = 0;
        public static int TotalMeadowsStars { get; set; } = 0;
    }
    public static readonly float CurrentPlayerVersion = 1.2f;
    public static int PlayerDeaths;
    public static float SFXVolume = 1;
    public static float MusicVolume = 1;
    public static float SpecialVisualOpacity = 1;
    public static bool PauseDuringPowerSelect = true;
    public static bool PauseDuringCardSelect = true;
    public static bool BriefDescriptionsByDefault = true;
    public static void SaveSettingsToggles()
    {
        SaveBool("PauseDuringPowerSelect", PauseDuringPowerSelect);
        SaveBool("PauseDuringCardSelect", PauseDuringCardSelect);
        SaveBool("BriefDescriptionsByDefault", BriefDescriptionsByDefault);
    }
    public static void SaveSettingSliders()
    {
        SaveFloat("SFX", SFXVolume);
        SaveFloat("Music", MusicVolume);
        SaveFloat("SpecialOpacity", SpecialVisualOpacity);
    }
    public static void ResetAll()
    {
        SaveData.DeleteAll();
        LoadAll();
        SaveAll();
    }
    public static void SaveAll()
    {
        SaveInt("Deaths", PlayerDeaths);
        SaveSettingSliders();
        UnlockCondition.SaveAllData();
        PowerUp.SaveAllData();
        CoinManager.Save();
        SaveInt("HighscoreWave", WaveDirector.HighScoreWaveNum);
        SaveInt("PlayerGoldSpent", Player.GoldSpentTotal);
        SaveData.SaveToJson();
    }
    public static void LoadAll()
    {
        SaveData.LoadFromJson();
        float PreviousPlayerVersion = GetFloat("VersionNumber", 1.0f);
        if(CurrentPlayerVersion != PreviousPlayerVersion)
        {
            SaveFloat("VersionNumber", CurrentPlayerVersion);
            VersionResetProcedure();
        }
        PauseDuringPowerSelect = GetBool("PauseDuringPowerSelect", true);
        PauseDuringCardSelect = GetBool("PauseDuringCardSelect", true);
        BriefDescriptionsByDefault = GetBool("BriefDescriptionsByDefault", true);
        PlayerDeaths = GetInt("Deaths");
        SFXVolume = GetFloat("SFX", 1);
        MusicVolume = GetFloat("Music", 1);
        SpecialVisualOpacity = GetFloat("SpecialOpacity", 1);
        Player.GoldSpentTotal = GetInt("PlayerGoldSpent", 0);
        UnlockCondition.LoadAllData();
        PowerUp.LoadAllData();
        CoinManager.Load();
        WaveDirector.HighScoreWaveNum = GetInt("HighscoreWave", 0);
    }
    public static void SaveInt(string tag, int value) => SaveData.Write(tag, value);
    public static int GetInt(string tag, int defaultValue = 0) => SaveData.Read(tag, defaultValue);
    public static void SaveFloat(string tag, float value) => SaveData.Write(tag, value);
    public static float GetFloat(string tag, float defaultValue = 0) => SaveData.Read(tag, defaultValue);
    public static void SaveBool(string tag, bool value) => SaveData.Write(tag, value);
    public static bool GetBool(string tag, bool defaultValue = false) => SaveData.Read(tag, defaultValue);
    public static void SaveString(string tag, string value) => SaveData.Write(tag, value);
    public static string GetString(string tag) => SaveData.Read(tag, string.Empty);
    public static void SaveTierList(TierList list)
    {
        string CSV = string.Empty;
        for(int i = 0; i < list.Categories.Length; ++i)
        {
            char tier = TierList.TierNames[i];
            CSV += tier;
            CSV += ":";
            TierCategory cat = list.Categories[i];
            List<CompendiumElement> l = TierListCompendiumPage.GetCPUEChildren(cat.Grid.transform, out int c);
            for(int j = 0; j < l.Count; ++j)
            {
                CompendiumElement cpue = l[j];
                if(cpue != null)
                {
                    CSV += cpue.TypeID;
                    CSV += ",";
                }
            }
        }
        //Output should look something like this:
        /*
         *0
         *S:1,2,3,4,5,
         *A:6,7,8,9,10,
         *B:11,12,13,14,15,
         *etc.
         */
        Debug.Log(CSV);
        string tag = list.TierListType == 2 ? "TierList2" : list.TierListType == 1 ? "TierList1" : "TierList";
        SaveString(tag, CSV);
    }
    public static void LoadTierList(TierList list)
    {
        string tag = list.TierListType == 2 ? "TierList2" : list.TierListType == 1 ? "TierList1" : "TierList";
        string CSV = GetString(tag);
        TierList.ReadingFromSave = true;
        string[] words = CSV.Split(',', ':');
        int CurrentCategory = -1;
        try
        {
            for (int i = 0; i < words.Length; ++i)
            {
                string word = words[i];
                if (TierList.TierNames.Contains(word)) //SABCDE
                {
                    ++CurrentCategory;
                }
                else //Nums
                {
                    list.SetSelectedCategory(CurrentCategory);
                    int PowerID = int.Parse(word);
                    if (PowerID >= 0)
                        list.PlacePower(PowerID, false);
                }
            }
        }
        catch { }
        TierList.ReadingFromSave = false;
    }
    public static void VersionResetProcedure()
    {
        PlayerPrefs.DeleteAll();
        SaveData.DeleteKey<int>("LastSelectedChar");
        string TypeName;
        for(int i = 0; i < Main.GlobalEquipData.Characters.Count; ++i)
        {
            TypeName = Main.GlobalEquipData.Characters[i].GetComponent<Equipment>().TypeName;
            SaveData.DeleteKey<int>($"{TypeName}Hat");
            SaveData.DeleteKey<int>($"{TypeName}Acc");
            SaveData.DeleteKey<int>($"{TypeName}Wep");
        }
        UnlockCondition.Get<GachaponUnlock>().SetComplete(false, false);
        UnlockCondition.Get<ThoughtBubbleUnlock>().SetComplete(false, false);
    }
}
[Serializable]
public class SaveData
{
    public static readonly string FileName = "SaveData.json";
    public static readonly string SaveDirectory = Application.persistentDataPath + $"/{FileName}";
    public Dictionary<string, int> IntData = new();
    public Dictionary<string, bool> BoolData = new();
    public Dictionary<string, float> FloatData = new();
    public Dictionary<string, string> StringData = new();
    public void DeleteAll()
    {
        IntData.Clear();
        BoolData.Clear();
        FloatData.Clear();
        StringData.Clear();
    }
    public void Write<T>(string tag, T data)
    {
        if(data is int)
            IntData[tag] = (int)(object)data;
        else if(data is bool)
            BoolData[tag] = (bool)(object)data;
        else if(data is float)
            FloatData[tag] = (float)(object)data;
        else if(data is string)
            StringData[tag] = (string)(object)data;
        else
            throw new Exception("Tried to write data of improper type. Data must be int, bool, float, or string");
    }
    public T Read<T>(string tag, T defaultValue = default)
    {
        if (typeof(T) == typeof(int))
            return IntData.TryGetValue(tag, out int r) ? (T)(object)r : defaultValue;
        else if (typeof(T) == typeof(bool))
            return BoolData.TryGetValue(tag, out bool r) ? (T)(object)r : defaultValue;
        else if (typeof(T) == typeof(float))
            return FloatData.TryGetValue(tag, out float r) ? (T)(object)r : defaultValue;
        else if (typeof(T) == typeof(string))
            return StringData.TryGetValue(tag, out string r) ? (T)(object)r : defaultValue;
        else
            throw new Exception("Tried to read data of improper type. Data must be int, bool, float, or string");
    }
    public void DeleteKey<T>(string tag, T resetValue = default)
    {
        Write(tag, resetValue);
    }
    //For now, it is okay for savedata to be unencrypted.
    public void SaveToJson()
    { 
        string json = JsonConvert.SerializeObject(this, Formatting.Indented);
        Debug.Log($"Saved data to: {SaveDirectory}");
        System.IO.File.WriteAllText(SaveDirectory, json);
    }
    /// <summary>
    /// Returns a new savedata object if it cannot be loaded
    /// </summary>
    /// <returns></returns>
    public static SaveData LoadFromJson()
    {
        if(System.IO.File.Exists(SaveDirectory))
        {
            string json = System.IO.File.ReadAllText(SaveDirectory);
            return (SaveData)JsonConvert.DeserializeObject(json, typeof(SaveData));
        }
        return new();
    }
}