using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Panal : Item
{
    [SerializeField] private float desvanecimiento;

    [SerializeField] private float duracion; // cuanto tiempo dura el efecto de ralentizar
    [SerializeField] private float multiplicador; // cuanta ralentizacion aplica

    private Image im;
    private void Start()
    {
        im = GetComponent<Image>();
    }

    public override void Execute()
    {
        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra

        this.transform.SetParent(canva.transform);

        transform.position = playerContext.transform.position + new Vector3(0, 2, 0);

        StartCoroutine(GoToEnemy());


    }

    IEnumerator GoToEnemy()
    {
        Vector3 desfase = new Vector3(0, 40, 0); // posicion ligeramente sobre la cabeza
        for (int i = 120; i > 0; i--) // 120 por ejemplo
        {
            yield return null;
            Vector3 mov = (playerContext.EnemyPlayer.transform.localPosition + desfase - this.transform.localPosition) / i;
            this.transform.localPosition += mov;
        }
        this.transform.SetParent(playerContext.EnemyPlayer.transform.transform);
        
        playerContext.EnemyPlayer.GetComponent<PlayerContext>().speed *= multiplicador; // le aplica la ralentizacion
        
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

        playerContext.EnemyPlayer.GetComponent<PlayerContext>().speed /= multiplicador; // le quita la ralentizacion


        Destroy(this.gameObject);
    }


}
