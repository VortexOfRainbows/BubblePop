using System.ComponentModel;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class RestockMachine : MonoBehaviour
{
    public SpriteRenderer[] Numbers;
    public SpriteRenderer DisplayNumber;
    public float NumberSwapTimer = 0;
    public void SetRestockAmountToShopStock(GachaponShop Owner)
    {
        int index = Owner.RestockRemaining - 1;
        if(index < 0)
        {
            DisplayNumber.sprite = null;
            return;
        }
        if (index >= 10)
            index = 10;
        DisplayNumber.sprite = Numbers[index].sprite;
        DisplayNumber.transform.localPosition = Numbers[index].transform.localPosition;
        DisplayNumber.transform.localScale *= 0;
        NumberSwapTimer = 0;
    }
    public void FixedUpdate()
    {
        NumberSwapTimer += Time.fixedDeltaTime * 2.5f;
        if (NumberSwapTimer > 1)
            NumberSwapTimer = 1;
        float sin = Mathf.Sin(NumberSwapTimer * Mathf.PI);
        float scale = sin * sin * 0.5f + NumberSwapTimer;
        DisplayNumber.transform.localScale = Vector3.one * (0.5f + 0.5f * scale);
    }
}
