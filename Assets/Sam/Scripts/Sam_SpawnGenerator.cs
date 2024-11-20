using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sam_SpawnGenerator : MonoBehaviour
{
    [System.Serializable]
    public class SpawnArea
    {
        public string name;
        public Collider spawnCollider; // Reference to a collider
    }

    public GameObject[] itemsToSpawn; // Items to spawn
    public SpawnArea[] spawnAreas; // Areas to spawn items
    public int itemsPerArea = 5; // Number of items per area

    void Start()
    {
        foreach (var area in spawnAreas)
        {
            SpawnItemsInCollider(area);
        }
    }

    void SpawnItemsInCollider(SpawnArea area)
    {
        for (int i = 0; i < itemsPerArea; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInCollider(area.spawnCollider);
            if (spawnPosition != Vector3.zero)
            {
                GameObject item = Instantiate(
                    itemsToSpawn[Random.Range(0, itemsToSpawn.Length)],
                    spawnPosition,
                    Quaternion.identity
                );
                item.name = $"{item.name} ({area.name})";
            }
        }
    }

    Vector3 GetRandomPositionInCollider(Collider collider)
    {
        Vector3 randomPoint;
        int attempts = 100; // Prevent infinite loops
        do
        {
            randomPoint = new Vector3(
                Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                Random.Range(collider.bounds.min.z, collider.bounds.max.z)
            );
        } while (!collider.bounds.Contains(randomPoint) || !PointIsInsideCollider(collider, randomPoint) && --attempts > 0);

        return attempts > 0 ? randomPoint : Vector3.zero;
    }

    bool PointIsInsideCollider(Collider collider, Vector3 point)
    {
        // Raycast down to see if we hit the collider
        Ray ray = new Ray(point + Vector3.up * 100, Vector3.down);
        return collider.Raycast(ray, out RaycastHit hit, Mathf.Infinity);
    }
}