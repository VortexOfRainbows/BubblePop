using System.Collections.Generic;
using UnityEngine;

public class PlayerStatUI : MonoBehaviour
{
    public static GameObject Prefab = null;
    public static GameObject PrefabShield = null;
    public static List<PlayerHeartUI> Hearts = new();
    public static List<PlayerHeartUI> Shields = new();
    private static int CurrentLife = 0;
    private static int MaxLife = 0;
    private static int CurrentShield = 0;
    private static int MaxShield = 0;
    private static PlayerStatUI Instance { get => s_Instance == null ? s_Instance = FindFirstObjectByType<PlayerStatUI>() : s_Instance; set => s_Instance = value; }
    private static PlayerStatUI s_Instance;
    public TMPro.TextMeshProUGUI moneyText;
    public GameObject Money;
    public static void ClearHearts()
    {
        foreach (PlayerHeartUI heart in Hearts)
            if (heart != null)
                Destroy(heart.gameObject);
        foreach (PlayerHeartUI shield in Shields)
            if (shield != null)
                Destroy(shield.gameObject);
        Hearts.Clear();
        Shields.Clear();
    }
    public void Start()
    {
        Instance = this;
        ClearHearts();
        CurrentLife = MaxLife = 0;
        Money.SetActive(false);
    }
    public void Update()
    {
        if(Main.WavesUnleashed)
        {
            Money.SetActive(true);
            int money = CoinManager.Current; // : CoinManager.Savings;
            moneyText.text = $"${money}";
            moneyText.enabled = true;
        }
    }
    public static void SetHeartsToPlayerLife()
    {
        Prefab = Prefab != null ? Prefab : Resources.Load<GameObject>("UI/Heart");
        PrefabShield = PrefabShield != null ? PrefabShield : Resources.Load<GameObject>("UI/Shield");
        UpdateHearts(Player.Instance);
        CurrentLife = MaxLife;
        CurrentShield = MaxShield;
    }
    public static void UpdateHearts(Player player)
    {
        if(MaxLife != player.TotalMaxLife || MaxShield != player.TotalMaxShield)
        {
            MaxLife = player.TotalMaxLife;
            MaxShield = player.TotalMaxShield;
            if (Hearts.Count != MaxLife || Shields.Count != MaxShield)
            {
                ClearHearts();
                while (Hearts.Count < MaxLife)
                {
                    Hearts.Add(Instantiate(Prefab, Instance.transform.GetChild(0)).GetComponent<PlayerHeartUI>());
                    int i = Hearts.Count - 1;
                    Hearts[i].BobbingOffsetDegrees = 90 * i;
                }
                while (Shields.Count < MaxShield)
                {
                    Shields.Add(Instantiate(PrefabShield, Instance.transform.GetChild(0)).GetComponent<PlayerHeartUI>());
                    int i = Shields.Count - 1;
                    Shields[i].BobbingOffsetDegrees = 90 * i;
                }
            }
            SetHearts(player.Life, player.GetShield());
        }
    }
    public static void SetHearts(int numLife, int numShield)
    {
        UpdateHearts(Player.Instance);
        CurrentLife = numLife;
        CurrentShield = numShield;
        for(int i = 0; i < Hearts.Count; ++i)
        {
            PlayerHeartUI heart = Hearts[i];
            if(i < CurrentLife)
                heart.Filled();
            else
                heart.Empty();
        }
        for (int i = 0; i < Shields.Count; ++i)
        {
            PlayerHeartUI shield = Shields[i];
            if (i < CurrentShield)
                shield.Filled();
            else
                shield.Empty();
        }
    }
}
