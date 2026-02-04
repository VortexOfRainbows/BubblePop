using UnityEngine;
public class RockGolem : RockSpider
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Rock Golem");
    }
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 15;
        data.BaseMaxCoin = 5;
        data.BaseMinCoin = 1;
        data.BaseMaxGem = 3;
        data.Cost = 15f;
        data.Rarity = 4;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        scale *= 1.7f;
        offset.y += 0.55f;
    }
    public GameObject Parent { get; set; } = null;
    public override void OnSpawn()
    {

    }
    public bool hasSpawned = false;
    public float SpawnedInAlpha = 0;
    public override void UIAI()
    {
        SpawnedInAlpha = 1;
        UpdateDirection(-1);
        //UpdateLegRotations();
        //Animate();
    }
    public override void AI()
    {
        if (Parent == null)
        {
            GetComponent<CircleCollider2D>().enabled = true;
            SpawnedInAlpha += 0.05f;
            if (!hasSpawned)
            {
                GameObject prevP = gameObject;
                for(int i = 1; i < 10; ++i)
                {
                    RockGolem r = Instantiate(EnemyID.RockGolem, transform.position, Quaternion.identity).GetComponent<RockGolem>();
                    r.Parent = prevP;
                    r.Visual.transform.localPosition = new Vector3(r.Visual.transform.localPosition.x, r.Visual.transform.localPosition.y, 0.07f * i);
                    prevP = r.gameObject;
                    r.transform.localScale *= 1f;
                }
            }
            Head.gameObject.SetActive(true);
            Vector2 toPlayer = Player.Position - (Vector2)transform.position;
            Vector2 norm = toPlayer.normalized;
            RB.velocity += norm * 0.15f;
            RB.velocity *= 0.95f;

            Head.transform.SetEulerZ(norm.x * 20 * norm.y);
            norm.x *= 0.3f;
            norm.y *= 0.25f;
            norm.y += 0.15f;
            Head.transform.localPosition = norm;
        }
        else
        {
            GetComponent<CircleCollider2D>().enabled = false;
            if(!hasSpawned)
                UpdateRendererColor(Color.red.WithAlpha(0), 1);
            Head.gameObject.SetActive(false);
            Vector2 pos = transform.position;
            Vector2 parent = Parent.transform.position;
            Vector2 parentToMe = pos - parent;
            float magnitude = parentToMe.magnitude;
            float snapDist = 2.0f;
            if(magnitude > snapDist)
            {
                Vector2 old = transform.position;
                transform.position = parent + parentToMe.normalized * snapDist;
                Vector2 oldToNew = (Vector2)transform.position - old;
                RB.velocity = Vector2.Lerp(RB.velocity, oldToNew * magnitude / Time.fixedDeltaTime, 0.05f);
            }
            SpawnedInAlpha +=  RB.velocity.magnitude * Time.fixedDeltaTime * 0.5f;
            transform.position -= (Vector3)(RB.velocity * Time.fixedDeltaTime);
        }
        hasSpawned = true;
        UpdateDirection(Utils.SignNoZero(RB.velocity.x));
        UpdateLegRotations();
        SpawnedInAlpha = Mathf.Clamp01(SpawnedInAlpha);
    }
    public new void Update()
    {
        if (IsDummy)
            return;
        if (DamageTaken > 0)
        {
            UpdateRendererColor(Color.Lerp(Color.white, Color.red, 0.8f).WithAlpha(SpawnedInAlpha), Utils.DeltaTimeLerpFactor(0.05f + DamageTaken / 500f));
            DamageTaken -= 20f * Time.deltaTime;
        }
        else
        {
            DamageTaken = 0;
            UpdateRendererColor(Color.Lerp(Color.red, Color.white, SpawnedInAlpha).WithAlpha(SpawnedInAlpha), Utils.DeltaTimeLerpFactor(0.1f));
        }
        lastPos = transform.position;
    }
    public Transform[] LegAnchors;
    public float movementOffset = 0;
    public void UpdateLegRotations()
    {
        float[] radians = new float[] {Mathf.PI, Mathf.PI * 3 / 2, Mathf.PI / 2, 0};
        movementOffset = Mathf.LerpAngle(movementOffset, (RB.velocity.ToRotation() + Mathf.PI / 2) * Mathf.Rad2Deg, 0.05f);
        Color c = IsDummy ? Color.white : Color.Lerp(Color.red, Color.white, SpawnedInAlpha);
        for (int i = 0; i < LegPoints.Length; ++i)
        {
            float r = radians[i] + movementOffset * Mathf.Deg2Rad;
            float sin = Mathf.Sin(r - 3 * Mathf.PI / 4);
            Vector2 v = new Vector2(1, 1).RotatedBy(r); 
            Vector3 defaultPos = v;
            defaultPos.x *= 0.75f;
            defaultPos.y *= 0.425f;
            defaultPos.y += 0.025f;

            Vector3 anchorPos = v;
            anchorPos.x *= 0.2f;
            anchorPos.y *= 0.2f;
            anchorPos.y -= 0.175f;

            LegPoints[i].color = Color.Lerp(Color.black, c, 0.8f + 0.2f * sin).WithAlpha(SpawnedInAlpha);

            bool isBackLeg = sin < 0;
            if (isBackLeg)
                anchorPos.z = defaultPos.z = 1.05f;
            float scaleBack = 0.85f + 0.15f * sin;
            LegAnchors[i].localPosition = LegAnchors[i].localPosition = defaultPos * scaleBack;
            LegConnectors[i].transform.localPosition = anchorPos * scaleBack;
            LegAnchors[i].localScale = Vector3.one * scaleBack;
            LegConnectors[i].transform.localScale = LegConnectors[i].transform.localScale.SetXY(LegConnectors[i].transform.localScale.x, 1.0f + 0.2f * sin);
        }
    }
    public override void UpdateDirection(float i)
    {
        if (i >= 0)
            i = -1;
        else
            i = 1;
        Body.transform.localScale = new Vector3(i * Mathf.Abs(Body.transform.localScale.x), Body.transform.localScale.y, 1);
    }
}
