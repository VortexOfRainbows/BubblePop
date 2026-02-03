using System.Collections.Generic;
using UnityEngine;
public static class SpriteBatch
{
    public static ref GameObject Renderer => ref Main.PrefabAssets.SpritebatchPrefab;
    public static void Draw(Sprite sprite, Vector3 position, Vector2 scale, float rotation, Color color, int order = 0, Material mat = null)
    {
        var call = new SpriteBatchCall
        {
            sprite = sprite,
            position = position,
            scale = new Vector3(scale.x, scale.y, 1),
            rotation = rotation,
            color = color,
            order = order
        };
        if (mat != null)
            call.material = mat;
        Calls.Add(call);
    }
    public class SpriteBatchCall
    {
        public Sprite sprite;
        public Material material = Main.TextureAssets.AdditiveShader;
        public int order = 0;
        public Vector3 scale;
        public float rotation;
        public Vector3 position;
        public Color color;
    }
    public static readonly List<SpriteBatchCall> Calls = new();
    public static readonly List<SpriteRenderer> Renderers = new();
    public static void Setup()
    {
        Calls.Clear();
        Renderers.Clear();
        OnUpdate();
    }
    public static void OnUpdate()
    {
        int i = 0;
        for (; i < Calls.Count; ++i)
            SetRenderer(i, Calls[i]);
        for (int j = Renderers.Count - 1; j >= i; --j)
            ClearRenderer(j);
        Calls.Clear();
    }
    public static void SetRenderer(int i, SpriteBatchCall call)
    {
        if(Renderers.Count <= i)
        {
            var rend = GameObject.Instantiate(Renderer, Main.SpritebatchParent).GetComponent<SpriteRenderer>();
            Renderers.Add(rend);
        }
        SpriteRenderer renderer = Renderers[i];
        renderer.sprite = call.sprite;
        renderer.material = call.material;
        renderer.sortingOrder = call.order;
        renderer.color = call.color;
        renderer.transform.localScale = call.scale;
        renderer.transform.SetEulerZ(call.rotation);
        renderer.transform.position = call.position;
    }
    public static void ClearRenderer(int i)
    {
        GameObject.Destroy(Renderers[i].gameObject); 
        Renderers.RemoveAt(i);
    }
}