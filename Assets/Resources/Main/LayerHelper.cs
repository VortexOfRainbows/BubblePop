using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public static class LayerHelper
{
    //This class mostly exists as a reminder for which sorting orders are populated by which assets

    //-----------------------------------
    //           FlOOR LAYER (effected by tile shadows)
    //-----------------------------------

    //-------------BACKGROUND------------
    public const int FloorTileSortingOrder = -1;
    public const int WallTileSortingOrder = 0;





    //-----------------------------------
    //           DEFAULT LAYER (not effected by tile shadows)
    //-----------------------------------

    //-------------BACKGROUND------------
    public const int FloorObjSortingLayer = -2; //Power Pillows
    public const int PickupSortingOrder = -1; //Coins, Keys, Powers, Etc.
    public const int PortalSortingOrder = -1;
    public const int TrailSortingOrder = -1;

    //-------------FOREGROUND------------
    public const int PlayerEnemyWorldObj = 0;
    public const int FloraSortingOrder = 0; //All ground decor
    public const int SolidTileSortingOrder = 1;
    public const int SpecialHatSortingOrder = 2; //Bulb, Crown
    public const int WeaponSortingOrder = 3;


    //-----------------------------------
    //       Player Sorting Group
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
    public const int ShadowSortingOrder = -50;
    public const int InWorldUIElement = 10; //Boss/Skull health bars
    public const int ArbitraryCompendiumUISpriteMask = 30;
}
