using System.Collections;
using UnityEngine;
using static Map;

public class DashNSplash : Skill
{
    [SerializeField] private Sprite destruction;
    [SerializeField] private float dashSpeed;

    private PlayerInputController player;
    private Rigidbody rb;
    private GameObject area;
    private float showAreaTime = 0.25f;

    private void Start()
    {
        player = this.gameObject.GetComponent<PlayerInputController>();
        area = this.gameObject.transform.GetChild(0).GetChild(1).gameObject;
        area.SetActive(false);
        rb = this.gameObject.GetComponent<Rigidbody>();
    }
    public override void Execute()
    {

        StartCoroutine(Dash());


        // Mensaje identificador
        Debug.Log($"{gameObject.name} ha usado la habilidad " +
                  $"{SkillName} (Tipo: {Type}).");
    }

    IEnumerator Dash()
    {
        float angle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        float tiempo = 0f;

        while (tiempo < 0.1)
        {
            rb.linearVelocity = player.orientation * dashSpeed;

            tiempo += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;


        area.SetActive(true);
        this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Map.Draw(this.transform.localPosition, destruction, player.team, direction);

        yield return new WaitForSeconds(showAreaTime);
        area.SetActive(false);
    }
}
