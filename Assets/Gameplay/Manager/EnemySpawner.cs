using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangedEnemyPrefab;
    Vector3[] meleeSpawnPoints = new Vector3[]
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(2f, 1f, 0f),
        new Vector3(-3f, 2f, 0f),
    };

    Vector3[] rangedSpawnPoints = new Vector3[]
    {
        new Vector3(1f, -2f, 0f),
        new Vector3(-2f, -1f, 0f)
    };


    

    void Start()
    {

        int enemyLayer = LayerMask.NameToLayer("Enemy");

        foreach (Vector3 pos in meleeSpawnPoints)
        {
            GameObject enemy = Instantiate(meleeEnemyPrefab, pos, Quaternion.identity);
            enemy.layer = enemyLayer;
        }

        foreach (Vector3 pos in rangedSpawnPoints)
        {
            GameObject enemy = Instantiate(rangedEnemyPrefab, pos, Quaternion.identity);
            enemy.layer = enemyLayer;
        }
    }

    void Update()
    {
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if(currentEnemies == 0)
        {
            // Respawn enemies
            SceneManager.LoadScene("Shop");
        }
    }


    

    
}
