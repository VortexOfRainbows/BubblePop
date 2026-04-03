using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTester : MonoBehaviour
{
    [SerializeField] GameObject world;
    [SerializeField] GameObject main;
    void Awake()
    {
        world.GetComponent<World>().nodes[0] = transform.GetChild(0);
        world.SetActive(true);
        main.SetActive(true);
    }
}
