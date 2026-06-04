using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour, IWeapon
{
    public GameObject arrowPrefab;
    public Transform firePoint;

    [Header("Debug")]
    [SerializeField]
    private ElementType debugElement = ElementType.Fire;

    private int damage = 2;

    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public void Attack()
    {
        if (arrowPrefab == null || firePoint == null) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldMouse = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = worldMouse - firePoint.position;

        GameObject arrowGO =
            Instantiate(
                arrowPrefab,
                firePoint.position,
                Quaternion.identity
            );

        Arrow arrow = arrowGO.GetComponent<Arrow>();

        arrow.SetDamage(damage);
        arrow.SetElement(debugElement);   // ← NEW
        arrow.Shoot(direction);
    }
}
