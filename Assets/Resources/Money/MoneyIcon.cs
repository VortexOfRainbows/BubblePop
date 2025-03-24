using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class MoneyIcon : MonoBehaviour
{
    public static float ScaleFactor = 1.1f;
    public Vector3 one => Vector3.one;
    public Canvas myCanvas;
    public Image Icon;
    public Image Icon2;
    public GameObject Sparkle;
    public GameObject Sparkle2;
    public float SparkleTimer = 0;
    private void FixedUpdate()
    {
        if(!Main.GameStarted)
        {
            if (Utils.IsMouseHoveringOverThis(true, Icon.rectTransform, canvas: myCanvas))
            {
                Icon.gameObject.transform.localScale = Vector3.Lerp(Icon.gameObject.transform.localScale, one * ScaleFactor, 0.1f);
                PopUpTextUI.Enable("Savings", "Can be used to purchase fancy equipment");
                UpdateSparkle(Sparkle, true);
            }
            else
            {
                Icon.gameObject.transform.localScale = Vector3.Lerp(Icon.gameObject.transform.localScale, one * 1.0f, 0.1f);
                UpdateSparkle(Sparkle, false);
            }
        }
        else
        {
            if (Utils.IsMouseHoveringOverThis(false, Icon2.rectTransform, 50, canvas: myCanvas))
            {
                Icon2.gameObject.transform.localScale = Vector3.Lerp(Icon2.gameObject.transform.localScale, one * ScaleFactor, 0.1f);
                PopUpTextUI.Enable("Money", "Can be used to purchase fancy equipment\n10% is converted into savings at the end of a run");
                UpdateSparkle(Sparkle2, true);
            }
            else
            {
                Icon2.gameObject.transform.localScale = Vector3.Lerp(Icon2.gameObject.transform.localScale, one * 1.0f, 0.1f);
                UpdateSparkle(Sparkle2, false);
            }
        }
    }
    private void UpdateSparkle(GameObject obj, bool on)
    {
        obj.SetActive(on);
        if(on)
        {
            SparkleTimer++;
            obj.transform.localScale = one * (1.2f + 0.1f * Mathf.Sin(SparkleTimer * 4 * Mathf.Deg2Rad));
        }
    }
}
