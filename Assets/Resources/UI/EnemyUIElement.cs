using UnityEngine;
using UnityEngine.UI;

public class EnemyUIElement : MonoBehaviour
{
    public Image CardGraphic;
    public Image CardGraphicBG;
    public Enemy MyEnemy = null;
    public int MyID = 0;
    public void SetEnemy(GameObject enemyBasePrefab)
    {
        MyEnemy = enemyBasePrefab.GetComponent<Enemy>();
    }
    public void Init(int type)
    {
        Init(EnemyID.AllEnemyList[MyID = type]);
        Init();
    }
    public void Init(GameObject enemyBasePrefab)
    {
        SetEnemy(enemyBasePrefab);
        Init();
    }
    public void Init()
    {

    }
}
