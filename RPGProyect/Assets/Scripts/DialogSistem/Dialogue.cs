using UnityEngine;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4,7)] private string[] dialogueLines;

    private float typingTime = 0.05f;

    private bool isPlayerInRange = false;
    private bool didDialogueStart = false;
    private int lineIndex = 0;

    void Update()
    {
      if (isPlayerInRange && Input.GetButtonDown("Fire1"))
        {
            if (!didDialogueStart)
            {
                StartDialogue();
            }
            else if (dialogueText.text == dialogueLines[lineIndex]) // Si se termino de imprimir la linea, podremos pasar a la siguiente
            {
                NextDialogueLine();
            }
            else  // Adelantar para mostrar todo el texto
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[lineIndex];
            }
        }
    }

    private void StartDialogue()
    {
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex = 0;
        Time.timeScale = 0f; // Tiene como limitación que todo se detiene por completo (animaciones, impresión de diálogo, movimientos, etc) 
        // ---- En todo caso mejor solo limitar el movimiento del personaje y/o animaciones durante el diálogo ----
        StartCoroutine("ShowLine");
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if(lineIndex < dialogueLines.Length) // Verificamos si ya se imprimieron todas la lineas de diálogo
        {
            StartCoroutine(ShowLine());
        }
        else // Si no hay más línea de dialogo 
        {
            didDialogueStart = false;
            dialoguePanel.SetActive(false);
            dialogueMark.SetActive(true);
            Time.timeScale = 1f; 
        }
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        
        foreach (char ch in dialogueLines[lineIndex]) // Imprimimos caracter por caracter en el espacio de panel asignado con un tiempo de impresion asignada
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime); // Ignoramos la escala de tiempo, pasando a tomar en cuenta el tiempo real
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            dialogueMark.SetActive(true);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueMark.SetActive(false);
        }
    }
}
