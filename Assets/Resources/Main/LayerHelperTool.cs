using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "LayerHelperTool", menuName = "LayerHelperTool")]
public class LayerHelperTool : ScriptableObject
{
    public string Path = string.Empty;
    public List<int> IgnoreOrder = new();
    public List<GameObject> final;
    public bool IgnoreNodes = true;
    public void SearchForNests()
    {
        final = new();
        var objs = Resources.LoadAll<GameObject>(Path);
        foreach (GameObject obj in objs)
        {
            if (obj.TryGetComponent<WorldNode>(out _) && IgnoreNodes)
                continue;
            var sr = obj.GetComponentsInChildren<SpriteRenderer>();
            if (sr.Length > 1)
            {
                if (obj.TryGetComponent<SortingGroup>(out _))
                    continue;
                Debug.Log("Discovered Object Without Sorting Group: " + obj.name, obj);
                final.Add(obj);
            }
        }
    }
    public void Search()
    {
        final = new();
        var objs = Resources.LoadAll<GameObject>(Path);
        foreach (GameObject obj in objs)
        {
            if(obj.TryGetComponent<WorldNode>(out _) && IgnoreNodes)
                continue; 
            var sr = obj.GetComponentsInChildren<SpriteRenderer>();
            if (sr.Length > 0)
            {
                bool hasFoundAtLeastOne = false;
                string concat = string.Empty;
                concat += $"{obj.name}\n";
                foreach (SpriteRenderer r in sr)
                {
                    if (!IgnoreOrder.Contains(r.sortingOrder))
                    {
                        hasFoundAtLeastOne = true;
                        concat += $"{r.name}: {r.sortingOrder.ToString().WithColor("#CCFF00")}\n";
                    }
                }
                if (hasFoundAtLeastOne)
                {
                    Debug.Log(concat, obj);
                    final.Add(obj);
                }
            }
        }
    }
}

[CustomEditor(typeof(LayerHelperTool))]
public class LayerToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector fields
        DrawDefaultInspector();

        // Reference to the target ScriptableObject
        LayerHelperTool myData = (LayerHelperTool)target;

        // Add a button in the inspector
        if (GUILayout.Button("Search"))
        {
            myData.Search();

            // Mark the object as dirty so changes are saved
            EditorUtility.SetDirty(myData);
        }
    }
}
#endif