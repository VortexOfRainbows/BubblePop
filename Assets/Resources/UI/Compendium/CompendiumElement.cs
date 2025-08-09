using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CompendiumElement : MonoBehaviour
{
    public RectTransform rectTransform;
    public Image BG;
    public int TypeID = 0;
    public bool GrayOut { get; private set; } = false;
    public virtual void Init(int i, Canvas canvas)
    {

    }
    public virtual CompendiumElement Instantiate(TierList parent, TierCategory cat, Canvas canvas, int i, int position)
    {
        return null;
    }
    public virtual void SetHovering(bool canHover)
    {

    }
    public virtual void SetGrayOut(bool value)
    {
        GrayOut = value;
    }
    public virtual bool IsLocked()
    {
        return false;
    }
    public virtual int GetCount()
    {
        return 0;
    }
    public virtual int GetRare()
    {
        return 1;
    }
}
