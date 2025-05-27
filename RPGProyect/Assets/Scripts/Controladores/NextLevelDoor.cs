using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class NextLevelDoor : MonoBehaviour
{
    public List<string> nextFloorScenes;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Add current floor to visited list
            GameManager.Instance.AddVisitedFloor(SceneManager.GetActiveScene().name);

            // Filter out floors already visited
            List<string> availableScenes = nextFloorScenes
                .Where(sceneName => !GameManager.Instance.visitedFloors.Contains(sceneName))
                .ToList();

            if (availableScenes.Count > 0)
            {
                string randomScene = availableScenes[Random.Range(0, availableScenes.Count)];
                         FindFirstObjectByType<FadeController>().FadeToScene(randomScene);
            }
            else
            {
                Debug.Log("No new floors available!");
                // Optional: Load a final scene or loop back
                // SceneManager.LoadScene("EndGameScene");
            }
        }
    }
}
