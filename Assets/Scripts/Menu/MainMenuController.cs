using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string characterSelectScreen;

    public void PlayGame()
    {
        SceneManager.LoadScene(characterSelectScreen);
    }

    public void OpenSettings()
    {
        // Pendiente de implementar
        Debug.Log("Ajustes próximamente");
    }

    public void ExitGame()
    {
        Debug.Log("Cerrando juego...");

        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
