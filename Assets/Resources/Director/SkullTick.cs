using UnityEngine;
using UnityEngine.UI;

public class SkullTick : MonoBehaviour
{
    public RectTransform Skull;
    public float MyPercent { get; set; }
    private float SpawnInTimer = 0f;
    private float DespawnTimer = 0f;
    private float GeneralUpdateTimer = 0f;
    private float DefaultYPos = 0;
    public void Start()
    {
        Skull.transform.localScale = new Vector3(0, 0, 1);
        DefaultYPos = Skull.transform.localPosition.y;
    }
    public void UpdateSkull(float currentPercent)
    {
        if (DefaultYPos == 0)
            return;
        if (GeneralUpdateTimer < MyPercent)
                GeneralUpdateTimer = MyPercent * 2;
        if (MyPercent < currentPercent)
        {
            if(SpawnInTimer <= 0)
            {
                Skull.transform.localScale = Vector3.one * 0.3f;
            }
            else if(SpawnInTimer <= 0.8f)
            {
                SpawnInTimer /= 0.8f;
                SpawnInTimer = Mathf.Sqrt(SpawnInTimer);
                Skull.transform.localScale = Vector3.one * (0.3f + 0.5f * SpawnInTimer);
            }
            else
            {
                Skull.transform.LerpLocalScale(Vector2.one * 0.6f, Utils.DeltaTimeLerpFactor(0.1f));
            }
            SpawnInTimer += Time.unscaledDeltaTime;
        }
        else
        {
            if (DespawnTimer <= 1 && SpawnInTimer > 0)
            {
                DespawnTimer += Time.unscaledDeltaTime * 2.25f;
                float iPer = Mathf.Max(0, 1 - DespawnTimer);
                Image i = Skull.GetComponent<Image>();
                i.color = i.color.WithAlpha(Mathf.Sin(iPer * 0.5f * Mathf.PI));
                Skull.transform.LerpLocalScale(Vector3.one * (0.6f + 0.1f * Mathf.Sin(DespawnTimer * Mathf.PI)), Utils.DeltaTimeLerpFactor(0.2f));
            }
            else if(DespawnTimer > 1)
                Skull.gameObject.SetActive(false);
        }
        GeneralUpdateTimer += Time.unscaledDeltaTime;
        Skull.transform.localPosition = new Vector3(0, DefaultYPos + Mathf.Sin(GeneralUpdateTimer * Mathf.PI * 0.5f) * 3f, 0);
    }
}
