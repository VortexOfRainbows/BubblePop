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

    public GameObject[] box;
    public GameObject[] visual;

    private GameObject[] activeEquips = new GameObject[4];
    public void Start()
    {
        RenderBoxes();
    }
    public void Update()
    {
        for(int i = 0; i < 4; i++)
            UpdateEquipment(activeEquips[i], box[i], visual[i]);
    }
    public void UpdateEquipment(GameObject equipment, GameObject box, GameObject visual)
    {
        Equipment e = equipment.GetComponent<Equipment>();
        //Debug.Log(e.UIVisualOffset);
        Vector2 offset = Vector2.zero;
        float rot = 0f;
        float scale = 1f;
        e.ModifyUIOffsets(ref offset, ref rot, ref scale);
        equipment.transform.localPosition = offset;
        equipment.transform.eulerAngles = new Vector3(0, 0, rot);
        visual.transform.localScale = Vector3.one * 50f * scale;
        if (Utils.IsMouseHoveringOverThis(true, box.GetComponent<RectTransform>(), 50, GetComponent<Canvas>()))
        {
            PopUpTextUI.Enable(e.Name(), e.Description());
            //Debug.Log("Inside :)");
        }
    }
    public void RenderBoxes()
    {
        activeEquips[0] = RenderSingular(Hats[0], visual[0]);
        activeEquips[1] = RenderSingular(Accessories[0], visual[1]);
        activeEquips[2] = RenderSingular(Weapons[0], visual[2]);
        activeEquips[3] = RenderSingular(Characters[0], visual[3]);
    }
    public GameObject RenderSingular(GameObject prefab, GameObject parent)
    {
        GameObject obj = Instantiate(prefab.gameObject, parent.transform);
        obj.layer = UILayer;
        obj.transform.localPosition = Vector3.zero;
        foreach(Transform t in obj.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = UILayer;
        }
        return obj;
    }
}
