using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;
using static NPC;
using static UnityEditorInternal.ReorderableList;

public class Portrait : MonoBehaviour
{
    private static readonly string dir = "UI/GameOverlay/Dialogue/";
    public class CharacterExpressions
    {
        public static Sprite Load(string path) => Resources.Load<Sprite>($"{dir}{path}");
        public CharacterExpressions(string CharacterName, params string[] Expressions)
        {
            this.CharacterName = CharacterName;
            foreach (var expression in Expressions)
                ExpressionSprites[expression.ToLower()] = Load($"{CharacterName}{expression}");
            if(!ExpressionSprites.TryGetValue("default", out _))
                ExpressionSprites["default"] = Load($"{CharacterName}Default");
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
    public static CharacterExpressions Gachapon { get; private set; }
    public static CharacterExpressions Fizzy { get; private set; }
    public static CharacterExpressions KingOil { get; private set; }
    public static Dictionary<NPC.NPCType, CharacterExpressions> NPCExpression { get; private set; }
    public static void LoadPortraitSprites()
    {
        Bubblemancer = new("Bubblemancer", "Dizzy", "Focused", "Smug", "Tired");
        ThoughtBubble = new("TB");
        Gachapon = new("Gacha");
        Fizzy = new("Fizzy");
        KingOil = new("Oil");
        NPCExpression = new()
        {
            { NPCType.Bubblemancer, Bubblemancer },
            { NPCType.Fizzy, Fizzy },
            { NPCType.OilKing, KingOil },
            { NPCType.Gachapon, Gachapon },
            { NPCType.ThoughtBubble, ThoughtBubble }
        };
    }
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
    public void Update()
    {
        float dir = Utils.SignNoZero(transform.localScale.x);
        Vector3 scale = transform.localScale;
        scale.x *= dir;
        if(ActiveSpeaker)
        {
            scale = Vector3.Lerp(scale, Vector3.one * 1.1f, Utils.DeltaTimeLerpFactor(0.1f));
            transform.LerpLocalPosition(new Vector2(transform.localPosition.x, 20), Utils.DeltaTimeLerpFactor(0.1f));
        }
        else
        {
            scale = Vector3.Lerp(scale, Vector3.one * 0.85f, Utils.DeltaTimeLerpFactor(0.1f));
            transform.LerpLocalPosition(new Vector2(transform.localPosition.x, 0), Utils.DeltaTimeLerpFactor(0.1f));
        }
        scale.x *= dir;
        transform.localScale = scale;
    }
}
