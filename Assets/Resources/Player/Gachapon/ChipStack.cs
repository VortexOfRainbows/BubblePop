using System.Collections.Generic;
using UnityEngine;

public class ChipStack : MonoBehaviour
{
    public List<GameObject> Chips;
    public Transform Transform;
    public Player owner;
    public bool IsFull()
    {
        return Chips.Count >= owner.ChipHeight;
    }
}