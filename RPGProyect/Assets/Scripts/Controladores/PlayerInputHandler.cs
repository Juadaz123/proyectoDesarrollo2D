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

    }

    private void OnEnable()
    {
        playerInput.Enable(); // Habilitar el Action Map 'Gameplay' al activar el objeto
    }

    private void OnDisable()
    {
        playerInput.Disable(); // Deshabilitar el Action Map 'Gameplay' al desactivar el objeto
    }

}