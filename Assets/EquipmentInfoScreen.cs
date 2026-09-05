using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentInfoScreen : MonoBehaviour
{
    public EquipmentUIElement MyElem;
    public PowerUpLayout Layout;
    public TextMeshProUGUI Title;
    public List<RectTransform> Rects { get; private set; } = new();
    public void SetUIElement(Equipment e, float verticalPadding = 15)
    {
        if (MyElem != null && MyElem.ActiveEquipment != null && MyElem.ActiveEquipment.IndexInAllEquipPool == e.IndexInAllEquipPool)
            return;
        int totalInherentEements = 1 + (MyElem != null ? 1 : 0) + (Title != null ? 1 : 0);
        for(int i = 0; i < Rects.Count; ++i)
            if (i >= totalInherentEements)
                Destroy(Rects[i].gameObject);
        Rects.Clear();
        if (MyElem != null)
        {
            if (MyElem.ActiveEquipment != null)
                Destroy(MyElem.ActiveEquipment.gameObject);
            MyElem.UpdateEquipment(e);
            foreach (SpriteRenderer s in MyElem.ActiveEquipment.GetComponentsInChildren<SpriteRenderer>())
                s.maskInteraction = SpriteMaskInteraction.None;
        }
        List<PowerUp> powers = e.GetPowerPoolForDisplay();
        Layout.GenerateSingle(powers);
        if(Title != null)
        {
            Title.text = e.GetName();
            Rects.Add(Title.rectTransform);
        }
        if(MyElem != null)
        {
            Rects.Add(MyElem.GetComponent<RectTransform>());
        }
        Rects.Add(Layout.GetComponent<RectTransform>());
        foreach(Ability a in e.GetAbility())
            Rects.Add(a.CreateAbilityBlurb(transform, Layout.myCanvas).GetComponent<RectTransform>());
        TightenRectangleSpacing(verticalPadding);
    }
    public void TightenRectangleSpacing(float verticalPadding)
    {
        RectTransform r = transform.GetComponent<RectTransform>();
        float top = r.rect.yMax;
        float bot = top - verticalPadding;
        for (int i = 0; i < Rects.Count; ++i)
        {
            RectTransform current = Rects[i];
            float height = current.rect.height * current.transform.localScale.x;
            current.localPosition = new Vector2(current.localPosition.x, bot - height * (1 - current.pivot.y));
            bot -= height + verticalPadding;
        }
        r.sizeDelta = new Vector2(r.sizeDelta.x, top - bot + verticalPadding / 2);
    }
    public void OnUpdate(Canvas canvas)
    {
        if(MyElem != null && MyElem.ActiveEquipment != null)
            MyElem.UpdateActive(canvas, out bool hovering, out bool clicked, MyElem.GetComponent<RectTransform>());
    }
}
