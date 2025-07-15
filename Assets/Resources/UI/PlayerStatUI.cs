using System.Collections.Generic;
using UnityEngine;

public class PlayerStatUI : MonoBehaviour
{
    public static GameObject Prefab;
    public static List<PlayerHeartUI> Hearts = new();
    private static int CurrentLife = 0;
    private static int MaxLife = 0;
    private static PlayerStatUI Instance { get => s_Instance == null ? s_Instance = FindFirstObjectByType<PlayerStatUI>() : s_Instance; set => s_Instance = value; }
    private static PlayerStatUI s_Instance;
    public static void ClearHearts()
    {
        foreach (PlayerHeartUI heart in Hearts)
            if (heart != null)
                Destroy(heart.gameObject);
        Hearts.Clear();
    }
    public void Start()
    {
        Instance = this;
        ClearHearts();
        CurrentLife = MaxLife = 0;
    }
    public static void SetHeartsToPlayerLife()
    {
        Prefab = Resources.Load<GameObject>("UI/Heart");
        MaxLife = Player.Instance.MaxLife;
        CurrentLife = MaxLife;
        if(Hearts.Count != MaxLife)
        {
            ClearHearts();
            while (Hearts.Count < MaxLife)
            {
                Hearts.Add(Instantiate(Prefab, Instance.transform.GetChild(0)).GetComponent<PlayerHeartUI>());
                int i = Hearts.Count - 1;
                Hearts[i].BobbingOffsetDegrees = 90 * i;
            }
        }
    }
    public static void SetHearts(int num)
    {
        CurrentLife = num;
        for(int i = 0; i < Hearts.Count; ++i)
        {
            PlayerHeartUI heart = Hearts[i];
            if(i < CurrentLife)
                heart.Filled();
            else
                heart.Empty();
        }
    }
}
