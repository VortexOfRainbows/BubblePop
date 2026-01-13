using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class PowerUpCheatUI : MonoBehaviour
{
    public PowerUpButton ChoiceTemplate;
    public GridLayoutGroup GridParent;
    public bool CheatCanvas { get; set; } = false;
    public void Start()
    {
        if(CheatCanvas)
        {
            for (int i = 0; i < PowerUp.Reverses.Count; ++i)
            {
                PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
                p.SetType(i);
                p.gameObject.SetActive(true);
            }
        }
    }
    public void Init()
    {
        Player player = Player.Instance;
        for (int i = 0; i < player.Powers.Count; i++)
        {
            PowerUpButton p = Instantiate(ChoiceTemplate, GridParent.transform);
            p.SetType(player.Powers[i]);
            p.gameObject.SetActive(true);
        }
        gameObject.SetActive(true);
    }
    public void Update()
    {
        UpdateContentSize();
    }
    public void UpdateContentSize()
    {
        int c = GridParent.transform.childCount;
        if (c <= 0)
            return;
        Vector3 lastElement = GridParent.transform.GetChild(c - 1).localPosition;
        RectTransform r = GridParent.GetComponent<RectTransform>();
        float dist = -lastElement.y -GridParent.padding.bottom * 3;
        r.sizeDelta = new Vector2(r.sizeDelta.x, dist);
    }
}
