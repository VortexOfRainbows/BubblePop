using UnityEngine;

public static class PlayerData
{
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
}