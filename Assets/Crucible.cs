using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Crucible : MonoBehaviour
{
    public Transform Connector1, Joint1, Connector2, Joint2;

    public Transform Connector3, Joint3;

    public Transform HandConnector1L, HandJoint1L, HandConnector2L, HandJoint2L, 
        HandConnector1R, HandJoint1R, HandConnector2R, HandJoint2R;

    public Transform Spiral;

    public PowerUpObject HeldPower;
    public void Start()
    {
        foreach(SpriteRenderer r in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            r.sortingOrder = -6;
        }
    }
    public void ConnectArms()
    {
        JustRotate(Joint3, Joint2);

        Connect(Connector1, Joint1);
        Connector2.position = new Vector3(Joint1.position.x, Joint1.position.y, Connector2.position.z);
        Connect(Connector2, Joint2);
        Connector3.position = new Vector3(Joint2.position.x, Joint2.position.y, Connector3.position.z);
        Connect(Connector3, Joint3);

        HandConnector1L.position = new Vector3(Joint3.position.x, Joint3.position.y, HandConnector1L.position.z);
        HandConnector1R.position = new Vector3(Joint3.position.x, Joint3.position.y, HandConnector1R.position.z);
        Connect(HandConnector1L, HandJoint1L);
        Connect(HandConnector1R, HandJoint1R);
        HandConnector2L.position = new Vector3(HandJoint1L.position.x, HandJoint1L.position.y, HandConnector2L.position.z);
        HandConnector2R.position = new Vector3(HandJoint1R.position.x, HandJoint1R.position.y, HandConnector2R.position.z);
        Connect(HandConnector2L, HandJoint2L);
        Connect(HandConnector2R, HandJoint2R);
    }
    float rotateCounter;
    public void SpinHands(Transform j1, Transform j2, float dirMult = 1)
    {
        float r = rotateCounter * Mathf.Deg2Rad * 90;
        Vector2 rotator = new Vector2(-0.275f * dirMult, 0).RotatedBy(r);
        rotator.y *= 0.0f;
        rotator.y -= 0.2f;
        j1.localPosition = rotator;

        rotator = new Vector2(-0.275f * dirMult, 0).RotatedBy(r);
        rotator.x *= 0.2f / 0.275f;
        rotator.y *= 0.0f;
        rotator.y -= 0.5f;
        j2.localPosition = rotator;
    }
    public void Connect(Transform start, Transform end, float lengthMult = 3.6f)
    {
        Vector2 ConnectorToJoint = end.localPosition - start.localPosition;
        float dist = ConnectorToJoint.magnitude;
        float r = ConnectorToJoint.ToRotation();
        start.SetEulerZ(r * Mathf.Rad2Deg + 90);
        start.localScale = new Vector3(start.localScale.x, dist * lengthMult, 1);
    }
    public  void JustRotate(Transform start, Transform end)
    {
        Vector2 ConnectorToJoint = end.position - start.position;
        float r = ConnectorToJoint.ToRotation();
        start.SetEulerZ(r * Mathf.Rad2Deg - 90);
    }
    public float Counter = 0;
    public float Counter2 = 0;
    public bool Active = false;
    public void FixedUpdate() //change this to fixed update
    {
        if (Input.GetKey(KeyCode.E) || Active)
        {
            Active = true;
            Counter += Time.fixedDeltaTime * 2;
        }
        else Counter = 0;
        Counter2++;
        float percent = Counter;
        float per1 = Mathf.Clamp(percent, 0, 1);
        float per2 = Mathf.Clamp((percent - 0.7f) * 2, 0, 1);
        float per3 = Mathf.Clamp((percent - 1.2f), 0, 1);
        float per4 = Mathf.Clamp((percent - 2.2f), 0, 1);
        float per5 = Mathf.Clamp((percent - 2.7f), 0, 4.5f) / 4.5f;
        float per6 = Mathf.Clamp((percent - 7.2f), 0, 2) / 2f;
        per4 = per4 * Mathf.Sin(per4 * Mathf.PI / 2f);
        float sin = 0.5f - 0.5f * Mathf.Sin((per1 - per3) * Mathf.PI + Mathf.PI * 0.5f);
        if (per1 < 1 && Active)
        {
            HeldPower.gameObject.SetActive(true);
            rotateCounter = 2 * sin;
        }
        else if(!Active)
        {
            rotateCounter = Mathf.LerpAngle(rotateCounter * 180, 0, 0.1f) / 180;
        }

        if (per5 >= 1)
        {
            per5 *= 1 - per6;
            per4 *= 1 - per6;
            if(per6 >= 1)
            {
                FinishConsuming();
                Counter = 0;
                Active = false;
            }
            HeldPower.gameObject.SetActive(false);
        }
        if (per4 >= 0.5f || per6 > 0)
        {
            rotateCounter += Time.deltaTime * per5 * 25f;
        }

        Vector2 armRotation = new Vector2(0, -0.35f - 0.2f * per1 + 0.2f * per2).RotatedBy(sin * 45 * Mathf.Deg2Rad);
        float down = per4 * 0.45f + per5 * 0.55f;
        armRotation.y -= down * 1.05f;

        Joint3.localPosition = armRotation;

        Joint1.transform.localPosition = Vector3.Lerp(new Vector3(-1.35f, 1.1f, -1), new Vector3(-1.45f, 1.0f, -1), sin).Lerp(new Vector3(-1.5f, 0.75f, -1), down);
        Joint2.transform.localPosition = Vector3.Lerp(new Vector3(-0, 2f, 0), new Vector3(-0.3f, 2.2f, 0), sin).Lerp(new Vector3(0, 1.2f, 0), down);

        float scaleMult = Mathf.Clamp(per2 - per5 * 0.5f, 0, 1);
        HeldPower.transform.localScale = new Vector3(scaleMult, scaleMult, 1);
        rotateCounter %= 4;
        float r = (2 + rotateCounter) * 90;
        HeldPower.transform.SetEulerZ(r);
        float sin2 = Mathf.Sin(r * Mathf.Deg2Rad);
        Joint1.transform.SetEulerZ(sin2 * sin2 * 45 - 180);
        Joint2.transform.SetEulerZ(sin2 * sin2 * 45 - 180);
        SpinHands(HandJoint1L, HandJoint2L);
        SpinHands(HandJoint1R, HandJoint2R, -1);

        float scale = 1.0f + 0.04f * Mathf.Sin(Mathf.Deg2Rad * Counter2 * 0.5f);
        Spiral.localScale = Vector3.Lerp(Spiral.transform.localScale, new Vector3(1f / scale, 1f * scale, 1), 0.1f);

        ConnectArms();
    }
    public void FinishConsuming()
    {
        CoinManager.SpawnGem(transform.position + new Vector3(0, -1.35f), 0.5f);
    }
}
