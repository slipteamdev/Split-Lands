using System.Collections;
using UnityEngine;

public class SlashSkill : Skill
{
    [SerializeField] private Sprite BaseDestruction;
    [SerializeField] private Sprite BerserkDestruction;
    [SerializeField] private float BerserkCooldownSpeedMultiplier = 1f;
    private Sprite destruction;

    private PlayerInputController player;
    private GameObject area;
    private float showAreaTime = 0.25f;

    private void Start()
    {
        player = this.gameObject.GetComponent<PlayerInputController>();
        area = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        area.SetActive(false);
        NormalMode();
    }
    public override void Execute()
    {
        float angle = Mathf.Atan2(playerContext.Orientation.y, playerContext.Orientation.x) * Mathf.Rad2Deg;
        area.SetActive(true);
        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        this.GetComponent<Movement>().UpdateSpritePos(); //pa que no se vuelva loco

        Draw(destruction, playerContext.Orientation);

        StartCoroutine(hideArea());


        // Mensaje identificador
        Debug.Log($"{gameObject.name} ha usado la habilidad " +
                  $"{SkillName} (Tipo: {Type}).");
    }

    IEnumerator hideArea()
    {
        yield return new WaitForSeconds(showAreaTime);
        area.SetActive(false);
    }

    public void NormalMode()
    {
        destruction = BaseDestruction;
        SelfCooldownMultiplier = 1f;
    }

    public void BerserkMode()
    {
        destruction = BerserkDestruction;
        SelfCooldownMultiplier = BerserkCooldownSpeedMultiplier;
    }
}

