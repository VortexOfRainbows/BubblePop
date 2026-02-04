using UnityEngine;

public class RockGolem : RockSpider
{
    public override void InitializeDescription(ref DetailedDescription description)
    {
        description.WithName("Rock Golem");
    }
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.04f;
        additiveColorPower += 0.2f;
    }
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 150;
        data.BaseMaxCoin = 50;
        data.BaseMinCoin = 5;
        data.BaseMaxGem = 3;
        data.Cost = 8f;
    }
    public override void ModifyUIOffsets(ref Vector2 offset, ref float scale)
    {
        scale *= 1.4f;
    }
    public RockGolem Parent { get; set; } = null;
    public override void OnSpawn()
    {
        //do not do base.OnSpawn()
    }
    public bool hasSpawned = false;
    public override void AI()
    {
        if (Parent == null)
        {
            if(!hasSpawned)
            {
                RockGolem prevP = this;
                for(int i = 1; i < 10; ++i)
                {
                    RockGolem r = Instantiate(EnemyID.RockGolem, transform.position, Quaternion.identity).GetComponent<RockGolem>();
                    r.Parent = prevP;
                    r.Visual.transform.localPosition = new Vector3(r.Visual.transform.localPosition.x, r.Visual.transform.localPosition.y, 0.05f * i);
                    prevP = r;
                }
                hasSpawned = true;
            }
            Head.gameObject.SetActive(true);

            Vector2 toPlayer = Player.Position - (Vector2)transform.position;
            Vector2 norm = toPlayer.normalized;
            RB.velocity += norm * 0.1f;
            RB.velocity *= 0.95f;
        }
        else
        {
            Head.gameObject.SetActive(false);

            Vector2 pos = transform.position;
            Vector2 parent = Parent.transform.position;
            Vector2 parentToMe = pos - parent;
            float magnitude = parentToMe.magnitude;
            float snapDist = 2;
            if(magnitude > snapDist)
            {
                Vector2 old = transform.position;
                transform.position = parent + parentToMe.normalized * snapDist;
                Vector2 oldToNew = (Vector2)transform.position - old;
                RB.velocity = oldToNew * magnitude / Time.fixedDeltaTime;
            }
            else
            {
                RB.velocity *= 0.95f;
            }
            transform.position -= (Vector3)(RB.velocity * Time.fixedDeltaTime);
        }
        UpdateDirection(Utils.SignNoZero(RB.velocity.x));
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
