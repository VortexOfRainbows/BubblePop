using UnityEngine;
public class EnemySoap : Enemy
{
    public override void ModifyInfectionShaderProperties(ref Color outlineColor, ref Color inlineColor, ref float inlineThreshold, ref float outlineSize, ref float additiveColorPower)
    {
        inlineThreshold = 0.3f;
    }
    public SpriteRenderer sRender;
    public Sprite Soap1;
    public Sprite Soap2;
    //aiState 0 = aiming
    //aiState 1 = attacking
    protected Vector2 targetedPlayerPosition = Vector2.zero;
    protected float timer = 0;
    protected float moveSpeed = 3f;
    public override float CostMultiplier => 2;
    public override void InitStatics(ref EnemyID.StaticEnemyData data)
    {
        data.BaseMaxLife = 5;
        data.BaseMaxCoin = 5;
        data.Card = Resources.Load<Sprite>("NPCs/Old/Wsoap");
    }
    // Update is called once per frame
    public override void AI()
    {
        timer++;
        if (timer > 120) 
        {
            timer = 0;
            targetedPlayerPosition = FindTargetedPlayerPosition();
        }
        if (timer > 50 && timer < 90)
        {
            if(targetedPlayerPosition == Vector2.zero)
                targetedPlayerPosition = FindTargetedPlayerPosition();
            Vector2 toPlayer = targetedPlayerPosition - (Vector2)transform.position;
            if (timer == 51)
            {
                if(this is EnemySoapTiny)
                    AudioManager.PlaySound(SoundID.SoapSlide, transform.position, 1f, 1.2f);
                else
                    AudioManager.PlaySound(SoundID.SoapSlide, transform.position, 1f, 1f);
                RB.velocity *= 0.5f;
                RB.velocity += toPlayer.normalized * 6f;
            }
            Vector2 norm = RB.velocity.normalized;
            if(Random.Range(0, 2) == 0)
                ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(1) - norm * 1.5f, .3f, norm * Utils.RandFloat(5f, 15f), 1.5f, 0.6f, 1, new Color(1, 0.85f, 0.99f));
            else if (Random.Range(0, 3) != 0)
                ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(1) - norm * 1.5f, Utils.RandFloat(0.45f, 0.75f), norm * Utils.RandFloat(5f, 15f), 1.5f, Utils.RandFloat(0.5f, 0.7f), 0, new Color(1, 0.85f, 0.99f));
            RB.velocity += toPlayer.normalized * 0.5f;
            float rotation = RB.velocity.ToRotation() * Mathf.Rad2Deg;
            if (rotation > 90)
                rotation -= 180;
            else if (rotation < -90)
                rotation += 180;
            Visual.transform.localEulerAngles = new Vector3(0, 0, rotation);
        }
        if(timer > 90)
            RB.velocity *= 0.91f;
    }
    public override void OnKill()
    {
        AudioManager.PlaySound(SoundID.SoapDie, sRender.transform.position, 1f, 1f);
        DeathParticles(15, 0.5f, new Color(1, 0.85f, 0.99f));
        if(this is not EnemySoapTiny)
        {
            Spawn(EnemyID.OldSmallSoap, transform.position, IsSkull).GetComponent<EnemySoapTiny>().sRender.sprite = Soap1;
            Spawn(EnemyID.OldSmallSoap, transform.position, IsSkull).GetComponent<EnemySoapTiny>().sRender.sprite = Soap2;
        }
    }
    private Vector2 FindTargetedPlayerPosition() {
        float offsetX = Random.Range(-5f, 5f);
        float offsetY = Random.Range(-5f, 5f);
        return new Vector2 (Player.Position.x + offsetX, Player.Position.y + offsetY);
    }
    public override string Name()
    {
        return "Soap";
    }
}
