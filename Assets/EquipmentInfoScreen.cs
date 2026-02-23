using System.Collections.Generic;
using UnityEngine;

public class EquipmentInfoScreen : MonoBehaviour
{
    public EquipmentUIElement MyElem;
    public PowerUpLayout Layout;
    public void SetUIElement(Equipment e)
    {
        if (MyElem.ActiveEquipment != null && MyElem.ActiveEquipment.IndexInAllEquipPool == e.IndexInAllEquipPool)
            return;
        if (MyElem.ActiveEquipment != null)
            Destroy(MyElem.ActiveEquipment.gameObject);
        MyElem.UpdateEquipment(e);
        foreach (SpriteRenderer s in MyElem.ActiveEquipment.GetComponentsInChildren<SpriteRenderer>())
            s.maskInteraction = SpriteMaskInteraction.None;
        List<PowerUp> powers = MyElem.ActiveEquipment.GetPowerPoolForDisplay();
        Layout.GenerateSingle(powers);
    }
}
