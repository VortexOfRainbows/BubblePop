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





    //-----------------------------------
    //           DEFAULT LAYER (not effected by tile shadows)
    //-----------------------------------

    //-------------BACKGROUND------------
    public const int FloorObjSortingLayer = -2; //Power Pillows
    public const int PickupSortingOrder = -1; //Coins, Keys, Powers, Etc.

    //-------------FOREGROUND------------
    public const int PlayerSortingOrder = 0;
    public const int EnemySortingOrder = 0;
    public const int FloraSortingOrder = 0;
    public const int SolidTileSortingOrder = 0;
    public const int WallTileSortingOrder = 0;
    public const int WorldObjSortingOrder = 0; //Crucible, Chests, Stuff Like That





    //-----------------------------------
    //          DEFAULT LAYER (not yet integrated)
    //-----------------------------------
    public const int WeaponSortingOrder = 4;
    public const int Projectile = 5;
    public const int ShadowSortingOrder = -50;
    public const int InWorldUIElement = 10; //Boss/Skull health bars
}
