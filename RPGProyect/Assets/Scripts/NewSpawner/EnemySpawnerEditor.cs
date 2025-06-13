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

        if (GUILayout.Button("Spawn Next Waves"))
        {
            spawner.SpawnNextWaves();
        }

        if (GUILayout.Button("Destroy All Enemies"))
        {
            spawner.DestroyAllEnemies();
        }

        if (GUILayout.Button("Reset Waves"))
        {
            spawner.ResetWaves();
        }
    }
}


