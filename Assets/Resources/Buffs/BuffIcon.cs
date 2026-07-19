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
    public Buff Buff { get; private set; }
    public Entity owner;
    public SpriteRenderer Render;
    public TextMeshPro Text;
    public SpriteRenderer Visual;
    public bool UpdateBuff(int i)
    {
        Visual.gameObject.SetActive(true);
        int stack = 0;
        if (Buff != null)
            stack = Buff.Stacks;
        Text.gameObject.SetActive(stack > 1);
        Text.text = stack.ToString();
        if (stack <= 0 || !Buff.Active)
        {
            Destroy(gameObject);
            return false;
        }
        return true;
    }
    public void SetBuff(Buff buff)
    {
        Buff = buff;
        Render.sprite = buff.GetSprite();
    }
}
