using UnityEngine;

public class Roadblock : MonoBehaviour
{
    public Wormhole portal;
    public byte ProgressionLevel = 0;
    public void FixedUpdate()
    {
        if(Main.PylonProgressionNumber > ProgressionLevel)
        {
            portal.Closing = true;
            //The logic here should be like this:
            //If same level as portal, on
            //If one level behind the portal, on if wave is active
            //If two levels behind the portal, close
        }
        if (portal == null)
            Destroy(gameObject);
    }
}
