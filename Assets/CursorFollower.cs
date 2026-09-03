using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    public Canvas parentCanvas;
    public RectTransform rect;
    public void Update()
    {
        //rect.transform.position = Input.mousePosition;
    }
    public void LateUpdate()
    {
        transform.position = Input.mousePosition;
    }
}
