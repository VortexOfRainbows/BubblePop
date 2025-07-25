using UnityEngine;

public class Compendium : MonoBehaviour
{
    public bool Active = false;
    public void ToggleActive()
    {
        ToggleActive(!Active);
    }
    public void ToggleActive(bool on)
    {
        Active = on;
    }
    public void FixedUpdate()
    {
        if (Active)
        {
            transform.position = transform.position.Lerp(new Vector3(0, 0, 0), 0.1f);
            if (transform.position.x > -0.5f)
                transform.position = Vector3.zero;
        }
        else
        {
            transform.position = transform.position.Lerp(new Vector3(-1920, 0, 0), 0.1f);
            if (transform.position.x < -1919.5f)
                transform.position = new Vector3(-1920, 0, 0);
        }
    }
}
