using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class Pylon : MonoBehaviour
{
    public SpriteRenderer Crystal;
    public SpriteRenderer Glow;
    public SpriteRenderer Base;
    public bool Active = false;
    public void Start()
    {
        
    }
    public void FixedUpdate()
    {
        if ((Player.Position - (Vector2)transform.position).magnitude < 8 || Main.WavesUnleashed)
            Active = true;
        else
            Active = false;
        if (Active)
            ActiveAnim();
        else
            DisableAnimation();
    }
    public float animCounter = 0;
    public void ActiveAnim()
    {
        Main.CurrentPylon = this;
        animCounter++;

        float sin = Mathf.Sin(animCounter * Mathf.Deg2Rad * 1.4f) * 0.3f;
        float lerp = 0.035f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 3 + sin, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.8f, lerp);

        if(Main.WavesUnleashed)
        {

        }
    }
    public void DisableAnimation()
    {
        float lerp = 0.045f;
        Crystal.transform.localPosition = Crystal.transform.localPosition.Lerp(new Vector3(0, 1, 1), lerp);
        Crystal.transform.localScale = Crystal.transform.localScale.Lerp(Vector3.one * 0.6f, lerp);
    }
}
