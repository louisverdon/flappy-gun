using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages a collection of ground tiles to create an infinitely scrolling ground plane.
/// This version is designed to be robust against floating-point errors and sprite setup issues.
/// </summary>
public class GroundController : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The ground tile prefab. It must have a Collider2D and be tagged 'Ground'.")]
    public GameObject groundTilePrefab;

    [Tooltip("The player's transform, which the ground will follow.")]
    public Transform playerTransform;

    [Header("Behavior")]
    [Tooltip("The number of ground tiles to maintain active at once. 5-7 is a good range.")]
    public int numberOfTiles = 7;

    [Tooltip("The vertical position (Y-axis) where the ground tiles will be placed.")]
    public float groundLevelY = -5f;

    // A tiny overlap to prevent visual gaps between tiles due to floating point inaccuracies.
    private const float tileOverlap = 0.01f;

    private List<GameObject> activeTiles;
    private float tileWidth;

    void Start()
    {
        // --- Error Checking ---
        if (groundTilePrefab == null || playerTransform == null) {
            Debug.LogError("GroundController: Ground Tile Prefab or Player Transform is not assigned!", this);
            enabled = false; return;
        }
        SpriteRenderer renderer = groundTilePrefab.GetComponentInChildren<SpriteRenderer>();
        if (renderer == null || renderer.sprite == null) {
             Debug.LogError("GroundController: The Ground Tile Prefab or its children must contain a SpriteRenderer with a Sprite assigned.", this);
            enabled = false; return;
        }

        // --- Width Calculation ---
        // Use the renderer's bounds for the most reliable world-space width.
        tileWidth = renderer.bounds.size.x;

        if (tileWidth <= 0) {
            Debug.LogError("GroundController: Calculated tile width is zero. Check the ground prefab's sprite and scale.", this);
            enabled = false; return;
        }
        
        Debug.Log("[GroundController] Calculated tile width: " + tileWidth);

        // --- Initial Tile Placement ---
        activeTiles = new List<GameObject>();
        for (int i = 0; i < numberOfTiles; i++)
        {
            // Position tiles exactly next to each other based on their index.
            float xPos = (i - numberOfTiles / 2) * tileWidth;
            Vector3 position = new Vector3(xPos, groundLevelY, 0);
            GameObject tile = Instantiate(groundTilePrefab, position, Quaternion.identity, transform);
            activeTiles.Add(tile);
        }
    }

    void LateUpdate()
    {
        GameObject rightmostTile = activeTiles.OrderByDescending(t => t.transform.position.x).First();
        GameObject leftmostTile = activeTiles.OrderBy(t => t.transform.position.x).First();

        // Check if the player has moved far enough into the last tiles to require recycling.
        // This provides a safe buffer zone.
        if (playerTransform.position.x > rightmostTile.transform.position.x - (tileWidth * 0.5f))
        {
            // Move the left-most tile to the far right, with a slight overlap to guarantee no gaps.
            leftmostTile.transform.position = new Vector3(rightmostTile.transform.position.x + tileWidth - tileOverlap, groundLevelY, 0);
        }
        else if (playerTransform.position.x < leftmostTile.transform.position.x + (tileWidth * 0.5f))
        {
            // Move the right-most tile to the far left, with a slight overlap.
            rightmostTile.transform.position = new Vector3(leftmostTile.transform.position.x - tileWidth + tileOverlap, groundLevelY, 0);
        }
    }
} 