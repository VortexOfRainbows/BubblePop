using UnityEngine;

public class RestockMachine : MonoBehaviour
{
    public SpriteRenderer[] Numbers;
    public SpriteRenderer DisplayNumber;
    public Transform GumballMachine;
    public GameObject Smile, OpenSmile;
    public float NumberSwapTimer = 0;
    public float AnimationTimer = 0;
    public void SetRestockAmountToShopStock(GachaponShop Owner)
    {
        int index = Owner.RestockRemaining;
        if (index >= Numbers.Length)
            index = Numbers.Length - 1;
        DisplayNumber.sprite = Numbers[index].sprite;
        DisplayNumber.transform.localPosition = Numbers[index].transform.localPosition;
        DisplayNumber.transform.localScale *= 0;
        NumberSwapTimer = 0;
    }
    public void FixedUpdate()
    {
        Animate();
        AnimateNumber();
    }
    public void Animate()
    {
        AnimationTimer += Time.fixedDeltaTime * 0.8f;
        float sin = Mathf.Sin(AnimationTimer * Mathf.PI);
        float sin2 = Mathf.Sin(AnimationTimer * Mathf.PI * 0.5f);
        float sin3 = Mathf.Sin(NumberSwapTimer * Mathf.PI) * 0.04f;
        float stretch = sin * 0.025f;
        GumballMachine.transform.localScale = new Vector3(1 + stretch + sin3, 1 - stretch + sin3, 1);
        transform.localScale = new Vector3(1 - stretch * 0.3f, 1 + stretch * 0.3f, 1);
        float tilt = 2.25f * sin2;
        GumballMachine.transform.SetLocalEulerZ(tilt);
    }
    public void AnimateNumber()
    {
        NumberSwapTimer += Time.fixedDeltaTime * 2.5f;
        if (NumberSwapTimer > 1)
            NumberSwapTimer = 1;
        //bool smile = NumberSwapTimer > 0 && NumberSwapTimer < 1;
        //Smile.SetActive(!smile);
        //OpenSmile.SetActive(smile);
        float sin = Mathf.Sin(NumberSwapTimer * Mathf.PI);
        float scale = sin * sin * 0.5f + NumberSwapTimer;
        DisplayNumber.transform.localScale = Vector3.one * (0.5f + 0.5f * scale);
        Smile.transform.localScale = OpenSmile.transform.localScale = DisplayNumber.transform.localScale;
    }
}
