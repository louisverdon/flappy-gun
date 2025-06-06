using UnityEngine;

// Corresponds to "Ennemis (\"Flappy Birds\")" - Apparition
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign in Inspector
    public float spawnInterval = 2f; // Time between spawns
    public float spawnEdgeOffset = 1f; // How far from camera edge to spawn
    // public float minSpawnY = -3f; // Min Y position for spawn
    // public float maxSpawnY = 3f;  // Max Y position for spawn

    private float timer;
    private bool isSpawning = false;

    void Start()
    {
        timer = spawnInterval; // Start with a spawn ready or with a delay Random.Range(0, spawnInterval)
        isSpawning = false; // Ensure it doesn't spawn until told to
    }

    void Update()
    {
        if (!isSpawning)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    public void StartSpawning()
    {
        isSpawning = true;
        timer = spawnInterval; // Reset timer to spawn an enemy quickly
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    void SpawnEnemy()
    {
        Camera mainCamera = GameManager.Instance.mainCamera;

        if (enemyPrefab == null || mainCamera == null) 
        {
            Debug.LogError("EnemyPrefab or MainCamera not set for Spawner! Check GameManager reference or scene setup.");
            return;
        }

        float screenHeightWorld = mainCamera.orthographicSize * 2;
        float screenWidthWorld = screenHeightWorld * mainCamera.aspect;

        float spawnY = Random.Range(mainCamera.transform.position.y - mainCamera.orthographicSize * 0.8f, 
                                  mainCamera.transform.position.y + mainCamera.orthographicSize * 0.8f);
        // Use minSpawnY and maxSpawnY if defined relative to world, otherwise use camera relative positions.

        bool spawnOnLeft = Random.value > 0.5f;
        float spawnX;
        int direction;

        if (spawnOnLeft)
        {
            spawnX = mainCamera.transform.position.x - (screenWidthWorld / 2) - spawnEdgeOffset;
            direction = 1; // Moves right
        }
        else
        {
            spawnX = mainCamera.transform.position.x + (screenWidthWorld / 2) + spawnEdgeOffset;
            direction = -1; // Moves left
        }

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        EnemyController enemyController = enemyInstance.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.SetDirection(direction);
        }
    }
} 