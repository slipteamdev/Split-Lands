using System.Collections;
using UnityEngine;
using static Map;

public class BasicSkill : Skill
{
    [SerializeField] private Sprite destruction;
    
    private PlayerInputController player;
    private GameObject area;
    private float showAreaTime = 0.25f;

    private void Start()
    {
        player = this.gameObject.GetComponent<PlayerInputController>();
        area = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        area.SetActive(false);
    }
    public override void Execute()
    {
        float angle = Mathf.Atan2(player.orientation.y, player.orientation.x) * Mathf.Rad2Deg;
        area.SetActive(true);
        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Map.Draw(this.transform.localPosition, destruction, player.team, player.orientation);

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
}
