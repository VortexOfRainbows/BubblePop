using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifierCard : MonoBehaviour
{
    public CanvasGroup MyGroup;
    public static readonly Vector3 backScale = new(0.55f, 0.33f, 1.0f);
    public Vector2 RestPosition { get; set; }
    public Vector2 InitialSpawnPosition { get; set; } = Vector3.zero;
    public GameObject BackSide, FrontSide;
    public CardData cardData = null;
    public Image BG, Box1, Box2;
    public Image BG2, Key;
    public TextMeshProUGUI TitleText;
    public CardScrollArea Modifiers;
    public CardScrollArea Rewards;
    public float DifficultyMultiplier = 1f;
    public EnemyUIElement CardVisual;
    public EnemyUIElement SecondaryCardVisual;
    public Transform SkullAnchor;
    public Transform RotateFaker;
    public Canvas SkullAnchorCanvas;
    public Canvas TextCanvas;
    public Color DifficultyColor { get; set; }
    public void Start()
    {
        cardData ??= new(this);
        int num = (int)(DifficultyMultiplier + Player.Instance.PersonalWaveCardBonus);
        if (num == 1)
            DifficultyColor = BG.color = Box1.color = Box2.color = BG2.color = Key.color = ColorHelper.Bronze;
        else if(num == 2)
            DifficultyColor = BG.color = Box1.color = Box2.color = BG2.color = Key.color = ColorHelper.Silver;
        else if(num == 3)
            DifficultyColor = BG.color = Box1.color = Box2.color = BG2.color = Key.color = ColorHelper.DimGold;
        else if (num == 4)
            DifficultyColor = BG.color = Box1.color = Box2.color = BG2.color = Key.color = ColorHelper.IridiumPurple;
    }
    public void UpdateText()
    {
        string shortLineBreak = "<size=12>\n</size>";
        string concat = string.Empty;
        if (cardData.ModifierClause.PermanentModifiers.Count > 0 || (!cardData.EnemyClause.AlreadyInPool && cardData.EnemyClause.Enemy.IsPermanent))
        {
            concat = "Permanently: \n".WithSizeAndColor(28, "#FFAAAA");
            if(!cardData.EnemyClause.AlreadyInPool && cardData.EnemyClause.Enemy.IsPermanent)
                concat += cardData.EnemyClause.Enemy.Description();
            foreach (DirectorModifier mod in cardData.ModifierClause.PermanentModifiers)
                concat += mod.Description() + '\n';
            if (cardData.ModifierClause.TemporaryModifiers.Count > 0)
                concat += shortLineBreak;
        }
        if (cardData.ModifierClause.TemporaryModifiers.Count > 0 || (!cardData.EnemyClause.AlreadyInPool && !cardData.EnemyClause.Enemy.IsPermanent))
        {
            concat += "This Wave Only: \n".WithSizeAndColor(28, "#FFAAAA");
            if (!cardData.EnemyClause.AlreadyInPool && !cardData.EnemyClause.Enemy.IsPermanent)
                concat += cardData.EnemyClause.Enemy.Description();
            foreach (DirectorModifier mod in cardData.ModifierClause.TemporaryModifiers)
                concat += mod.Description() + '\n';
        }
        Modifiers.SetText(concat);

        concat = string.Empty;
        if (cardData.RewardClause.PreRewards.Count > 0)
        {
            concat = "Immediately: \n".WithSizeAndColor(28, ColorHelper.LesserGrayHex);
            foreach (Reward r in cardData.RewardClause.PreRewards)
                concat += r.Description() + '\n';
            if (cardData.RewardClause.PostRewards.Count > 0)
                concat += shortLineBreak;
        }
        if(cardData.RewardClause.PostRewards.Count > 0)
        {
            concat += "On Completion: \n".WithSizeAndColor(28, ColorHelper.LesserGrayHex);
            foreach (Reward r in cardData.RewardClause.PostRewards)
                concat += r.Description() + '\n';
        }
        Rewards.SetText(concat);

        TitleText.text = cardData.CardName();
        CardVisual.Init(cardData.EnemyClause.Enemy.EnemiesToAdd[0].GetIndex());
        CardVisual.MaskActive(false);
        if(cardData.EnemyClause.Enemy.EnemiesToAdd.Count > 1)
        {
            SecondaryCardVisual.AddedDrawOrder = CardVisual.AddedDrawOrder - 4;
            SecondaryCardVisual.Init(cardData.EnemyClause.Enemy.EnemiesToAdd[1].GetIndex());
            SecondaryCardVisual.MaskActive(false);

            CardVisual.EnemyScaler.transform.localPosition = new Vector3(-70, -55);
            SecondaryCardVisual.EnemyScaler.transform.localPosition = new Vector3(70, 55, 5);
        }
        else
            CardVisual.EnemyScaler.transform.localPosition = Vector3.zero;
    }
    public void GenerateCardData()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
        cardData ??= new(this);
        cardData.Generate();
        UpdateText();
        ResetAnimation();
    }
    public void UpdateSizing()
    {
        Modifiers.UpdateSizing();
        Rewards.UpdateSizing();
    }
    public void UpdateSelectVisuals(bool selected = false, bool hovering = false)
    {
        if (selected)
            hovering = selected;
        float growSpeed = Utils.DeltaTimeLerpFactor(0.06f * FlipTimer + (HasBeenFlipped ? 0.08f : 0f));
        transform.LerpLocalScale(selected ? Vector2.one * 1.04f : Vector2.one, growSpeed);
        BG.color = Box1.color = Box2.color = Key.color = Color.Lerp(BG.color, selected ? ColorHelper.UI.Yellow : (hovering ? Color.Lerp(Color.yellow, DifficultyColor, 0.8f) : DifficultyColor), Utils.DeltaTimeLerpFactor(0.2f));
        CardVisual.EnemyScaler.transform.LerpLocalScale(selected ? Vector2.one * 1.05f : Vector2.one, growSpeed);
        SecondaryCardVisual.EnemyScaler.transform.LerpLocalScale(selected ? new Vector2(-1.05f, 1.05f) : new Vector2(-1.0f, 1.0f), growSpeed);
    }
    public bool HasBeenFlipped { get; set; } = false;
    public float FlipTimer { get; private set; } = 0;
    public float SpawnTimer { get; private set; } = 0;
    public void ResetAnimation()
    {
        FrontSide.SetActive(false);
        CardVisual.MaskActive(false);
        SecondaryCardVisual.MaskActive(false);
        SkullAnchorCanvas.gameObject.SetActive(false);
        TextCanvas.gameObject.SetActive(false);
        SkullAnchorCanvas.sortingLayerID = TextCanvas.sortingLayerID = SortingLayer.NameToID("UICamera");
        HasBeenFlipped = false;
        FlipTimer = 0;
        SpawnTimer = (DifficultyMultiplier - 1) * - 0.06f;
        transform.localPosition = new Vector3(0, -350);
        transform.localScale = backScale;
        transform.localEulerAngles = new Vector3(0, 0, -20 * (DifficultyMultiplier - 2));
        InitialSpawnPosition = new Vector2(100 * (DifficultyMultiplier - 2), (DifficultyMultiplier == 2 ? -260 : -290));
        RestPosition = new Vector2(450 * (DifficultyMultiplier - 2), 0);
        BackSide.SetActive(true);
        CardVisual.UpdateColor(!WaveDirector.EnemyPool.Contains(CardVisual.MyEnemyPrefab.gameObject), false);
        if (cardData.EnemyClause.Enemy.EnemiesToAdd.Count > 1)
        {
            if (SecondaryCardVisual.MyEnemyPrefab != null)
            {
                SecondaryCardVisual.gameObject.SetActive(true);
                SecondaryCardVisual.UpdateColor(!WaveDirector.EnemyPool.Contains(SecondaryCardVisual.MyEnemyPrefab.gameObject), false);
            }
        }
        else
        {
            SecondaryCardVisual.gameObject.SetActive(false);
        }
        MyGroup.alpha = 0;
    }
    public bool Spawning = false;
    public void SpawnAnimation()
    {
        if (!Spawning)
            Spawning = true;
        MyGroup.alpha = SpawnTimer * 5;
        MyGroup.alpha = Mathf.Clamp01(MyGroup.alpha);

        SpawnTimer += Time.unscaledDeltaTime;
        if(SpawnTimer > 0.4f)
        {
            float lerpFactor = Utils.DeltaTimeLerpFactor(0.1f);
            transform.LerpLocalEulerZ(0, lerpFactor);
            Utils.LerpSnap(transform, RestPosition, lerpFactor, 1f);
            if (!HasBeenFlipped)
            {
                FlipTimer += Time.unscaledDeltaTime * 4.75f;
                if (FlipTimer > 2)
                {
                    HasBeenFlipped = true;
                    FlipTimer = 2;
                }
            }
        }
        else if (SpawnTimer > 0)
        {
            float percent = Mathf.Clamp01(SpawnTimer * 2.75f);
            Vector2 offset = new Vector2(0, 60 * Mathf.Sin( percent * Mathf.PI)).RotatedBy(transform.localEulerAngles.z * Mathf.Deg2Rad);
            Utils.LerpSnap(transform, InitialSpawnPosition + offset, Utils.DeltaTimeLerpFactor(0.1f), 1f);
        }
        UpdateFlippage();
        UpdateSkulls();
    }
    public void UpdateFlippage()
    {
        CardVisual.MaskActive(FlipTimer >= 1);
        //SecondaryCardVisual.MaskActive(FlipTimer >= 1);
        SkullAnchorCanvas.gameObject.SetActive(FlipTimer >= 1);
        TextCanvas.gameObject.SetActive(FlipTimer >= 1);
        BackSide.SetActive(FlipTimer < 1);
        FrontSide.SetActive(FlipTimer >= 1);
        float angle;
        if (FlipTimer < 1)
            angle = FlipTimer * 90;
        else
            angle = 180 - FlipTimer * 90;
        float toScaleX = Mathf.Cos(angle * Mathf.Deg2Rad);
        RotateFaker.localScale = new Vector3(toScaleX, 1, 1);
    }
    public void DespawnAnimation()
    {
        if (Spawning)
        {
            SpawnTimer = (DifficultyMultiplier - 1) * -0.06f;
            Spawning = false;
        }
        SpawnTimer += Time.unscaledDeltaTime;
        float percent = 1 - FlipTimer / 2;
        float growSpeed = 0.025f + 0.2f * percent;
        transform.LerpLocalScale(backScale, Utils.DeltaTimeLerpFactor(growSpeed));
        transform.localPosition = RestPosition + new Vector2(0, 100 * (Mathf.Sin(percent * percent * Mathf.PI) - percent));
        MyGroup.alpha = 1 - percent;
        if (SpawnTimer > 0.0f)
        {
            if (HasBeenFlipped)
            {
                FlipTimer -= Time.unscaledDeltaTime * 4.75f;
                if (FlipTimer < 0)
                {
                    HasBeenFlipped = false;
                    FlipTimer = 0;
                }
                UpdateFlippage();
            }
        }
        UpdateSkulls();
    }
    public void UpdateSkulls()
    {
        float height = 900f / 2;
        Utils.LerpSnap(SkullAnchor, new Vector2(0, height + Mathf.Min(1, 1 - 2 * Mathf.Max(0, FlipTimer - 1.5f)) * -35 - 10), Utils.DeltaTimeLerpFactor(0.5f));
    }
}
