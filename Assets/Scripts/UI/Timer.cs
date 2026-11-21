using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Timer Instance;

    [SerializeField] private GameObject pausaPanel;

    public float baseTime = 180;
    public static float remainingTime;

    public static bool running = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        remainingTime = baseTime;
        running = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                running = false;
            }
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
