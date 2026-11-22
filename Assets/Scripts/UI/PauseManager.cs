using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausaPanel;
    [SerializeField] private Image controlJ1;
    [SerializeField] private Image controlJ2;
    [SerializeField] private Sprite WASD;
    [SerializeField] private Sprite Arrows;
    [SerializeField] private Sprite GamePad;
    private bool running = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void init(GameObject p1, GameObject p2)
    {
        SetInputImage(p1.GetComponent<PlayerInput>(), controlJ1);
        SetInputImage(p2.GetComponent<PlayerInput>(), controlJ2);
    }

    public void SetInputImage(PlayerInput input, Image control)
    {
        if (input.currentControlScheme == "WASD")
        {
            control.sprite = WASD;
        }
        else if (input.currentControlScheme == "Arrows")
        {
            control.sprite = Arrows;
        }
        else if (input.currentControlScheme == "GamePad")
        {
            control.sprite = GamePad;
        }
    }

    public void Pausa()
    {
        if (running)
        {
            Time.timeScale = 0f;
            running = false;
            pausaPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            running = true;
            pausaPanel.SetActive(false);
        }
    }

    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
