using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int coins = 0;
    public int scrolls = 0;
    public int keys = 0;

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

    public void AddCoins(int amount)
    {
        coins += amount;
        UIManager.Instance.UpdateCoins(coins);
    }

    public void AddScrolls(int amount)
    {
        scrolls += amount;
        UIManager.Instance.UpdateScrolls(scrolls);
    }

    public void AddKeys(int amount)
    {
        keys += amount;
        UIManager.Instance.UpdateKeys(keys);
    }

    public bool UseCoin(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UIManager.Instance.UpdateCoins(coins);
            return true;
        }
        return false;
    }

    public bool UseScroll(int amount)
    {
        if (scrolls >= amount)
        {
            scrolls -= amount;
            UIManager.Instance.UpdateScrolls(scrolls);
            return true;
        }
        return false;
    }

    public bool UseKey(int amount)
    {
        if (keys >= amount)
        {
            keys -= amount;
            UIManager.Instance.UpdateKeys(keys);
            return true;
        }
        return false;
    }
}
