using UnityEngine;

public class StretchConnector : Animator
{
    public Transform Destination;
    public float AngleOffset = 0;
    public float OriginalDistance = -1;
    public float XScaleMult = 1.0f;
    public float YScaleContribution = 0.0f;
    public override void UpdateAnimation()
    {
        Vector2 toOther = Destination.position - transform.position;
        float dist = toOther.magnitude / Destination.transform.lossyScale.x;
        if (OriginalDistance == -1)
            OriginalDistance = dist;
        float r = toOther.ToRotation();
        float scaleFactor = dist / OriginalDistance;
        transform.LerpLocalEulerZ(r * Mathf.Rad2Deg + AngleOffset, 1f);
        transform.LerpLocalScale(new Vector2(scaleFactor * XScaleMult, 1 + scaleFactor * YScaleContribution), 1f);
    }
}
