using System;
using TMPro;
using UnityEngine;

public class BuffIcon : MonoBehaviour
{
    public static void Load()
    {
        BuffIconParentPrefab = Resources.Load<GameObject>("Buffs/BuffVisualAnchor");
        Prefab = Resources.Load<GameObject>("Buffs/BuffIcon");
    }
    public static GameObject BuffIconParentPrefab;
    public static GameObject Prefab;
    public Type myBuff;
    public Entity owner;
    public SpriteRenderer Render;
    public TextMeshPro Text;
    public SpriteRenderer Visual;
    public int Total => owner.UniqueBuffTypes.Keys.Count;
    public bool UpdateBuff(int i)
    {
        Visual.gameObject.SetActive(true);
        if (!owner.UniqueBuffTypes.TryGetValue(myBuff, out int value))
            value = 0;
        Text.text = value.ToString();
        if (value <= 0)
        {
            Destroy(gameObject);
            return false;
        }
        return true;
    }
    public void SetBuff(Buff buff)
    {
        myBuff = buff.GetType();
        Render.sprite = buff.GetSprite();
        Visual.color = buff.BackgroundColor();
    }
}
