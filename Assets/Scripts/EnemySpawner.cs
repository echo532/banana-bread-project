using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 5;
    [SerializeField] private float spawnMargin = 1f; // Margin from screen edges
    [SerializeField] private float minSpawnDistance = 2f; // Minimum distance between spawned enemies
    
    private Camera mainCamera;
    private List<Vector3> spawnedPositions = new List<Vector3>();
    
    void Start()
    {
        mainCamera = Camera.main;
        SpawnEnemies();
    }
    
    void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned to EnemySpawner!");
            return;
        }
        
        Debug.Log($"Starting to spawn {enemyCount} enemies...");
        
        Vector2 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        Debug.Log($"Screen bounds: {screenBounds}");
        
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition(screenBounds);
            spawnedPositions.Add(spawnPosition);
            
            // Instantiate enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.name = "Enemy_" + i;
            
            Debug.Log($"Spawned {enemy.name} at {spawnPosition}");
        }
        
        Debug.Log($"Total enemies spawned: {spawnedPositions.Count}");
        Debug.Log($"Total enemy GameObjects in scene: {GameObject.FindGameObjectsWithTag("Untagged").Length}");
    }
    
    Vector3 GetValidSpawnPosition(Vector2 screenBounds)
    {
        Vector3 position;
        int attempts = 0;
        int maxAttempts = 50;
        
        do
        {
            // Random position within screen bounds
            float x = Random.Range(-screenBounds.x + spawnMargin, screenBounds.x - spawnMargin);
            float y = Random.Range(-screenBounds.y + spawnMargin, screenBounds.y - spawnMargin);
            position = new Vector3(x, y, 0f);
            attempts++;
            
            // If we've tried too many times, just use this position
            if (attempts >= maxAttempts)
            {
                break;
            }
        }
        while (!IsPositionValid(position));
        
        return position;
    }
    
    bool IsPositionValid(Vector3 position)
    {
        // Check if position is far enough from all previously spawned enemies
        foreach (Vector3 spawnedPos in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPos) < minSpawnDistance)
            {
                return false;
            }
        }
        return true;
    }
}