using UnityEngine;

public class ForgeCapsule : MonoBehaviour
{
    public PowerUpObject HeldPower;
    public void Start()
    {
        HeldPower.group.sortingOrder = LayerHelper.CrucibleSortingOrder;
        HeldPower.gameObject.SetActive(false);

        int powerType = PowerUp.RandomFromPool(0f, -1, -1);
        HeldPower.Type = powerType;
        HeldPower.Start();
        HeldPower.adornment.enabled = false;
    }
}
