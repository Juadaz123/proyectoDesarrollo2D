// EnemyController.cs - CLASE BASE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    // --- Variables de Configuración ---
    public Transform playerTransform;
    [SerializeField] protected Rigidbody2D enemyRb2D;

    [Tooltip("Lista de posibles acciones/comportamientos para el enemigo.")]
    [SerializeField] protected List<EnemyActionEntry> enemyActions = new List<EnemyActionEntry>();

    // --- Referencias de Comportamiento ---
    protected ITurnos currentEnemyBehaviour;
    protected int currentActionIndex;

    // --- Referencia a SlimeAnimation (protegida para acceso por clases derivadas) ---
    protected SlimeAnimation slimeAnimation;

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

    protected bool canPerformAction = true;

    // --- Métodos de Ciclo de Vida de Unity ---
    void Start()
    {
        InitializeController();
        PickRandomBehavior();
    }

    // Método de inicialización separado para que las clases derivadas puedan llamarlo si es necesario
    protected virtual void InitializeController()
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

        slimeAnimation = GetComponent<SlimeAnimation>();
        if (slimeAnimation == null)
        {
            Debug.LogError("EnemyController: No se encontró el componente SlimeAnimation en este GameObject.");
        }
    }


    protected virtual void Update()
    {
        if (currentEnemyBehaviour != null && enemyActions.Count > 0 && canPerformAction)
        {
            canPerformAction = false;
            currentEnemyBehaviour.Turno(Vector2.zero, enemyRb2D, enemyActions[currentActionIndex].movementData, this);
        }
    }

    // --- Métodos para Cambiar y Seleccionar Comportamientos ---

    public void OnBehaviorCompleted()
    {
        canPerformAction = true;

        if (slimeAnimation != null)
        {
            slimeAnimation.ResetAllActionAnimations();
        }

        PickRandomBehavior();
    }

    protected void PickRandomBehavior()
    {
        if (enemyActions.Count == 0)
        {
            Debug.LogWarning("No hay acciones de enemigo configuradas en EnemyController.");
            currentEnemyBehaviour = null;
            return;
        }
        currentActionIndex = Random.Range(0, enemyActions.Count);
        SetEnemyBehaviour(currentActionIndex);

        if (slimeAnimation != null && playerTransform != null)
        {
            slimeAnimation.RotateSlime(playerTransform.position);
        }
    }

    public virtual void SetEnemyBehaviour(int index)
    {
        if (index < 0 || index >= enemyActions.Count)
        {
            Debug.LogError($"Índice de acción de enemigo fuera de rango: {index}. Acciones disponibles: {enemyActions.Count}");
            return;
        }

        currentActionIndex = index;
        EnemyActionEntry selectedAction = enemyActions[currentActionIndex];

        if (slimeAnimation == null)
        {
            Debug.LogError("SlimeAnimation no está asignado en EnemyController. No se pueden reproducir animaciones.");
            return;
        }

        switch (selectedAction.behaviorType)
        {
            case BehaviorType.MoveEnemy:
                currentEnemyBehaviour = new EnemyBehaviourBase(playerTransform, this);
                slimeAnimation.SetWalkingAnimation(true);
                break;
            case BehaviorType.FollowPlayer:
                currentEnemyBehaviour = new EnemyBehaviourBase(playerTransform, this);
                slimeAnimation.PlayAttackAnimation(1);
                break;
            case BehaviorType.FollowWithYOffset:
                currentEnemyBehaviour = new EnemyBehaviour2(playerTransform, this);
                slimeAnimation.PlayAttackAnimation(1);
                break;
            case BehaviorType.SineFollow:
                currentEnemyBehaviour = new EnemyBehaviour3(playerTransform, this);
                slimeAnimation.PlayAttackAnimation(2);
                break;
            default:
                Debug.LogWarning($"Tipo de comportamiento no implementado: {selectedAction.behaviorType}. Usando FollowPlayer por defecto.");
                currentEnemyBehaviour = new EnemyBehaviourBase(playerTransform, this);
                slimeAnimation.PlayAttackAnimation(1);
                break;
        }

        Debug.Log($"Enemigo ahora usando comportamiento: {selectedAction.name} ({selectedAction.behaviorType})");
    }

    // Método auxiliar para que los patrones del jefe puedan iniciar un FollowPlayer
    protected void ExecuteFollowPlayer(MovementData data)
    {
        currentEnemyBehaviour = new EnemyBehaviourBase(playerTransform, this);
        currentEnemyBehaviour.Turno(Vector2.zero, enemyRb2D, data, this);
        if (slimeAnimation != null)
        {
            slimeAnimation.PlayAttackAnimation(1);
        }
        // Debug.Log("Ejecutando comportamiento: FollowPlayer.");
    }

        protected void ExecuteFollowWithYOffset(MovementData data)
    {
        currentEnemyBehaviour = new EnemyBehaviour2(playerTransform, this);
        currentEnemyBehaviour.Turno(Vector2.zero, enemyRb2D, data, this);
        if (slimeAnimation != null)
        {
            slimeAnimation.PlayAttackAnimation(1); // O la animación apropiada para este ataque
        }
        Debug.Log("Ejecutando comportamiento: Follow With Y Offset.");
    }

    // Método auxiliar para que los patrones del jefe puedan iniciar un SineFollow
    protected void ExecuteSineFollow(MovementData data)
    {
        currentEnemyBehaviour = new EnemyBehaviour3(playerTransform, this);
        currentEnemyBehaviour.Turno(Vector2.zero, enemyRb2D, data, this);
        if (slimeAnimation != null)
        {
            slimeAnimation.PlayAttackAnimation(2); // O la animación apropiada para este ataque
        }
        Debug.Log("Ejecutando comportamiento: Sine Follow.");
    }
}