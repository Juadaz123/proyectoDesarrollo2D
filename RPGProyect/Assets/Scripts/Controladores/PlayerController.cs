using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem; // Importar el nuevo Input System

public class PlayerController : MonoBehaviour
{
    // --- Variables de Configuración (visibles en el Inspector) ---
    [Header("Variables de uso y control Player")]
    [SerializeField] private float freeMovementSpeed = 5f;
    [SerializeField] private List<ActionData> actions = new List<ActionData>(); // Lista de acciones con datos
    [SerializeField] private int arrowCount = 10; // Número de flechas disponibles
    [SerializeField] private MovementData defaultmovementdata; // Un MovementData por defecto si alguna acción no tiene uno propio

    // --- Referencias a Componentes y Scripts ---
    [SerializeField] private Rigidbody2D rb2D; // Componente de física para el movimiento
    private ITurnos turnos; // Interfaz para el sistema de turnos (ej. MovePlayer)
    private ControlTurnos controlTurnos; // Script principal que gestiona los turnos
    private SoldierAnimationScript soldierAnim; // Script para el control de animaciones del jugador
    private ActionMenuUI actionMenuUI; // Referencia al script de UI del menú de acciones

    // --- NUEVO: Referencia al GameObject del collider de ataque del jugador ---
    [Header("Configuración de Ataque del Jugador")]
    [Tooltip("Arrastra aquí el GameObject del collider de ataque del jugador (el que tiene el script DamageDealer).")]
    [SerializeField] private GameObject _playerMeleeAttackColliderObject;
    private DamageDealer _playerMeleeDamageDealer; // Referencia al script DamageDealer en ese GameObject

    // --- Variables de Estado ---
    private int currentActionIndex = 0; // Índice de la acción seleccionada actualmente
    //variable bool para el uso de MovementFree
    private bool isFreeMoving = false;

    // --- NUEVA VARIABLE PARA EL CONTROL DE CLICS ---
    private bool _canPerformAction = true; // Controla si el jugador puede realizar una acción en este momento


    [Header("Configuración de Proyectiles")]
    [SerializeField] private LaucherBehaviour laucher;
    [SerializeField] private float projectileLaunchForce = 20f;


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
    private void Start()
    {
        laucher = GetComponent<LaucherBehaviour>();
        rb2D = GetComponent<Rigidbody2D>(); // Obtener referencia al Rigidbody2D en este GameObject

        controlTurnos = FindFirstObjectByType<ControlTurnos>();
        if (controlTurnos == null)
        {
            Debug.LogError("PlayerController: No se encontró el ControlTurnos!");
        }
        else
        {
            // Suscribirse al evento de fin de turno del ControlTurnos
            controlTurnos.OnTurnEnded += OnTurnEndedHandler; //
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

    // Asegúrate de desuscribirte para evitar memory leaks
    private void OnDestroy()
    {
        if (controlTurnos != null)
        {
            controlTurnos.OnTurnEnded -= OnTurnEndedHandler; //
        }
    }

    // Manejador del evento de fin de turno
    private void OnTurnEndedHandler() //
    {
        _canPerformAction = true; // El turno ha terminado, el jugador puede volver a hacer una acción en su siguiente turno
        Debug.Log("PlayerController: Turno finalizado. Habilitando acción."); //
    }


    // --- Métodos de Entrada y Lógica de Acciones ---

    public void OnClickPerformed(InputAction.CallbackContext context)
    {
        // --- Solución 1: Bloqueo por bandera booleana ---
        if (!context.performed || !_canPerformAction) //
        {
            Debug.Log("PlayerController: No se puede realizar una acción en este momento (Cooldown o no es tu turno)."); //
            return; //
        }

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
                controlTurnos.EndTurn();
                return;
            }
            else
            {
                arrowCount--;
                Debug.Log($"Flechas restantes: {arrowCount}");

                if (laucher != null)
                {
                    laucher.InstanciarNuevoObjeto();
                }
                else
                {
                    Debug.LogError("ProjectilePooler no está asignado. No se pueden lanzar flechas.");
                }
            }
        }

        // --- Establecer la bandera a false antes de iniciar la acción ---
        _canPerformAction = false; //
        Debug.Log("PlayerController: Iniciando acción. Bloqueando clics adicionales."); //

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
        // --- Al finalizar la corrutina de la acción, el turno debería finalizar si todo salió bien. ---
        // controlTurnos.EndTurn(); // Esto es importante para que el sistema de turnos avance y se reactive _canPerformAction en el siguiente turno.
        // El restablecimiento de _canPerformAction ahora se maneja en el evento OnTurnEndedHandler
    }


    // --- NUEVOS MÉTODOS DE INICIO/DETENCIÓN DE MOVIMIENTO LIBRE ---
    public void StartFreeMovement()
    {

        controlTurnos.enabled = false;
        isFreeMoving = true;
        Debug.Log("PlayerController: Iniciando movimiento libre.");
    }


    public void StopFreeMovement()
    {
        isFreeMoving = false;
        rb2D.linearVelocity = Vector2.zero; // Asegurarse de detener el movimiento
        SetWalkingAnimation(false); // Detener la animación
        Debug.Log("PlayerController: Deteniendo movimiento libre.");
    }

    private void FixedUpdate()
    {
        if (isFreeMoving)
        {
            ApplyFreeMovement();
        }else if(!isFreeMoving && controlTurnos.enabled== false) {

            rb2D.linearVelocity = Vector2.zero;
            SetWalkingAnimation(false); // Asegurarse de detener la animación de caminar
        }
    }

    private void ApplyFreeMovement()
    {

        Vector2 directionToMouse = GetDirectionToMouse();

        // 3. Mover el Rigidbody2D hacia el mouse
        rb2D.linearVelocity = directionToMouse * freeMovementSpeed;

        // 4. Rotar el personaje para que mire hacia la dirección del mouse
        if (soldierAnim != null)
        {
            soldierAnim.RotatePlayer(directionToMouse);
        }

        // 5. Activar la animación de caminar
        SetWalkingAnimation(true);
    }

//btener direccion del mouse (usado por free movement y controlFlechas)
    public Vector3 GetDirectionToMouse()
    {
        // 1. Obtener la posición del mouse en el mundo
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = transform.position.z; // Mantener la misma Z que el jugador

        // 2. Calcular la dirección desde el jugador hacia el mouse
        return ((mouseWorldPosition - transform.position).normalized);
    }

    public void HandleInteraction()
    {
        Debug.Log("PlayerController: Manejando interacción (botón derecho del ratón en modo libre).");
        // Aquí podrías lanzar un raycast, activar un UI, etc.
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