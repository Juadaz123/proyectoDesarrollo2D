// PlayerInputHandler.cs
using UnityEngine;
using UnityEngine.InputSystem; // Importar el nuevo Input System


public class PlayerInputHandler : MonoBehaviour
{
    // --- Referencias ---
    private PlayerController playerController;

    // Referencia al Input Actions Asset
    private PlayerInputAction playerInput;

    //referencia al spawner (para saber si aun exiten enemigos)
    private EnemySpawner enemySpawner;

    //--varaibles
    private bool inFreeMovementMode = false;

// METODOS DE UNITY
    private void Awake()
    {
        enemySpawner = FindFirstObjectByType<EnemySpawner>();
        if (enemySpawner == null)
        {
            Debug.LogError($"PlayerInputHandler: No se encontró ningun Objeto con el Script EnemySpawner");
        }
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerInputHandler: No se encontró el PlayerController en el mismo GameObject.");
        }
        playerInput = new PlayerInputAction(); // Instanciar el Input Actions Asset

        // --- Suscribir métodos a los eventos del Input System ---
        // Para el movimiento, el contexto se pasa para obtener la posición del mouse.
        playerInput.Gameplay.Mover.performed += playerController.OnClickPerformed;

        playerInput.Gameplay.Ataque0Mover.performed += ctx => playerController.SelectAction(0);
        playerInput.Gameplay.Ataque1.performed += ctx => playerController.SelectAction(1);
        playerInput.Gameplay.Ataque2.performed += ctx => playerController.SelectAction(2);
        playerInput.Gameplay.Ataque3.performed += ctx => playerController.SelectAction(3);
        playerInput.Gameplay.Ataque4.performed += ctx => playerController.SelectAction(4);

    // --- Suscribir métodos a los eventos del Input System para el mapa 'Movement Free' ---
    // NO usamos ctx.ReadValue<Vector2>() para el botón.
    // En su lugar, usaremos el evento 'started' y 'canceled' para el movimiento libre.
    playerInput.MovementFree.movimiento.started += ctx => playerController.StartFreeMovement();
    playerInput.MovementFree.movimiento.canceled += ctx => playerController.StopFreeMovement();
    playerInput.MovementFree.interactuar.performed += ctx => playerController.HandleInteraction();


    }

    private void Update() {
        CheckAndSwitchInputMap();
    }

    private void OnEnable()
    {
        playerInput.Gameplay.Enable();
        playerInput.MovementFree.Disable();
        inFreeMovementMode = false;
    }

    private void OnDisable()
    {
        playerInput.Disable(); // Deshabilitar el Action Map 'Gameplay' al desactivar el objeto
    }

    //Detector de enemigos
    private bool AnyHaveEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length == 0;
    }

    //comrpibador de enemigos
    private void CheckAndSwitchInputMap()
    {
        if (AnyHaveEnemies())
        {
            // Si no hay enemigos y no estamos ya en modo de movimiento libre
            if (!inFreeMovementMode)
            {
                playerController.StopFreeMovement();
                playerInput.Gameplay.Disable(); // Deshabilita el mapa de juego
                playerInput.MovementFree.Enable(); // Habilita el mapa de movimiento libre
                Debug.Log("Cambiando a modo de movimiento libre: No hay enemigos.");
                inFreeMovementMode = true;
            }
        }
        else if (inFreeMovementMode)
        {
            playerInput.MovementFree.Disable(); // Deshabilita el mapa de movimiento libre
            playerInput.Gameplay.Enable(); // Habilita el mapa de juego
            Debug.Log("Cambiando a modo de juego: Hay enemigos presentes.");
            inFreeMovementMode = false;
        }

    }

}