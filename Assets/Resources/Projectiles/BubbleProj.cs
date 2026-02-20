using System.Data.Common;
using UnityEngine;
public class SmallBubble : Projectile
{
    public int RandomLifeShorten = 0;
    public override void Init()
    {
        Color c = Player.ProjectileColor;
        c.a = 0.68f;
        SpriteRenderer.color = c;
        SpriteRenderer.sprite = Main.TextureAssets.BubbleSmall;
        RandomLifeShorten = Utils.RandInt(41);
        timer2 = 0;
        if(Data.Length > 0)
            Data1 = 0;
        transform.localScale *= 0.3f;
        Damage = 1;
        Penetrate = 1;
        Friendly = false;
        SpriteRendererGlow.gameObject.SetActive(false);
        RB.velocity *= 1 + 0.1f * PlayerOwner.FasterBulletSpeed;

        if (PlayerOwner.EternalBubbles > 0)
        {
            int bonus = PlayerOwner.EternalBubbles;
            if (bonus > 9)
            {
                RandomLifeShorten -= 10 * (bonus - 9);
                bonus = 9;
            }
            RandomLifeShorten -= 40 + 40 * bonus;
        }
    }
    public override void AI()
    {
        float deathTime = 180;
        deathTime -= RandomLifeShorten;
        if (++timer2 > 3)
            Friendly = true;
        Vector2 velo = RB.velocity;
        if (timer > deathTime - 100 + RandomLifeShorten)
            velo *= 0.95f;
        else if (timer > deathTime - 130 + RandomLifeShorten)
            velo *= 0.9725f;
        else if (timer > deathTime - 160 + RandomLifeShorten)
            velo *= 0.99f;
        RB.velocity = velo;
        float speed = RB.velocity.magnitude;
        float rtSpeed = Mathf.Sqrt(speed);
        RB.rotation += rtSpeed * Mathf.Sign(RB.velocity.x);
        float targetScale = 0.66f;
        if(Data.Length > 0)
        {
            float rtData1 = 0.1f * Data1 + Mathf.Sqrt(Data1);
            rtSpeed += rtData1;
            targetScale += rtData1 * 0.4f;
        }
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.075f + 0.02f * rtSpeed);

        float FadeOutTime = 20;
        if (timer > deathTime + FadeOutTime)
        {
            Kill();
        }
        if ((int)timer % 4 == 0)
        {
            Vector2 norm = RB.velocity.normalized;
            ParticleManager.NewParticle((Vector2)transform.position - norm * 0.2f + Utils.RandCircle(transform.lossyScale.x * 0.4f), .225f, norm * -.75f, 0.6f, Utils.RandFloat(0.225f, 0.35f), 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        }
        if (timer > deathTime)
        {
            float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0.68f * alphaOut);
        }
        timer++;
    }
    public override void OnKill()
    {
        int c = Data.Length > 0 ? (int)Data1 * 2 + 3 : 3;
        for (int i = 0; i < c; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, 0.5f), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f) * transform.localScale.x, Utils.RandFloat(0.3f, 0.5f), circular * Utils.RandFloat(4, 6), 4f, 0.36f, 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        }
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.5f, 1.1f);
    }
}
public class BigBubble : Projectile
{
    public override void Init()
    {
        SpriteRenderer.sprite = Main.TextureAssets.BubbleSprite;
        Color c = Player.ProjectileColor;
        c.a = 0.68f;
        SpriteRenderer.color = c;
        transform.localScale *= 0.3f;
        Damage = 1;
        Penetrate = -1;
        Friendly = false;
        SpriteRendererGlow.gameObject.SetActive(false);
    }
    public override void OnKill()
    {
        AudioManager.PlaySound(SoundID.BubblePop, transform.position, 0.8f, 0.9f);
        if (PlayerOwner.BubbleBlast > 0)
        {
            float amt = 1 + (3 + Data2) * PlayerOwner.BubbleBlast;
            float speed = 3.5f + (Data2 * 1.25f + PlayerOwner.ChargeShotDamage * 0.75f);
            for (int i = 0; i < amt; i++)
                NewProjectile<SmallBubble>(transform.position, new Vector2(speed * Mathf.Sqrt(Utils.RandFloat(0.2f, 1.2f)), 0).RotatedBy((i + Utils.RandFloat(1)) / (int)amt * Mathf.PI * 2f), 1, PlayerOwner);
        }
        for (int i = 0; i < 30; i++)
        {
            Vector2 circular = new Vector2(.6f + transform.localScale.x * 0.4f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + circular * Utils.RandFloat(0, 1), Utils.RandFloat(0.3f, 0.6f), circular * Utils.RandFloat(4, 7), 3f, Utils.RandFloat(0.3f, 0.5f), 0, Player.ProjectileColor);
        }
    }
    public override void AI()
    {
        BigBubbleAI();
    }
    public void BigBubbleAI()
    {
        if (PlayerOwner.Weapon is not BubblemancerWand wand)
        {
            return;
        }
        int attackRight = (int)wand.AttackRight;
        Vector2 toMouse = PlayerOwner.Control.MousePosition - PlayerOwner.Position;
        if (attackRight >= 50 && timer <= 0)
        {
            int maximumTarget = PlayerOwner.OldCoalescence + 2;
            float maximumSize = 0.8f + maximumTarget * .3f;

            int target = (int)(attackRight - 50) / 100;
            float targetSize = target * maximumSize / maximumTarget + 0.8f + attackRight / 240f;
            targetSize *= 1f + Mathf.Sqrt(PlayerOwner.ChargeShotDamage) * 0.4f;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetSize, 0.1f);
            timer = -attackRight;
            Vector2 circular = new Vector2(targetSize, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            if (Utils.RandFloat(1) < 0.2f)
                ParticleManager.NewParticle((Vector2)transform.position + circular, .2f, -circular.normalized * 6 + PlayerOwner.RB.velocity * 0.9f, 0.2f, 0.3f, 0, Player.ProjectileColor);
            if (attackRight >= Data1)
            {
                for (int i = 0; i < 30; i++)
                {
                    circular = new Vector2(targetSize * 0.5f + 1f, 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
                    ParticleManager.NewParticle((Vector2)transform.position + circular, .3f, -circular.normalized * Utils.RandFloat(5, 10) + PlayerOwner.RB.velocity * 0.9f, 0.2f, Utils.RandFloat(0.2f, 0.4f), 0, Player.ProjectileColor);
                }
                Data1 += 100;
            }
            float r = PlayerOwner.Weapon.transform.eulerAngles.z * Mathf.Deg2Rad;
            Vector2 awayFromWand = PlayerOwner.Weapon is BubbleGun ? new Vector2(1.8f + targetSize * 0.5f, 0.2f * Mathf.Sign(PlayerOwner.Animator.PointDirOffset)).RotatedBy(r) : new Vector2(1.4f, (0.51f + targetSize * 0.49f) * Mathf.Sign(PlayerOwner.Animator.PointDirOffset)).RotatedBy(r);

            transform.position = Vector2.Lerp(transform.position, (Vector2)PlayerOwner.Weapon.transform.position + awayFromWand, 0.15f);
            RB.velocity *= 0.8f;
            RB.velocity += PlayerOwner.RB.velocity * 0.1f;
            Damage = (1 + target) * (2 + PlayerOwner.ChargeShotDamage);
            Data2 = target;
            //rb.rotation = toMouse.ToRotation() * Mathf.Rad2Deg;
        }
        else if (timer <= 0)
        {
            AudioManager.PlaySound(SoundID.ShootBubbles, transform.position, 1.1f, 0.6f);
            if (toMouse.magnitude < 6)
                toMouse = toMouse.normalized * 6;
            Vector2 mouse = PlayerOwner.Position + toMouse;
            toMouse = mouse - (Vector2)transform.position;
            RB.velocity = toMouse * 0.1f + toMouse.normalized * (10f + Mathf.Min(24, 24f * (timer + 50) / -200f));
            timer = 1;

            for (int i = 0; i < 30; i++)
                ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(transform.localScale.x), Utils.RandFloat(.3f, .5f), RB.velocity * Utils.RandFloat(2f), 0.5f, Utils.RandFloat(.4f, 1f), 0, Player.ProjectileColor);

            RB.rotation = RB.velocity.ToRotation() * Mathf.Rad2Deg;
        }
        else if (timer > 0)
        {
            RB.rotation += Mathf.Sqrt(RB.velocity.magnitude) * Mathf.Sign(RB.velocity.x);
            if (Utils.RandFloat(1) < 0.15f)
                ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(transform.localScale.x * 0.5f), Utils.RandFloat(.3f, .4f), RB.velocity * Utils.RandFloat(1f), 0.4f, Utils.RandFloat(.3f, .6f), 0, Player.ProjectileColor);
            Friendly = true;
            RB.velocity *= 1 - 0.0035f - timer / 4000f;
            timer++;
            if (PlayerOwner.SoapySoap > 0 && timer <= 120)
            {
                int count = (int)(2.0f + Data2 * 2f + PlayerOwner.SoapySoap * 3f); //2 + 1.5 + 3.5 = 7
                int interval = 120 / count;
                if (interval <= 0)
                    interval = 1;
                if (timer % interval == 0)
                {
                    Vector2 norm = RB.velocity.normalized;
                    float veloMult = Utils.RandFloat(0.25f, 3f + PlayerOwner.FasterBulletSpeed * 1.25f);
                    NewProjectile<SmallBubble>((Vector2)transform.position + Utils.RandCircle(transform.lossyScale.x * 0.5f), Utils.RandCircle(2) - norm * veloMult, 1, PlayerOwner);
                }
            }
            if (timer > 160)
            {
                float alphaOut = 1 - (timer - 160) / 20f;
                SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, 0.68f * alphaOut);
            }
            if (timer > 180)
                Kill();
        }
    }
    public override void OnHitTarget(Entity target)
    {
        AudioManager.PlaySound(SoundID.BubblePop, gameObject.transform.position, 0.8f, 1.5f);
        gameObject.transform.localScale *= 0.8f;
        RB.velocity *= 0.8f;
        Damage = Damage * 0.8f;
        if (Damage < 0)
            Kill();
        else
        {
            int c = 5 + (int)Damage * 3;
            for (int i = 0; i < c; i++)
                ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(transform.localScale.x), Utils.RandFloat(.3f, .4f), RB.velocity.normalized * Utils.RandFloat(1f), 0.4f, Utils.RandFloat(.4f, .8f), 0, default);
        }
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        if (timer > 10)
        {
            Vector2 closest = collision.ClosestPoint(transform.position);
            Vector2 diff = closest - RB.position;
            if (diff.magnitude > 1)
                return false;
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            {
                bool goingInThatDirection = Mathf.Sign(RB.velocity.x) == Mathf.Sign(diff.x);
                if (goingInThatDirection)
                    RB.velocity = new Vector2(RB.velocity.x * -1.1f, RB.velocity.y);
            }
            else
            {
                bool goingInThatDirection = Mathf.Sign(RB.velocity.y) == Mathf.Sign(diff.y);
                if (goingInThatDirection)
                    RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * -1.1f);
            }
        }
        return false;
    }
    public override bool OnInsideTile()
    {
        return timer > 12;
    }
}
public class ThunderBubble : Projectile
{
    private Color ColorVar;
    public bool Recalled = false;
    public float ScaleFactor => 2.0f * PlayerOwner.ZapRadiusMult;
    public override void Init()
    {
        ColorVar = Color.Lerp(Player.ProjectileColor, Color.blue, 0.5f);
        SpriteRenderer.color = ColorVar.WithAlphaMultiplied(0.3f);
        SpriteRenderer.sprite = Main.TextureAssets.BubbleSmall;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
        SpriteRendererGlow.color = ColorVar * 0.6f;
        timer2 = 0;
        transform.localScale *= 0.3f;
        Damage = 1;
        Penetrate = -1;
        Friendly = true;
        immunityFrames = 25;
        UpdateSize();

        float total = PlayerOwner.Electroluminescence;
        for(int i = 0; i < total; ++i)
        {
            ThunderLightSpearCaster TLSC = NewProjectile<ThunderLightSpearCaster>(transform.position, Vector2.zero, 2.0f + PlayerOwner.LightSpear * 0.5f, PlayerOwner, i, total, ScaleFactor * 1.1f, Utils.SignNoZero(RB.velocity.x)).GetComponent<ThunderLightSpearCaster>();
            TLSC.FakeParent = transform;
        }
    }
    public void UpdateSize()
    {
        float scaleFactor = ScaleFactor;
        SpriteRendererGlow.transform.localScale = scaleFactor * Vector3.one * 2f;
        cmp.c2D.radius = scaleFactor;
    }
    public override void AI()
    {
        Vector2 velo = RB.velocity;
        float speed = velo.magnitude;
        float percent = Mathf.Max(0, 2 - timer / 50f);
        if(!Recalled && percent > 0)
           transform.position += percent * Time.fixedDeltaTime * (Vector3)velo;

        float deathTime = 10800; //Only really times out when you stop recall
        float FadeOutTime = 60;
        if (timer > deathTime + FadeOutTime || PlayerOwner.Weapon is not Book)
        {
            Kill();
        }
        float targetScale = 1.1f;
        if(Book.InClosingAnimation && timer <= deathTime)
        {
            if(!Recalled)
            {
                AudioManager.PlaySound(SoundID.ElectricZap.GetVariation(0), transform.position, 0.3f, 0.66f);
                if(PlayerOwner.EchoBubbles > 0)
                {
                    NewProjectile<LatentCharge>(transform.position, Vector2.zero, 0, PlayerOwner, PlayerOwner.EchoBubbles);
                }
                Damage *= 2 + PlayerOwner.ThunderBubbleReturnDamageBonus;
                Recalled = true;
                //cmp.c2D.enabled = false;
            }
            timer = deathTime;
            Vector2 weaponPos = PlayerOwner.Weapon.transform.position;
            Vector2 toWeapon = weaponPos - (Vector2)transform.position;
            float perc = Book.ClosingPercent;
            perc = perc * perc;
            velo = toWeapon * 0.2f;
            transform.position = Vector2.Lerp(transform.position, weaponPos, perc);
            float iPer = 1 - Mathf.Max(0, (perc - 0.7f) / 0.3f);
            SpriteRenderer.color = ColorVar.WithAlphaMultiplied(0.3f * iPer);
            SpriteRendererGlow.color = ColorVar.WithAlphaMultiplied(iPer * iPer) * 0.6f * iPer * (1 - perc);
            transform.localScale = iPer * targetScale * Vector3.one;
            SpriteRendererGlow.transform.localScale = iPer * ((1 - perc) * 0.5f + 0.5f) * 4.0f * PlayerOwner.ZapRadiusMult * Vector3.one;
        }
        else
        {
            UpdateSize();
            if (Recalled)
            {
                Kill();
                return;
            }
            if (Recalled && timer <= deathTime)
                timer = deathTime + 1;
            if (timer > deathTime)
            {
                float alphaOut = 1 - (timer - deathTime) / FadeOutTime;
                SpriteRenderer.color = ColorVar.WithAlphaMultiplied(0.3f * alphaOut);
                SpriteRendererGlow.color = ColorVar * 0.6f * alphaOut;
                transform.localScale = alphaOut * targetScale * Vector3.one;
            }
            else
            {
                float rtSpeed = Mathf.Sqrt(speed);
                RB.rotation += rtSpeed * Mathf.Sign(RB.velocity.x);
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, 0.075f + 0.02f * rtSpeed);
            }
            timer++;
        }
        if ((int)++timer2 % 6 != 0)
        {
            float perc = (1 - Book.ClosingPercent) * 0.1f;
            ParticleManager.NewParticle(RB.position + velo * Time.fixedDeltaTime * 2f, transform.localScale.x * 2.3f, velo * 0.5f, 2f, Recalled ? perc : Utils.RandFloat(0.1f, 0.15f), 2,
                SpriteRendererGlow.color.WithAlphaMultiplied(.6f));
        }
        else if (timer <= deathTime)
        {
            Vector2 norm = velo.normalized;
            Vector2 targetPos = Recalled ? PlayerOwner.Weapon.transform.position : RB.position - norm * transform.localScale.x * 2.5f;
            Pylon.SummonLightning2(transform.position, targetPos, ColorVar, 0.15f);
        }
        RB.velocity = velo;
        if(timer % 50 == 0)
        {
            if (Damage < 1)
                Damage += 0.1f;
        }
    }
    public override bool CanBeAffectedByHoming()
    {
        return !Book.InClosingAnimation;
    }
    public override bool DoHomingBehavior(Enemy target, Vector2 norm, float scale)
    {
        float currentSpeed = RB.velocity.magnitude + PlayerOwner.HomingRangeSqrt * 0.01f;
        float modAmt = 0.04f + PlayerOwner.HomingRangeSqrt * 0.04f;
        RB.velocity = Vector2.Lerp(RB.velocity * (1 - modAmt), norm * currentSpeed, modAmt).normalized * currentSpeed;
        return false;
    }
    public override void OnKill()
    {
        if(PlayerOwner.TotalBookBalls > 0)
            PlayerOwner.TotalBookBalls--;
        //int c = Data.Length > 0 ? (int)Data1 * 2 + 3 : 3;
        //for (int i = 0; i < c; i++)
        //{
        //    Vector2 circular = new Vector2(Utils.RandFloat(0, 0.5f), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
        //    ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f) * transform.localScale.x, Utils.RandFloat(0.3f, 0.5f), circular * Utils.RandFloat(4, 6), 4f, 0.36f, 0, Player.ProjectileColor.WithAlphaMultiplied(0.8f));
        //}
    }
    public override bool OnInsideTile()
    {
        return !Recalled;
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        if (Recalled)
            return false;
        Vector2 closest = collision.ClosestPoint(transform.position);
        Vector2 diff = closest - RB.position;
        if (diff.magnitude > 1)
            return false;
        if(Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            bool goingInThatDirection = Mathf.Sign(RB.velocity.x) == Mathf.Sign(diff.x);
            if(goingInThatDirection)
                RB.velocity = new Vector2(RB.velocity.x * -1.0f, RB.velocity.y);
        }
        else
        {
            bool goingInThatDirection = Mathf.Sign(RB.velocity.y) == Mathf.Sign(diff.y);
            if(goingInThatDirection)
                RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * -1.0f);
        }
        return false;
    }
    public override void OnHitTarget(Entity target)
    {
        float recalled = Recalled ? 0.8f : 1.2f;
        AudioManager.PlaySound(SoundID.ElectricZap, target.transform.position, 0.2f, recalled);
        Pylon.SummonLightning2(transform.position, target.transform.position, ColorVar, 0.6f);
    }
}
public class ThoughtBubbleThunderAura : Projectile
{
    private Color c;
    public override void Init()
    {
        c = PlayerOwner.Hat is Crown ? new Color(1, 0, 0, 0.7f) : new Color(1, 0.9f, .5f, 0.5f) * 0.9f;
        SpriteRenderer.enabled = false;
        transform.localScale *= 1.5f;
        Penetrate = -1;
        immunityFrames = 50;
        Friendly = false;
        C2D.radius *= 4 * (PlayerOwner.TrailOfThoughts / 10f + 1);
        SpriteRendererGlow.transform.localScale = Vector3.one * 2f;
        SpriteRendererGlow.color = Color.black;
        SpriteRendererGlow.sprite = Main.TextureAssets.Shadow;
    }
    public override void AI()
    {
        if (timer2++ < 8)
        {
            SpriteRendererGlow.transform.LerpLocalScale(Vector2.one * 2, 0.3f);
            SpriteRendererGlow.color = Color.Lerp(SpriteRendererGlow.color, c, 0.2f);
        }
        else
        {
            SpriteRendererGlow.transform.LerpLocalScale(Vector2.one, 0.06f);
            if(Data1 == 1)
                Friendly = true;
            SpriteRendererGlow.color = Color.Lerp(SpriteRendererGlow.color, Color.black, 0.12f);
        }
        if(timer2 > 25)
        {
            Kill();
        }
    }
    public override void OnKill()
    {
        int count = Data.Length > 0 ? (int)Data1 * 2 + 3 : 3;
        for (int i = 0; i < count; i++)
        {
            Vector2 circular = new Vector2(Utils.RandFloat(0, 0.5f), 0).RotatedBy(Utils.RandFloat(Mathf.PI * 2));
            ParticleManager.NewParticle((Vector2)transform.position + Utils.RandCircle(0.5f) * transform.localScale.x, Utils.RandFloat(0.3f, 0.5f), circular * Utils.RandFloat(4, 6), 4f, 0.36f, 2, c.WithAlphaMultiplied(0.8f));
        }
    }
    public override bool OnInsideTile()
    {
        return false;
    }
    public override bool OnTileCollide(Collider2D collision)
    {
        return false;
    }
    public override void OnHitTarget(Entity target)
    {
        Pylon.SummonLightning2(transform.position, target.transform.position, c, 0.6f);
    }
}