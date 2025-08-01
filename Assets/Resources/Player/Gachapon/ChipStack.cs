using System.Collections.Generic;
using UnityEngine;

public class ChipStack : MonoBehaviour
{
    public List<GameObject> Chips;
    public Transform Transform;
    public bool IsFull()
    {
        return Chips.Count >= Player.Instance.ChipHeight;
    }
}