/// <summary>
/// THIS CLASS DOESN'T REALLY **DO** ANYTHING
/// 
/// It exists as a reminder for which sorting orders are populated by which assets
/// </summary>
public static class LayerHelper
{
    //-----------------------------------
    //           FlOOR LAYER (effected by tile shadows)
    //-----------------------------------

    public const int FloorTileSortingOrder = 0;
    public const int HazardSortingOrder = 1;
    public const int HazardTopSortingOrder = 2;
    public const int WallTileSortingOrder = 3;

    //-----------------------------------
    //           DEFAULT LAYER (not effected by tile shadows)
    //-----------------------------------

    public const int ShadowSortingOrder = -50; //Behind everything (shadows)
    public const int FloorObjAndFloraSortingLayer = -5; //Power Pillows
    public const int BarrierVisualLayer = -4; //Red shadermask visual
    public const int CrucibleSortingOrder = -3;
    public const int ChestSortingOrder = -3;
    public const int ForgeChamberSortingOrder = -3;
    public const int PickupSortingOrder = -2; //Coins, Keys, Etc.
    public const int PowerOrder = -1; //Powers
    public const int PortalSortingOrder = -1;
    public const int TrailSortingOrder = -1;
    public const int ShopGachaponSortingOrder = -1;
    public const int Enemy = -1;

    public const int Player = 0;
    public const int SolidTileSortingOrder = 1;
    public const int RestockMachineTopSortingOrder = 1; //Restock Machine (TOP)
    public const int ForgeTopSortingOrder = 1; //Forge Machine (TOP)
    public const int SpecialHatSortingOrder = 2; //Bulb, Crown
    public const int OverBorderShadowSortingOrder = 2;
    public const int ChestAirborneSortingOrder = 2;
    public const int PowerAirborneSortingOrder = 2;
    public const int WeaponSortingOrder = 3;

    public const int TreeSortingOrder = 20;

    //-----------------------------------
    //       Player Sorting Group (sublayers)
    //-----------------------------------
    public const int CapeBack = -3;
    public const int Skateboard = -2;
    public const int BehindBody = -1;
    public const int Body = 0;
    public const int Face = 1;
    public const int OverBody = 2;
    public const int Hat = 3;

    //-----------------------------------
    //          DEFAULT LAYER (not yet integrated)
    //-----------------------------------
    public const int Projectile = 5;
    public const int InWorldUIBuff = 11; //Boss/Skull health bars
    public const int InWorldUIElement = 10; //Boss/Skull health bars
    public const int ParticleFront = 10;
    public const int ParticleBack = -15;
    public const int ParticleLightning = -6;
    public const int ArbitraryCompendiumUISpriteMask = 30;
    public const int ParticleSnow = 4;
}
