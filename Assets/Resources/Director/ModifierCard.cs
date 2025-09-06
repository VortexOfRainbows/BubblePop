using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifierCard : MonoBehaviour
{
    public static readonly Vector3 backScale = new(0.5f, 0.3f, 1.0f);
    public Vector3 RestPosition;
    public GameObject BackSide;
    public CardData cardData = null;
    public Image BG;
    public TextMeshProUGUI TitleText;
    public CardScrollArea Modifiers;
    public CardScrollArea Rewards;
    public float DifficultyMultiplier = 1f;
    public EnemyUIElement CardVisual;
    public Transform SkullAnchor;
    public void Start() => cardData ??= new(this);
    public void UpdateText()
    {
        string shortLineBreak = "<size=12>\n</size>";
        string concat = string.Empty;
        if (cardData.ModifierClause.PermanentModifiers.Count > 0 || (!cardData.EnemyClause.AlreadyInPool && cardData.EnemyClause.Enemy.IsPermanent))
        {
            concat = "Permanently: \n".WithSizeAndColor(28, "#FFAAAA");
            if(!cardData.EnemyClause.AlreadyInPool && cardData.EnemyClause.Enemy.IsPermanent)
                concat += cardData.EnemyClause.Enemy.Description() + '\n';
            foreach (DirectorModifier mod in cardData.ModifierClause.PermanentModifiers)
                concat += mod.Description() + '\n';
            if (cardData.ModifierClause.TemporaryModifiers.Count > 0)
                concat += shortLineBreak;
        }
        if (cardData.ModifierClause.TemporaryModifiers.Count > 0 || (!cardData.EnemyClause.AlreadyInPool && !cardData.EnemyClause.Enemy.IsPermanent))
        {
            concat += "This Wave Only: \n".WithSizeAndColor(28, "#FFAAAA");
            if (!cardData.EnemyClause.AlreadyInPool && !cardData.EnemyClause.Enemy.IsPermanent)
                concat += cardData.EnemyClause.Enemy.Description() + '\n';
            foreach (DirectorModifier mod in cardData.ModifierClause.TemporaryModifiers)
                concat += mod.Description() + '\n';
        }
        Modifiers.SetText(concat);

        concat = string.Empty;
        if (cardData.RewardClause.PreRewards.Count > 0)
        {
            concat = "Immediately: \n".WithSizeAndColor(28, DetailedDescription.LesserGray);
            foreach (Reward r in cardData.RewardClause.PreRewards)
                concat += r.Description() + '\n';
            if (cardData.RewardClause.PostRewards.Count > 0)
                concat += shortLineBreak;
        }
        if(cardData.RewardClause.PostRewards.Count > 0)
        {
            concat += "On Completion: \n".WithSizeAndColor(28, DetailedDescription.LesserGray);
            foreach (Reward r in cardData.RewardClause.PostRewards)
                concat += r.Description() + '\n';
        }
        Rewards.SetText(concat);

        TitleText.text = cardData.CardName();
        CardVisual.Init(cardData.EnemyClause.Enemy.EnemyToAdd.GetIndex());
    }
    public void GenerateCardData()
    {
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
        transform.LerpLocalScale(selected ? Vector2.one * 1.05f : Vector2.one, growSpeed);
        BG.color = Color.Lerp(BG.color, selected ? Color.yellow : (hovering ? Color.Lerp(Color.yellow, Color.white, 0.8f) : Color.white), Utils.DeltaTimeLerpFactor(0.2f));
        CardVisual.CardGraphic.transform.LerpLocalScale(selected ? Vector2.one * 1.05f : Vector2.one, growSpeed);
    }
    public bool HasBeenFlipped { get; set; } = false;
    private float FlipTimer = 0;
    private float SpawnTimer = 0;
    public void ResetAnimation()
    {
        HasBeenFlipped = false;
        FlipTimer = SpawnTimer = 0;
        transform.position = WaveMeter.Instance.DeckPosition.position + new Vector3(200 * WaveMeter.Instance.DeckPosition.lossyScale.x, 0);
        transform.localPosition = transform.localPosition;
        transform.localScale = backScale;
        transform.localEulerAngles = Vector3.zero;
        BackSide.SetActive(true);
    }
    public void SpawnAnimation()
    {
        if(SpawnTimer < 0.35f)
        {
            Utils.LerpSnapNotLocal(transform, WaveMeter.Instance.DeckPosition.position, Utils.DeltaTimeLerpFactor(0.1f), 1f);
        }
        else
        {
            Utils.LerpSnap(transform, RestPosition, Utils.DeltaTimeLerpFactor(0.1f), 1f);
        }
        SpawnTimer += Time.unscaledDeltaTime;
        if (!HasBeenFlipped && SpawnTimer > 0.45f)
        {
            FlipTimer += Time.unscaledDeltaTime * 4.5f;
            if (FlipTimer > 2)
            {
                HasBeenFlipped = true;
                FlipTimer = 2;
            }
            UpdateFlippage();
        }
        UpdateSkulls();
    }
    public void UpdateFlippage()
    {
        BackSide.SetActive(FlipTimer < 1);
        Vector3 angle = transform.localEulerAngles;
        if (FlipTimer < 1)
            angle.y = FlipTimer * 90;
        else
            angle.y = 180 - FlipTimer * 90;
        transform.localEulerAngles = angle;
    }
    public void DespawnAnimation()
    {
        float growSpeed = 0.1f * (2 - FlipTimer);
        transform.LerpLocalScale(backScale, Utils.DeltaTimeLerpFactor(growSpeed));
        if (FlipTimer < 0.5f)
            Utils.LerpSnapNotLocal(transform, new Vector2(WaveMeter.Instance.DeckPosition.position.x + 200 * WaveMeter.Instance.DeckPosition.lossyScale.x, transform.position.y), Utils.DeltaTimeLerpFactor(0.1f), 1f);
        if (HasBeenFlipped)
        {
            FlipTimer -= Time.unscaledDeltaTime * 4.5f;
            if (FlipTimer < 0)
            {
                HasBeenFlipped = false;
                FlipTimer = 0;
            }
            UpdateFlippage();
        }
        UpdateSkulls();
    }
    public void UpdateSkulls()
    {
        float height = 900f / 2;
        Utils.LerpSnap(SkullAnchor, new Vector2(0, height + Mathf.Min(1, 1 - 2 * Mathf.Max(0, FlipTimer - 1.5f)) * -35 - 10), Utils.DeltaTimeLerpFactor(0.5f));
    }
}
