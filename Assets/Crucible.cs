using UnityEngine;

public class Crucible : MonoBehaviour
{
    public Transform Connector1, Joint1, Connector2, Joint2;

    public Transform Connector3, Joint3;

    public Transform HandBlock, HandConnector1L, HandJoint1L, HandConnector2L, HandJoint2L, 
        HandConnector1R, HandJoint1R, HandConnector2R, HandJoint2R;
    public void ConnectArms()
    {
        Connect(Connector1, Joint1);
        Connector2.position = new Vector3(Joint1.position.x, Joint1.position.y, Connector2.position.z);
        Connect(Connector2, Joint2);
        Connector3.position = new Vector3(Joint2.position.x, Joint2.position.y, Connector3.position.z);
        Connect(Connector3, Joint3);

        HandConnector1L.position = new Vector3(HandBlock.position.x, HandBlock.position.y, HandConnector1L.position.z);
        HandConnector1R.position = new Vector3(HandBlock.position.x, HandBlock.position.y, HandConnector1R.position.z);
        Connect(HandConnector1L, HandJoint1L);
        Connect(HandConnector1R, HandJoint1R);
        HandConnector2L.position = new Vector3(HandJoint1L.position.x, HandJoint1L.position.y, HandConnector2L.position.z);
        HandConnector2R.position = new Vector3(HandJoint1R.position.x, HandJoint1R.position.y, HandConnector2R.position.z);
        Connect(HandConnector2L, HandJoint2L);
        Connect(HandConnector2R, HandJoint2R);
    }
    public void Connect(Transform start, Transform end, float lengthMult = 4)
    {
        Vector2 ConnectorToJoint = end.position - start.position;
        float dist = ConnectorToJoint.magnitude;
        float r = ConnectorToJoint.ToRotation();
        start.SetEulerZ(r * Mathf.Rad2Deg + 90);
        start.localScale = new Vector3(start.localScale.x, dist * lengthMult, 1);
    }
    public void Update()
    {
        ConnectArms();
    }
}
