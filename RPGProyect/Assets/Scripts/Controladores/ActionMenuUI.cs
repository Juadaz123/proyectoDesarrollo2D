using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para Button, Image, Text

// Este script se encarga de la creación y gestión de los botones de acción en la UI (Canvas)
public class ActionMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject actionButtonPrefab; // Prefab del botón UI que se instanciará
    [SerializeField] private Transform actionButtonContainer; // Objeto padre en el Canvas donde se colocarán los botones

    private PlayerController playerController; // Referencia al PlayerController para llamar a sus métodos
    private List<Button> actionButtons = new List<Button>(); // Lista para guardar referencias a los botones creados

    // Método para inicializar el menú de UI y generar los botones
    // Es llamado por PlayerController en su método Start()
    public void Initialize(PlayerController controller, List<PlayerController.ActionData> actions)
    {
        playerController = controller;
        GenerateActionButtons(actions);
    }

    // Genera los botones de acción dinámicamente en el Canvas
    private void GenerateActionButtons(List<PlayerController.ActionData> actions)
    {
        // Limpiar botones existentes antes de generar nuevos
        foreach (Transform child in actionButtonContainer)
        {
            Destroy(child.gameObject);
        }
        actionButtons.Clear(); // Limpiar la lista de referencias de botones

        for (int i = 0; i < actions.Count; i++)
        {
            // Instanciar el prefab del botón y colocarlo en el contenedor
            GameObject buttonGO = Instantiate(actionButtonPrefab, actionButtonContainer);
            Button button = buttonGO.GetComponent<Button>();
            Image buttonImage = buttonGO.GetComponent<Image>();
            Text buttonText = buttonGO.GetComponentInChildren<Text>(); // Asume que el texto es un componente hijo

            // Configurar el texto y la imagen del botón
            if (buttonText != null)
            {
                // buttonText.text = actions[i].name; // Asigna el nombre de la acción como texto del botón
            }
            if (buttonImage != null && actions[i].movementData.actionicon != null)
            {
                buttonImage.sprite = actions[i].movementData.actionicon; // Asigna el sprite del icono
            }

            int actionIndex = i; // Captura el índice para usarlo en el listener (importante para lambdas)
            // Añade un listener al evento onClick del botón para llamar a SelectAction en PlayerController
            // button.onClick.AddListener(() => playerController.SelectAction(actionIndex));
            actionButtons.Add(button); // Añadir el botón a la lista de referencias
        }

        // Resaltar el primer botón por defecto si existen botones
        if (actionButtons.Count > 0)
        {
            HighlightButton(0);
        }
    }

    // Resalta visualmente el botón de la acción actualmente seleccionada
    public void HighlightButton(int selectedIndex)
    {
        for (int i = 0; i < actionButtons.Count; i++)
        {
            if (actionButtons[i] != null)
            {
                // Ejemplo simple: cambiar el color del botón para resaltar
                if (i == selectedIndex)
                {
                    actionButtons[i].image.color = Color.yellow; // Color de resaltado
                }
                else
                {
                    actionButtons[i].image.color = Color.white; // Color normal
                }
            }
        }
    }
}
