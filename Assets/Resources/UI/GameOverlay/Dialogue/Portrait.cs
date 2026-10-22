using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;
using static NPC;

public class Portrait : MonoBehaviour
{
    public static Sprite[] Bubblemancer { get; private set; }
    public static Sprite[] ThoughtBubble { get; private set; }
    public static Sprite[] Gachapon { get; private set; }
    public static Sprite[] Fizzy { get; private set; }
    public static Sprite[] KingOil { get; private set; }
    public static Dictionary<NPC.NPCType, Sprite[]> NPCExpression { get; private set; }
    private static readonly string dir = "UI/GameOverlay/Dialogue/";
    public static Sprite Load(string path)
    {
        return Resources.Load<Sprite>($"{dir}{path}");
    }
    public static void LoadPortraitSprites()
    {
        Bubblemancer = new[] { Load("Bubblemancer0"), 
            Load("Bubblemancer1"), Load("Bubblemancer2"), Load("Bubblemancer3"), Load("Bubblemancer4") };
        ThoughtBubble = new[] { Load("TB0") };
        Gachapon = new[] { Load("Gacha0") };
        Fizzy = new[] { Load("Fizzy0") };
        KingOil = new[] { Load("Oil0") };

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
    public int MyNPCType { get; private set; } = 0;
    public bool ActiveSpeaker { get; set; } = false;
    public void SetExpression(NPCType npc = NPCType.None, int Num = -1)
    {
        if(npc != MyNPC && npc != NPCType.None)
            MyNPC = npc;
        if(MyNPCType != Num && Num != -1)
            MyNPCType = Num;

        Sprite[] expressions = NPCExpression[MyNPC];
        MyNPCType = Mathf.Clamp(MyNPCType, 0, expressions.Length - 1);
        CharacterPortrait.sprite = expressions[MyNPCType];
        CharacterPortrait.SetNativeSize();
    }
    public void Update()
    {
        float dir = Utils.SignNoZero(transform.localScale.x);
        Vector3 scale = transform.localScale;
        scale.x *= dir;
        if(ActiveSpeaker)
            scale = Vector3.Lerp(scale, Vector3.one * 1.1f, Utils.DeltaTimeLerpFactor(0.2f));
        else
            scale = Vector3.Lerp(scale, Vector3.one * 0.85f, Utils.DeltaTimeLerpFactor(0.1f));
        scale.x *= dir;
        transform.localScale = scale;
    }
}
