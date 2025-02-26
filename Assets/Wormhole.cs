using System.Collections.Generic;
using UnityEngine;

public class Wormhole : MonoBehaviour
{
    public GameObject SelfPrefab;
    public Material glow;
    private List<GameObject> segments;
    private List<GameObject> segments2;
    private float timer;
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 0);
        FixedUpdate();   
    }
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (segments == null || Input.GetMouseButtonDown(1))
        {
            transform.localScale = new Vector3(0, 0, 0);
            timer = 0;
            if (segments != null)
            {
                foreach (GameObject obj in segments)
                    Destroy(obj);
                foreach (GameObject obj in segments2)
                    Destroy(obj);
            }
            segments = new List<GameObject>();
            segments2 = new List<GameObject>();
            for (int i = 0; i < 24; i++)
            {
                segments.Add(Instantiate(SelfPrefab, transform));
                segments[i].transform.localScale = Vector3.one * 2;
                segments[i].transform.eulerAngles = new Vector3(0, 0, i * 15);
                segments[i].GetComponent<SpriteRenderer>().color = new Color(1f, .15f, .1f) * .5f;
                segments[i].GetComponent<SpriteRenderer>().flipX = true;
                segments[i].GetComponent<SpriteRenderer>().sortingOrder = -10;
                segments[i].GetComponent<SpriteRenderer>().material = glow;
                segments[i].SetActive(true);
            }
            for (int i = 0; i < 24; i++)
            {
                segments2.Add(Instantiate(SelfPrefab, transform));
                segments2[i].transform.localScale = Vector3.one * 2;
                segments2[i].transform.eulerAngles = new Vector3(0, 0, i * 15 + 180);
                segments2[i].GetComponent<SpriteRenderer>().color = new Color(1f, .15f, .1f) * .5f;
                segments2[i].GetComponent<SpriteRenderer>().flipX = false;
                segments2[i].GetComponent<SpriteRenderer>().sortingOrder = -10;
                segments2[i].GetComponent<SpriteRenderer>().material = glow;
                segments2[i].SetActive(true);
            }
        }
        if (segments != null)
        {
            for (int i = 0; i < 24; i++)
            {
                segments[i].transform.eulerAngles = new Vector3(0, 0, segments[i].transform.eulerAngles.z + Time.deltaTime * 40 * -1);
                segments[i].transform.localScale = Vector3.one;
                segments2[i].transform.eulerAngles = new Vector3(0, 0, segments2[i].transform.eulerAngles.z + Time.deltaTime * 40 * 1);
                segments2[i].transform.localScale = Vector3.one;
            }
        }
        if (timer < 1.5f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.05f);
        }
        //else if(timer > 1.5f)
        //{
        //    if (transform.localScale.x < 0)
        //        return;
        //    transform.localScale = transform.localScale * 0.97f - Vector3.one * 0.03f;
        //}
    }
}
