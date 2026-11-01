using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float time = 180;
    public TextMeshProUGUI text;

    private int minutes;
    private int seconds;
    private bool running = true;

    private void Start()
    {
        UpdateTimer(time);
    }
    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            time -= Time.deltaTime;
            if (time <= 0f)
            {
                time = 0f;
                running = false;
                // ACABA PARTIDA
            }
            UpdateTimer(time);
        }

    }
    private void UpdateTimer(float tiempoEnSegundos)
    {
        // Obtener minutos y segundos enteros
        minutes = Mathf.FloorToInt(tiempoEnSegundos / 60);
        seconds = Mathf.FloorToInt(tiempoEnSegundos % 60);

        // Formato con dos dígitos para minutos y segundos (00:00)
        text.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
