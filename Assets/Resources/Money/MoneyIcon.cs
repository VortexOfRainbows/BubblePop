using UnityEngine;
using UnityEngine.UI;

public class MoneyIcon : MonoBehaviour
{
    public int Type = 0;
    public static float ScaleFactor = 1.1f;
    public Canvas myCanvas;
    public Image Icon;
    public GameObject Sparkle;
    public float SparkleTimer = 0;
    public Vector3 InitialScale = Vector3.zero;
    private void FixedUpdate()
    {
        if (InitialScale == Vector3.zero)
            InitialScale = Icon.gameObject.transform.localScale;
        if (Utils.IsMouseHoveringOverThis(true, Icon.rectTransform, 0, myCanvas))
        {
            Icon.gameObject.transform.localScale = Vector3.Lerp(Icon.gameObject.transform.localScale, InitialScale * ScaleFactor, 0.1f);
            if(Type == 0)
            {
                PopUpTextUI.Enable("Money", "Can be used to purchase fancy powerups");
            }
            else
            {
                PopUpTextUI.Enable("Key", "Can be used to open chests");
            }
            UpdateSparkle(Sparkle, true);
        }
        else
        {
            Icon.gameObject.transform.localScale = Vector3.Lerp(Icon.gameObject.transform.localScale, InitialScale * 1.0f, 0.1f);
            UpdateSparkle(Sparkle, false);
        }
    }
    private void UpdateSparkle(GameObject obj, bool on)
    {
        obj.SetActive(on);
        if(on)
        {
            SparkleTimer++;
            obj.transform.localScale = Vector2.one * (1.2f + 0.1f * Mathf.Sin(SparkleTimer * 4 * Mathf.Deg2Rad));
        }
    }
}
