using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI timerText;

    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if (progressSlider != null)
        {
            progressSlider.value = Map.normalicedProgress;
        }
    }

    private void UpdateTimer()
    {
        // Formato con dos digitos para minutos y segundos (00:00)
        timerText.text = string.Format("{0:0}:{1:00}", Mathf.FloorToInt(Timer.remainingTime / 60), Mathf.FloorToInt(Timer.remainingTime % 60));
    }
}
