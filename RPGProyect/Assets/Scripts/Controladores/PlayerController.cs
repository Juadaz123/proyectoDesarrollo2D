using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem; // Importar el nuevo Input System

public class PlayerController : MonoBehaviour
{
    // --- Variables de Configuración (visibles en el Inspector) ---
    [SerializeField] private List<ActionData> actions = new List<ActionData>(); // Lista de acciones con datos
    [SerializeField] private int arrowCount = 10; // Número de flechas disponibles
    [SerializeField] private MovementData defaultmovementdata; // Un MovementData por defecto si alguna acción no tiene uno propio

    // --- Referencias a Componentes y Scripts ---
    private Rigidbody2D rb2D; // Componente de física para el movimiento
    private ITurnos turnos; // Interfaz para el sistema de turnos (ej. MovePlayer)
    private ControlTurnos controlTurnos; // Script principal que gestiona los turnos
    private SoldierAnimationScript soldierAnim; // Script para el control de animaciones del jugador
    private PlayerInputAction playerInput; // Referencia al Input Actions Asset
    private ActionMenuUI actionMenuUI; // Referencia al script de UI del menú de acciones

    // --- NUEVO: Referencia al GameObject del collider de ataque del jugador ---
    [Header("Configuración de Ataque del Jugador")]
    [Tooltip("Arrastra aquí el GameObject del collider de ataque del jugador (el que tiene el script DamageDealer).")]
    [SerializeField] private GameObject _playerMeleeAttackColliderObject;
    private DamageDealer _playerMeleeDamageDealer; // Referencia al script DamageDealer en ese GameObject

    // --- Variables de Estado ---
    private int currentActionIndex = 0; // Índice de la acción seleccionada actualmente
    public bool CanMove = true; // Permite o restringe el movimiento (considera si es necesaria con el sistema de turnos)

    // --- Estructura para almacenar los datos de cada acción (¡Única definición!) ---
    [System.Serializable]
    public class ActionData
    {
        public string name;
        [Tooltip("El ScriptableObject MovementData asociado a esta acción, que ahora también contiene el daño y tipo de daño.")]
        public MovementData movementData; // Este MovementData ahora contiene el daño y tipo de daño
        [Tooltip("Índice para la animación de ataque (ej. 1, 2, 3 para arco).")]
        public int attackAnimationIndex;
        [Tooltip("Indica si esta acción es un ataque (para lógica de flechas, aplicación de daño).")]
        public bool isAttack;
        [Tooltip("Indica si esta acción es de curación.")]
        public bool isHeal;
    }

    // --- Métodos de Ciclo de Vida de Unity ---

    private void Awake()
    {
        playerInput = new PlayerInputAction(); // Instanciar el Input Actions Asset

        // Suscribir métodos a los eventos del Input System
        playerInput.Gameplay.Mover.performed += OnClickPerformed; 
        
        playerInput.Gameplay.Ataque1.performed += ctx => SelectAction(1); 
        playerInput.Gameplay.Ataque2.performed += ctx => SelectAction(2); 
        playerInput.Gameplay.Ataque3.performed += ctx => SelectAction(3); 
        playerInput.Gameplay.Ataque4.performed += ctx => SelectAction(4); 
    }

    private void OnEnable()
    {
        playerInput.Enable(); // Habilitar el Action Map 'Gameplay' al activar el objeto
    }

    private void OnDisable()
    {
        playerInput.Disable(); // Deshabilitar el Action Map 'Gameplay' al desactivar el objeto
    }

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>(); // Obtener referencia al Rigidbody2D en este GameObject

        controlTurnos = FindFirstObjectByType<ControlTurnos>(); 
        if (controlTurnos == null)
        {
            Debug.LogError("PlayerController: No se encontró el ControlTurnos!");
        }

        turnos = new MovePlayer(controlTurnos); 

        soldierAnim = GetComponent<SoldierAnimationScript>();
        if (soldierAnim == null)
        {
            Debug.LogError("PlayerController: No se encontró el SoldierAnimationScript!");
        }

        actionMenuUI = FindFirstObjectByType<ActionMenuUI>();
        if (actionMenuUI == null)
        {
            Debug.LogError("PlayerController: No se encontró el ActionMenuUI!");
        }
        else
        {
            actionMenuUI.Initialize(this, actions); 
        }

        // --- NUEVO: Obtener la referencia al DamageDealer del collider de ataque ---
        if (_playerMeleeAttackColliderObject != null)
        {
            _playerMeleeDamageDealer = _playerMeleeAttackColliderObject.GetComponent<DamageDealer>();
            if (_playerMeleeDamageDealer == null)
            {
                Debug.LogError($"PlayerController: El objeto de ataque de cuerpo a cuerpo '{_playerMeleeAttackColliderObject.name}' no tiene un componente 'DamageDealer'. Asegúrate de añadirlo.");
            }
        }
        else
        {
            Debug.LogWarning("PlayerController: No se ha asignado el GameObject del collider de ataque de cuerpo a cuerpo en el Inspector. Los ataques de cuerpo a cuerpo no funcionarán correctamente.");
        }

        SelectAction(0); // Seleccionar la primera acción por defecto al inicio del juego
    }

    // --- Métodos de Entrada y Lógica de Acciones ---

    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = transform.position.z; 
        Vector2 directionToMouse = (mouseWorldPosition - transform.position).normalized;

        if (soldierAnim != null)
        {
            soldierAnim.RotatePlayer(directionToMouse);
        }

        if (!controlTurnos.IsMyTurn)
        {
            controlTurnos.UpdateTurn();
        }        
        if (controlTurnos == null || !controlTurnos.IsMyTurn)
        {
            return; 
        }

        ActionData selectedAction = actions[currentActionIndex];
        MovementData dataToUse = selectedAction.movementData != null ? selectedAction.movementData : defaultmovementdata;

        if (dataToUse == null)
        {
            Debug.LogWarning($"La acción '{selectedAction.name}' no tiene un MovementData asignado y no hay defaultmovementdata para usar.");
            return;
        }

        if (selectedAction.isAttack && selectedAction.attackAnimationIndex == 3) // Si es un ataque de arco
        {
            if (arrowCount <= 0) 
            {
                Debug.Log("¡No hay flechas disponibles para el ataque de arco!");
                return; 
            }
            else
            {
                arrowCount--; 
                Debug.Log($"Flechas restantes: {arrowCount}");
            }
        }
        
        // --- ¡Aquí es donde iniciamos la corrutina principal de la acción! ---
        StartCoroutine(PerformActionCoroutine(selectedAction, dataToUse)); 
    }

    public void SelectAction(int index)
    {
        if (index >= 0 && index < actions.Count)
        {
            currentActionIndex = index; 
            Debug.Log($"Acción seleccionada: {actions[currentActionIndex].name}"); 
            
            if (actionMenuUI != null)
            {
                actionMenuUI.HighlightButton(currentActionIndex);
            }
        }
        else
        {
            Debug.LogWarning($"Índice de acción fuera de rango: {index}. Acciones disponibles: {actions.Count}");
        }
    }

    void Update()
    {
        // Estos Inputs son redundantes si ya tienes las suscripciones en Awake() con el Input System.
        if (Input.GetKeyDown(KeyCode.Q)) SelectAction(0);  
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectAction(1);  
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectAction(2); 
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectAction(3); 
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectAction(4); 
    }

    // --- Corrutina Principal para la Ejecución de Acciones ---
    private IEnumerator PerformActionCoroutine(ActionData actionToPerform, MovementData dataToUse)
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); 
        Vector2 direction = mouseWorld - (Vector2)rb2D.position; 
        
        // Asumiendo que turnos.Turno() maneja el movimiento/animación de la acción
        turnos.Turno(direction, rb2D, dataToUse, this);

        // 4. Disparar animaciones y esperar sus tiempos de recarga (cooldown)
        if (actionToPerform.isAttack)
        {
            // --- ¡NUEVO y CRUCIAL! Pasamos el MovementData al DamageDealer aquí ---
            if (_playerMeleeDamageDealer != null)
            {
                _playerMeleeDamageDealer.SetDamageData(dataToUse);
                // Si tu lógica de activación ya funciona, no necesitas _playerMeleeDamageDealer.EnableCollider() aquí.
                // Esta línea asume que otro sistema (ej. Animation Events) activará el collider.
            }

            soldierAnim.PlayAttackAnimation(true, actionToPerform.attackAnimationIndex);
            yield return StartCoroutine(ResetAttackAnimation(dataToUse.cooldown));
            
            // Si tu lógica de desactivación ya funciona, no necesitas _playerMeleeDamageDealer.DisableCollider() aquí.
            // Esta línea asume que otro sistema (ej. Animation Events) desactivará el collider.
        }
        else if (actionToPerform.isHeal)
        {
            LifeScript playerLife = GetComponent<LifeScript>();
            if (playerLife != null)
            {
                playerLife.Heal(10f); // Considera usar un valor del MovementData para la curación
            }
            soldierAnim.PlayHealAnimation(true);
            yield return StartCoroutine(ResetAttackAnimation(dataToUse.cooldown));
        }
        
        yield return new WaitForSeconds(0.1f); 
    }

    private IEnumerator ResetAttackAnimation(float cooldown)
    {
        yield return new WaitForSeconds(cooldown); 
        soldierAnim.ResetAllActionAnimations();
    }

    public void SetWalkingAnimation(bool isWalking)
    {
        soldierAnim.SetWalkingAnimation(isWalking);
    }

    public bool IsPlayerStopped()
    {
        return rb2D.linearVelocity.sqrMagnitude < 0.01f; 
    }
}