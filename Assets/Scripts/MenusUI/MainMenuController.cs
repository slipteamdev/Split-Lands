using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string characterSelectScreen;
    [SerializeField] private string SettingsScene;
    [SerializeField] private string ShopScene;
    [Space]
    [SerializeField] private Button[] buttons;

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

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null) buttons[0].Select();
    }
}