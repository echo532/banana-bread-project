using UnityEngine;
using TMPro;
using System.Collections;
public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lifetime = 1f;
    public System.Action OnDestroyEvent;
    private TextMeshPro text;
    private Color color;
    private float size;
    private FontStyles style;
    private float popDuration;
    private float maxScale;
    private float shakeDuration;
    private float shakeMagnitude;
    public RectTransform dmgTextRect;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        color = text.color;
        size = text.fontSize;
        style = text.fontStyle; //FontStyle.Normal FontStyle.Bold FontStyle.Italic FontStyle.BoldAndItalic
        dmgTextRect.pivot = new Vector2(0.5f, 1.1f);
    }

    public void SetDamage(int damage, string element, bool playerhit, bool crit)
    {
        text.text = damage.ToString();

        //default damage values
        color = Color.gray;
        moveSpeed = 0.5f;
        style = FontStyles.Normal;
        popDuration = 0.1f;
        maxScale = 1.2f;
        shakeDuration = 0f;
        shakeMagnitude = 0f;
        dmgTextRect.pivot = new Vector2(0.5f, 1.1f);


        if(playerhit)
        {
            color = Color.yellow;
            moveSpeed = 0.8f;
            size = 15f;
            style = FontStyles.Bold;
            popDuration = 0.2f;
            maxScale = 2.0f;
            shakeDuration = 0.15f;
            shakeMagnitude = 0.075f;
            dmgTextRect.pivot = new Vector2(0.5f, 0.8f);

        } else
        {
            if (crit)
            {
                moveSpeed = 1f;
                size = 10f;
                style = FontStyles.Bold;
                popDuration = 0.3f;
                maxScale = 2.5f;
                shakeDuration = 0.1f;
                shakeMagnitude = 0.3f;
                dmgTextRect.pivot = new Vector2(0.5f, 0.9f);
            }
            else if(1 == 2)
            {
                //tick damage
            }

            if(element == "fire")
            {
                color = Color.red;
            
            } else if(element == "ice")
            {
                color = Color.cyan;
            }      
        }

        text.color = color;
        text.fontSize = size;
        text.fontStyle = style;

        StopAllCoroutines();
        StartCoroutine(AnimateText(popDuration, maxScale));
    }
    IEnumerator AnimateText(float popDuration, float maxScale)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        startPos += new Vector3(Random.Range(-0.3f, 0.3f), 0, 0); // slight random offset
        Vector3 baseScale = transform.localScale;

        Color currentColor = color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;

            // Move upward
            transform.position = startPos + Vector3.up * moveSpeed * elapsed;

            // Fade out
            currentColor.a = Mathf.Lerp(1f, 0f, t);
            text.color = currentColor;

            // Pop scale
            if (elapsed < popDuration)
            {
                float popT = elapsed / popDuration;
                float scale = Mathf.Lerp(1f, maxScale, popT);
                transform.localScale = baseScale * scale;
            }
            else
            {
                float shrinkT = (elapsed - popDuration) / (lifetime - popDuration);
                float scale = Mathf.Lerp(maxScale, 1f, shrinkT);
                transform.localScale = baseScale * scale;
            }

            // 💥 Optional text shake for playerhit
            if (shakeDuration > 0f && elapsed < shakeDuration)
            {
                transform.localPosition += new Vector3(
                    Random.Range(-shakeMagnitude, shakeMagnitude),
                    Random.Range(-shakeMagnitude, shakeMagnitude),
                    0
                );
            }

            yield return null;
        }

        OnDestroyEvent?.Invoke();
        Destroy(gameObject);
    }
}