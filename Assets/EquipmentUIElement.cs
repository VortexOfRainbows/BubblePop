using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUIElement : MonoBehaviour
{
    public GameObject Visual;
    public Equipment ActiveEquipment;
    public GameObject Self => gameObject;
    public void UpdateActive(Canvas canvas)
    {
        //Debug.Log(e.UIVisualOffset);
        Vector2 offset = Vector2.zero;
        float rot = 0f;
        float scale = 1f;
        ActiveEquipment.ModifyUIOffsets(ref offset, ref rot, ref scale);
        ActiveEquipment.transform.localPosition = offset;
        ActiveEquipment.transform.eulerAngles = new Vector3(0, 0, rot);
        Visual.transform.localScale = Vector3.one * 50f * scale;
        bool hovering = false;
        if (Utils.IsMouseHoveringOverThis(true, Self.GetComponent<RectTransform>(), 50, canvas))
        {
            PopUpTextUI.Enable(ActiveEquipment.Name(), ActiveEquipment.Description());
            hovering = true;
        }
        if (hovering)
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 1.2f, 0.15f);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.15f);
    }
}
