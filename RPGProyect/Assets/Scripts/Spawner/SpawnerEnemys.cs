using UnityEngine;

public class SpawnerEnemys : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int numberOfEnemies = 0, minCantEnemys = 2, maxCantEnemys = 8;
    private BoxCollider2D spawnArea;

    void Start()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        numberOfEnemies = Random.Range(minCantEnemys, maxCantEnemys);
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPosition = GetRandomPositionInBoxCollider2D();
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    // Función para generar las posiciones de los enemigos dentro de los límites del collider
    Vector2 GetRandomPositionInBoxCollider2D()
    {
        Bounds bounds = spawnArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(x, y);
    }
}
