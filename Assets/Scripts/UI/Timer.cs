using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Timer Instance;



    public float baseTime = 180;
    public static float remainingTime;

    public static bool running = true;

    // Sonido de warning cuando faltan 10 segundos
    private bool warningCountdown = false;

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
        warningCountdown = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            remainingTime -= Time.deltaTime;

            // Cuando quedan 10 segundos o menos suena el warning
            if (remainingTime <= 10f && !warningCountdown)
            {
                warningCountdown = true;
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlayCountdownSFX();
                }
            }

            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                running = false;
            }
        }

    }



}
