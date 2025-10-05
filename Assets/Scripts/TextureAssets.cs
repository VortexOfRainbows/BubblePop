using UnityEngine;

public partial class Main : MonoBehaviour
{
    public static class TextureAssets
    {
        public static Sprite BathBombSprite;
        public static Sprite BubbleSprite;
        public static Sprite BubbleSmall;
        public static Sprite[] BathBombShards;
        public static Sprite Feather;
        public static Sprite Laser;
        public static Sprite Sparkle;
        public static Sprite Shadow;
        public static void Load()
        {
            BubbleSmall = Resources.Load<Sprite>("Projectiles/SmallBubble");
            BubbleSprite = Resources.Load<Sprite>("Projectiles/Bubble2");
            BathBombSprite = Resources.Load<Sprite>("Projectiles/BathBomb/BathBomb");
            BathBombShards = new Sprite[] { Resources.Load<Sprite>("Projectiles/BathBomb/BBS1"), Resources.Load<Sprite>("Projectiles/BathBomb/BBS2"), 
                Resources.Load<Sprite>("Projectiles/BathBomb/BBS3"), Resources.Load<Sprite>("Projectiles/BathBomb/BBS4") };
            Feather = Resources.Load<Sprite>("Projectiles/Feather");
            Laser = Resources.Load<Sprite>("Projectiles/Laser");
            Sparkle = Resources.Load<Sprite>("Projectiles/Sparkle");
            Shadow = Resources.Load<Sprite>("Shadow");
        }
    }
}

