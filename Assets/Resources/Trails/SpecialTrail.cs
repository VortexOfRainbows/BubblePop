using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTrail : MonoBehaviour
{
    public static GameObject TrailPrefab => Resources.Load<GameObject>("Trails/FollowerTrail");
    public static SpecialTrail NewTrail(Transform parent, Color c, float width = 1f, float length = 1f, float texScaleY = 0.2f, bool manuallyUpdated = false)
    {
        SpecialTrail t = Instantiate(TrailPrefab).GetComponent<SpecialTrail>();
        t.Trail.startColor = c;
        t.originalAlpha = c.a;
        t.Trail.endColor = c.WithAlpha(0);
        t.Trail.time = length;
        t.Trail.textureScale = new Vector2(1, texScaleY);
        t.Trail.startWidth = width;
        t.Trail.endWidth = width - 1;
        t.FakeParent = parent;
        t.transform.position = parent.transform.position;
        t.ManuallyUpdated = manuallyUpdated;
        return t;
    }
    public Transform FakeParent;
    public TrailRenderer Trail;
    public float timer;
    public float originalAlpha;
    public bool ManuallyUpdated = false;
    public float decayMultiplier = 1.0f;
    public List<Vector3> positions = new();
    public void AIUpdate()
    {
        if (FakeParent == null)
        {
            Trail.autodestruct = true;
            timer += Time.fixedDeltaTime * decayMultiplier;
            float iPer = (1 - timer / Trail.time);
            Trail.startColor = Trail.startColor.WithAlpha(originalAlpha * iPer * iPer);
        }
        else
        {
            transform.position = FakeParent.position;
        }
    }
    private void FixedUpdate()
    {
        if (!ManuallyUpdated || FakeParent == null)
            AIUpdate();
    }
}
