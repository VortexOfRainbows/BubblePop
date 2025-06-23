using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PrefabPlacer : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset;

    [MenuItem("Tools/Prefab Placer")]
    public static void ShowEditor()
    {
        var window = GetWindow<PrefabPlacer>();
        window.titleContent = new GUIContent("Prefab Placer");
    }

    private void CreateGUI()
    {
        m_VisualTreeAsset.CloneTree(rootVisualElement);
    }
}
