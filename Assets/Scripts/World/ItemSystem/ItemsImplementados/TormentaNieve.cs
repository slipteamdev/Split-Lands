using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class TormentaNieve : Item
{
    [SerializeField] private float duration;
    [SerializeField] private float movementSpeedMultiplier;
    [SerializeField] private float cooldownSpeedMultiplier;


    [SerializeField] private float desvanecimiento = 2f;
    private Image im;
    private GiraGira gira;

    public override void Start()
    {
        base.Start();

        im = GetComponent<Image>();
        gira = GetComponent<GiraGira>();
        gira.enabled = false;
    }
    public override void Execute()
    {
        if (PlayerOwner == null) return;
        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra

        this.transform.SetParent(canva.transform);
        ItemsManager.RemoveObject(this.gameObject);

        transform.position = playerContext.transform.position + new Vector3(0, 2, 0);

        PlayerContext other = PlayerOwner.GetComponent<PlayerContext>().EnemyPlayer.GetComponent<PlayerContext>();
        other.ApplyMovementSpeedModifier(movementSpeedMultiplier, duration);
        other.ApplyCooldownSpeedModifier(cooldownSpeedMultiplier, duration);


        gira.enabled = true;
        StartCoroutine(GoToEnemy());

        // SFX
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayEventSFX();
        }
    }


    IEnumerator GoToEnemy()
    {
        Vector3 desfase = new Vector3(0, 40, 0); // posicion ligeramente sobre la cabeza
        for (int i = 45; i > 0; i--) // 45 por ejemplo
        {
            yield return new WaitForFixedUpdate();
            Vector3 mov = (playerContext.EnemyPlayer.transform.localPosition + desfase - this.transform.localPosition) / i;
            this.transform.localPosition += mov;
        }
        this.transform.SetParent(playerContext.EnemyPlayer.transform.GetChild(1).transform);


        PlayerContext other = PlayerOwner.GetComponent<PlayerContext>().EnemyPlayer.GetComponent<PlayerContext>();
        other.ApplyMovementSpeedModifier(movementSpeedMultiplier, duration);
        other.ApplyCooldownSpeedModifier(cooldownSpeedMultiplier, duration);

        PlayerOwner.GetComponent<PlayerContext>().EnemyPlayer.GetComponent<PlayerContext>().tormenta[0].SetActive(true);
        PlayerOwner.GetComponent<PlayerContext>().EnemyPlayer.GetComponent<PlayerContext>().tormenta[1].SetActive(true);


        Color colorInicial = im.color;
        Color colorFinal = colorInicial;
        colorFinal.a = 0f;

        Vector3 escalaInicial = im.transform.localScale;
        Vector3 escalaFinal = escalaInicial * 2f;

        float tiempo = 0f;
        while (tiempo < desvanecimiento)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / desvanecimiento;

            im.color = Color.Lerp(colorInicial, colorFinal, t);
            im.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, t);

            yield return null;
        }
        im.color = colorFinal;
        im.transform.localScale = escalaFinal;

        yield return new WaitForSeconds(duration - tiempo);

        PlayerOwner.GetComponent<PlayerContext>().EnemyPlayer.GetComponent<PlayerContext>().tormenta[0].SetActive(false);
        PlayerOwner.GetComponent<PlayerContext>().EnemyPlayer.GetComponent<PlayerContext>().tormenta[1].SetActive(false);

        Destroy(this.gameObject);
    }
}
