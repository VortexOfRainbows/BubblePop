using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTester : MonoBehaviour
{
    [SerializeField] GameObject world;
    void Awake()
    {
        world.GetComponent<World>().nodes[0] = transform.GetChild(0);
    }
}
