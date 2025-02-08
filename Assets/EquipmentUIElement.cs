using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public class EquipmentUIElement : MonoBehaviour
{
    public GameObject Visual;
    public Equipment ActiveEquipment;
    public GameObject Self => gameObject;
    public int ActiveEquipmentIndex = 0;
    public int ParentEquipSlot = -1;
    public Vector3 targetScale = Vector3.one;
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
    public bool UpdateActive(Canvas canvas)
    {
        UpdateOrientation();
        if (Utils.IsMouseHoveringOverThis(true, Self.GetComponent<RectTransform>(), 50, canvas))
        {
            PopUpTextUI.Enable(ActiveEquipment.Name(), ActiveEquipment.Description());
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale * 1.2f, 0.15f);
            return Input.GetMouseButtonDown(0);
        }
        else
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 0.15f);
        return false;
    }
}
