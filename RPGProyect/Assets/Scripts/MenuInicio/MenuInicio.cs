using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuInicio : MonoBehaviour
{
    [SerializeField] private string lobbySceneName = "NombreDeTuEscena";

    public void GoToTUORIAL()
    {
        SceneManager.LoadScene(lobbySceneName);
    }
}
