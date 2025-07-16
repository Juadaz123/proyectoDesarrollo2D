using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaginadorUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> paginas;
    [SerializeField] private Button botonAnterior;
    [SerializeField] private Button botonSiguiente;

    private int paginaActual = 0;

    private void Start()
    {
        botonAnterior.onClick.AddListener(Prev);
        botonSiguiente.onClick.AddListener(Next);

        // Asegura que solo una página esté visible con alpha = 1
        for (int i = 0; i < paginas.Count; i++)
        {
            CanvasGroup cg = paginas[i].GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = (i == paginaActual) ? 1f : 0f;
                cg.interactable = (i == paginaActual);
                cg.blocksRaycasts = (i == paginaActual);
            }
        }

        ActualizarBotones();
    }

    private void Next()
    {
        if (paginaActual < paginas.Count - 1)
        {
            int oldIndex = paginaActual;
            paginaActual++;
            StartCoroutine(CambiarPagina(oldIndex, paginaActual));
        }
    }

    private void Prev()
    {
        if (paginaActual > 0)
        {
            int oldIndex = paginaActual;
            paginaActual--;
            StartCoroutine(CambiarPagina(oldIndex, paginaActual));
        }
    }

    private IEnumerator CambiarPagina(int oldIndex, int newIndex)
    {
        CanvasGroup oldGroup = paginas[oldIndex].GetComponent<CanvasGroup>();
        CanvasGroup newGroup = paginas[newIndex].GetComponent<CanvasGroup>();

        if (oldGroup != null) yield return StartCoroutine(FadeCanvasGroup(oldGroup, 1f, 0f, 0.3f));
        if (newGroup != null) yield return StartCoroutine(FadeCanvasGroup(newGroup, 0f, 1f, 0.3f));

        ActualizarBotones();
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        canvasGroup.interactable = endAlpha == 1f;
        canvasGroup.blocksRaycasts = endAlpha == 1f;
    }

    private void ActualizarBotones()
    {
        botonAnterior.gameObject.SetActive(paginaActual > 0);
        botonSiguiente.gameObject.SetActive(paginaActual < paginas.Count - 1);
    }
}