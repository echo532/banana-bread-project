using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    Vector3[] spawnPoints = new Vector3[]
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(-3f, 2f, 0f),
        new Vector3(1f, -2f, 0f),
        new Vector3(-2f, -1f, 0f)
    };

    

    void Start()
    {

        int enemyLayer = LayerMask.NameToLayer("Enemy");

        foreach (Vector3 pos in spawnPoints)
        {
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            enemy.layer = enemyLayer;
        }
    }
    

    
}
