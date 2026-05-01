using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public static class LayerHelper
{
    //This class mostly exists as a reminder for which sorting orders are populated by which assets

    //-----------------------------------
    //          DEFAULT LAYER
    //-----------------------------------
    public const int PlayerSortingOrder = 0;
    public const int EnemySortingOrder = 0;
    public const int FloraSortingOrder = 0;
    public const int SolidTileSortingOrder = 0;
    public const int WallTileSortingOrder = 0;
    public const int FloorTileSortingOrder = -1;



    //-----------------------------------
    //          DEFAULT LAYER (not yet integrated)
    //-----------------------------------
    public const int WeaponSortingOrder = 4;
    public const int ShadowSortingOrder = -50;
}
