using UnityEngine;
using UnityEngine.UI;

public class EnemyUIElement : MonoBehaviour
{
    public Image CardGraphic;
    public Image CardGraphicBG;
    public Enemy MyEnemy { get; set; } = null;
    public bool HasHoverVisual { get; set; } = false;
    public bool Unlocked => false;
    public int MyID = 0;
    public void SetEnemy(Enemy enemyBasePrefab)
    {
        MyEnemy = enemyBasePrefab;
    }
    public void Init(int type) => Init(EnemyID.SpawnableEnemiesList[MyID = type]);
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
}
