using System.Runtime.Serialization;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class BuffAnchorLayoutGroup : MonoBehaviour
{
    public SpriteRenderer background;
    public void UpdateLayout()
    {
        float totalWidth = 0f;
        float childWidth = 1f;
        for (int i = 1; i < transform.childCount; ++i){
            BuffIcon icon = transform.GetChild(i).GetComponent<BuffIcon>();
            if (icon == null || !icon.Visual.gameObject.activeSelf)
                continue;
            totalWidth += childWidth;
        }
        float startX = -totalWidth / 2f;
        for (int i = 1; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            BuffIcon icon = child.GetComponent<BuffIcon>();
            if (icon == null || !icon.Visual.gameObject.activeSelf)
                continue;
            child.localPosition = new Vector2(startX + childWidth / 2f, 0f);
            startX += childWidth;
        }
        background.enabled = totalWidth != 0;
        background.size = new Vector2(totalWidth, background.size.y);
    }
//#if UNITY_EDITOR
//    public void Update() => UpdateLayout();
//#endif
}
