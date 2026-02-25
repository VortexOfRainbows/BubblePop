using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentInfoScreen : MonoBehaviour
{
    public EquipmentUIElement MyElem;
    public PowerUpLayout Layout;
    public TextMeshProUGUI Title;
    public List<RectTransform> Rects = new();
    public void SetUIElement(Equipment e)
    {
        if (MyElem.ActiveEquipment != null && MyElem.ActiveEquipment.IndexInAllEquipPool == e.IndexInAllEquipPool)
            return;
        for(int i = 0; i < Rects.Count; ++i)
            if (i >= 3)
                Destroy(Rects[i].gameObject);
        Rects.Clear();
        if (MyElem.ActiveEquipment != null)
            Destroy(MyElem.ActiveEquipment.gameObject);
        MyElem.UpdateEquipment(e);
        foreach (SpriteRenderer s in MyElem.ActiveEquipment.GetComponentsInChildren<SpriteRenderer>())
            s.maskInteraction = SpriteMaskInteraction.None;
        List<PowerUp> powers = MyElem.ActiveEquipment.GetPowerPoolForDisplay();
        Layout.GenerateSingle(powers);
        Title.text = e.GetName();
        Rects.Add(Title.rectTransform);
        Rects.Add(MyElem.GetComponent<RectTransform>());
        Rects.Add(Layout.GetComponent<RectTransform>());
        foreach(Ability a in e.GetAbility())
            Rects.Add(a.CreateAbilityBlurb(transform, Layout.myCanvas).GetComponent<RectTransform>());
        TightenRectangleSpacing();
    }
    public void TightenRectangleSpacing()
    {
        float verticalPadding = 15;
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
        if(MyElem.ActiveEquipment != null)
            MyElem.UpdateActive(canvas, out bool hovering, out bool clicked, MyElem.GetComponent<RectTransform>());
    }
}
