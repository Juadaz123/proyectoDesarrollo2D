using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI scrollText;
    public TextMeshProUGUI keyText;

    [SerializeField] private List<GameObject> listaCorazones;
    [SerializeField] private Sprite corazonLleno;
    [SerializeField] private Sprite corazonMitad;
    [SerializeField] private Sprite corazonQuitado;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateCoins(int amount)
    {
        coinText.text = "Coins: " + amount;
    }

    public void UpdateScrolls(int amount)
    {
        scrollText.text = "Scrolls: " + amount;
    }

    public void UpdateKeys(int amount)
    {
        keyText.text = "Keys: " + amount;
    }

    public void ActualizarVidaJugador(float currentLife, float maxLife)
    {
        int totalCorazones = listaCorazones.Count;
        float vidaPorCorazon = maxLife / totalCorazones;

        for (int i = 0; i < totalCorazones; i++)
        {
            Image imageCorazon = listaCorazones[i].GetComponent<Image>();

            float vidaMin = i * vidaPorCorazon;
            float vidaMax = (i + 1) * vidaPorCorazon;

            // Vida que "le corresponde" a este corazón
            float vidaDelCorazon = Mathf.Clamp(currentLife - vidaMin, 0, vidaPorCorazon);

            if (vidaDelCorazon >= vidaPorCorazon)
            {
                imageCorazon.sprite = corazonLleno;
            }
            else if (vidaDelCorazon > 0)
            {
                imageCorazon.sprite = corazonMitad;
            }
            else
            {
                imageCorazon.sprite = corazonQuitado;
            }
        }
    }
}

