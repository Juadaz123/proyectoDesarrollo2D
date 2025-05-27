using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public enum ItemType { Coin, Scroll, Key }
    public ItemType itemType;
    public int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            switch (itemType)
            {
                case ItemType.Coin:
                    InventoryManager.Instance.AddCoins(amount);
                    break;
                case ItemType.Scroll:
                    InventoryManager.Instance.AddScrolls(amount);
                    break;
                case ItemType.Key:
                    InventoryManager.Instance.AddKeys(amount);
                    break;
            }

            Destroy(gameObject);
        }
    }
}



