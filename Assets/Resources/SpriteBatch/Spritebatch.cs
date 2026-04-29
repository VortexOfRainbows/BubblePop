using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
        if (renderer == null)
            return;
        renderer.sprite = call.sprite;
        renderer.material = call.material;
        renderer.sortingOrder = call.order;
        renderer.color = call.color;
        renderer.transform.localScale = call.scale;
        renderer.transform.SetLocalEulerZ(call.rotation);
        renderer.transform.position = call.position;
    }
    public static void ClearRenderer(int i)
    {
        if(Renderers[i] != null)
            GameObject.Destroy(Renderers[i].gameObject); 
        Renderers.RemoveAt(i);
    }
}
public static class LightBatch
{
    public static ref GameObject Light => ref Main.PrefabAssets.LightPrefab;
    public static void Request(Vector2 position, Color color, float intensity, float radius = 1.0f, float falloffStrength = 0.5f)
    {
        var call = new LightBatchCall
        {
            position = position,
            color = color,
            intensity = intensity,
            radius = radius,
            falloffStrength = falloffStrength,
        };
        Calls.Add(call);
    }
    public class LightBatchCall
    {
        public Vector2 position;
        public Color color;
        public float intensity;
        public float radius;
        public float falloffStrength;
    }
    public static readonly List<LightBatchCall> Calls = new();
    public static readonly List<Light2D> Lights = new();
    public static void Setup()
    {
        Calls.Clear();
        Lights.Clear();
        OnUpdate();
    }
    public static void OnUpdate()
    {
        int i = 0;
        for (; i < Calls.Count; ++i)
            SetLight(i, Calls[i]);
        for (int j = Lights.Count - 1; j >= i; --j)
            ClearLight(j);
        Calls.Clear();
    }
    public static void SetLight(int i, LightBatchCall call)
    {
        if (Lights.Count <= i)
        {
            var rend = GameObject.Instantiate(Light, Main.SpritebatchParent).GetComponent<Light2D>();
            Lights.Add(rend);
        }
        Light2D light = Lights[i];
        if (light == null)
            return;
        light.falloffIntensity = call.falloffStrength;
        light.pointLightOuterRadius = call.radius;
        light.intensity = call.intensity;
        light.color = call.color;
        light.transform.position = call.position;
    }
    public static void ClearLight(int i)
    {
        if (Lights[i] != null)
            GameObject.Destroy(Lights[i].gameObject);
        Lights.RemoveAt(i);
    }
}