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

    // --- Variables de Estado ---
    private int currentActionIndex = 0; // Índice de la acción seleccionada actualmente
    public bool CanMove = true; // Permite o restringe el movimiento (considera si es necesaria con el sistema de turnos)

    // --- Estructura para almacenar los datos de cada acción (¡Única definición!) ---
    // Esta es la clase que se usa para definir cada tipo de acción en el Inspector.
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
        // El clic del ratón ejecutará la acción actualmente seleccionada
        playerInput.Gameplay.Mover.performed += OnClickPerformed; 
        
        // Teclas para seleccionar diferentes acciones
        playerInput.Gameplay.Ataque1.performed += ctx => SelectAction(1); 
        playerInput.Gameplay.Ataque2.performed += ctx => SelectAction(2); 
        playerInput.Gameplay.Ataque3.performed += ctx => SelectAction(3); 
        playerInput.Gameplay.Ataque4.performed += ctx => SelectAction(4); 
        // Asegúrate de que estas acciones estén definidas en tu PlayerInputAction Asset y que los índices coincidan con tu lista 'actions'
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

        // Buscar y obtener referencia al ControlTurnos en la escena
        controlTurnos = FindFirstObjectByType<ControlTurnos>(); 
        if (controlTurnos == null)
        {
            Debug.LogError("PlayerController: No se encontró el ControlTurnos!");
        }

        // Instanciar el sistema de turnos para el jugador, pasándole el ControlTurnos
        turnos = new MovePlayer(controlTurnos); 

        // Obtener referencia al SoldierAnimationScript en este GameObject
        soldierAnim = GetComponent<SoldierAnimationScript>();
        if (soldierAnim == null)
        {
            Debug.LogError("PlayerController: No se encontró el SoldierAnimationScript!");
        }

        // Obtener referencia al ActionMenuUI y inicializarlo
        actionMenuUI = FindFirstObjectByType<ActionMenuUI>();
        if (actionMenuUI == null)
        {
            Debug.LogError("PlayerController: No se encontró el ActionMenuUI!");
        }
        else
        {
            // Pasar esta instancia de PlayerController y la lista de acciones a la UI para que la muestre
            actionMenuUI.Initialize(this, actions); 
        }

        SelectAction(0); // Seleccionar la primera acción por defecto al inicio del juego
    }

    // --- Métodos de Entrada y Lógica de Acciones ---

    // Método llamado cuando se realiza un clic (desde el Input System)
    private void OnClickPerformed(InputAction.CallbackContext context)
    {
        if(!controlTurnos.IsMyTurn){
        controlTurnos.UpdateTurn();}        // Solo permite la acción si es el turno del jugador y el sistema de turnos existe
        if (controlTurnos == null || !controlTurnos.IsMyTurn)
        {
            return; // Salir si no es el turno o no hay ControlTurnos
        }

        // Obtiene la ActionData de la acción actualmente seleccionada
        ActionData selectedAction = actions[currentActionIndex];

        // Obtener el MovementData para esta acción
        MovementData dataToUse = selectedAction.movementData != null ? selectedAction.movementData : defaultmovementdata;

        if (dataToUse == null)
        {
            Debug.LogWarning($"La acción '{selectedAction.name}' no tiene un MovementData asignado y no hay defaultmovementdata para usar.");
            return;
        }
        // Lógica específica para el ataque de arco: verificar y decrementar flechas
        if (selectedAction.isAttack && selectedAction.attackAnimationIndex == 3) // Si es un ataque de arco
        {
            if (arrowCount <= 0) // Y no hay flechas
            {
                Debug.Log("¡No hay flechas disponibles para el ataque de arco!");
                return; // No ejecutar el ataque
            }
            else
            {
                arrowCount--; // Decrementa el conteo de flechas
                Debug.Log($"Flechas restantes: {arrowCount}");
            }
        }
        
        // --- ¡Aquí es donde iniciamos la corrutina principal de la acción! ---
        // Toda la lógica de ejecución de la acción (movimiento, animaciones, fin de turno)
        // se maneja dentro de esta corrutina para controlar los tiempos.
        StartCoroutine(PerformActionCoroutine(selectedAction, dataToUse)); 
    }

    // Método para seleccionar una acción por índice (llamado por teclado o botones UI)
    public void SelectAction(int index)
    {
        // Asegura que el índice esté dentro de los límites de la lista de acciones
        if (index >= 0 && index < actions.Count)
        {
            currentActionIndex = index; // Actualiza la acción seleccionada
            // Muestra en la consola cuál acción ha sido seleccionada
            Debug.Log($"Acción seleccionada: {actions[currentActionIndex].name}"); 
            
            // Notificar a la UI para resaltar el botón de la acción seleccionada
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

    // --- Lógica Opcional de Teclas Antiguas en Update (considera migrar al Input System) ---
    void Update()
    {
        // Estos Inputs son redundantes si ya tienes las suscripciones en Awake() con el Input System.
        // Si las usas como atajos directos, pueden permanecer, pero el Input System es más flexible.
        if (Input.GetKeyDown(KeyCode.Q)) SelectAction(0);  // Seleccionar acción de Mover
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectAction(1);  // Seleccionar ataque Débil
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectAction(2); // Seleccionar ataque Fuerte
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectAction(3); // Seleccionar ataque Arco
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectAction(4); // Seleccionar Curarse (asegúrate de que exista y sea la 5ta acción)
    }

    // --- Corrutina Principal para la Ejecución de Acciones ---
    // ESTA ES LA CORRUTINA PerformActionCoroutine QUE DEBE ESTAR EN TU CÓDIGO
    private IEnumerator PerformActionCoroutine(ActionData actionToPerform, MovementData dataToUse)
    {
        // 1. Calcular la dirección del clic del ratón en el mundo del juego
        // Usamos Mouse.current.position.ReadValue() del Input System para la posición del ratón.
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); 
        Vector2 direction = mouseWorld - (Vector2)rb2D.position; 

        
       
            // Esto es donde se realiza el movimiento o el cálculo del ataque en tu sistema ITurnos/MovePlayer.
        turnos.Turno(direction, rb2D, dataToUse, this);

        // 4. Disparar animaciones y esperar sus tiempos de recarga (cooldown)
        // Lógica para animaciones de ataque
        if (actionToPerform.isAttack)
        {
            // Asumo que SoldierAnimationScript.PlayAttackAnimation(bool, int) es un método genérico
            // que reproduce la animación de ataque según el índice proporcionado (1, 2, 3).
            soldierAnim.PlayAttackAnimation(true, actionToPerform.attackAnimationIndex);
            // Esperar a que la animación termine su duración (o cooldown) antes de continuar.
            yield return StartCoroutine(ResetAttackAnimation(dataToUse.cooldown));
            
            //a usar un metodo con un collider
        }
        // Lógica para animaciones de curación
        else if (actionToPerform.isHeal)
        {
            LifeScript playerLife = GetComponent<LifeScript>();
            if (playerLife != null)
            {
                playerLife.Heal(10f);
            }
            //pienso cambiar esto por logica de inventario luego

            soldierAnim.PlayHealAnimation(true);
            // Esperar a que la animación de curación termine su duración (o cooldown).
            yield return StartCoroutine(ResetAttackAnimation(dataToUse.cooldown));
        }
        // Puedes añadir más bloques 'else if' para otros tipos de acciones con animaciones específicas.
        /* 
        quiero usar un swicht pero no me acuerdo como y da flojera leer la docu 
        */
        // 5. Pequeña espera para asegurar que la acción se procese visualmente o lógicamente
        yield return new WaitForSeconds(0.1f); 

    }

    // --- Corrutina para Resetear Animaciones ---
    private IEnumerator ResetAttackAnimation(float cooldown)
    {
        yield return new WaitForSeconds(cooldown); // Espera el tiempo de cooldown de la acción
        soldierAnim.ResetAllActionAnimations();
        
    }

    // --- Métodos de Utilidad ---

    // Método para activar/desactivar animaciones de caminar (puede ser llamado por tu lógica de movimiento)
    public void SetWalkingAnimation(bool isWalking)
    {
        soldierAnim.SetWalkingAnimation(isWalking);
    }

    // Método para verificar si el jugador ha detenido su movimiento. Útil para el sistema de turnos.
    public bool IsPlayerStopped()
    {
        // Compara la magnitud al cuadrado de la velocidad con un umbral pequeño para eficiencia.
        return rb2D.linearVelocity.sqrMagnitude < 0.01f; 
    }
}