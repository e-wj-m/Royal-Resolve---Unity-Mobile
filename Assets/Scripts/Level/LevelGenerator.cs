using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;

/// <summary>
/// A cleaner, more teachable version of the Level Generator.
/// Separates "Movement", "Management", and "Generation" into clear steps.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("How fast the world moves left (simulating player running right)")]
    public float levelSpeed = 5f;

    [Header("Generation Settings")]
    public GameObject chunkPrefab;
    public int chunkWidth = 10; // Changed to int for cleaner tile logic
    public int groundHeightLevel = -5; // No more magic number
    
    [Header("Tiles")]
    public TileBase groundTile;
    public TileBase obstacleTile;
    [Range(0f, 1f)] public float obstacleRate = 0.2f;

    [Header("Optimization")]
    public int initialPoolSize = 5;

    // State
    private Camera mainCamera;
    private ObjectPool<GameObject> chunkPool;
    private List<GameObject> activeChunks = new List<GameObject>();
    private float nextSpawnXPosition = 0f;

    void Start()
    {
        mainCamera = Camera.main;

        // Initialize the pool
        chunkPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(chunkPrefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            defaultCapacity: initialPoolSize
        );

        // Pre-warm the level so the screen isn't empty at start
        FillScreenWithChunks();
    }

    void Update()
    {
        // 1. Move everything
        MoveLevel();

        // 2. Check if we need to recycle old chunks
        RecycleOffScreenChunks();

        // 3. Check if we need to spawn new chunks
        SpawnChunksAsNeeded();
    }

    private void MoveLevel()
    {
        // We move the world to the left to create the illusion of movement
        float distanceToMove = levelSpeed * Time.deltaTime;

        foreach (GameObject chunk in activeChunks)
        {
            chunk.transform.position += Vector3.left * distanceToMove;
        }

        // We also need to adjust our "cursor" for where the next piece goes
        nextSpawnXPosition -= distanceToMove;
    }

    private void RecycleOffScreenChunks()
    {
        if (activeChunks.Count == 0) return;

        GameObject leftMostChunk = activeChunks[0];
        
        // Calculate the right edge of this chunk
        float chunkRightEdge = leftMostChunk.transform.position.x + chunkWidth;
        float screenLeftEdge = GetScreenLeftEdge();

        // If the chunk is completely past the left edge of the screen...
        if (chunkRightEdge < screenLeftEdge)
        {
            activeChunks.RemoveAt(0); // Remove from our tracking list
            chunkPool.Release(leftMostChunk); // Return to pool
        }
    }

    private void SpawnChunksAsNeeded()
    {
        float screenRightEdge = GetScreenRightEdge();

        // While our "next spawn position" is visible on screen (or just about to be)
        while (nextSpawnXPosition < screenRightEdge)
        {
            SpawnChunk();
        }
    }

    private void SpawnChunk()
    {
        GameObject newChunk = chunkPool.Get();

        // Position it correctly
        newChunk.transform.position = new Vector3(nextSpawnXPosition, 0, 0);

        // Generate the contents of this chunk
        GenerateTileContents(newChunk);

        // Add to our list so we can move it later
        activeChunks.Add(newChunk);

        // Advance the cursor
        nextSpawnXPosition += chunkWidth;
    }

    private void GenerateTileContents(GameObject chunk)
    {
        Tilemap tilemap = chunk.GetComponentInChildren<Tilemap>();
        tilemap.ClearAllTiles();

        for (int x = 0; x < chunkWidth; x++)
        {
            // Place Floor
            Vector3Int floorPos = new Vector3Int(x, groundHeightLevel, 0);
            tilemap.SetTile(floorPos, groundTile);

            // Place Obstacle?
            if (Random.value < obstacleRate)
            {
                Vector3Int obstaclePos = new Vector3Int(x, groundHeightLevel + 1, 0);
                tilemap.SetTile(obstaclePos, obstacleTile);
            }
        }
    }

    private void FillScreenWithChunks()
    {
        float screenRightEdge = GetScreenRightEdge();
        // Just keep spawning until we cover the screen
        while (nextSpawnXPosition < screenRightEdge + chunkWidth)
        {
            SpawnChunk();
        }
    }

    // Helper functions to make the math readable
    private float GetScreenLeftEdge()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        return mainCamera.transform.position.x - (cameraWidth / 2);
    }

    private float GetScreenRightEdge()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        return mainCamera.transform.position.x + (cameraWidth / 2);
    }
}
