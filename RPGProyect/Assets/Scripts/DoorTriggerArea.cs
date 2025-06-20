using UnityEngine;
using static EnemySpawner;

public class DoorTriggerArea : MonoBehaviour
{
    private EnemySpawner spawner;

    private void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner == null)
        {
            Debug.LogError("[DoorTriggerArea] EnemySpawner not found in scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TilemapSpawnConfig config = spawner.GetConfigByDoor(gameObject);
            if (config != null)
            {
                spawner.SpawnNextWaveForConfig(config);
            }
        }
    }
}
