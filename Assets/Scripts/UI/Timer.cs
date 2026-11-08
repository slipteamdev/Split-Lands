using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Timer Instance;

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
                // ACABA PARTIDA
            }
        }

    }

}
