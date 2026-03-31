using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint; // empty GameObject at bow tip

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse != null)
        {
            
            if (mouse.leftButton.wasPressedThisFrame)
            {
                FireArrow();
            }
        }
    }

    void FireArrow()
    {
        if (arrowPrefab == null || firePoint == null) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = (worldMouse - firePoint.position);

        GameObject arrowGO = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        
        Arrow arrow = arrowGO.GetComponent<Arrow>();
        arrow.Shoot(direction);
    }

    
}
