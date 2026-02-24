using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentInfoScreen : MonoBehaviour
{
    public EquipmentUIElement MyElem;
    public PowerUpLayout Layout;
    public TextMeshProUGUI Title, Description;
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
        Title.text = e.GetName();
        Description.text = e.GetDescription();
        Description.transform.localPosition = Layout.transform.localPosition 
            + new Vector3(0, -Layout.GetComponent<RectTransform>().rect.height * Layout.transform.localScale.x - 5, 0);
        RectTransform r = Description.transform.GetComponent<RectTransform>();
        //r.sizeDelta = new Vector2(r.sizeDelta.x, -440 + 20);
    }
    public void OnUpdate(Canvas canvas)
    {
        if(MyElem.ActiveEquipment != null)
            MyElem.UpdateActive(canvas, out bool hovering, out bool clicked, MyElem.GetComponent<RectTransform>());
    }
}
