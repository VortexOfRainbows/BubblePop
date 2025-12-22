using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        
    }
}
