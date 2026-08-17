using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class SpecialLine : MonoBehaviour
{
    public static GameObject LinePrefab => Resources.Load<GameObject>("Trails/SpecialLine");
    public static SpecialLine NewLine(List<Vector3> positions, Color c, float distanceTraveled, float width = 1f, int orderInLayer = 2)
    {
        SpecialLine t = Instantiate(LinePrefab, Main.GenericSuperParent).GetComponent<SpecialLine>();
        t.Line.positionCount = positions.Count;
        t.Line.SetPositions(positions.ToArray());
        //t.Line.endColor = t.Line.startColor = c;
        t.Line.widthMultiplier = t.OriginalSize = width;
        //t.ManuallyUpdated = manuallyUpdated;
        t.Line.sortingOrder = orderInLayer;
        t.DistanceTraveled = distanceTraveled;
        return t;
    }
    public LineRenderer Line;
    public float timer;
    public float OriginalSize;
    //public bool ManuallyUpdated { get; private set; } = false;
    //public bool ManualPosition { get; set; } = false;
    //public float decayMultiplier = 1.0f;
    public float TimeToDie = 0.45f;
    public float DistanceTraveled = 1f;
    public void AIUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > TimeToDie)
        {
            Destroy(gameObject);
            return;
        }
        float percent = timer / TimeToDie;
        percent *= percent;
        percent = 1 - percent;
        Line.material.SetFloat("_TimeLeftPercent", percent);
        Line.material.SetFloat("_RateMult", DistanceTraveled * Utils.TwoPI / 8f);
        Line.widthMultiplier = OriginalSize * percent;
    }
    private void FixedUpdate()
    {
        //if (!ManuallyUpdated)
            AIUpdate();
    }
}
