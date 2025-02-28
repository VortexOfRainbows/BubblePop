using UnityEngine;
using UnityEngine.Experimental.AI;

public static class PlayerData
{
    public static int PlayerDeaths;
    public static void SaveAll()
    {
        SaveInt("Deaths", PlayerDeaths);
        UnlockCondition.SaveAllData();
        PowerUp.SaveAllData();
    }
    public static void LoadAll()
    {
        PlayerDeaths = GetInt("Deaths");
        UnlockCondition.LoadAllData();
        PowerUp.LoadAllData();
    }
    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
    }
    public static void SaveInt(string tag, int value)
    {
        PlayerPrefs.SetInt(tag, value);
    }
    public static int GetInt(string tag)
    {
        return PlayerPrefs.GetInt(tag);
    }
    public static void SaveEquipmentData(string tag, Body.EquipSaveData value)
    {
        PlayerPrefs.SetInt(tag, value.Type);
        PlayerPrefs.SetInt(tag + "Cat", value.ParentCategory);
    }
    public static Body.EquipSaveData LoadEquipmentData(string tag)
    {
        return new Body.EquipSaveData(PlayerPrefs.GetInt(tag), PlayerPrefs.GetInt(tag + "Cat"));
    }
}