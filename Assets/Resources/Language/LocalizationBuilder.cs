using Hjson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Localization
{
    public static string CurrentLanguage => "en-US";
    public static string ResourceDirectory => "Assets/Resources/Language/";
    public static string JsonFilePath => $"Assets/Resources/Language/{CurrentLanguage}.json";
    public static Dictionary<string, string> CondensedTranslation = new();
    public static string Get(string key)
    {
        if (CondensedTranslation.TryGetValue(key, out string value))
            return value;
        else
        {
#if !UNITY_EDITOR
            Debug.LogWarning($"Localization key: {key} not found in dictionary. Returning key as placeholder.".WithColor("#FF0000"));
            return "";
#else
            return LocalizationBuilder.RequestPlaceholderValue(key);
#endif
        }
    }
}
#if UNITY_EDITOR
public static class LocalizationBuilder
{
    public static string ResourcePath => "Assets/Resources/Language/";
    public static string ResourceName => "en-US.hjson";
    public static string CondensedName => "en-US.json";
    public static string ResourceDirectory => ResourcePath + ResourceName;
    public static string CondensedDirectory => ResourcePath + CondensedName;
    public static Dictionary<string, object> ExpandedTranslation = LoadFromJson();
    public static ref Dictionary<string, string> CondensedTranslation => ref Localization.CondensedTranslation;
    //public static Dictionary<string, string> FinalizedTranslation = new();
    public static FileSystemWatcher Watcher;
    /// <summary>
    /// Update the localization file if:
    /// 1. A key is requested that doesn't exist in the dictionary, which will add it to the dictionary with a placeholder value (the key itself) and mark that the dictionary needs to be reloaded.
    /// 2. The dictionary file itself is updated externally, which can be detected by comparing the last modified time of the file to a stored value. If the file has been modified since the last check, it will reload the dictionary from the file.
    /// </summary>
    public static void FileChangeUpdate(object source, FileSystemEventArgs e)
    {
        Debug.Log("Localization File Change Detected. Rebuilding Dictionary.".WithColor("#FF8800"));
        LoadFromJson();
        Debug.Log("Complete.".WithColor("#FF8800"));
    }
    public static void ReloadRequiredUpdate()
    {
        Debug.Log("Localization File Missing Required Keys. Rebuilding File.".WithColor("#FF0088"));
        SaveToJson();
        RequiresDictionaryReload = false;
        Debug.Log("Complete.".WithColor("#FF0088"));
    }
    public static void InstallHandler()
    {
        Watcher = new(ResourcePath, ResourceName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
        };
        Watcher.Changed += FileChangeUpdate;
        Watcher.EnableRaisingEvents = true;
    }
    public static void UninstallHandler()
    {
        if (Watcher != null)
        {
            Watcher.Changed -= FileChangeUpdate;
            Watcher.Dispose();
        }
    }
    public static bool RequiresDictionaryReload { get; set; } = false;
    public static string RequestPlaceholderValue(string key)
    {
        RequiresDictionaryReload = true;
        string[] keys = key.Split('.');
        var Dictionary = ExpandedTranslation;
        for (int i = 0; i < keys.Length; ++i)
        {
            string k = keys[i];
            if (Dictionary.TryGetValue(k, out object value))
            {
                if (value is string str)
                    return str;
                else if (value is Dictionary<string, object> dict)
                    Dictionary = dict;
                else
                    break;
            }
            else
            {
                if(i == keys.Length - 1)
                {
                    Dictionary[k] = key;
                    return key;
                }
                else
                {
                    Dictionary[k] = new Dictionary<string, object>();
                    Dictionary = (Dictionary<string, object>)Dictionary[k];
                }
            }
        }
        //Dictionary[key] = key;
        return key;
    }
    public static void ResetDictionaries()
    {
        ExpandedTranslation = new();
        CondensedTranslation = new();
    }
    public static void UpdateLocalizationFiles()
    {
        Debug.Log("Building Dictionary...".WithColor("#FF8800"));
        ResetDictionaries();
        ExpandedTranslation = LoadFromJson();
        SaveToJson();
        Debug.Log("Done...".WithColor("#FF8800"));
    }
    public static void SaveToJson()
    {
        //Debug.Log($"Dict: {ExpandedTranslation}");
        string standardJson = JsonConvert.SerializeObject(ExpandedTranslation, Formatting.Indented); 
        var jsonObject = HjsonValue.Parse(standardJson);
        using StringWriter stringWriter = new();
        HjsonOptions options = new();
        HjsonValue.Save(jsonObject, stringWriter, options);
        File.WriteAllText(ResourceDirectory, stringWriter.ToString());
        ProcessExpandedToCondensed();
    }
    public static void ProcessExpandedToCondensed()
    {
        Debug.Log("Creating Finalized Dictionary (.json)".WithColor("#FF00FF"));
        if (CondensedTranslation != null) 
            CondensedTranslation.Clear();
        else
            CondensedTranslation = new();
        foreach (var kvp in ExpandedTranslation)
        {
            //Debug.Log($"Adding Key: {kvp.Key} with Value Type: {kvp.GetType()}");
            ProcessNode(kvp.Key, kvp.Value, string.Empty);
        }
        string standardJson = JsonConvert.SerializeObject(CondensedTranslation, Formatting.Indented);
        File.WriteAllText(CondensedDirectory, standardJson);
        Debug.Log("Done...".WithColor("#FF00FF"));
    }
    private static void ProcessNode(string key, object value, string parentKey)
    {
        string fullKey = string.IsNullOrEmpty(parentKey) ? key : $"{parentKey}.{key}";
        if (value is string str)
        {
            CondensedTranslation[fullKey] = str;
        }
        else if (value is Dictionary<string, object> dict)
        {
            foreach (var kvp in dict)
            {
                //Debug.Log($"Adding Key: {kvp.Key} with Value Type: {kvp.GetType()}");
                ProcessNode(kvp.Key, kvp.Value, fullKey);
            }
        }
        else
        {
            Debug.LogWarning($"Unexpected value type for key: {fullKey}. Expected string or dictionary, got {(value == null ? "None" : value.GetType())}".WithColor("#FF0000"));
        }
    }
    public static Dictionary<string, object> LoadFromJson()
    {
        if (File.Exists(ResourceDirectory))
        {
            string hjson = File.ReadAllText(ResourceDirectory);
            string json = HjsonValue.Parse(hjson).ToString();

            JToken rootToken = JToken.Parse(json);

            if (rootToken.ToNativeType() is Dictionary<string, object> data)
            {
                ExpandedTranslation = data;
                ProcessExpandedToCondensed();
                return ExpandedTranslation;
            }
        }
        Debug.LogWarning($"Localization file not found at: {ResourceDirectory}, generating fresh set".WithColor("#FF0000"));
        ExpandedTranslation = new();
        SaveToJson();
        return ExpandedTranslation;
    }
    //public static void CopyOldPowerDescriptionsToNewSystem()
    //{
    //    Debug.Log("Porting Old Power Descriptions To Updated System".WithColor("#00FFFF"));
    //    foreach(PowerUp p in PowerUp.PowerUps.Values)
    //    {
    //        string Name = p.GetType().FullName;
    //        if (ExpandedTranslation["Power"] is Dictionary<string, object> value)
    //        {
    //            if (value[Name] is Dictionary<string, object> power)
    //            {
    //                if(p.DetailedDescription.Name != null)
    //                    power["Title"] = p.DetailedDescription.Name;
    //                if (p.DetailedDescription.ShortDescription != null)
    //                    power["Brief"] = p.DetailedDescription.ShortDescription;
    //                if(p.DetailedDescription.Description != null)
    //                    power["Description"] = p.DetailedDescription.Description;
    //            }
    //            else
    //                Debug.Log("Type Error: 2");
    //        }
    //        else
    //            Debug.Log("Type Error: 1");
    //    }
    //    SaveToJson();
    //    Debug.Log("Done".WithColor("#00FFFF"));
    //}
}
#endif
