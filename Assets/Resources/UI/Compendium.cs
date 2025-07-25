using UnityEngine;

public class Compendium : MonoBehaviour
{
    public Canvas MyCanvas;
    public Transform ContentLayout;
    private bool Active = false;
    private bool HasInit = false;
    public void Start()
    {
        //UIManager.ActivePrimaryCanvas = MyCanvas;
    }
    public void ToggleActive()
    {
        ToggleActive(!Active);
    }
    public void ToggleActive(bool on)
    {
        Active = on;
    }
    public void Init()
    {
        for(int i = 0; i < PowerUp.Reverses.Count; ++i)
        {
            CompendiumPowerUpElement CPUE = Instantiate(CompendiumPowerUpElement.Prefab, ContentLayout, false).GetComponent<CompendiumPowerUpElement>();
            CPUE.Init(i, MyCanvas);
        }
    }
    public void FixedUpdate()
    {
        if (Active)
        {
            if (!HasInit)
            {
                Init();
                HasInit = true;
            }
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
