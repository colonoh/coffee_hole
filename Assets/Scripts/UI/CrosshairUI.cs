using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private float size = 4f;
    [SerializeField] private Color color = Color.white;

    private void Awake()
    {
        // If this script is on a Canvas, create the crosshair image as a child
        var canvas = GetComponent<Canvas>();
        if (canvas == null) return;

        GameObject dotObj = new GameObject("CrosshairDot");
        dotObj.transform.SetParent(transform, false);

        Image dot = dotObj.AddComponent<Image>();
        dot.color = color;
        dot.raycastTarget = false;

        RectTransform rt = dot.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(size, size);
    }
}
