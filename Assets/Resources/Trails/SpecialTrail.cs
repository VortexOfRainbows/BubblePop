using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTrail : MonoBehaviour
{
    public static GameObject TrailPrefab => Resources.Load<GameObject>("Trails/FollowerTrail");
    public static SpecialTrail NewTrail(Transform parent, Color c, float width = 1f, float length = 1f)
    {
        SpecialTrail t = Instantiate(TrailPrefab).GetComponent<SpecialTrail>();
        t.Trail.startColor = c;
        t.originalAlpha = c.a;
        t.Trail.endColor = c.WithAlpha(0);
        t.Trail.time = length;
        t.Trail.textureScale = new Vector2(1, 1f / width);
        //t.Trail.startWidth = 1f;
        //t.Trail.endWidth = 1f;
        t.FakeParent = parent;
        t.transform.position = parent.transform.position;
        return t;
    }
    public Transform FakeParent;
    public TrailRenderer Trail;
    public float timer;
    public float originalAlpha;
    public void FixedUpdate()
    {
        if(FakeParent == null)
        {
            Trail.autodestruct = true;
            timer += Time.fixedDeltaTime;
            float iPer = (1 - timer / Trail.time);
            Trail.startColor = Trail.startColor.WithAlpha(originalAlpha * iPer * iPer);
        }
        else
        {
            transform.position = FakeParent.position;
        }
    }
}
