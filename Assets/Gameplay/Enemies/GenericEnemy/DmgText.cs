using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lifetime = 1f;
    public System.Action OnDestroyEvent;
    private TextMeshPro text;
    private Color color;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        color = text.color;

    }

    public void SetDamage(int damage, string element)
    {
        text.text = damage.ToString();
        if(element == "fire")
        {
            color = Color.red;
            moveSpeed = 0.5f;

        }

                
        

    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        color.a -= Time.deltaTime / lifetime;
        text.color = color;

        if (color.a <= 0)
        {
            OnDestroyEvent?.Invoke(); // tell spawner it’s gone
            Destroy(gameObject);
        }
    }
}