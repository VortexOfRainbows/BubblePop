using UnityEngine;
using UnityEngine.UI;

public class PlayerHeartUI : MonoBehaviour
{
    public float BobbingOffsetDegrees;
    public Image Outline;
    public Image Fill;
    public Image Shadow;
    public GameObject Visual;
    private bool empty = false;
    private float bobbingAmt = 6;
    public void Update()
    {
        float elapsedTime = Time.time * Mathf.Deg2Rad * 40;
        Visual.transform.localPosition = new Vector3(0, bobbingAmt * Mathf.Sin(elapsedTime + BobbingOffsetDegrees * Mathf.Deg2Rad), 0);
    }
    public void FixedUpdate()
    {
        if(empty)
        {
            Visual.transform.LerpLocalScale(new Vector2(0.9f, 0.9f), 0.1f);
            bobbingAmt = Mathf.Lerp(bobbingAmt, 2, 0.04f);
        }
        else
        {
            Visual.transform.LerpLocalScale(Vector2.one, 0.1f);
            bobbingAmt = Mathf.Lerp(bobbingAmt, 6, 0.1f);
        }
    }
    public void Empty()
    {
        Fill.enabled = false;
        empty = true;
    }
    public void Filled()
    {
        Fill.enabled = true;
        empty = false;
    }
}
