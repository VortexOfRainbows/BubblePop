using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EquipmentUIElement : MonoBehaviour
{
    public const int UILayer = 5;
    private Color actualColor = default;
    private Color slotColor = default;
    public GameObject Visual;
    public Equipment ActiveEquipment { get; private set; }
    public TextMeshPro Text;
    public GameObject PriceVisual;
    public GameObject Self => gameObject;
    public Vector3 targetScale = Vector3.one;
    public Image Slot;
    public bool Unlocked => ActiveEquipment.IsUnlocked || (DisplayOnly && !CompendiumElement);
    public bool CanAfford => true; // CoinManager.Savings >= ActiveEquipment.GetPrice() || ActiveEquipment.GetPrice() <= 0;
    public bool DisplayOnly = false;
    public bool CompendiumElement = false;
    public void UpdateEquipment(Equipment equip)
    {
        ActiveEquipment = GenerateEquipment(equip, Visual.transform);
        UpdateOrientation();
    }
    public Equipment GenerateEquipment(Equipment prefab, Transform parent)
    {
        Equipment obj = Instantiate(prefab, parent);
        obj.gameObject.layer = UILayer;
        obj.transform.localPosition = Vector3.zero;
        obj.OriginalPrefab = prefab;
        foreach (Transform t in obj.GetComponentsInChildren<Transform>())
            t.gameObject.layer = UILayer;
        return obj;
    }
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
        if(Text != null)
            Text.color = Color.white;
        if (Unlocked)
        {
            //if (CanAfford)
            //{
            //    Slot.color = slotColor;
            //}
            //else
            //{
            //    //UpdateColor(Color.Lerp(actualColor, Color.red, 0.1f));
            //    Text.color = Color.red;
            //    Slot.color = Color.Lerp(slotColor, new Color(0.9f, 0.0f, 0.0f, slotColor.a), 0.5f);
            //}
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
    public void SetCompendiumLayering(int ID, int offset, int maskBehavior = 1)
    {
        //ActiveEquipment.spriteRender.sortingLayerID = ID;
        //ActiveEquipment.spriteRender.sortingOrder += offset;
        foreach (SpriteRenderer s in ActiveEquipment.GetComponentsInChildren<SpriteRenderer>())
        {
            s.sortingLayerID = ID;
            s.sortingOrder += offset;
            s.maskInteraction = (SpriteMaskInteraction)maskBehavior;
        }
    }
    public void UpdateActive(Canvas canvas, out bool hovering, out bool clicked, RectTransform hoverArea)
    {
        if(PriceVisual != null && Text != null)
        {
            int cost = 0; // ActiveEquipment.GetPrice();
            Text.text = $"${cost}";
            PriceVisual.SetActive(cost != 0 && Unlocked);
        }
        
        hovering = clicked = false;
        UpdateUnlockRelated();
        UpdateOrientation();
        if (Utils.IsMouseHoveringOverThis(true, hoverArea, 0, canvas) && (!CompendiumElement || !DisplayOnly))
        {
            string name = Unlocked ? ActiveEquipment.GetName() : DetailedDescription.TextBoundedByRarityColor(ActiveEquipment.GetRarity() - 1, "???");
            string desc = Unlocked ? (CompendiumElement ? "" : ActiveEquipment.GetDescription()) : ActiveEquipment.GetUnlockReq();
            PopUpTextUI.Enable(name, desc);
            float scaleUp = 1.1f;
            if (!DisplayOnly)
            {
                clicked = Input.GetMouseButtonDown(0);
                scaleUp = 1.2f;
            }
            transform.LerpLocalScale(targetScale * scaleUp, 0.15f);
            hovering = true;
        }
        else
            transform.LerpLocalScale(targetScale * 1.0f, 0.1f);
    }
}
