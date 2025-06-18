using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnemySpawner spawner = (EnemySpawner)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Spawner Controls", EditorStyles.boldLabel);

        if (GUILayout.Button("Destroy All Enemies"))
        {
            spawner.DestroyAllEnemies();
        }

        if (GUILayout.Button("Reset All Waves"))
        {
            spawner.ResetWaves();
        }

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Spawn Next Wave by Config", EditorStyles.boldLabel);

        foreach (var config in spawner.spawnConfigs)
        {
            if (GUILayout.Button($"Spawn Next Wave in {config.name}"))
            {
                spawner.SpawnNextWaveForConfig(config);
            }
        }
    }
}



