using System.Collections;
using UnityEngine;

public class DashNSplash : Skill
{
    [SerializeField] private Sprite destructionSplash;
    [SerializeField] private Sprite destructionDash;
    [SerializeField] private float dashDistance = 70f;   // Distancia total a recorrer
    [SerializeField] private float dashSpeed = 500f;
    [SerializeField] private float endLag = 1f; // Tiempo que el jugador se queda sin poder actuar tras el ataque (balanceo)

    private Rigidbody rb;
    private GameObject area;
    private float showAreaTime = 0.25f;

    private void Start()
    {
        area = this.gameObject.transform.GetChild(0).GetChild(1).gameObject;
        area.SetActive(false);
        rb = playerContext.Rb;
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
        //float angle = Random.Range(0f, 360f);
        //Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        playerContext.BlockMoveInput();
        playerContext.BlockItem();
        playerContext.BlockSkill();
        float dashDuration = dashDistance / dashSpeed;
        float tiempo = 0f;

        Vector2 dashOrientation = playerContext.Orientation;
        while (tiempo < dashDuration)
        {

            if (destructionDash != null)
            {
                Draw(destructionDash);
            }
            rb.linearVelocity = dashOrientation * dashSpeed;
            tiempo += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate(); ;
        }

        rb.linearVelocity = Vector2.zero;
        this.transform.localRotation = Quaternion.identity;


        area.SetActive(true);
        //area.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        Draw(destructionSplash);

        this.transform.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(showAreaTime);
        area.SetActive(false);
        yield return new WaitForSeconds(endLag);
        playerContext.AllowMoveInput();
        playerContext.AllowItem();
        playerContext.AllowSkill();
    }
}
