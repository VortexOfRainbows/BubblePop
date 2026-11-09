using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static NPC;

public class Portrait : MonoBehaviour
{
    private static readonly string Directory = "UI/Dialogue/";
    public class CharacterExpressions
    {
        private void AutopopulateExpressions()
        {
            //load all the assets in the given path and add them to the ExpressionSprites dictionary
            var sprites = Resources.LoadAll($"{Directory}{CharacterName}", typeof(Sprite)).Cast<Sprite>();
            foreach (var sprite in sprites)
            {
                ExpressionSprites[sprite.name.ToLower()] = sprite;
                //Debug.Log($"Loaded Expression: {CharacterName}.{sprite.name.ToLower()}");
            }
        }
        public CharacterExpressions(string CharacterName)
        {
            this.CharacterName = CharacterName;
            AutopopulateExpressions();
        }
        public string CharacterName { get; private set; }
        public Dictionary<string, Sprite> ExpressionSprites = new();
        public Sprite GetExpression(string Expression, string Fallback = "default")
        {
            Expression = Expression.ToLower();
            if (ExpressionSprites.TryGetValue(Expression, out Sprite val))
                return val;
            else if (ExpressionSprites.TryGetValue(Fallback, out val))
                return val;
            throw new System.Exception("Fallback expression does not exist");
        }
    }
    public static CharacterExpressions Bubblemancer { get; private set; }
    public static CharacterExpressions ThoughtBubble { get; private set; }
    public static CharacterExpressions Shady { get; private set; }
    public static CharacterExpressions Gachapon { get; private set; }
    public static CharacterExpressions Fizzy { get; private set; }
    public static CharacterExpressions KingOil { get; private set; }
    public static Dictionary<NPCType, CharacterExpressions> NPCExpression { get; private set; }
    public static void LoadPortraitSprites()
    {
        Bubblemancer = new("Bubblemancer");
        ThoughtBubble = new("ThoughtBubble");
        Shady = new("Shady");
        Gachapon = new("Gachapon");
        Fizzy = new("Fizzy");
        KingOil = new("KingOil");
        NPCExpression = new()
        {
            { NPCType.Bubblemancer, Bubblemancer },
            { NPCType.Fizzy, Fizzy },
            { NPCType.OilKing, KingOil },
            { NPCType.Gachapon, Gachapon },
            { NPCType.ThoughtBubble, ThoughtBubble },
            { NPCType.Shady, Shady }
        };
    }
    public Image BG;
    public Image CharacterPortrait;
    public NPCType MyNPC { get; private set; }
    public string MyExpression { get; private set; } = "default";
    public bool ActiveSpeaker { get; set; } = false;
    public void SetExpression(NPCType npc = NPCType.None, string Expression = default)
    {
        if(npc != MyNPC && npc != NPCType.None)
            MyNPC = npc;
        if(Expression != default)
            MyExpression = Expression;

        CharacterExpressions expressions = NPCExpression[MyNPC];
        CharacterPortrait.sprite = expressions.GetExpression(MyExpression);
        CharacterPortrait.SetNativeSize();

        if (MyNPC == NPCType.Bubblemancer)
            CharacterPortrait.transform.localPosition = new Vector3(-20, 0, 0);
        else
            CharacterPortrait.transform.localPosition = Vector3.zero;
    }
    public void DisabledUpdate()
    {
        float dir = Utils.SignNoZero(transform.localScale.x);
        Vector3 scale = Vector3.one * 0.85f;
        scale.x *= dir;
        transform.localScale = scale;
        BG.color = ColorHelper.DarkCornflower.WithAlpha(0.9f);
        transform.localPosition = new Vector2(transform.localPosition.x, 0);
    }
    public void Update()
    {
        float dir = Utils.SignNoZero(transform.localScale.x);
        Vector3 scale = transform.localScale;
        scale.x *= dir;
        float lerpT = Utils.DeltaTimeLerpFactor(0.1f);
        if (ActiveSpeaker)
        {
            BG.color = BG.color.Lerp(ColorHelper.Cornflower.WithAlpha(0.9f), lerpT);
            scale = Vector3.Lerp(scale, Vector3.one * 1.1f, lerpT);
            transform.LerpLocalPosition(new Vector2(transform.localPosition.x, 20), lerpT);
        }
        else
        {
            BG.color = BG.color.Lerp(ColorHelper.DarkCornflower.WithAlpha(0.9f), lerpT);
            scale = Vector3.Lerp(scale, Vector3.one * 0.85f, lerpT);
            transform.LerpLocalPosition(new Vector2(transform.localPosition.x, 0), lerpT);
        }
        scale.x *= dir;
        transform.localScale = scale;
    }
}
