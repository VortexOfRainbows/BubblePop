using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpUIElement : MonoBehaviour
{
    public Image outer;
    public Image inner;
    public PowerUp MyPower => PowerUp.Get(Type);
    public Sprite Sprite => MyPower.sprite;
    public int Type => Player.Instance.GetPower(Index);
    public TMPro.TextMeshProUGUI Count;
    public int Index = 0;
    public bool PlayerHasPower()
    {
        return Index < Player.Instance.PowerCount;
    }
    public void TurnedOn()
    {
        inner.sprite = Sprite;
        inner.SetNativeSize();
        outer.SetNativeSize();
        inner.gameObject.SetActive(true);
    }
    public void TurnedOff()
    {
        inner.gameObject.SetActive(false);
    }
    public void WhileOn()
    {
        Count.text = MyPower.Stack.ToString();
    }
    public void FixedUpdate()
    {
        if(PlayerHasPower())
        {
            if (!inner.gameObject.activeSelf)
                TurnedOn();
            WhileOn();
        }
        else
            TurnedOff();
    }
}
