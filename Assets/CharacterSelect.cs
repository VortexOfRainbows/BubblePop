using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public const int UILayer = 5;
    public GameObject[] Hats;
    public GameObject[] Accessories;
    public GameObject[] Weapons;
    public GameObject[] Characters;
    private Canvas myCanvas;
    public EquipmentUIElement[] UIElems;
    public void Start()
    {
        myCanvas = GetComponent<Canvas>();
        RenderBoxes();
    }
    public void Update()
    {
        for (int i = 0; i < 4; i++)
            UIElems[i].UpdateActive(myCanvas);
    }
    public void RenderBoxes()
    {
        UIElems[0].ActiveEquipment = RenderSingular(Hats[0], UIElems[0].Visual);
        UIElems[1].ActiveEquipment = RenderSingular(Accessories[0], UIElems[1].Visual);
        UIElems[2].ActiveEquipment = RenderSingular(Weapons[0], UIElems[2].Visual);
        UIElems[3].ActiveEquipment = RenderSingular(Characters[0], UIElems[3].Visual);
    }
    public Equipment RenderSingular(GameObject prefab, GameObject parent)
    {
        Equipment obj = Instantiate(prefab.GetComponent<Equipment>(), parent.transform);
        obj.gameObject.layer = UILayer;
        obj.transform.localPosition = Vector3.zero;
        foreach(Transform t in obj.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = UILayer;
        }
        return obj;
    }
}
