using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public static GameObject Prefab => Resources.Load<GameObject>("NPCs/Champion/SkullHealthBar");
    public SpriteRenderer Inner, Highlight, Skull, PointerSkull;
    public Enemy Host;
    public static BossHealthBar Spawn(Enemy host)
    {
        BossHealthBar e = Instantiate(Prefab, host.transform).GetComponent<BossHealthBar>();
        e.transform.localPosition = new Vector3(0, -1.5f + host.HealthBarOffset);
        e.Host = host;
        e.Inner.color = e.Inner.color.WithAlpha(0);
        e.Highlight.color = e.Highlight.color.WithAlpha(0);
        e.Skull.color = e.Skull.color.WithAlpha(0);
        e.PointerSkull.color = e.PointerSkull.color.WithAlpha(0);
        e.IntendedSize = Mathf.Min(3.5f, host.GetComponent<BoxCollider2D>().size.x * host.transform.localScale.x * 0.5f + 0.55f);
        return e;
    }
    public void Update()
    {
        if(Host != null)
        {
            UpdateSize();
            SetAlpha();
        }
        if (!WaveDirector.WaveActive)
        {
            UpdatePosition();
        }
        else
            Skull.transform.localPosition = Vector3.zero;
    }
    public float IntendedSize { get; set; }
    public float Alpha { get; set; } = -0.7f;
    public float PointerSkullAlpha { get; set; } = -0.5f;
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
    public void UpdatePosition()
    {
        Vector2 screenSize = Main.ActivePrimaryCanvas.pixelRect.size;
        Vector2 middle = screenSize / 2;
        Camera main = Camera.main;
        Vector2 myPos = main.WorldToScreenPoint(transform.position);

        float PaddingAllowed = 30;
        float targetDistanceX = screenSize.x / 2 - PaddingAllowed;
        float targetDistanceY = screenSize.y / 2 - PaddingAllowed;
        Vector2 toMyPos = myPos - middle;
        float currentDistanceX = Mathf.Abs(toMyPos.x);
        float currentDistanceY = Mathf.Abs(toMyPos.y);
        float ratioX = currentDistanceX == 0 ? 1 : targetDistanceX / currentDistanceX;
        float ratioY = currentDistanceY == 0 ? 1 : targetDistanceY / currentDistanceY;

        Vector2 relativePosition = toMyPos;
        if (ratioX < 1 && ratioY < 1)
        {
            if(ratioX < ratioY)
                relativePosition *= ratioX;
            else
                relativePosition *= ratioY;
        }
        else if (ratioX < 1)
            relativePosition *= ratioX;
        else if (ratioY < 1)
            relativePosition *= ratioY;

        myPos = main.ScreenToWorldPoint(middle + relativePosition);

        PointerSkull.transform.position = new Vector3(myPos.x, myPos.y, PointerSkull.transform.position.z);

        float distanceFromPointer = PointerSkull.transform.localPosition.magnitude;
        float scaleFactor = Mathf.Clamp((distanceFromPointer - 1), 0, 1);

        if(distanceFromPointer > 1)
            PointerSkullAlpha += Time.unscaledDeltaTime * 1.5f;
        else
            PointerSkullAlpha -= Time.unscaledDeltaTime;
        PointerSkullAlpha = Mathf.Clamp(PointerSkullAlpha, -0.5f, scaleFactor);
        PointerSkull.color = PointerSkull.color.WithAlpha(Mathf.Max(PointerSkullAlpha, 0));
    }
}
