using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsController : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
