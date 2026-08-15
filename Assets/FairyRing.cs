using UnityEngine;

public class FairyRing : MonoBehaviour
{
    public GameObject[] FairyObjs;
    public float Radius = 5;
    public int Count = 10;
    public bool RandomYield = true;
    public void Spawn(Transform decorParent)
    {
        float offset = Utils.RandFloat(Utils.TwoPI);
        for(int i = 0; i < Count; ++i)
        {
            float angle = i * Utils.TwoPI / Count;
            Vector3 pos = new Vector2(0, Radius).RotatedBy(angle + offset);
            pos.y -= 0.5f;
            Instantiate(FairyObjs[Utils.RandInt(FairyObjs.Length)], transform.position + new Vector3(0, -0.4f) + pos + (Vector3)Utils.RandCircle(0.1f), Quaternion.identity, decorParent); //-0.5 offset is for mushrooms, might want to standardize later
        }

        if(RandomYield)
        {
            int type = Utils.RandInt(4);
            Coin output = null;
            if (type == 0 || type == 1)
                output = CoinManager.SpawnKey(transform.position, 100);
            else if (type == 2)
                output = CoinManager.SpawnHeart(transform.position, 100);
            else if (type == 3)
                output = CoinManager.SpawnShield(transform.position, 100);
            if (output != null)
                output.rb.velocity *= 0;
        }

        Destroy(gameObject);
    }
}