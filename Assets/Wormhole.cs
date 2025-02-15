using System.Collections.Generic;
using UnityEngine;

public class Wormhole : MonoBehaviour
{
    public GameObject SelfPrefab;
    public Material glow;
    private List<GameObject> segments;
    private List<GameObject> segments2;
    void Start()
    {
        
    }

    void Update()
    {
        if (segments == null || Input.GetMouseButtonDown(1))
        {
            if (segments != null)
            {
                foreach (GameObject obj in segments)
                    Destroy(obj);
                foreach (GameObject obj in segments2)
                    Destroy(obj);
            }
            segments = new List<GameObject>();
            segments2 = new List<GameObject>();
            for (int i = 0; i < 12; i++)
            {
                segments.Add(Instantiate(SelfPrefab, transform));
                segments[i].transform.localScale = Vector3.one * (1 + i / 12f);
                segments[i].transform.eulerAngles = new Vector3(0, 0, i * 30);
                segments[i].GetComponent<SpriteRenderer>().color *= 0.4f;
                segments[i].GetComponent<SpriteRenderer>().flipX = false;
                segments[i].SetActive(true);
            }
            for (int i = 0; i < 12; i++)
            {
                segments2.Add(Instantiate(SelfPrefab, transform));
                segments2[i].transform.localScale = Vector3.one * (1 + i / 12f);
                segments2[i].transform.eulerAngles = new Vector3(0, 0, i * 30 + 180);
                segments2[i].GetComponent<SpriteRenderer>().color = new Color(0.7f, .3f, .9f) * 0.5f;
                segments2[i].GetComponent<SpriteRenderer>().flipX = false;
                segments2[i].GetComponent<SpriteRenderer>().sortingOrder = -10;
                segments2[i].GetComponent<SpriteRenderer>().material = glow;
                segments2[i].SetActive(true);
            }
        }
        if(segments != null)
            for (int i = 0; i < 12; i++)
            {
                segments[i].transform.eulerAngles = new Vector3(0, 0, segments[i].transform.eulerAngles.z + Time.deltaTime * 40 * (1 + i / 4f));
            }
        if (segments2 != null)
            for (int i = 0; i < 12; i++)
            {
                segments2[i].transform.eulerAngles = new Vector3(0, 0, segments2[i].transform.eulerAngles.z + Time.deltaTime * 40 * (1 + i / 4f));
            }
    }
}
