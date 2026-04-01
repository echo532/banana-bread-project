using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lifetime = 1f;

    private TextMeshPro text;
    private Color color;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        color = text.color;
    }

    public void SetDamage(int damage)
    {
        text.text = damage.ToString();
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Fade out
        color.a -= Time.deltaTime / lifetime;
        text.color = color;

        if (color.a <= 0)
        {
            Destroy(gameObject);
        }
           }
}