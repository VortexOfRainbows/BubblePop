using UnityEngine;
using UnityEngine.UI;

public class EnemyUIElement : MonoBehaviour
{
    public void MaskActive(bool active)
    {
        Mask.gameObject.SetActive(active);
        SpriteMask.gameObject.SetActive(active); //potentially erroneous
    }
    public const int UILayer = 5;
    public Transform Mask;
    public Transform SpriteMask;
    public Transform EnemyScaler;
    public Image CardGraphicBG;
    private Vector3 originalMaskScale = Vector3.zero;
    public EnemyID.StaticEnemyData StaticData => MyEnemyPrefab.StaticData;
    public Enemy MyEnemyPrefab { get; private set; } = null;
    private Enemy MyEnemy { get; set; } = null;
    public bool HasHoverVisual { get; set; } = true;
    public bool CompendiumElement { get; set; } = false;
    public bool Unlocked => MyEnemy.StaticData.Unlocked;
    public int MyID = 0;
    public int AddedDrawOrder { get; set; } = 40;
    public bool IsMainSelectedInCompendium = false;
    public void SetEnemy(Enemy enemyBasePrefab)
    {
        if (originalMaskScale == Vector3.zero)
            originalMaskScale = SpriteMask.transform.localScale;
        MyEnemyPrefab = enemyBasePrefab;
        if (MyEnemy != null)
        {
            Destroy(MyEnemy.gameObject);
            MyEnemy = null;
        }
        if (MyEnemy == null)
        {
            float size = CardGraphicBG.rectTransform.rect.width / 130f; //The default UI box is 130, so that is what we change the scale factor by
            SpriteMask.transform.localScale = originalMaskScale * size;
            MyEnemy = Enemy.Spawn(MyEnemyPrefab.gameObject, transform.position, false);
            MyEnemy.SetDummy();
            MyEnemy.Animate();
            MyEnemy.UIAI();
            MyEnemy.transform.SetParent(EnemyScaler);
            Vector2 offset = Vector2.zero;
            float scale = 40f / (MyEnemy.Visual.transform.localScale.x) * size;
            MyEnemy.ModifyUIOffsets(ref offset, ref scale);
            MyEnemy.transform.localScale = Vector3.one;
            MyEnemy.transform.localScale *= scale;

            if(IsMainSelectedInCompendium && MyEnemy is EnemyBossDuck leonard)
            {
                leonard.PlayMusic = true;
            }

            var obj = MyEnemy.gameObject;
            obj.gameObject.layer = UILayer;
            obj.transform.localPosition = (Vector3)offset * scale;
            foreach (Transform t in obj.GetComponentsInChildren<Transform>())
                t.gameObject.layer = UILayer;
            int layer = Main.UICameraLayerID;
            foreach (SpriteRenderer r in MyEnemy.GetComponentsInChildren<SpriteRenderer>())
            {
                if (r.sortingOrder <= -50) //This is shadow
                    r.enabled = false; //disable shadow
                else
                {
                    if (AddedDrawOrder != 50)
                        r.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    r.sortingOrder += AddedDrawOrder;
                }
                r.sortingLayerID =layer; //UI camera layer
            }
        }
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
        CardGraphicBG.sprite = StaticData.CardBG;
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
            //Need to make a question mark visual here!
            MyEnemy.UpdateRendererColor(Color.black, 1f);
            CardGraphicBG.color = Color.gray; //Might wanna use grayscale shader here instead
        }
        else if (grayOut)
        {
            //Need to make a question gray visual here!
            //CardGraphic.color =
            MyEnemy.AdjustRenderColorFromDefault(Color.black, 0.5f);
            CardGraphicBG.color = PowerUpUIElement.GrayColor;
        }
        else
        {
            //Need to make a question white visual here!
            //CardGraphic.color =
            MyEnemy.UpdateRendererColorToDefault(1f);
            CardGraphicBG.color = Color.white;
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
            string name = DetailedDescription.TextBoundedByRarityColor(StaticData.Rarity - 1, Unlocked ? MyEnemy.Name() : "???", false);
            //string desc = Unlocked ? (CompendiumElement ? "" : ActiveEquipment.GetDescription()) : ActiveEquipment.GetUnlockReq();
            PopUpTextUI.Enable(name, "");
            float scaleUp = 1.1f;
            if (HasHoverVisual)
                clicked = Input.GetMouseButtonDown(0);
            //Mask.transform.LerpLocalScale(targetScale * scaleUp, 0.15f);
            EnemyScaler.LerpLocalScale(targetScale * scaleUp, Utils.DeltaTimeLerpFactor(0.15f));
            hovering = true;
        }
        else
        { 
            //Mask.transform.LerpLocalScale(targetScale * 1.0f, 0.1f);
            EnemyScaler.LerpLocalScale(targetScale * 1.0f, Utils.DeltaTimeLerpFactor(0.1f));
        }
    }
}
