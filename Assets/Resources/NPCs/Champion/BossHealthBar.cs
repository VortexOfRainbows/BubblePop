using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public static GameObject Prefab => Resources.Load<GameObject>("NPCs/Champion/SkullHealthBar");
    public SpriteRenderer Inner, Highlight, Skull;
    public Enemy Host;
    public static BossHealthBar Spawn(Enemy host)
    {
        BossHealthBar e = Instantiate(Prefab, host.transform).GetComponent<BossHealthBar>();
        e.transform.localPosition = new Vector3(0, -1.5f + host.HealthBarOffset);
        e.Host = host;
        e.Inner.color = e.Inner.color.WithAlpha(0);
        e.Highlight.color = e.Highlight.color.WithAlpha(0);
        e.Skull.color = e.Skull.color.WithAlpha(0);
        e.IntendedSize = Mathf.Min(3.5f, host.GetComponent<BoxCollider2D>().size.x * host.transform.localScale.x * 0.5f + 0.55f);
        return e;
    }
    public void Update()
    {
        UpdateSize();
        SetAlpha();
    }
    public float IntendedSize { get; set; }
    public float Alpha { get; set; } = -0.7f;
    public void SetAlpha()
    {
        if (Alpha < 1)
        {
            Alpha += Time.deltaTime * 1f;
            if (Alpha > 1)
                Alpha = 1;
            if(Alpha > 0)
            {
                Inner.color = Inner.color.WithAlpha(Alpha);
                Highlight.color = Highlight.color.WithAlpha(Alpha);
                Skull.color = Skull.color.WithAlpha(Alpha);
            }
        }
    }
    public void UpdateSize()
    {
        float percent = Host.Life / Host.MaxLife;
        if(percent <= 0)
        {
            Inner.gameObject.SetActive(false);
            return;
        }
        Vector2 size = new(IntendedSize * percent + 0.55f, 0.4f);
        Inner.size = size;
        Highlight.size = new Vector2(size.x - 0.1f, size.y - 0.15f);
    }
}
