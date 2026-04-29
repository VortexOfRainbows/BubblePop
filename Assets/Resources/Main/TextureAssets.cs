using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Main : MonoBehaviour
{
    public static class TextureAssets
    {
        public static Material InfectorShader;
        public static Material AdditiveShader;
        public static Material AlphaShader;
        public static Material SpriteLit;
        public static Material SpriteGlowmask;
        public static Material SpriteUnlit => SpriteGlowmask;


        public static Sprite BathBombSprite;
        public static Sprite BubbleSprite;
        public static Sprite BubbleSmall;
        public static Sprite[] BathBombShards;
        public static Sprite Feather;
        public static Sprite Laser;
        public static Sprite Sparkle;
        public static Sprite Shadow;
        public static Sprite GradientLine;
        public static Sprite T3ChestUma, T3ChestUmaOpen, T3Chest, T3ChestOpen, T2Chest, T2ChestOpen, T1Chest, T1ChestOpen, BlackMarketCrate;
        public static Sprite[] SlotSymbol = new Sprite[4];
        public static Sprite GoldProj, CoinProj, GemProj, TokenProj, FireProj;
        public static Sprite PowerUpPlaceholder;
        public static Sprite[] Flowers; //= Resources.LoadAll<Sprite>("World/Decor/Nature/Sprites/Flowers");
        public static Sprite[] TallGrass; //= Resources.LoadAll<Sprite>("World/Decor/Nature/Sprites/TallGrass");
        public static Sprite[] ShortGrass;// = Resources.LoadAll<Sprite>("World/Decor/Nature/Sprites/ShortGrass");
        public static void Load()
        {
            BubbleSmall = Resources.Load<Sprite>("Projectiles/SmallBubble");
            BubbleSprite = Resources.Load<Sprite>("Projectiles/Bubble2");
            BathBombSprite = Resources.Load<Sprite>("Projectiles/BathBomb/BathBomb");
            BathBombShards = new Sprite[] { Resources.Load<Sprite>("Projectiles/BathBomb/BBS1"), Resources.Load<Sprite>("Projectiles/BathBomb/BBS2"), 
                Resources.Load<Sprite>("Projectiles/BathBomb/BBS3"), Resources.Load<Sprite>("Projectiles/BathBomb/BBS4"), Resources.Load<Sprite>("Projectiles/BathBomb/BBS5") };
            Feather = Resources.Load<Sprite>("Projectiles/Feather");
            Laser = Resources.Load<Sprite>("Projectiles/Laser");
            Sparkle = Resources.Load<Sprite>("Projectiles/Sparkle");
            GradientLine = Resources.Load<Sprite>("LongGradient");
            Shadow = Resources.Load<Sprite>("Shadow");
            InfectorShader = Resources.Load<Material>("Materials/InfectionShader/Infection");
            AdditiveShader = Resources.Load<Material>("Materials/Additive");
            AlphaShader = Resources.Load<Material>("Materials/Alpha");
            SpriteLit = Resources.Load<Material>("Materials/SpriteLit");
            SpriteGlowmask = Resources.Load<Material>("Materials/SpriteGlowmask");

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
            Flowers = new Sprite[] {
                Resources.Load<Sprite>("World/Decor/Nature/Sprites/Flowers1")
            };
            ShortGrass = new Sprite[] {
                Resources.Load<Sprite>("World/Decor/Nature/Sprites/ShortGrass1"),
                Resources.Load<Sprite>("World/Decor/Nature/Sprites/ShortGrass2"),
                Resources.Load<Sprite>("World/Decor/Nature/Sprites/ShortGrass3"),
                Resources.Load<Sprite>("World/Decor/Nature/Sprites/ShortGrass4"),
            };
            TallGrass = new Sprite[] {
                Resources.Load<Sprite>("World/Decor/Nature/Sprites/TallGrass1"),
                Resources.Load<Sprite>("World/Decor/Nature/Sprites/TallGrass2"),
                Resources.Load<Sprite>("World/Decor/Nature/Sprites/TallGrass3"),
            };
        }
    }
    public static class PrefabAssets
    {
        public static GameObject DefaultProjectile;
        public static GameObject BatterUpTokenPrefab;
        public static PowerUpObject PowerUpObj;
        public static GameObject PlayerPrefab = Resources.Load<GameObject>("Player/Player");
        public static GameObject SpritebatchPrefab = Resources.Load<GameObject>("SpriteBatch/SpritebatchDraw");
        public static GameObject AbilityBlurbPrefab = Resources.Load<GameObject>("UI/AbilityBlurb");
        public static GameObject QuestPrefab = Resources.Load<GameObject>("UI/Quest");
        public static GameObject Roadblock { get; private set; }
        public static readonly List<GameObject> Trees = new();
        public static readonly List<GameObject> Stumps = new();
        public static void Load()
        {
            Trees.Clear();
            Stumps.Clear();
            DefaultProjectile = Resources.Load<GameObject>("Projectiles/Projectile");
            PowerUpObj = Resources.Load<GameObject>("PowerUps/Prefabs/PowerUpObj").GetComponent<PowerUpObject>();
            BatterUpTokenPrefab = Resources.Load<GameObject>("Player/Gachapon/SlotMachine/BatterUpToken");
            Roadblock = Resources.Load<GameObject>("World/Roadblock");
            Trees.Add(Resources.Load<GameObject>("World/Decor/Nature/FluffyTree"));
            Trees.Add(Resources.Load<GameObject>("World/Decor/Nature/PointyTree"));
            Stumps.Add(Resources.Load<GameObject>("World/Decor/Nature/PlainStump"));
            Stumps.Add(Resources.Load<GameObject>("World/Decor/Nature/PlainStumpMoss"));
            Stumps.Add(Resources.Load<GameObject>("World/Decor/Nature/StumpWBranch"));
            Stumps.Add(Resources.Load<GameObject>("World/Decor/Nature/StumpWBranchMoss"));
        }
    }
}

