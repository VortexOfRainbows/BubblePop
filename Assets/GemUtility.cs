using System.Collections.Generic;
using UnityEngine;

public class GemUtility : MonoBehaviour
{
    public class GemAnimationVisual
    {
        public GemAnimationVisual(float start)
        {
            startTime = start;
            endDuration = 0;
            startPosition = Vector2.zero;
        }
        public float startTime;
        public float endDuration;
        public Vector2 startPosition;
    }
    public void AddGem(float time)
    {
        GemAnimateList.Add(new GemAnimationVisual(time));
    }
    public List<GemAnimationVisual> GemAnimateList = new();
    public virtual float SpeedMult => 1.0f;
    public void AnimateGems(Vector2 destination, int minLayer = -12, int maxLayer = -12)
    {
        if (GemAnimateList.Count > 0)
        {
            Material defaultMat = Main.TextureAssets.SpriteLit;
            Vector2 finalPos = destination;
            Sprite s = Resources.Load<Sprite>("Money/Gem");
            finalPos.y += 0.4f;
            for (int i = GemAnimateList.Count - 1; i >= 0; --i)
            {
                if (GemAnimateList[i] == null)
                {
                    GemAnimateList.RemoveAt(i);
                    continue;
                }
                GemAnimateList[i].startTime -= Time.deltaTime * SpeedMult;
                if (GemAnimateList[i].startTime <= 0)
                {
                    if (GemAnimateList[i].endDuration == 0)
                    {
                        GemAnimateList[i].startPosition = Player.FindClosest(destination, out Vector2 _, out float _).Position;
                        AudioManager.PlaySound(SoundID.CoinPickup, GemAnimateList[i].startPosition, 1, 1, 0);
                    }
                    GemAnimateList[i].endDuration += Time.deltaTime * 2 * SpeedMult;
                    float percent = GemAnimateList[i].endDuration;
                    Vector2 position = Vector2.Lerp(GemAnimateList[i].startPosition, finalPos, percent);
                    float sin = Mathf.Sin(percent * Mathf.PI);
                    sin *= sin;
                    position.y += sin * 1.6f;
                    int layer = minLayer;
                    if(minLayer != maxLayer && percent > 0.4f)
                        layer = maxLayer;
                    SpriteBatch.Draw(s, position, (0.5f + 0.5f * sin) * 0.5f * Vector2.one, 0, Color.white.WithAlpha(0.4f + 0.6f * sin), layer, defaultMat);
                    if (percent > 1)
                    {
                        OnGemRemoval();
                        GemAnimateList.RemoveAt(i);
                    }
                }
            }
        }
    }
    public virtual void OnGemRemoval()
    {

    }
}
