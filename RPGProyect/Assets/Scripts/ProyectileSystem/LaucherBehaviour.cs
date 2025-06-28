using UnityEngine;
using UnityEngine.Windows;
using System.Collections.Generic;

public class LaucherBehaviour : MonoBehaviour
{

    //rendirme del objectpooling a usar un instantiate
    [Header("Parametros del lanzador de proyectiles")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void InstanciarNuevoObjeto()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("¡No hay un Prefab asignado en el Inspector! Asigna un Prefab a la variable 'Prefab Para Instanciar'.");
            return; // Salir si no hay prefab
        }

        if (player == null)
        {
            Debug.LogError("¡No hay un Player Controller asignado! Asigna el script 'PlayerController' a la variable 'Player Controller'.");
            return; // Salir si no hay playerController
        }

        // 1. Obtener la dirección desde el Player Controller
        Vector3 directionToMouse = player.GetDirectionToMouse();

        // 2. Calcular el ángulo en Z a partir de la dirección
        // Atan2 calcula el ángulo en radianes entre el eje X positivo y un punto (y, x).
        // Necesitamos convertirlo a grados para usarlo en EulerAngles.
        float angleZ = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        // 3. Crear el Quaternion de rotación usando el ángulo en Z
        // Quaternion.Euler(x, y, z) crea una rotación a partir de ángulos de Euler.
        // Solo nos interesa el ángulo en Z para la rotación hacia la cámara.
        Quaternion rotacionHaciaMouse = Quaternion.Euler(0, 0, angleZ);

        // 4. Instanciar el objeto con la posición y rotación calculadas
        GameObject nuevoObjeto = Instantiate(projectilePrefab, transform.position, rotacionHaciaMouse);

        nuevoObjeto.name = "ObjetoInstanciado_" + Time.time;
        Debug.Log("Objeto instanciado: " + nuevoObjeto.name + " con rotación Z: " + angleZ + " grados.");
    }




}