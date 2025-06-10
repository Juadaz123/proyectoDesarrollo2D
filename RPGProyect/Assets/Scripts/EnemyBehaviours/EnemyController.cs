// EnemyController.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    // --- Variables de Configuración ---
    public Transform playerTransform;
    [SerializeField ] private Rigidbody2D enemyRb2D;

    [Tooltip("Lista de posibles acciones/comportamientos para el enemigo.")]
    [SerializeField] private List<EnemyActionEntry> enemyActions = new List<EnemyActionEntry>();

    // --- Referencias de Comportamiento ---
    private ITurnos currentEnemyBehaviour;
    private int currentActionIndex;

    // --- Enumerador para tipos de comportamiento (para el Inspector) ---
    public enum BehaviorType
    {
        FollowPlayer,
        FollowWithYOffset,
        SineFollow
    }

    // --- Estructura para almacenar los datos de cada acción del enemigo ---
    [System.Serializable]
    public class EnemyActionEntry
    {
        public string name;
        public BehaviorType behaviorType;
        public MovementData movementData; // El ScriptableObject MovementData para este comportamiento

    }

    private bool canPerformAction = true;


    // --- Métodos de Ciclo de Vida de Unity ---
    void Start()
    {
        if (enemyRb2D == null)
        {
            enemyRb2D = gameObject.GetComponent<Rigidbody2D>();
        }
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            else Debug.LogError("EnemyController: Player no encontrado y no asignado!");
        }

        PickRandomBehavior();
    }

    void FixedUpdate()
    {
        // Solo llamamos Turno si podemos realizar una acción y el comportamiento existe
        if (currentEnemyBehaviour != null && enemyActions.Count > 0 && canPerformAction)
        {
            canPerformAction = false; // Desactivamos el permiso para la siguiente acción
            // El 'context' pasado a Turno es el propio EnemyController
            currentEnemyBehaviour.Turno(Vector2.zero, enemyRb2D, enemyActions[currentActionIndex].movementData, this);
        }
    }

    // --- Métodos para Cambiar y Seleccionar Comportamientos ---

    public void OnBehaviorCompleted()
    {
        Debug.Log("Comportamiento del enemigo completado. Eligiendo siguiente acción...");
        PickRandomBehavior();
    }

    private void PickRandomBehavior()
    {
        if (enemyActions.Count == 0)
        {
            Debug.LogWarning("No hay acciones de enemigo configuradas en EnemyController.");
            currentEnemyBehaviour = null;
            return;
        }

        currentActionIndex = Random.Range(0, enemyActions.Count);
        SetEnemyBehaviour(currentActionIndex);
    }

    public void SetEnemyBehaviour(int index)
    {
        if (index < 0 || index >= enemyActions.Count)
        {
            Debug.LogError($"Índice de acción de enemigo fuera de rango: {index}. Acciones disponibles: {enemyActions.Count}");
            return;
        }

        currentActionIndex = index;
        EnemyActionEntry selectedAction = enemyActions[currentActionIndex];

        switch (selectedAction.behaviorType)
        {
            case BehaviorType.FollowPlayer:
                currentEnemyBehaviour = new EnemyBehaviourBase(playerTransform, this);
                break;
            case BehaviorType.FollowWithYOffset:
                // Ahora no se pasa el offset desde aquí, EnemyBehaviour2 lo decide
                currentEnemyBehaviour = new EnemyBehaviour2(playerTransform, this);
                break;
            case BehaviorType.SineFollow:
                // Ahora no se pasan amplitud/frecuencia desde aquí, EnemyBehaviourSineFollow los decide
                currentEnemyBehaviour = new EnemyBehaviour3(playerTransform, this);
                break;
            default:
                Debug.LogWarning($"Tipo de comportamiento no implementado: {selectedAction.behaviorType}. Usando FollowPlayer por defecto.");
                currentEnemyBehaviour = new EnemyBehaviourBase(playerTransform, this);
                break;
        }

        Debug.Log($"Enemigo ahora usando comportamiento: {selectedAction.name} ({selectedAction.behaviorType})");
    }
}