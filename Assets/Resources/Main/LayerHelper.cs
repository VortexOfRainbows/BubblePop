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

    public const int FloorTileSortingOrder = -1;
    public const int WallTileSortingOrder = 0;

    //-----------------------------------
    //           DEFAULT LAYER (not effected by tile shadows)
    //-----------------------------------

    public const int ShadowSortingOrder = -50; //Behind everything (shadows)
    public const int FloorObjSortingLayer = -2; //Power Pillows
    public const int PickupSortingOrder = -1; //Coins, Keys, Powers, Etc.
    public const int PortalSortingOrder = -1;
    public const int TrailSortingOrder = -1;
    public const int ShopGachaponSortingOrder = -1;
    public const int ChestSortingOrder = -1;
    public const int Enemy = -1;

    public const int Player = 0;
    public const int FloraSortingOrder = 0; //All ground decor
    public const int CrucibleSortingOrder = 0;
    public const int SolidTileSortingOrder = 1;
    public const int RestockMachineTopSortingOrder = 1; //Restock Machine (TOP)
    public const int SpecialHatSortingOrder = 2; //Bulb, Crown
    public const int OverBorderShadowSortingOrder = 2;
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
    public const int InWorldUIElement = 10; //Boss/Skull health bars
    public const int ParticleFront = 10;
    public const int ParticleBack = -15;
    public const int ParticleLightning = -6;
    public const int ArbitraryCompendiumUISpriteMask = 30;
}
