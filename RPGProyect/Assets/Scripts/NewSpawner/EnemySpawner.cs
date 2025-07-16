using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class TilemapSpawnConfig
    {
        public string name;
        public Tilemap spawnTilemap;
        public List<GameObject> enemyPrefabs;
        public int totalWaves;
        public int enemiesPerWave;
        public float spawnDelay = 0.5f;
        public GameObject doorTriggerArea;

        [Header("Optional Reward Drop")]
        public GameObject dropPrefab;
        public Transform dropLocationOverride; 

        [HideInInspector] public int currentWave = 0;
        [HideInInspector] public bool rewardDropped = false;
    }


    public List<TilemapSpawnConfig> spawnConfigs;
    public bool startAutomatically = false;
    [SerializeField] private GameObject spawnIndicatorPrefab;
    [SerializeField] private float indicatorDuration = 1.5f;


    private void Start()
    {
        if (startAutomatically)
        {
            foreach (var config in spawnConfigs)
            {
                SpawnWaveByArea(config);
            }
        }
    }

    public void SpawnWaveByArea(TilemapSpawnConfig config)
    {
        if (config.currentWave < config.totalWaves)
        {
            SpawnEnemiesInConfig(config);
            config.currentWave++;
            Debug.Log($"[Spawner] Spawned wave {config.currentWave}/{config.totalWaves} for {config.name}");
        }
        else
        {
            Debug.Log($"[Spawner] All waves already spawned for {config.name}");
        }
    }

    private void SpawnEnemiesInConfig(TilemapSpawnConfig config)
    {
        List<Vector3> spawnPositions = GetValidTilePositions(config.spawnTilemap);
        if (spawnPositions.Count == 0)
        {
            Debug.LogWarning($"[Spawner] No valid spawn positions found for {config.name}");
            return;
        }

        StartCoroutine(SpawnWithIndicators(spawnPositions, config));
    }

    private IEnumerator SpawnWithIndicators(List<Vector3> allPositions, TilemapSpawnConfig config)
    {
        List<Vector3> chosenPositions = new List<Vector3>();

        // Randomly select positions for this wave
        for (int i = 0; i < config.enemiesPerWave; i++)
        {
            if (allPositions.Count == 0) break;

            int randomIndex = Random.Range(0, allPositions.Count);
            Vector3 chosenPos = allPositions[randomIndex];
            chosenPositions.Add(chosenPos);

            // Remove it from the pool to avoid duplicate spawns
            allPositions.RemoveAt(randomIndex);
        }

        // Spawn indicators only at selected positions
        foreach (var pos in chosenPositions)
        {
            if (spawnIndicatorPrefab != null)
            {
                GameObject indicator = Instantiate(spawnIndicatorPrefab, pos, Quaternion.identity);
                Destroy(indicator, indicatorDuration);
            }
        }

        // Wait before spawning enemies
        yield return new WaitForSeconds(indicatorDuration);

        // Spawn enemies at those exact positions
        foreach (var spawnPos in chosenPositions)
        {
            GameObject enemyPrefab = config.enemyPrefabs[Random.Range(0, config.enemyPrefabs.Count)];
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log($"[Spawner] Enemies spawned after indicators for {config.name}");
    }



    private List<Vector3> GetValidTilePositions(Tilemap tilemap)
    {
        List<Vector3> positions = new List<Vector3>();
        if (tilemap == null) return positions;

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Vector3Int tilePos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                    positions.Add(tilemap.GetCellCenterWorld(tilePos));
                }
            }
        }

        return positions;
    }

    //Destroy all active enemies
    public void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if(enemy != null)
            DestroyImmediate(enemy);
        }
        Debug.Log("[Spawner] All enemies destroyed.");
    }

    // ?? NEW: Reset wave counts for all areas
    public void ResetWaves()
    {
        foreach (var config in spawnConfigs)
        {
            config.currentWave = 0;
            config.rewardDropped = false;
        }
        Debug.Log("[Spawner] Waves and reward drop states reset.");
    }


    //Spawn the next wave for a specific config
    public void SpawnNextWaveForConfig(TilemapSpawnConfig config)
    {
        if (config.currentWave < config.totalWaves)
        {
            SpawnEnemiesInConfig(config);
            config.currentWave++;
            Debug.Log($"[Spawner] Spawned wave {config.currentWave}/{config.totalWaves} for {config.name}");
        }
        else if (!config.rewardDropped && config.dropPrefab != null)
        {
            Vector3 dropPosition;

            if (config.dropLocationOverride != null)
            {
                dropPosition = config.dropLocationOverride.position;
            }
            else if (config.spawnTilemap != null)
            {
                dropPosition = config.spawnTilemap.localBounds.center;
            }
            else
            {
                dropPosition = Vector3.zero;
            }

            Instantiate(config.dropPrefab, dropPosition, Quaternion.identity);
            config.rewardDropped = true;
            Debug.Log($"[Spawner] Dropped reward prefab for {config.name}");
        }
        else
        {
            Debug.Log($"[Spawner] All waves completed for {config.name} (No drop or already dropped)");
        }
    }


    //Get a config by its assigned door trigger object
    public TilemapSpawnConfig GetConfigByDoor(GameObject doorTrigger)
    {
        foreach (var config in spawnConfigs)
        {
            if (config.doorTriggerArea == doorTrigger)
            {
                return config;
            }
        }
        Debug.LogWarning($"[Spawner] No spawn config found for door {doorTrigger.name}");
        return null;
    }
}

