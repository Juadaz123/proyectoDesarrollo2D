using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public FadeController fadeController;
    public InventoryManager inventoryManager;  // ← reference here

    public List<string> visitedFloors = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            fadeController = GetComponent<FadeController>();
            inventoryManager = GetComponent<InventoryManager>();  // ← fetch it here

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeController != null)
        {
            fadeController.FadeIn();
        }
        else
        {
            Debug.LogWarning("FadeController is null!");
        }
    }

    public void AddVisitedFloor(string floorName)
    {
        if (!visitedFloors.Contains(floorName))
        {
            visitedFloors.Add(floorName);
            Debug.Log("Visited: " + floorName);
        }
    }
}
