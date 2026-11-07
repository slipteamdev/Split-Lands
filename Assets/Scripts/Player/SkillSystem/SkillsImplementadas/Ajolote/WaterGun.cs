using System.Collections;
using UnityEngine;

public class WaterGun : Skill
{
    [SerializeField] private Sprite destruction;

    private GameObject area;
    private float showAreaTime = 0.25f;

    private void Start()
    {
        area = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
        area.SetActive(false);
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

}
