using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class TilemapSpawnConfig
    {
        public string name;
        public Tilemap tilemap;
        public List<GameObject> enemyPrefabs;
        public int enemiesPerWave = 5;
        public int totalWaves = 3;

        [HideInInspector] public int currentWave = 0;
    }

    public List<TilemapSpawnConfig> spawnConfigs = new List<TilemapSpawnConfig>();
    private bool spawning = false;
    public bool startAutomatically = true;

    private void Start()
    {
        ResetWaves();
        if (startAutomatically)
        {
            SpawnNextWaves();
        }
    }


    private void Update()
    {
        if (!spawning && AreAllEnemiesDead())
        {
            SpawnNextWaves();
        }
    }

    public bool AreAllEnemiesDead()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length == 0;
    }

    public void SpawnNextWaves()
    {
        spawning = true;

        bool anyWavesLeft = false;

        foreach (var config in spawnConfigs)
        {
            Debug.Log($"[Spawner] Config: {config.name}, CurrentWave: {config.currentWave}, TotalWaves: {config.totalWaves}");

            if (config.currentWave < config.totalWaves)
            {
                anyWavesLeft = true;
                SpawnEnemiesInConfig(config);
                config.currentWave++;
            }
        }

        if (!anyWavesLeft)
        {
            Debug.Log("All waves completed.");
        }

        spawning = false;
    }



    private void SpawnEnemiesInConfig(TilemapSpawnConfig config)
    {


        List<Vector3> validPositions = GetValidTilePositions(config.tilemap);

        if (validPositions.Count == 0)
        {
            Debug.LogWarning($"No valid tiles in {config.tilemap.name}");
            return;
        }

        for (int i = 0; i < config.enemiesPerWave; i++)
        {
            Vector3 spawnPos = validPositions[Random.Range(0, validPositions.Count)];
            GameObject enemyPrefab = config.enemyPrefabs[Random.Range(0, config.enemyPrefabs.Count)];
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemyInstance.tag = "Enemy";
        }

        Debug.Log($"Spawned wave {config.currentWave + 1}/{config.totalWaves} in {config.tilemap.name}");
    }


    private List<Vector3> GetValidTilePositions(Tilemap tilemap)
    {
        List<Vector3> validPositions = new List<Vector3>();

        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap is null in GetValidTilePositions.");
            return validPositions;
        }

        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int origin = tilemap.origin;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Vector3 worldPos = tilemap.CellToWorld(pos) + tilemap.cellSize / 2;
                validPositions.Add(worldPos);
            }
        }

        return validPositions;
    }


    public void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if(enemy != null)
            DestroyImmediate(enemy);
        }
    }

    public void ResetWaves()
    {
        foreach (var config in spawnConfigs)
        {
            config.currentWave = 0;
        }
        Debug.Log("Waves reset.");
    }
}

