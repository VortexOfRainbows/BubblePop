using UnityEngine;

[CreateAssetMenu(fileName = "OverlayMaterials", menuName = "ScriptableObjects/OverlayMaterials", order = 2)]
public class OverlayMaterials : ScriptableObject
{
    public Material[] Overlays;
}