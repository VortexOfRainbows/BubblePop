using Unity.VisualScripting;
using UnityEngine;

public class EquipmentInfoScreen : MonoBehaviour
{
    public EquipmentUIElement MyElem;
    public void SetUIElement(Equipment e)
    {
        if (MyElem.ActiveEquipment != null)
            Destroy(MyElem.ActiveEquipment.gameObject);
        MyElem.UpdateEquipment(e);
        foreach (SpriteRenderer s in MyElem.ActiveEquipment.GetComponentsInChildren<SpriteRenderer>())
            s.maskInteraction = SpriteMaskInteraction.None;
    }
}
