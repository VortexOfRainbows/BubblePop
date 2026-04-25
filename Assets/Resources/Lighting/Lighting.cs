using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public static class Lighting
{
    //TODO: Update this so it doesn't use Resources.Load every time, maybe use a static constructor or something
    public static GameObject ShadowParent;
    public static GameObject OcclusionShadowPrefab => Resources.Load<GameObject>("Lighting/OcclusionTileShadow");
    public static GameObject TileShadowPrefab => Resources.Load<GameObject>("Lighting/TileShadow");
    public static GameObject ShadowPT => Resources.Load<GameObject>("Lighting/TileShadowPT");
    public static Light2D GlobalLight { get; private set; }
    public static Light2D OcclusionLight { get; private set; }
    public static List<TileLightSegment> TileShadows { get; private set; } = new();
    public class TileLightSegment
    {
        public List<Vector3> OriginalVertices;
        public List<Vector3> FinalVertices;
        public Light2D MyLight;
        public TileLightSegment(List<Vector3> Vertices)
        {
            OriginalVertices = Vertices;
            FinalVertices = new(Vertices);
            MyLight = null;
        }
        public void Extend(float x, float y) => Extend(new Vector2(x, y));
        public void Extend(Vector2 direction)
        {
            if (FinalVertices.Count > OriginalVertices.Count)
                FinalVertices = new(OriginalVertices);
            for (int i = OriginalVertices.Count - 1; i >= 0; --i)
            {
                Vector3 extend = OriginalVertices[i];
                extend.x += direction.x;
                extend.y += direction.y;
                FinalVertices.Add(extend);
            }
            FinalVertices.Reverse();
        }
        public void Unleash()
        {
            if (MyLight == null)
            {
                MyLight = GameObject.Instantiate(TileShadowPrefab, Vector2.zero, Quaternion.identity).GetComponent<Light2D>();
                MyLight.transform.parent = ShadowParent.transform;
                MyLight.name = $"Shadow {FinalVertices.Count}";
            }
            MyLight.SetShapePath(FinalVertices.ToArray());
        }
    }
    public static void PossibleEdgesAtIntersectionPoint(Vector3 point, out bool possibleBotEdge, out bool possibleTopEdge, out bool possibleRightEdge, out bool possibleLeftEdge)
    {
        Vector3Int forSpeed = World.RealTileMap.Map.WorldToCell(point + new Vector3(0.5f, 0.5f));
        bool tileTopRight = World.SolidTile(forSpeed);
        forSpeed.x -= 1;
        bool tileTopLeft = World.SolidTile(forSpeed);
        forSpeed.y -= 1;
        bool tileBotLeft = World.SolidTile(forSpeed);
        forSpeed.x += 1;
        bool tileBotRight = World.SolidTile(forSpeed);
        int count = 0;
        if (tileTopRight) ++count;
        if (tileTopLeft) ++count;
        if (tileBotLeft) ++count;
        if (tileBotRight) ++count;
        //Debug.Log($"AdjacentTileCount[{point}] = {count}");
        possibleBotEdge = possibleTopEdge = possibleRightEdge = possibleLeftEdge = count == 2;
        if (count == 3)
        {
            possibleBotEdge = tileBotRight && tileBotLeft;
            possibleTopEdge = tileTopLeft && tileTopRight;
            possibleRightEdge = tileTopRight && tileBotRight;
            possibleLeftEdge = tileTopLeft && tileBotLeft;
        }
        else if (count == 1)
        {
            if (tileBotLeft)
                possibleBotEdge = possibleLeftEdge = true;
            if (tileTopRight)
                possibleRightEdge = possibleTopEdge = true;
            if (tileTopLeft)
                possibleTopEdge = possibleLeftEdge = true;
            if (tileBotRight)
                possibleBotEdge = possibleRightEdge = true;
        }
    }
    public static void CreateLightingVertices()
    {
        GlobalLight = Camera.main.transform.parent.GetComponentInChildren<Light2D>();
        Debug.Log("SETTING UP LIGHTING");
        World.StartTimeForLightSetup = Time.realtimeSinceStartup;
        ShadowParent = new GameObject("Shadow Parent");
        TilemapCollider2D collider = World.RealTileMap.Map.GetComponent<TilemapCollider2D>(); //temp
        CompositeCollider2D composite = collider.composite;
        TileShadows = new();
        int paths = composite.pathCount;
        for (int i = 1; i < paths; ++i) //i = 1 to skip the first path, which is the exterior (does not need a shadow)
        {
            int count = composite.GetPathPointCount(i);
            Vector2[] points = new Vector2[count];
            List<Vector3> lightPoints = new(); //Unity why I need to do this? are you stupid? why is a light 2D use vector3 for its path
            composite.GetPath(i, points);
            bool continuityBroken = false;
            bool? prevValid = null;
            int start = -1;
            int end = count;
            for (int j = 0; j < end; ++j)
            {
                Vector3 point = points[j % count];
                Vector3 next = points[(j + 1) % count];
                //int k = (j - 1) % count;
                //if (k < 0)
                //    k = k + count;
                //Vector3 prev = points[k];
                bool valid = false;

                Vector3 travelDirection = next - point;
                bool movingHorizontal = Mathf.Abs(travelDirection.x) > 0.2f;
                bool movingVertical = Mathf.Abs(travelDirection.y) > 0.2f;
                PossibleEdgesAtIntersectionPoint(point, out bool posBotEdge, out bool posTopEdge, out bool posRightEdge, out bool posLeftEdge);
                PossibleEdgesAtIntersectionPoint(next, out bool posBotEdgeN, out bool posTopEdgeN, out bool posRightEdgeN, out bool posLeftEdgeN);
                bool top = posTopEdge && posTopEdgeN && movingHorizontal;
                bool bot = posBotEdge && posBotEdgeN && movingHorizontal;
                bool left = posLeftEdge && posLeftEdgeN && movingVertical;
                bool right = posRightEdge && posRightEdgeN && movingVertical;
                if (right || top)
                    valid = true;
                else
                    valid = false;
                if (prevValid == null)
                    prevValid = valid;
                if (start == -1)
                {
                    if (prevValid != valid && valid) //ON A TRANSITIONPT
                    {
                        var obj = GameObject.Instantiate(ShadowPT, point, Quaternion.identity);
                        obj.name = $"START[{i}:{j}]";
                        obj.transform.parent = ShadowParent.transform;
                        start = j;
                        end += start;
                    }
                }
                if (start != -1)
                {
                    if (continuityBroken)
                    {
                        if (lightPoints.Count >= 2)
                            TileShadows.Add(new(lightPoints));
                        lightPoints = new();
                        continuityBroken = false;
                    }
                    if (!continuityBroken)
                    {
                        if (prevValid.Value || valid)
                            lightPoints.Add(point);
                        continuityBroken = !valid;
                    }
                }
                prevValid = valid;
            }
            if (lightPoints.Count >= 2)
                TileShadows.Add(new(lightPoints));
        }
        foreach (TileLightSegment light in TileShadows)
        {
            light.Extend(-4, -2);
            light.Unleash();
        }
        World.RealTileMap.Map.GetCorners(out int x, out int x2, out int y, out int y2);
        CreateOcclusionSpriteLighting(x, x2, y, y2);
        Debug.Log("FINISHED LIGHT SETUP: " + (Time.realtimeSinceStartup - World.StartTimeForLightSetup));
    }
    //TODO: This should probably be threaded and done in partitions rather than the whole map at once
    public static void CreateOcclusionSpriteLighting(int left, int right, int bot, int top)
    {
        int w = right - left;
        int h = top - bot;
        int Resolution = 2;
        Texture2D lightTexture = new(w * Resolution, h * Resolution);
        Color c1 = Color.clear;
        Color c2 = Color.white;
        lightTexture.filterMode = FilterMode.Trilinear;
        for (int i = 0; i < w; ++i)
        {
            for (int j = 0; j < h; ++j)
            {
                Vector3Int pos = new Vector3Int(i + left, j + bot);
                Color c = World.SolidTile(pos) ? c2 : c1;
                int i2 = i * Resolution;
                int j2 = j * Resolution;
                for (int x = 0; x < Resolution; ++x)
                    for (int y = 0; y < Resolution; ++y)
                        lightTexture.SetPixel(i2 + x, j2 + y, c);
            }
        }
        lightTexture.Apply();
        Sprite lightSprite = Sprite.Create(lightTexture, new Rect(0, 0, lightTexture.width, lightTexture.height), Vector2.zero, 1f, 0, SpriteMeshType.FullRect);
        Vector3 pos2 = new(left * 2, bot * 2);
        Light2D L = GameObject.Instantiate(OcclusionShadowPrefab, pos2, Quaternion.identity).GetComponent<Light2D>();
        L.lightCookieSprite = lightSprite;
        OcclusionLight = L;
    }
    public static void UpdateLightIntensity(float intensity)
    {
        float intensityMultiplier = intensity;
        GlobalLight.intensity = 1.0f * intensityMultiplier;
        OcclusionLight.intensity = 0.2f * intensityMultiplier;
        foreach (TileLightSegment light in TileShadows)
            light.MyLight.intensity = 0.65f * intensityMultiplier;
    }

    #region stuff commented maybe for later

    //int w = x2 - x;
    //int h = y2 - y;
    //int squarePartitions = 4;
    //int segmentW = w / squarePartitions;
    //int segmentH = h / squarePartitions;
    //for (int i = 0; i < squarePartitions; ++i)
    //{
    //    for(int j = 0; j < squarePartitions; ++j)
    //    {
    //        int myLeft = x + segmentW * i;
    //        int myRight = myLeft + segmentW;
    //        int myBot = y + segmentH * j;
    //        int myTop = myBot + segmentH;
    //    }
    //}

    #endregion
}
