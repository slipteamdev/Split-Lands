using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string characterSelectScreen;
    [SerializeField] private string SettingsScene;
    [SerializeField] private string ShopScene;

    public void PlayGame()
    {
        SceneManager.LoadScene(characterSelectScreen);
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene(SettingsScene);
    }

    public void ExitGame()
    {
        Debug.Log("Cerrando juego...");

        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OpenShop()
    {
        SceneManager.LoadScene(ShopScene);
    }

}
