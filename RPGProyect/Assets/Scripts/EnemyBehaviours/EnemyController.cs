// EnemyController.cs - CORREGIDO Y CON ANIMACIONES
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    // --- Variables de Configuración ---
    public Transform playerTransform;
    [SerializeField] private Rigidbody2D enemyRb2D;

    [Tooltip("Lista de posibles acciones/comportamientos para el enemigo.")]
    [SerializeField] private List<EnemyActionEntry> enemyActions = new List<EnemyActionEntry>();

    // --- Referencias de Comportamiento ---
    private ITurnos currentEnemyBehaviour;
    private int currentActionIndex;

    // --- ¡NUEVO! Referencia a SlimeAnimation ---
    private SlimeAnimation slimeAnimation;

    // --- Enumerador para tipos de comportamiento (para el Inspector) ---
    public enum BehaviorType
    {
        MoveEnemy,
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
        public MovementData movementData; // Asumo que MovementData es una clase/struct definida en otro lugar.
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

        // --- ¡NUEVO! Obtener la referencia a SlimeAnimation ---
        slimeAnimation = GetComponent<SlimeAnimation>();
        if (slimeAnimation == null)
        {
            Debug.LogError("EnemyController: No se encontró el componente SlimeAnimation en este GameObject.");
        }

        PickRandomBehavior();
    }

    private void Update() 
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
        // Debug.Log("Comportamiento del enemigo completado. Eligiendo siguiente acción...");
        // SOLUCION 1: Resetear canPerformAction para permitir la próxima acción
        canPerformAction = true;
        
        // --- ¡NUEVO! Resetear las animaciones antes de elegir la siguiente acción ---
        if (slimeAnimation != null)
        {
            slimeAnimation.ResetAllActionAnimations();
        }
        
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
        //elige una de los comportamients  que tiene añadidos
        currentActionIndex = Random.Range(0, enemyActions.Count);
        SetEnemyBehaviour(currentActionIndex);

        // --- ¡NUEVO! Rotar al slime hacia el jugador al elegir un nuevo comportamiento ---
        if (slimeAnimation != null && playerTransform != null)
        {
            slimeAnimation.RotateSlime(playerTransform.position);
        }
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

        // Asegurarse de que slimeAnimation no es null antes de llamar a sus métodos
        if (slimeAnimation == null)
        {
            Debug.LogError("SlimeAnimation no está asignado en EnemyController. No se pueden reproducir animaciones.");
            return;
        }

        switch (selectedAction.behaviorType)
        {
            case BehaviorType.MoveEnemy:
                currentEnemyBehaviour = new EnemyBehaviourBase(playerTransform, this);
                slimeAnimation.SetWalkingAnimation(true); // Activar animación de caminata
                break;
            case BehaviorType.FollowPlayer:
                currentEnemyBehaviour = new EnemyBehaviourBase(playerTransform, this);
                slimeAnimation.PlayAttackAnimation(1); // Activar animación de ataque 1
                break;
            case BehaviorType.FollowWithYOffset:
                currentEnemyBehaviour = new EnemyBehaviour2(playerTransform, this);
                slimeAnimation.PlayAttackAnimation(1); // Activar animación de ataque 1
                break;
            case BehaviorType.SineFollow:
                currentEnemyBehaviour = new EnemyBehaviour3(playerTransform, this);
                slimeAnimation.PlayAttackAnimation(2); // Activar animación de ataque 2
                break;
            default:
                Debug.LogWarning($"Tipo de comportamiento no implementado: {selectedAction.behaviorType}. Usando FollowPlayer por defecto.");
                currentEnemyBehaviour = new EnemyBehaviourBase(playerTransform, this);
                slimeAnimation.PlayAttackAnimation(1); // Por defecto, una animación de ataque
                break;
        }

        Debug.Log($"Enemigo ahora usando comportamiento: {selectedAction.name} ({selectedAction.behaviorType})");
    }
}