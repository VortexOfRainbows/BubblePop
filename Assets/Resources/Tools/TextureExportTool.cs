using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "TextureExportTool", menuName = "TextureExportTool")]
public class TextureExportTool : ScriptableObject
{
    public void Generate()
    {
        Vector2Int size = new Vector2Int(512, 512);
        var sprite = new Texture2D(size.x, size.y);

        for (int i = 0; i < sprite.width; ++i)
        {
            for (int j = 0; j < sprite.height; ++j)
            {
                float pX = 2 * ((i + 0.5f) / size.x - 0.5f);
                float pY = 2 * ((j + 0.5f) / size.y - 0.5f);
                float distFromCenter = Mathf.Sqrt(pX * pX + pY * pY);
                if (distFromCenter > 1)
                    distFromCenter = 1;
                if (distFromCenter < 0.5f)
                    distFromCenter = 0.5f;
                sprite.SetPixel(i, j, new Color(1, 1, 1, 1 - distFromCenter));
            }
        }
        sprite.Apply();

        byte[] pngData = sprite.EncodeToPNG();
        if (pngData == null || pngData.Length == 0)
        {
            Debug.LogError("Failed to encode texture to PNG.");
            return;
        }
        // Save file
        File.WriteAllBytes("Assets/Resources/Tools/GenTexture.png", pngData);
        AssetDatabase.Refresh();
    }
}

[CustomEditor(typeof(TextureExportTool))]
public class TextureToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector fields
        DrawDefaultInspector();

        // Reference to the target ScriptableObject
        TextureExportTool myData = (TextureExportTool)target;

        // Add a button in the inspector
        if (GUILayout.Button("Generate"))
        {
            myData.Generate();

            // Mark the object as dirty so changes are saved
            EditorUtility.SetDirty(myData);
        }
    }
}
#endif  
