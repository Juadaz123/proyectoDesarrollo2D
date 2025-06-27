using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuInicio : MonoBehaviour
{
    [SerializeField] private SceneAsset Lobby;
    public void GoToLobby()
    {
        string lobbyScene = Lobby.name;
        SceneManager.LoadScene(lobbyScene);
    }
    
}
