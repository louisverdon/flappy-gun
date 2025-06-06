using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Procedurally spawns clouds in unexplored areas of the world based on a grid system.
/// </summary>
public class CloudSpawner : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("An array of cloud prefabs to be spawned randomly.")]
    public GameObject[] cloudPrefabs;
    [Tooltip("The player's transform, used to track the explored area.")]
    public Transform playerTransform;

    [Header("Spawning Grid")]
    [Tooltip("The size of each cell in the invisible world grid. Should be roughly the width of the screen.")]
    public float gridCellSize = 20f;
    [Tooltip("How many clouds to spawn when entering a new grid cell.")]
    public int cloudsPerCell = 5;

    [Header("Spawning Area")]
    [Tooltip("The minimum distance from the player where clouds can spawn.")]
    public float minSpawnRadius = 15f;
    [Tooltip("The maximum distance from the player where clouds can spawn.")]
    public float maxSpawnRadius = 30f;
    [Tooltip("The absolute minimum Y position for any cloud spawn.")]
    public float minSpawnY = 20f;

    // A set to keep track of grid cells where clouds have already been spawned.
    private HashSet<Vector2Int> spawnedCells = new HashSet<Vector2Int>();
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (playerTransform == null || cloudPrefabs.Length == 0) return;

        // Determine the player's current grid cell coordinate.
        Vector2Int currentCell = new Vector2Int(
            Mathf.FloorToInt(playerTransform.position.x / gridCellSize),
            Mathf.FloorToInt(playerTransform.position.y / gridCellSize)
        );

        // If we haven't spawned clouds for this cell yet, do it now.
        if (!spawnedCells.Contains(currentCell))
        {
            SpawnClouds(currentCell);
            // Mark this cell and its neighbors as spawned to avoid spawning too densely.
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    spawnedCells.Add(new Vector2Int(currentCell.x + x, currentCell.y + y));
                }
            }
        }
    }

    private void SpawnClouds(Vector2Int cell)
    {
        Debug.Log("Spawning clouds for new cell: " + cell);

        for (int i = 0; i < cloudsPerCell; i++)
        {
            // Pick a random cloud prefab.
            GameObject cloudPrefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];

            // Determine a spawn position in a rectangle just outside the camera's view.
            Vector2 screenPoint = new Vector2(Random.Range(-0.5f, 1.5f), Random.Range(-0.5f, 1.5f));
            // Ensure the point is actually outside the 0-1 viewport range.
            if (screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) {
                if(Random.value < 0.5f) screenPoint.x = Random.value < 0.5f ? -0.5f : 1.5f;
                else screenPoint.y = Random.value < 0.5f ? -0.5f : 1.5f;
            }

            Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, 10));
            spawnPosition.z = 0;

            // Enforce the minimum spawn height.
            if (spawnPosition.y < minSpawnY)
            {
                spawnPosition.y = minSpawnY;
            }

            // Instantiate the cloud.
            GameObject cloudInstance = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity, transform);

            // Add some variation to the clouds
            SpriteRenderer sr = cloudInstance.GetComponent<SpriteRenderer>();
            if(sr != null)
            {
                // Random size
                float randomScale = Random.Range(0.8f, 1.5f);
                cloudInstance.transform.localScale = new Vector3(randomScale, randomScale, 1);

                // Random transparency
                Color color = sr.color;
                color.a = Random.Range(0.4f, 0.8f);
                sr.color = color;
            }
        }
    }
} 