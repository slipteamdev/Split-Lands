using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Jaula : Item
{

    [SerializeField] private float desvanecimiento;

    [SerializeField] private float duracion; // cuanto tiempo dura el efecto de ralentizar

    private Image im;

    [SerializeField] private GameObject jaulaPrefab;
    private GameObject jaula;


    public override void Start()
    {
        base.Start();

        im = GetComponent<Image>();
    }

    public override void Execute()
    {
        if (PlayerOwner == null) return;
        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos

        StartCoroutine(GoToEnemy());

    }

    IEnumerator GoToEnemy()
    {
        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra

        this.transform.SetParent(canva.transform);
        ItemsManager.RemoveObject(this.gameObject);

        transform.position = playerContext.transform.position + new Vector3(0, 2, 0);


        Vector3 desfase = new Vector3(0, 40, 0); // posicion ligeramente sobre la cabeza
        for (int i = 45; i > 0; i--) // 45 por ejemplo
        {
            yield return new WaitForFixedUpdate();
            Vector3 mov = (playerContext.EnemyPlayer.transform.localPosition + desfase - this.transform.localPosition) / i;
            this.transform.localPosition += mov;
        }
        this.transform.SetParent(playerContext.EnemyPlayer.transform.GetChild(1).transform);

        // crea la jaula
        jaula = Instantiate(jaulaPrefab, playerContext.EnemyPlayer.transform.localPosition, Quaternion.identity, canva.gameObject.transform.GetChild(1));
        jaula.transform.localPosition = playerContext.EnemyPlayer.transform.localPosition;

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

        yield return new WaitForSeconds(duracion - tiempo);

        Destroy(jaula);
        Destroy(this.gameObject);
    }

}
