using UnityEngine;
using TMPro;

public class EquipmentUIElement : MonoBehaviour
{
    private Color actualColor = default;
    public GameObject Visual;
    public Equipment ActiveEquipment;
    public TextMeshPro Text;
    public GameObject PriceVisual;
    public GameObject Self => gameObject;
    public Vector3 targetScale = Vector3.one;
    public bool Unlocked => ActiveEquipment.IsUnlocked || DisplayOnly;
    private bool prevUnlockStatus = true;
    public bool DisplayOnly = false;
    public void UpdateOrientation()
    {
        Vector2 offset = Vector2.zero;
        float rot = 0f;
        float scale = 1f;
        ActiveEquipment.ModifyUIOffsets(false, ref offset, ref rot, ref scale);
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
    public void UpdateActive(Canvas canvas, out bool hovering, out bool clicked)
    {
        hovering = clicked = false;
        UpdateUnlockRelated();
        UpdateOrientation();

        int price = ActiveEquipment.GetPrice();
        Text.text = price.ToString();
        PriceVisual.SetActive(price != 0);

        if (Utils.IsMouseHoveringOverThis(true, Self.GetComponent<RectTransform>(), 50, canvas))
        {
            PopUpTextUI.Enable(ActiveEquipment.GetName(), ActiveEquipment.GetDescription());
            float scaleUp = 1.1f;
            if (!DisplayOnly)
            {
                clicked = Input.GetMouseButtonDown(0);
                scaleUp = 1.2f;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale * scaleUp, 0.15f);
            hovering = true;
        }
        else
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale * 1.0f, 0.1f);
    }
}
