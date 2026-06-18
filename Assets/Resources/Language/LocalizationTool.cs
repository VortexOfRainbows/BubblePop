using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class LocalizationTool : MonoBehaviour
{
    public static LocalizationTool Instance { get; private set; }
    public void Start()
    {
        if(Instance == null)
        {
            try {
                GameObject.DontDestroyOnLoad(this.gameObject);
            }
            catch { }
            Instance = this;
        }
        else
            GameObject.Destroy(this.gameObject);
    }
    public void RunUpdate()
    {
        LocalizationBuilder.UpdateLocalizationFiles();
    }
#if UNITY_EDITOR
    public void Update()
    {
        if (LocalizationBuilder.RequiresDictionaryReload)
            LocalizationBuilder.ReloadRequiredUpdate();
    }
    public void OnEnable()
    {
        LocalizationBuilder.InstallHandler();
    }
    public void OnDisable()
    {
        LocalizationBuilder.UninstallHandler();
    }
#endif
}
#if UNITY_EDITOR
//custom editor for pressing button to run update in inspector
[CustomEditor(typeof(LocalizationTool))]
public class LocalizationToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector fields
        DrawDefaultInspector();

        // Reference to the target ScriptableObject
        LocalizationTool myData = (LocalizationTool)target;

        // Add a button in the inspector
        if (GUILayout.Button("Reset/Update Localization"))
        {
            myData.RunUpdate();

            // Mark the object as dirty so changes are saved
            EditorUtility.SetDirty(myData);
        }
        if (GUILayout.Button("Test Adding Power"))
        {
            PowerDescription _ = new(PowerUp.Get<Choice>());
        }
        //if (GUILayout.Button("Port Old Equip Descriptions To New System"))
        //{
        //    LocalizationBuilder.CopyOldEquipmentDescriptionToNewSystem();
        //}
    }
}

[CustomEditor(typeof(DefaultAsset))]
public class HjsonInspectorPreview : Editor
{
    private string hjsonTextContent = "";
    private Vector2 scrollPosition;

    void OnEnable()
    {
        string assetPath = AssetDatabase.GetAssetPath(target);

        if (assetPath.EndsWith(".hjson", System.StringComparison.OrdinalIgnoreCase))
        {
            // Read the file string directly from disk
            hjsonTextContent = File.ReadAllText(assetPath);
        }
    }

    public override void OnInspectorGUI()
    {
        string assetPath = AssetDatabase.GetAssetPath(target);

        // Fallback to default layout rendering if it isn't an Hjson file
        if (!assetPath.EndsWith(".hjson", System.StringComparison.OrdinalIgnoreCase))
        {
            base.OnInspectorGUI();
            return;
        }

        GUI.enabled = true; // Ensures the text container area can be highlighted/copied

        // Draw a clean UI title panel header
        EditorGUILayout.LabelField("Hjson Configuration Preview", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Render the file content inside a scrollable, read-only text box wrapper
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextArea(
            hjsonTextContent,
            EditorStyles.textArea,
            GUILayout.ExpandHeight(true)
        );

        EditorGUILayout.EndScrollView();

        // Add a helper shortcut button to quickly open your system's external IDE
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Open File in External Editor", GUILayout.Height(30)))
        {
            AssetDatabase.OpenAsset(target);
        }
    }
}
#endif