using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public const int UILayer = 5;
    public GameObject[][] Equipments = new GameObject[4][];
    public GameObject[] Hats;
    public GameObject[] Accessories;
    public GameObject[] Weapons;
    public GameObject[] Characters;
    private Canvas myCanvas;
    public EquipmentUIElement[] UIElems;
    public void Start()
    {
        Equipments[0] = Hats;
        Equipments[1] = Accessories;
        Equipments[2] = Weapons;
        Equipments[3] = Characters;
        myCanvas = GetComponent<Canvas>();
        RenderBoxes();
    }
    public void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            bool wasPressed = UIElems[i].UpdateActive(myCanvas);
            if(wasPressed)
            {
                UIElems[i].ActiveEquipmentIndex = (UIElems[i].ActiveEquipmentIndex + 1) % Equipments[i].Length;
                GameObject equipmentPrefab = Equipments[i][UIElems[i].ActiveEquipmentIndex];
                RenderBox(i);
                if (UIElems[i].ActiveEquipment is Hat hat && Player.Instance.Hat.GetType() != UIElems[i].ActiveEquipment.GetType())
                {
                    GameObject oldHat = Player.Instance.Hat.gameObject;
                    Player.Instance.Hat = Instantiate(equipmentPrefab, Player.Instance.transform).GetComponent<Hat>();
                    Player.Instance.Hat.AliveUpdate();
                    Destroy(oldHat);
                }
                //The other variations of this code for the other equipments will have to be done at some point, though not necessarily now
            }
        }
    }
    public void RenderBoxes()
    {
        for (int i = 0; i < 4; i++)
            RenderBox(i);
    }
    public void RenderBox(int i)
    {
        if(UIElems[i].ActiveEquipment != null)
            Destroy(UIElems[i].ActiveEquipment.gameObject);
        UIElems[i].ActiveEquipment = RenderSingular(Equipments[i][UIElems[i].ActiveEquipmentIndex], UIElems[i].Visual);
        UIElems[i].UpdateOrientation();
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
