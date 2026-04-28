using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(fileName = "TileToWallTool", menuName = "TileToWallTool")]
public class TileToWallTool : ScriptableObject
{
    public Texture2D TileSprite;
    public Texture2D WallSprite;
    public void CopyFrom(int TileX, int TileY, int WallX, int WallY)
    {
        Vector2Int offset = new(128 * TileX, 128 * TileY);
        Vector2Int offsetWall = new(128 * WallX, 128 * WallY);
        for (int i = 0; i < 128; ++i)
        {
            for (int j = 0; j < 128; ++j)
            {
                var c = TileSprite.GetPixel(offset.x + i, offset.y + j);
                WallSprite.SetPixel(offsetWall.x + j, offsetWall.y + i, c);
            }
        }
    }
    public void Generate()
    {
        if(!TileSprite.isReadable)
        {
            Debug.Log("FAILED TO GENERATE: MAKE SURE SPRITE IS SET TO READ/WRITE IN ADVANCED SETTINGS!".WithColor("#FFFF00"));
            return;
        }
        WallSprite = new Texture2D(128 * 3, 128 * 3);

        CopyFrom(1, 1, 0, 1);
        CopyFrom(2, 2, 1, 1);
        CopyFrom(2, 2, 1, 2);
        CopyFrom(3, 3, 2, 1);
        CopyFrom(1, 3, 1, 0);
        CopyFrom(1, 2, 2, 2);
        CopyFrom(2, 1, 0, 2);
        CopyFrom(0, 1, 0, 0);
        CopyFrom(1, 0, 2, 0);

        WallSprite.Apply();

        byte[] pngData = WallSprite.EncodeToPNG();
        if (pngData == null || pngData.Length == 0)
        {
            Debug.LogError("Failed to encode texture to PNG.");
            return;
        }
        // Save file
        File.WriteAllBytes("Assets/Resources/World/Walls/GeneratedWall.png", pngData);
        AssetDatabase.Refresh();
        WallSprite = Resources.Load<Texture2D>("World/Walls/GeneratedWall");
    }
}

[CustomEditor(typeof(TileToWallTool))]
public class TileToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector fields
        DrawDefaultInspector();

        // Reference to the target ScriptableObject
        TileToWallTool myData = (TileToWallTool)target;

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
