using System.Collections;
using UnityEngine;

public class DashNSplashSkill : Skill
{
    [Header("Atributos Base")]
    [SerializeField] private Sprite BaseDestructionSplash;
    [SerializeField] private Sprite BaseDestructionDash;
    [SerializeField] private float BaseDashDistance = 70f;   // Distancia total a recorrer
    [SerializeField] private float BaseDashSpeed = 500f;
    [SerializeField] private float BaseEndLag = 1f; // Tiempo que el jugador se queda sin poder actuar tras el ataque (balanceo)

    [Header("Atributos Berserk")]
    [SerializeField] private Sprite BerserkDestructionSplash;
    [SerializeField] private Sprite BerserkDestructionDash;
    [SerializeField] private float BerserkDashDistance = 100f;   // Distancia total a recorrer
    [SerializeField] private float BerserkDashSpeed = 700f;
    [SerializeField] private float BerserkEndLag = 0.4f; // Tiempo que el jugador se queda sin poder actuar tras el ataque (balanceo)

    // Variables reales que se usan en ejecucion
    private Sprite destructionSplash;
    private Sprite destructionDash;
    private float dashDistance;   // Distancia total a recorrer
    private float dashSpeed;
    private float endLag; // Tiempo que el jugador se queda sin poder actuar tras el ataque (balanceo)

    private Rigidbody rb;
    private GameObject area;
    private float showAreaTime = 0.25f;

    private void Start()
    {
        area = this.gameObject.transform.GetChild(0).GetChild(1).gameObject;
        area.SetActive(false);
        rb = playerContext.Rb;
        NormalMode();
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

    public void NormalMode()
    {
        destructionSplash = BaseDestructionSplash;
        destructionDash = BaseDestructionDash;
        dashDistance = BaseDashDistance;
        dashSpeed = BaseDashSpeed;
        endLag = BaseEndLag;
    }

    public void BerserkMode()
    {
        destructionSplash = BerserkDestructionSplash;
        destructionDash = BerserkDestructionDash;
        dashDistance = BerserkDashDistance;
        dashSpeed = BerserkDashSpeed;
        endLag = BerserkEndLag;
    }


}
