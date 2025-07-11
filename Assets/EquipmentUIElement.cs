using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EquipmentUIElement : MonoBehaviour
{
    private Color actualColor = default;
    private Color slotColor = default;
    public GameObject Visual;
    public Equipment ActiveEquipment;
    public TextMeshPro Text;
    public GameObject PriceVisual;
    public GameObject Self => gameObject;
    public Vector3 targetScale = Vector3.one;
    public Image Slot;
    public bool Unlocked => ActiveEquipment.IsUnlocked || DisplayOnly;
    public bool CanAfford => true; // CoinManager.Savings >= ActiveEquipment.GetPrice() || ActiveEquipment.GetPrice() <= 0;
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
        if (actualColor == default)
        {
            slotColor = Slot.color;
            actualColor = ActiveEquipment.spriteRender.color;
        }
        Text.color = Color.white;
        if (Unlocked)
        {
            if (CanAfford)
            {
                Slot.color = slotColor;
            }
            else
            {
                //UpdateColor(Color.Lerp(actualColor, Color.red, 0.1f));
                Text.color = Color.red;
                Slot.color = Color.Lerp(slotColor, new Color(0.9f, 0.0f, 0.0f, slotColor.a), 0.5f);
            }
        }
        else
        {
            UpdateColor(Color.black);
        }
    }
    private void UpdateColor(Color c)
    {
        ActiveEquipment.spriteRender.color = c;
        foreach (SpriteRenderer s in ActiveEquipment.GetComponentsInChildren<SpriteRenderer>())
            s.color = c;
    }
    public void UpdateActive(Canvas canvas, out bool hovering, out bool clicked)
    {
        int cost = 0; // ActiveEquipment.GetPrice();
        Text.text = $"${cost}";
        PriceVisual.SetActive(cost != 0 && Unlocked);

        hovering = clicked = false;
        UpdateUnlockRelated();
        UpdateOrientation();


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
