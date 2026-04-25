using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public static class Lighting
{
    //TODO: Update this so it doesn't use Resources.Load every time, maybe use a static constructor or something
    public static GameObject ShadowParentRight;
    public static GameObject ShadowParentLeft;
    public static GameObject OcclusionShadowPrefab => Resources.Load<GameObject>("Lighting/OcclusionTileShadow");
    public static GameObject TileShadowPrefab => Resources.Load<GameObject>("Lighting/TileShadow");
    public static GameObject ShadowPT => Resources.Load<GameObject>("Lighting/TileShadowPT");
    public static Light2D GlobalLight { get; private set; }
    public static Light2D OcclusionLight { get; private set; }
    public static List<TileLightSegment> TileShadowsTopRight { get; private set; } = new();
    public static List<TileLightSegment> TileShadowsTopLeft { get; private set; } = new();
    public class PathWalker
    {
        public PathWalker(int size, List<TileLightSegment> parentList)
        {
            end = size;
            this.parentList = parentList;
        }
        public List<TileLightSegment> parentList;
        public List<Vector3> lightPoints = new();
        public bool continuityBroken = false;
        public bool? prevValid = null;
        public int start = -1;
        public int end;
        public void Update(bool valid, Vector3 point, int j)
        {
            if (prevValid == null)
                prevValid = valid;
            if (start == -1)
            {
                if (prevValid != valid && valid) //ON A TRANSITIONPT
                {
                    //var obj = GameObject.Instantiate(ShadowPT, point, Quaternion.identity);
                    //obj.name = $"START[{i}:{j}]";
                    //obj.transform.parent = ShadowParent.transform;
                    start = j;
                    end += start;
                }
            }
            if (start != -1)
            {
                if (continuityBroken)
                {
                    if (lightPoints.Count >= 2)
                        parentList.Add(new(lightPoints));
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
        public void FinalUpdate()
        {
            if (lightPoints.Count >= 2)
                parentList.Add(new(lightPoints));
        }
    }
    public class TileLightSegment
    {
        //public Rect LightingBounds;
        public List<Vector3> OriginalVertices;
        public Vector3[] FinalVertices;
        public Light2D MyLight;
        private int Size;
        public TileLightSegment(List<Vector3> Vertices)
        {
            OriginalVertices = Vertices;
            MyLight = null;
            FinalVertices = new Vector3[OriginalVertices.Count * 2];
            Size = OriginalVertices.Count;
            //LightingBounds = new();
            //foreach(Vector3 vert in Vertices)
            //{
            //    if (vert.x < LightingBounds.xMin)
            //        LightingBounds.xMin = vert.x;
            //    if (vert.x > LightingBounds.xMax)
            //        LightingBounds.xMax = vert.x;
            //    if (vert.y < LightingBounds.yMin)
            //        LightingBounds.yMin = vert.y;
            //    if (vert.y > LightingBounds.yMax)
            //        LightingBounds.yMax = vert.y;
            //}
            int i = 0;
            for (; i < Size; ++i)
                FinalVertices[i] = OriginalVertices[i];
            for (int j = Size - 1; j >= 0; --j)
                FinalVertices[i++] = OriginalVertices[j];
        }
        public void Extend(float x, float y) => Extend(new Vector2(x, y));
        public void Extend(Vector2 direction)
        {
            int i = 0;
            for (; i < Size; ++i)
            {
                Vector3 extend = OriginalVertices[i];
                extend.x += direction.x;
                extend.y += direction.y;
                FinalVertices[i] = extend;
            }
        }
        public void Prepare(Transform parentTransform)
        {
            MyLight = GameObject.Instantiate(TileShadowPrefab, Vector2.zero, Quaternion.identity).GetComponent<Light2D>();
            MyLight.transform.parent = parentTransform;
            MyLight.name = $"Shadow {Size}";
            MyLight.SetShapePath(FinalVertices);
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
    public static bool CreateLightingVertices()
    {
        TilemapCollider2D collider = World.RealTileMap.Map.GetComponent<TilemapCollider2D>(); //temp
        CompositeCollider2D composite = collider.composite;
        int paths = composite.pathCount;

        if (paths <= 0)
            return false;

        GlobalLight = Camera.main.transform.parent.GetComponentInChildren<Light2D>();
        Debug.Log("SETTING UP LIGHTING");
        StartTimeForLightSetup = Time.realtimeSinceStartup;
        ShadowParentLeft = new GameObject("ShadowParentLeft");
        ShadowParentRight = new GameObject("ShadowParentRight");
        TileShadowsTopLeft = new(paths);
        TileShadowsTopRight = new(paths);
        for (int i = 1; i < paths; ++i) //i = 1 to skip the first path, which is the exterior (does not need a shadow)
        {
            int count = composite.GetPathPointCount(i);
            Vector2[] points = new Vector2[count];
            composite.GetPath(i, points);
            int maxEnd = count;
            PathWalker leftWalker = new(count, TileShadowsTopLeft);
            PathWalker rightWalker = new(count, TileShadowsTopRight);
            for (int j = 0; j < maxEnd; ++j)
            {
                Vector3 point = points[j % count];
                Vector3 next = points[(j + 1) % count];
                Vector3 travelDirection = next - point;
                bool movingHorizontal = Mathf.Abs(travelDirection.x) > 0.2f;
                bool movingVertical = Mathf.Abs(travelDirection.y) > 0.2f;
                PossibleEdgesAtIntersectionPoint(point, out bool posBotEdge, out bool posTopEdge, out bool posRightEdge, out bool posLeftEdge);
                PossibleEdgesAtIntersectionPoint(next, out bool posBotEdgeN, out bool posTopEdgeN, out bool posRightEdgeN, out bool posLeftEdgeN);
                bool top = posTopEdge && posTopEdgeN && movingHorizontal;
                //bool bot = posBotEdge && posBotEdgeN && movingHorizontal;
                bool left = posLeftEdge && posLeftEdgeN && movingVertical;
                bool right = posRightEdge && posRightEdgeN && movingVertical;
                leftWalker.Update(left || top, point, j);
                rightWalker.Update(right || top, point, j);
                maxEnd = Mathf.Max(maxEnd, rightWalker.end, leftWalker.end);
            }
        }
        foreach (TileLightSegment light in TileShadowsTopRight)
            light.Prepare(ShadowParentRight.transform);
        foreach (TileLightSegment light in TileShadowsTopLeft)
            light.Prepare(ShadowParentLeft.transform);
        World.RealTileMap.Map.GetCorners(out int x, out int x2, out int y, out int y2);
        CreateOcclusionSpriteLighting(x, x2, y, y2);
        Debug.Log("FINISHED LIGHT SETUP: " + (Time.realtimeSinceStartup - StartTimeForLightSetup));
        return true;
    }
    //TODO: This should probably be threaded and done in partitions rather than the whole map at once
    public static void CreateOcclusionSpriteLighting(int left, int right, int bot, int top)
    {
        #region stuff commented maybe for later
        //TODO: If you do partitioning, it might be best to sample the sprite NOT all the way to the edge (which is where bilinear blurring may occur?)
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
    public static void UpdateLightDirection(Vector2 extends)
    {
        List<TileLightSegment> Lights;
        if(extends.x < 0) //Shadows go left, so use right-side path
            Lights = TileShadowsTopRight;
        else //Shadows go right, so use left-side path
            Lights = TileShadowsTopLeft;
        foreach (TileLightSegment light in Lights)
            light.Extend(extends);
    }
    public static void UpdateLightIntensity(float intensity)
    {
        float intensityMultiplier = intensity;
        GlobalLight.intensity = 1.0f * intensityMultiplier;
        OcclusionLight.intensity = 0.2f * intensityMultiplier;
        foreach (TileLightSegment light in TileShadowsTopRight)
            light.MyLight.intensity = 0.65f * intensityMultiplier;
        foreach (TileLightSegment light in TileShadowsTopLeft)
            light.MyLight.intensity = 0.65f * intensityMultiplier;
    }

    public static float StartTimeForLightSetup = 0;
    public static bool FinishedSettingUpLighting = false;
    public static float TimeCounter = 0;
    public static Vector2 prevExtends = Vector2.zero;
    private static bool IsPerformingVertexUpdate { get; set; } = false;
    private static int LightingTickRate = 5;
    private static int LightingCurrentTick = 0;
    public static void FixedUpdate()
    {
        if (!FinishedSettingUpLighting && CreateLightingVertices())
            FinishedSettingUpLighting = true;
        if(FinishedSettingUpLighting)
        {
            TimeCounter += Time.fixedDeltaTime;

            float percent = TimeCounter / 100f;
            if (++LightingCurrentTick % LightingTickRate != 0)
                return;

            Vector2 orbit = new Vector2(-6, 0).RotatedBy(percent * Mathf.PI * 2);
            orbit.y = orbit.y * 0.1f - 3;

            Vector2 nextExtends = orbit;
            if(!IsPerformingVertexUpdate)
            {
                prevExtends = nextExtends;
                //Debug.Log("Performing Lighting Vertex Update");
                StartTimeForLightSetup = Time.realtimeSinceStartup;
                IsPerformingVertexUpdate = true;
                var thread = new Thread(LightUpdate);
                thread.Start();
                //Debug.Log("Finished Lighting Vertex Update: " + (Time.realtimeSinceStartup - StartTimeForLightSetup));
            }
            if(!IsPerformingVertexUpdate)
            {
                ShadowParentRight.SetActive(prevExtends.x < 0);
                ShadowParentLeft.SetActive(prevExtends.x >= 0);
            }
            //if (FrameDelay > 100)
            //{
            //    FrameDelay++;
            //    //Just for testing
            //    float sin = 0.75f + 0.25f * Mathf.Sin(FrameDelay / 400f * Mathf.PI);
            //    Lighting.UpdateLightIntensity(sin);
            //}
        }
    }
    public static void LightUpdate()
    {
        UpdateLightDirection(prevExtends);
        IsPerformingVertexUpdate = false;
    }
}
