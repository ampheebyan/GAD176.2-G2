using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sam_PolySpawning : MonoBehaviour
{
    public GameObject[] itemsToSpawn;        // Items to spawn
    public Transform[] spawnRegions;        // Defined spawn regions (e.g., empty GameObjects)
    public int numberOfItemsToSpawn = 10;   // Total items to spawn
    public float spawnHeight = 1f;          // Adjust height of spawn items
    public LayerMask wallLayer;             // Layer assigned to walls for collision checks
    public float collisionCheckRadius = 0.5f; // Radius for collision checking

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        for (int i = 0; i < numberOfItemsToSpawn; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                Instantiate(
                    itemsToSpawn[Random.Range(0, itemsToSpawn.Length)],
                    spawnPosition,
                    Quaternion.identity
                );
            }
            else
            {
                Debug.LogWarning("Failed to find a valid spawn position after multiple attempts.");
            }
        }
    }

    Vector3 GetValidSpawnPosition()
    {
        int attempts = 50; // Avoid infinite loops by limiting attempts
        while (attempts > 0)
        {
            // Get a random spawn region
            Transform randomRegion = spawnRegions[Random.Range(0, spawnRegions.Length)];

            // Generate a random position within the region
            Vector3 randomPosition = GenerateRandomPointInRegion(randomRegion);

            // Check for collisions with walls or invalid spaces
            if (IsPositionValid(randomPosition))
            {
                return randomPosition;
            }

            attempts--;
        }

        return Vector3.zero; // Return zero vector if no valid position is found
    }

    Vector3 GenerateRandomPointInRegion(Transform region)
    {
        Vector3 center = region.position;
        Vector3 size = region.localScale / 2; // Assuming region is a cube or similar shape

        return new Vector3(
            Random.Range(center.x - size.x, center.x + size.x),
            spawnHeight, // Set the height for the spawned object
            Random.Range(center.z - size.z, center.z + size.z)
        );
    }

    bool IsPositionValid(Vector3 position)
    {
        // Check for collisions with the wall layer
        Collider[] hitColliders = Physics.OverlapSphere(position, collisionCheckRadius, wallLayer);

        // Position is valid if no colliders are detected
        return hitColliders.Length == 0;
    }

    void OnDrawGizmos()
    {
        // Visualize spawn regions in the editor
        Gizmos.color = Color.green;
        if (spawnRegions != null)
        {
            foreach (Transform region in spawnRegions)
            {
                Gizmos.DrawWireCube(region.position, region.localScale);
            }
        }
    }
}