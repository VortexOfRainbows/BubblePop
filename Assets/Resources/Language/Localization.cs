using Hjson;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

public static class Localization
{
    public static FileSystemWatcher Watcher;
    /// <summary>
    /// Update the localization file if:
    /// 1. A key is requested that doesn't exist in the dictionary, which will add it to the dictionary with a placeholder value (the key itself) and mark that the dictionary needs to be reloaded.
    /// 2. The dictionary file itself is updated externally, which can be detected by comparing the last modified time of the file to a stored value. If the file has been modified since the last check, it will reload the dictionary from the file.
    /// </summary>
    public static void FileChangeUpdate(object source, FileSystemEventArgs e)
    {
        Debug.Log("Localization File Change Detected. Rebuilding Dictionary.".WithColor("#FF8800"));
    }
    public static void ReloadRequiredUpdate()
    {
        Debug.Log("Localization File Missing Required Keys. Rebuilding File.".WithColor("#FF0088"));
        SaveToJson();
        RequiresDictionaryReload = false;
    }
    public static void InstallHandler()
    {
        Localization.Watcher = new(ResourcePath, ResourceName)
        {
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
        };
        Localization.Watcher.Changed += Localization.FileChangeUpdate;
        Localization.Watcher.EnableRaisingEvents = true;
    }
    public static void UninstallHandler()
    {
        if (Localization.Watcher != null)
        {
            Localization.Watcher.Changed -= Localization.FileChangeUpdate;
            Localization.Watcher.Dispose();
        }
    }
    public static bool RequiresDictionaryReload { get; set; } = false;
    public static string Get(string key)
    {
        if (KeyToTranslation.TryGetValue(key, out string value))
            return value;
        return RequestPlaceholderValue(key);
    }
    private static string RequestPlaceholderValue(string key)
    {
        RequiresDictionaryReload = true;
        KeyToTranslation[key] = key;
        return key;
    }
    public static Dictionary<string, string> KeyToTranslation = LoadFromJson();
    public static string ResourcePath => "Assets/Resources/Language/";
    public static string ResourceName => "en-US.hjson";
    public static string ResourceDirectory => ResourcePath + ResourceName;
    public static void UpdateLocalizationFiles()
    {
        Debug.Log("Building Dictionary...".WithColor("#FF8800"));
        KeyToTranslation ??= LoadFromJson();
        Debug.Log("Done...".WithColor("#FF8800"));
    }
    public static void SaveToJson()
    {
        string standardJson = JsonConvert.SerializeObject(KeyToTranslation, Formatting.Indented); 
        var jsonObject = HjsonValue.Parse(standardJson);
        using StringWriter stringWriter = new();
        // Configure output preferences (e.g., drop the root curly braces for cleaner configs)
        HjsonOptions options = new()
        {
            EmitRootBraces = false, // Hjson's signature style (removes outer { })
            KeepWsc = false        // Drops unnecessary whitespace/comments logic
        };
        // Process and save the structure into our text stream layout
        HjsonValue.Save(jsonObject, stringWriter, options);
        File.WriteAllText(ResourceDirectory, stringWriter.ToString());
    }
    public static Dictionary<string, string> LoadFromJson()
    {
        if (File.Exists(ResourceDirectory))
        {
            string hjson = File.ReadAllText(ResourceDirectory);
            string json = HjsonValue.Parse(hjson).ToString();
            var data = (Dictionary<string, string>)JsonConvert.DeserializeObject(json, typeof(Dictionary<string, string>));
            if (data != null)
                return data;
        }
        Debug.LogWarning($"Localization file not found at: {ResourceDirectory}, generating fresh set".WithColor("#FF0000"));
        KeyToTranslation = new();
        SaveToJson();
        return KeyToTranslation;
    }
}
