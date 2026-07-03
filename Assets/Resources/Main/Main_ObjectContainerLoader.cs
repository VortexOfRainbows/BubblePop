using UnityEngine;

public partial class Main : MonoBehaviour
{
    public static Transform GenericSuperParent { get; private set; } = null;
    public static void ResetContainers()
    {
        if(GenericSuperParent != null)
            Destroy(GenericSuperParent.gameObject);
        GenericSuperParent = new GameObject("UniversalContainer").transform;
    }
}