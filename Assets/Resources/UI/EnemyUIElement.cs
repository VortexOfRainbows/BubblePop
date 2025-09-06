using UnityEngine;
using UnityEngine.UI;

public class EnemyUIElement : MonoBehaviour
{
    public Image CardGraphic;
    public Image CardGraphicBG;
    public Enemy MyEnemy { get; set; } = null;
    public bool HasHoverVisual { get; set; } = true;
    public bool CompendiumElement { get; set; } = false;
    public bool Unlocked => MyEnemy.StaticData.Unlocked;
    public int MyID = 0;
    public void SetEnemy(Enemy enemyBasePrefab)
    {
        MyEnemy = enemyBasePrefab;
    }
    public void Init(int type) => Init(EnemyID.AllEnemiesList[MyID = type]);
    public void Init(GameObject enemyBasePrefab) => Init(enemyBasePrefab.GetComponent<Enemy>());
    public void Init(Enemy enemyBasePrefab)
    {
        SetEnemy(enemyBasePrefab);
        Init();
    }
    public void Init()
    {
        CardGraphic.sprite = MyEnemy.StaticData.Card;
        CardGraphicBG.sprite = MyEnemy.StaticData.CardBG;
    }
    [SerializeField] private bool InitOnStart = false;
    public void Start()
    {
        if(InitOnStart)
            Init(0);   
    }
    public void UpdateColor(bool locked, bool grayOut)
    {
        if(locked)
        {
            CardGraphic.color = Color.black;
            CardGraphicBG.color = Color.gray; //Might wanna use grayscale shader here instead
        }
        else if (grayOut)
        {
            CardGraphic.color = CardGraphicBG.color = PowerUpUIElement.GrayColor;
        }
        else
        {
            CardGraphic.color = CardGraphicBG.color = Color.white;
        }
    }
    public float HoverRadius { get; set; } = 64;
    public void UpdateActive(Canvas canvas, out bool hovering, out bool clicked, RectTransform hoverArea)
    {
        Vector2 targetScale = Vector2.one;
        hovering = clicked = false;
        float size = CompendiumElement ? 96 + HoverRadius - hoverArea.rect.width : 0;
        if (Utils.IsMouseHoveringOverThis(true, hoverArea, size, canvas, CompendiumElement) && (!CompendiumElement || HasHoverVisual))
        {
            //Debug.Log(MyEnemy.StaticData.Rarity);
            string name = DetailedDescription.TextBoundedByRarityColor(MyEnemy.StaticData.Rarity - 1, Unlocked ? MyEnemy.Name() : "???", false);
            //string desc = Unlocked ? (CompendiumElement ? "" : ActiveEquipment.GetDescription()) : ActiveEquipment.GetUnlockReq();
            PopUpTextUI.Enable(name, "");
            float scaleUp = 1.1f;
            if (HasHoverVisual)
                clicked = Input.GetMouseButtonDown(0);
            CardGraphic.transform.LerpLocalScale(targetScale * scaleUp, 0.15f);
            hovering = true;
        }
        else
            CardGraphic.transform.LerpLocalScale(targetScale * 1.0f, 0.1f);
    }
}
