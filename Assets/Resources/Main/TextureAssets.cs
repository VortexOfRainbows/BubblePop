using UnityEngine;
using UnityEngine.UI;

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
        public static Material InfectorShader;
        public static Sprite T3ChestUma, T3ChestUmaOpen, T3Chest, T3ChestOpen, T2Chest, T2ChestOpen, T1Chest, T1ChestOpen, BlackMarketCrate;
        public static Sprite[] SlotSymbol = new Sprite[4];
        public static Sprite GoldProj, CoinProj, GemProj, TokenProj, FireProj;
        public static Sprite PowerUpPlaceholder;
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
            InfectorShader = Resources.Load<Material>("Materials/InfectionShader/Infection");

            T3ChestUma = Resources.Load<Sprite>("Chests/T3ChestClosed2");
            T3ChestUmaOpen = Resources.Load<Sprite>("Chests/T3ChestOpen2");
            T3Chest = Resources.Load<Sprite>("Chests/T3ChestClosed");
            T3ChestOpen = Resources.Load<Sprite>("Chests/T3ChestOpen");
            T2Chest = Resources.Load<Sprite>("Chests/T2ChestClosed");
            T2ChestOpen = Resources.Load<Sprite>("Chests/T2ChestOpen");
            T1Chest = Resources.Load<Sprite>("Chests/T1ChestClosed");
            T1ChestOpen = Resources.Load<Sprite>("Chests/T1ChestOpen");
            SlotSymbol[0] = Resources.Load<Sprite>("Player/Gachapon/SlotMachine/Symbol1");
            SlotSymbol[1] = Resources.Load<Sprite>("Player/Gachapon/SlotMachine/Symbol2");
            SlotSymbol[2] = Resources.Load<Sprite>("Player/Gachapon/SlotMachine/Symbol3");
            SlotSymbol[3] = Resources.Load<Sprite>("Player/Gachapon/SlotMachine/Symbol4");

            CoinProj = Resources.Load<Sprite>("Projectiles/SlotMachine/CoinProj");
            GoldProj = Resources.Load<Sprite>("Projectiles/SlotMachine/GoldProj");
            GemProj = Resources.Load<Sprite>("Projectiles/SlotMachine/GemProj");
            TokenProj = Resources.Load<Sprite>("Money/GachaslotTokenFlat");
            FireProj = Resources.Load<Sprite>("Projectiles/Fire");
            PowerUpPlaceholder = Resources.Load<Sprite>($"PowerUps/Random");
            BlackMarketCrate = Resources.Load<Sprite>("Chests/BlackMarketCrate");
        }
    }
    public static class PrefabAssets
    {
        public static GameObject DefaultProjectile;
        public static GameObject BatterUpTokenPrefab;
        public static PowerUpObject PowerUpObj;
        public static GameObject CrucibleNode = Resources.Load<GameObject>("World/Nodes/SubNodes/CrucibleNode");
        public static void Load()
        {
            DefaultProjectile = Resources.Load<GameObject>("Projectiles/Projectile");
            PowerUpObj = Resources.Load<GameObject>("PowerUps/Prefabs/PowerUpObj").GetComponent<PowerUpObject>();
            BatterUpTokenPrefab = Resources.Load<GameObject>("Player/Gachapon/SlotMachine/BatterUpToken");
        }
    }
}

