using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image[] iconsPlayer1;
    [SerializeField] private Image[] cooldownsPlayer1;
    [SerializeField] private Image[] iconsPlayer2;
    [SerializeField] private Image[] cooldownsPlayer2;
    [SerializeField] private Image[] backgrounds;

    private Skill[] player1Skills;
    private Skill[] player2Skills;

    public void Init(GameObject player1Obj, GameObject player2Obj)
    {
        // Le pedimos al SkillManager la referencia a sus habilidades
        player1Skills = (player1Obj.GetComponent<SkillManager>()).GetSkills();
        player2Skills = (player2Obj.GetComponent<SkillManager>()).GetSkills();

        // Asigna el fondo de cada jugador al que le corresponde
        backgrounds[0].sprite = (player1Obj.GetComponent<PlayerContext>()).background;
        backgrounds[1].sprite = (player2Obj.GetComponent<PlayerContext>()).background;

        // Este bucle asigna los iconos a las habilidades
        for (int i = 0; i < player1Skills.Length; i++)
        {
            if (player1Skills[i].Icon != null) iconsPlayer1[i].sprite = player1Skills[i].Icon;
            if (player2Skills[i].Icon != null) iconsPlayer2[i].sprite = player2Skills[i].Icon;
        }
    }
    // Update is called once per frame
    void Update()
    {
        UpdateTimer();
        UpdateSlider();
        UpdateCooldowns();
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

    private void UpdateCooldowns()
    {
        if (player1Skills != null && player2Skills != null)
        {
            for (int i = 0; i < cooldownsPlayer1.Length; i++)
            {
                cooldownsPlayer1[i].fillAmount = player1Skills[i].CooldownNormalized;
                cooldownsPlayer2[i].fillAmount = player2Skills[i].CooldownNormalized;
            }
        }
    }
}
