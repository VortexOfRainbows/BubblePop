using System.Collections.Generic;
using UnityEngine;

public class PlayerStatUI : MonoBehaviour
{
    public static GameObject Prefab = null;
    public static GameObject PrefabShield = null;
    public static List<PlayerHeartUI> Hearts = new();
    public static List<PlayerHeartUI> Shields = new();
    private static PlayerStatUI Instance { get => s_Instance == null ? s_Instance = FindFirstObjectByType<PlayerStatUI>() : s_Instance; set => s_Instance = value; }
    private static PlayerStatUI s_Instance;
    public TMPro.TextMeshProUGUI moneyText;
    public GameObject Money;
    public TMPro.TextMeshProUGUI keyText;
    public GameObject Key;
    public TMPro.TextMeshProUGUI tokenText;
    public GameObject Tokens;
    public TMPro.TextMeshProUGUI GemText;
    public GameObject Gems;
    public static void RemoveUnusedContainers(List<PlayerHeartUI> containerList, int cutoff)
    {
        for (int i = containerList.Count - 1; i >= 0; --i)
        {
            PlayerHeartUI c = containerList[i];
            if (i >= cutoff)
            {
                containerList.RemoveAt(i);
                Destroy(c.gameObject);
            }
        }
    }
    public static void AddNeededContainers(List<PlayerHeartUI> containerList, int cutoff, bool shield)
    {
        while (containerList.Count < cutoff)
        {
            containerList.Add(Instantiate(shield ? PrefabShield: Prefab, Instance.transform.GetChild(0)).GetComponent<PlayerHeartUI>());
            int i = containerList.Count - 1;
            containerList[i].BobbingOffsetDegrees = 90 * i;
        }
    }
    private static void UpdateHearts(Player player)
    {
        int shields = player.GetShield();
        int hearts = player.MaxLife;
        int life = player.Life;
        RemoveUnusedContainers(Hearts, hearts);
        RemoveUnusedContainers(Shields, shields);
        AddNeededContainers(Hearts, hearts, false);
        AddNeededContainers(Shields, shields, true);
        for (int i = 0; i < Hearts.Count; ++i)
        {
            PlayerHeartUI heart = Hearts[i];
            if (i < life)
                heart.Filled();
            else
                heart.Empty();
        }
        for (int i = Shields.Count - 1; i >= 0; --i)
        {
            PlayerHeartUI shield = Shields[i];
            shield.Filled(); //All shields are always filled now, since containers no longer exist
        }
    }
    public void Start()
    {
        Instance = this;
        Money.SetActive(false);
        Key.SetActive(false);
        Gems.SetActive(false);
    }
    public void Update()
    {
        if(Main.WavesUnleashed)
        {
            Money.SetActive(true);
            int money = CoinManager.CurrentCoins;
            moneyText.text = $"${money}";
            moneyText.enabled = true;

            Key.SetActive(true);
            int keys = CoinManager.CurrentKeys;
            keyText.text = $"{keys}";
            keyText.enabled = true;

            Gems.SetActive(true);
            int gems = CoinManager.CurrentGems;
            GemText.text = $"{gems}";
            GemText.enabled = true;

            bool usingGachaSlots = Player.Instance.Weapon != null && Player.Instance.Weapon is SlotMachineWeapon;
            Tokens.SetActive(usingGachaSlots);
            if(usingGachaSlots)
            {
                int tokens = CoinManager.CurrentTokens; // : CoinManager.Savings;
                string hex = ColorHelper.TokenColor.ToHexString();
                string text = tokens > 0 ? $"{tokens}" : $"<color={(CoinManager.CurrentCoins >= Player.Instance.SlotMachineCoinCost ? "#FFFFFF" : "#FF4455")}>${Player.Instance.SlotMachineCoinCost}</color>";
                tokenText.text = $"<color={hex}>{text}/{Player.Instance.MaxTokens}</color>";
                tokenText.enabled = true;
            }
        }
    }
    public static void ResetLife()
    {
        Hearts.Clear();
        Shields.Clear();
        SetHeartsToPlayerLife();
    }
    public static void SetHeartsToPlayerLife()
    {
        Prefab = Prefab != null ? Prefab : Resources.Load<GameObject>("UI/PlayerStats/Heart");
        PrefabShield = PrefabShield != null ? PrefabShield : Resources.Load<GameObject>("UI/PlayerStats/Shield");
        UpdateHearts(Player.Instance);
    }
}
