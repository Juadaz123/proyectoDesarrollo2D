using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainer; 
    

    private PlayerController playerController;
    private List<Button> actionButtons = new List<Button>(); // Lista para guardar referencias a los botones creados

    // Método para inicializar el menú de UI y generar los botones
    //buscar el actionMenuContainer en awake
    private void Awake()
    {
        actionButtonContainer = GameObject.FindGameObjectWithTag("PanelMenu").GetComponent<Transform>();
    }
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
 
            // --- ACCESO A LOS NIETOS DEL BOTÓN INSTANCIADO ---
            // 1. Obtener la referencia al componente Image del hijo "MoveIcon"
            Image actionIconImage = null;
            Transform iconChild = buttonGO.transform.GetChild(0).GetChild(0); // Busca al nieto del primer hijo del boton llamado "MoveIcon"
            if (iconChild != null)
            {
                actionIconImage = iconChild.GetComponent<Image>();
            }
            else
            {
                Debug.LogWarning($"ActionMenuUI: No se encontró un hijo llamado 'ActionIcon' en el prefab del botón '{actionButtonPrefab.name}'.");
            }

            // 2. Obtener la referencia al componente Text del hijo para el nombre
            TextMeshProUGUI buttonText = null;
            Transform textChild = buttonGO.transform.GetChild(1).GetChild(0); // Busca al nieto del segundo hijo del boton llamado "AccionName"
            if (textChild != null)
            {
                buttonText = textChild.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning($"ActionMenuUI: No se encontró un hijo llamado 'NombreAccionText' en el prefab del botón '{actionButtonPrefab.name}'.");
            }
            // --- FIN DEL ACCESO A HIJOS ---

            if (buttonText != null)
            {
                buttonText.text = actions[i].name;
            }

            //asignar sprite desde el movementData
            if (actionIconImage != null && actions[i].movementData != null && actions[i].movementData.actionicon != null)
            {
                actionIconImage.sprite = actions[i].movementData.actionicon;
                actionIconImage.SetNativeSize(); //imagen al tamaño original
        }
            int actionIndex = i; // Captura el índice para usarlo en el listener (importante para lambdas)
            // Añade un listener al evento onClick del botón para llamar a SelectAction en PlayerController
            button.onClick.AddListener(() => playerController.SelectAction(actionIndex));
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
                CanvasGroup cg = actionButtons[i].GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    if (i == selectedIndex)
                    {
                        cg.alpha = 1f; // Boton seleccionado
                    }
                    else
                    {
                        cg.alpha = 0.7f; // Botones no seleccionados
                    }
                }
                else
                {
                    Debug.LogWarning("No se encontró CanvasGroup en el botón instanciado.");
                }
            }
        }
    }
}
