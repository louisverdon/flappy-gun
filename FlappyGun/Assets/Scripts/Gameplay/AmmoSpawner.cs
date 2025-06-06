using UnityEngine;
using System.Collections;

/// <summary>
/// Spawns ammo pickups at random intervals and positions within the camera's view.
/// </summary>
public class AmmoSpawner : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The ammo pickup prefab to spawn.")]
    public GameObject ammoPrefab;
    
    [Header("Spawning Behavior")]
    [Tooltip("The minimum time (in seconds) between each spawn.")]
    public float minSpawnInterval = 5f;
    [Tooltip("The maximum time (in seconds) between each spawn.")]
    public float maxSpawnInterval = 10f;
    [Tooltip("A buffer from the screen edges to prevent spawning too close to the border.")]
    [Range(0.05f, 0.4f)]
    public float screenEdgeBuffer = 0.1f;

    private Camera mainCamera;
    private bool isSpawning = false;

    void Start()
    {
        mainCamera = Camera.main;
        if (ammoPrefab == null) {
            Debug.LogError("AmmoSpawner: Ammo Prefab is not assigned!", this);
            enabled = false;
        }
    }

    public void StartSpawning()
    {
        if (isSpawning) return;
        isSpawning = true;
        StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    private IEnumerator SpawnRoutine()
    {
        // Initial delay before the first spawn
        yield return new WaitForSeconds(minSpawnInterval);

        while (isSpawning)
        {
            // Wait for a random amount of time before spawning the next pickup
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
            
            // Spawn the pickup
            SpawnAmmoPickup();
        }
    }

    private void SpawnAmmoPickup()
    {
        if (mainCamera == null) return;

        // Calculate a random spawn position within the camera's viewport, respecting the buffer.
        float randomX = Random.Range(screenEdgeBuffer, 1 - screenEdgeBuffer);
        float randomY = Random.Range(screenEdgeBuffer, 1 - screenEdgeBuffer);
        
        Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(randomX, randomY, 10f));
        spawnPosition.z = 0; // Ensure it's on the 2D plane

        Instantiate(ammoPrefab, spawnPosition, Quaternion.identity, transform);
    }
} 