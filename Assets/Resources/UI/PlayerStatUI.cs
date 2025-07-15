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
    public void Start()
    {
        Instance = this;
    }
    public static void Initialize()
    {
        Prefab = Resources.Load<GameObject>("UI/Heart");
        MaxLife = Player.Instance.MaxLife;
        CurrentLife = MaxLife;
        Hearts.Clear();
        while (Hearts.Count < MaxLife)
        {
            Hearts.Add(Instantiate(Prefab, Instance.transform.GetChild(0)).GetComponent<PlayerHeartUI>());
            int i = Hearts.Count - 1;
            Hearts[i].BobbingOffsetDegrees = 90 * i;
        }
    }
    public static void Set(int num)
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
