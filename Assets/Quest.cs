using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public static Quest SpawnBlurb(Transform parent)
    {
        Quest q = Instantiate(Main.PrefabAssets.QuestPrefab, parent).GetComponent<Quest>();
        q.transform.localPosition = new Vector3(350, q.transform.position.y, q.transform.position.z);
        return q;
    }
    public TextMeshProUGUI Text;
    public bool Complete { get; private set; } = false;
    public void SetComplete()
    {
        Complete = true;
    }
    public void DoUpdate()
    {
        if(Complete)
        {
            Utils.LerpSnap(transform, new Vector2(-350, transform.localPosition.y), Utils.DeltaTimeLerpFactor(0.1f));
        }
        else
        {
            Utils.LerpSnap(transform, new Vector2(50, transform.localPosition.y), Utils.DeltaTimeLerpFactor(0.1f));
        }
    }
    public void Update()
    {
        if (Complete)
            DoUpdate();
    }
}
