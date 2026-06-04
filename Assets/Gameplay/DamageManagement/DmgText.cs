using UnityEngine;
using TMPro;
using System.Collections;

public class DamageText : MonoBehaviour
{
    private TextMeshPro text;

    public float moveSpeed = 1f;
    public float lifetime = 1f;

    private Color color;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void SetDamage(int damage, string element, bool playerhit, string type)
    {
        text.text = damage.ToString();

        // ---------------------------
        // DEFAULT STYLE
        // ---------------------------
        color = Color.white;
        text.fontSize = 6f;
        text.fontStyle = FontStyles.Normal;
        moveSpeed = 1f;

        // ---------------------------
        // TYPE-BASED STYLING
        // ---------------------------
        if (type == "crit")
        {
            text.fontSize = 10f;
            text.fontStyle = FontStyles.Bold;
            moveSpeed = 1.3f;
        }
        else if (type == "tick")
        {
            text.fontSize = 4f;
            text.fontStyle = FontStyles.Normal;
            moveSpeed = 0.7f;
        }

        // ---------------------------
        // ELEMENT COLORS
        // ---------------------------
        if (element == "fire")
            color = Color.red;
        else if (element == "ice")
            color = Color.cyan;

        text.color = color;

        StopAllCoroutines();
        StartCoroutine(FloatAndFade());
    }

    IEnumerator FloatAndFade()
    {
        float t = 0f;
        Vector3 start = transform.position;

        while (t < lifetime)
        {
            t += Time.deltaTime;

            transform.position = start + Vector3.up * (moveSpeed * t);

            Color c = text.color;
            c.a = 1f - (t / lifetime);
            text.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }
}