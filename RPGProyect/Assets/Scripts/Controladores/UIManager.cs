using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI scrollText;
    public TextMeshProUGUI keyText;

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
}

