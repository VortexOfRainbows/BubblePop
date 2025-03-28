using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameObject Orbs;
    public GameObject BG;
    public GameObject Bubblemancer;
    public GameObject Ray1, Ray2, Ray3;
    public GameObject Frontlight, Backlight;
    public GameObject Title;
    private float Counter;
    public void Start()
    {

    }
    public void FixedUpdate()
    {
        Counter += Time.fixedDeltaTime;
        
        float sinusoid = Mathf.Sin(Counter * Mathf.PI / 2f);
        float sinusoid2 = Mathf.Sin(Counter * Mathf.PI / 9f);
        float sinusoid3 = Mathf.Sin(Counter * Mathf.PI / 3f);
        float angle = sinusoid2 * 3f;
        Title.transform.localScale = Vector3.one * (0.99f + 0.03f * sinusoid);
        Title.transform.localEulerAngles = new Vector3(0, 0, angle);
        Orbs.transform.localScale = Vector3.one * (0.99f + 0.05f * sinusoid2);
        RotateRay(ref Ray1);
        RotateRay(ref Ray2);
        RotateRay(ref Ray3);

        Bubblemancer.transform.localScale = new Vector3(1 - 0.02f * sinusoid3, 1 + 0.04f * sinusoid3);
    }
    private void RotateRay(ref GameObject ray, float rayRotateSpeed = 0.1f)
    {
        ray.transform.localEulerAngles = new Vector3(0, 0, ray.transform.localEulerAngles.z + rayRotateSpeed);
        ray.transform.position = ((Vector2)ray.transform.position).RotatedBy(Mathf.Deg2Rad * rayRotateSpeed, Title.transform.position);
    }
}
