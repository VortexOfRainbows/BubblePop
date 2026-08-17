using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class SpecialLine : MonoBehaviour
{
    public static GameObject LinePrefab => Resources.Load<GameObject>("Trails/SpecialLine");
    public Vector3[] OriginalPositions { get; set; }
    public Vector3[] HelixPositions { get; set; }
    public bool Helix { get; set; } = false;
    public float RandHelixOffset = 0;
    public float ColorRandOffset = 0;
    public static SpecialLine NewLine(List<Vector3> positions, Color c, float distanceTraveled, float width = 1f, int orderInLayer = 2, bool doHelix = true)
    {
        if (distanceTraveled <= 0)
            return null;
        SpecialLine t = Instantiate(LinePrefab, Main.GenericSuperParent).GetComponent<SpecialLine>();
        if (doHelix)
            width *= 0.5f;
        t.OriginalPositions = positions.ToArray();
        t.Line.positionCount = positions.Count;
        //t.Line.endColor = t.Line.startColor = c;
        t.Line.widthMultiplier = t.OriginalSize = width;
        //t.ManuallyUpdated = manuallyUpdated;
        t.Line.sortingOrder = orderInLayer;
        t.DistanceTraveled = distanceTraveled;
        t.Helix = doHelix;
        if(doHelix)
        {
            t.HelixPositions = new Vector3[positions.Count];
            t.RandHelixOffset = Utils.RandFloat(Utils.TwoPI);
            t.HelixUpdate();
            NewLine(positions, c, distanceTraveled, width * 2, orderInLayer, false);
        }
        else
            t.Line.SetPositions(t.OriginalPositions);
        t.Line.material.SetFloat("_RateOffset", t.ColorRandOffset = Utils.RandFloat(Utils.TwoPI));
        return t;
    }
    public void HelixUpdate()
    {
        if (OriginalPositions.Length < 0)
            return;
        float timeContibution = timer / TimeToDie * Mathf.PI * -1.5f + RandHelixOffset;
        Vector3 prev = OriginalPositions[0];
        float distanceTravelMult = DistanceTraveled * 0.35f;
        HelixPositions[0] = prev;
        for (int i = 1; i < OriginalPositions.Length; i++)
        {
            float percent = i / (float)OriginalPositions.Length;
            float totalSin = 0.75f + 0.25f * Mathf.Sin(percent * Mathf.PI);
            Vector2 toPrev = prev - OriginalPositions[i];
            float traversalPercent = percent * distanceTravelMult;
            float sin = Mathf.Sin(timeContibution + traversalPercent * Mathf.PI);
            float secondarySin = Mathf.Sin(traversalPercent * Mathf.PI * .5f) * 0.3f + 0.7f;
            Vector3 offset = new Vector2(0, secondarySin * OriginalSize * sin * totalSin).RotatedBy(toPrev.ToRotation());
            HelixPositions[i] = OriginalPositions[i] + offset;
            prev = OriginalPositions[i];
        }
        Line.SetPositions(HelixPositions);
    }
    public LineRenderer Line;
    public float timer;
    public float OriginalSize;
    //public bool ManuallyUpdated { get; private set; } = false;
    //public bool ManualPosition { get; set; } = false;
    //public float decayMultiplier = 1.0f;
    public float TimeToDie = 0.45f;
    public float DistanceTraveled = 1f;
    public bool InitDust = false;
    public void AIUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > TimeToDie)
        {
            Destroy(gameObject);
            return;
        }
        if (Helix)
            HelixUpdate();
        float distanceColorMult = DistanceTraveled * Utils.TwoPI / 10f;
        if (!InitDust)
        {
            Vector3[] array = Helix ? HelixPositions : OriginalPositions;
            for(int i = Utils.RandInt(15); i < array.Length; i += Utils.RandInt(15))
            {
                float per = i / (float)array.Length;
                Vector2 pos = array[i];
                Color color = Utils.PastelRainbow((ColorRandOffset + per) * distanceColorMult, 0.75f, ColorHelper.HotPink);
                ParticleManager.NewParticle(pos, Utils.RandFloat(4f, 5f) * OriginalSize, Vector2.zero, 2.5f, TimeToDie * per + TimeToDie, ParticleManager.ID.Pixel, color);
            }
            InitDust = true;
        }
        float percent = timer / TimeToDie;
        percent *= percent;
        percent = 1 - percent;
        Line.material.SetFloat("_TimeLeftPercent", percent);
        Line.material.SetFloat("_RateMult", distanceColorMult);
        ColorRandOffset += timer * Utils.TwoPI / DistanceTraveled;
        Line.material.SetFloat("_RandOffset", ColorRandOffset);
        Line.widthMultiplier = OriginalSize * percent;
    }
    private void FixedUpdate()
    {
        //if (!ManuallyUpdated)
            AIUpdate();
    }
}
