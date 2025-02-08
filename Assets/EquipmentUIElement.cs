using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class EquipmentUIElement : MonoBehaviour
{
    private Color actualColor = default;
    public GameObject Visual;
    public Equipment ActiveEquipment;
    public GameObject Self => gameObject;
    public int ActiveEquipmentIndex = 0;
    public int ParentEquipSlot = -1;
    public Vector3 targetScale = Vector3.one;
    public bool Unlocked => ActiveEquipment.UnlockCondition.IsUnlocked;
    private bool prevUnlockStatus = true;
    public void UpdateOrientation()
    {
        Vector2 offset = Vector2.zero;
        float rot = 0f;
        float scale = 1f;
        ActiveEquipment.ModifyUIOffsets(ref offset, ref rot, ref scale);
        ActiveEquipment.transform.localPosition = offset;
        ActiveEquipment.transform.eulerAngles = new Vector3(0, 0, rot);
        Visual.transform.localScale = Vector3.one * 50f * scale;
    }
    private void UpdateUnlockRelated()
    {
        bool currentUnlockStatus = Unlocked;
        if (actualColor == default)
            actualColor = ActiveEquipment.spriteRender.color;
        if(prevUnlockStatus != currentUnlockStatus)
        {
            if (currentUnlockStatus)
            {
                ActiveEquipment.spriteRender.color = actualColor;
                foreach (SpriteRenderer s in ActiveEquipment.GetComponentsInChildren<SpriteRenderer>())
                    s.color = actualColor;
            }
            else
            {
                ActiveEquipment.spriteRender.color = Color.black;
                foreach (SpriteRenderer s in ActiveEquipment.GetComponentsInChildren<SpriteRenderer>())
                    s.color = Color.black;
            }
        }
        prevUnlockStatus = currentUnlockStatus;
    }
    public bool UpdateActive(Canvas canvas)
    {
        UpdateUnlockRelated();
        UpdateOrientation();
        if (Utils.IsMouseHoveringOverThis(true, Self.GetComponent<RectTransform>(), 50, canvas))
        {
            PopUpTextUI.Enable(ActiveEquipment.GetName(), ActiveEquipment.GetDescription());
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale * 1.2f, 0.15f);
            return Input.GetMouseButtonDown(0);
        }
        else
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.15f);
        return false;
    }
}
