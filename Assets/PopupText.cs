using TMPro;
using UnityEngine;

public class PopupText : MonoBehaviour
{
    public static GameObject PopupTextPrefab => m_popupTextPrefab == null ? (m_popupTextPrefab = Resources.Load<GameObject>("PopupText/PopupText")) : m_popupTextPrefab;
    private static GameObject m_popupTextPrefab;
    public TextMeshPro tmp;
    private float timeLeft = 100;
    public Rigidbody2D rb;
    public Vector3 targetScale = Vector3.one;
    public static GameObject NewPopupText(Vector3 pos, Vector3 velo, Color clr, string text, bool bold = false, float size = 1f)
    {
        GameObject obj = Instantiate(PopupTextPrefab, pos, Quaternion.identity);
        TextMeshPro tmp = obj.GetComponent<TextMeshPro>();
        tmp.text = text;
        tmp.color = clr;
        tmp.fontStyle = bold ? (FontStyles)FontStyle.Bold : 0;
        obj.GetComponent<Rigidbody2D>().velocity = velo;
        obj.GetComponent<PopupText>().targetScale = obj.transform.localScale * size;
        obj.transform.localScale = Vector3.zero;
        return obj;
    }
    public void FixedUpdate()
    {
        if(timeLeft == 100)
        {
            transform.localScale = Vector3.zero;
        }
        if(timeLeft <= 0)
        {
            Destroy(gameObject);
            return;
        }
        float percent = timeLeft / 100f;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale * (0.5f + 0.5f * Mathf.Sin(percent * Mathf.PI / 3)), 0.1f);
        if(percent < 0.5f)
            tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 2 * percent);
        --timeLeft;
        rb.velocity *= 0.98f;
    }
}
