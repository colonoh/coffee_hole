using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    private Text scoreText;

    private void Awake()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas == null) return;

        GameObject textObj = new GameObject("ScoreText");
        textObj.transform.SetParent(transform, false);

        scoreText = textObj.AddComponent<Text>();
        scoreText.text = "Score: 0";
        scoreText.fontSize = 28;
        scoreText.color = Color.white;
        scoreText.alignment = TextAnchor.UpperLeft;
        scoreText.raycastTarget = false;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        scoreText.font = font;

        RectTransform rt = scoreText.rectTransform;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(20f, -20f);
        rt.sizeDelta = new Vector2(250f, 50f);
    }

    private void Update()
    {
        if (scoreText != null && GameManager.Instance != null)
            scoreText.text = "Score: " + GameManager.Instance.Score;
    }
}
