using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    public float moveUpDistance = 40f;
    public float duration = 1f;

    private TMP_Text popupText;
    private Vector3 startPos;
    private Color startColor;

    void Awake()
    {
        popupText = GetComponent<TMP_Text>();
        startPos = transform.localPosition;
        startColor = popupText.color;
    }

    public void Init(int score)
    {
        popupText.text = $"+{score}";
        popupText.color = startColor;
        float randomX = Random.Range(-10f, 10f);
        startPos = new Vector3(startPos.x + randomX, startPos.y, startPos.z);
        transform.localPosition = startPos;
        StartCoroutine(Animate());
    }

    private System.Collections.IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector3 endPos = startPos + Vector3.up * moveUpDistance;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            popupText.color = new Color(startColor.r, startColor.g, startColor.b, 1 - t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
