using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class ThoughtBubble : Body
{
    public GameObject TailPrefab;
    public List<GameObject> Tails;
    public override void Init()
    {
        PrimaryColor = new Color(1.00f, 1.05f, 1.1f);
    }
    public SpriteRenderer MouthR;
    protected override float AngleMultiplier => 0.1f;
    protected override float RotationSpeed => 1f;
    protected override void ModifyPowerPool(List<PowerUp> powerPool)
    {
        powerPool.Add<Choice>();
        powerPool.Add<Choice>();
        powerPool.Add<BubbleBirb>();
    }
    protected override string Description()
    {
        return "A mysterious Bubbletech Scientist. His origin is unknown; he appeared to Bubblemancer shortly after the first waves... why was he there?";
    }
    public override void FaceUpdate()
    {
        Vector2 toMouse = Utils.MouseWorld - (Vector2)transform.position;
        toMouse *= Mathf.Sign(p.lastVelo.x);
        Vector2 toMouse2 = toMouse.normalized;
        toMouse2.x += Mathf.Sign(toMouse2.x) * 4;
        float toMouseR = toMouse2.ToRotation();
        Vector2 looking = new Vector2(0.2f, 0).RotatedBy(toMouseR);
        looking.y *= 0.8f;
        if (looking.x < 0)
            toMouseR += Mathf.PI;
        Vector2 pos = new Vector2(0.1f, 0.16f * p.Direction) + looking;
        Face.transform.localPosition = Vector2.Lerp(Face.transform.localPosition, pos, 0.1f);
        Face.transform.eulerAngles = new Vector3(0, 0, toMouseR * Mathf.Rad2Deg);
        FaceR.flipX = !p.BodyR.flipY;
        MouthR.flipX = p.BodyR.flipY;
    }
    public override void AbilityUpdate(ref Vector2 playerVelo, Vector2 moveSpeed)
    {
        TailUpdate();
    }
    public void TailUpdate()
    {
        if(Tails == null || Tails.Count <= 0)
        {
            Tails = new List<GameObject>();
            for(int i = 0; i < 10; ++i)
            {
                Vector2 circular = new Vector2(0, 1 + i * 0.25f).RotatedBy(i * Mathf.Deg2Rad * 36);
                Tails.Add(Instantiate(TailPrefab, (Vector2)transform.position + circular, Quaternion.identity));
            }
        }
        Vector3 previousPos = transform.position;
        float tailSeparation = 0.95f;
        for(int i = 0; i < Tails.Count; ++i)
        {
            GameObject Tail = Tails[i];
            Vector2 tailToBody = previousPos - Tail.transform.position;
            tailToBody = tailToBody.normalized * tailSeparation;

            float deg = tailToBody.ToRotation() * Mathf.Rad2Deg;
            if (deg > 360)
                deg %= 360;
            else if (deg < 0)
                deg += 360;
            Tail.GetComponent<SpriteRenderer>().flipY = deg > 90 && deg < 270;
            Tail.transform.eulerAngles = new Vector3(0, 0, deg);
            Tail.transform.position = (Vector2)previousPos - tailToBody;
            previousPos = Tail.transform.position;
        }
    }
}
