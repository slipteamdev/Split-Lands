using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class Panal : Item
{


    private void Start()
    {

    }

    public override void Execute()
    {
        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra

        this.transform.SetParent(canva.transform);
        Vector2 direction = playerContext.Orientation;
        transform.position = playerContext.transform.position + new Vector3(2, 0, 0);

        StartCoroutine(GoToEnemy());


    }

    IEnumerator GoToEnemy()
    {
        Vector3 desfase = new Vector3(0, 2, 0);
        for (int i = 20; i > 0; i--)
        {
            yield return new WaitForSeconds(Time.time);
            Vector3 mov = (playerContext.EnemyPlayer.transform.localPosition + desfase - this.transform.localPosition) / i;
            this.transform.localPosition += mov;
        }
        /*
        float incremento = 0.02f;
        while () //mientras tenga color
        {
            yield return new WaitForSeconds(Time.time);
            Vector3 mov = (playerContext.EnemyPlayer.transform.position + desfase - this.transform.position) / i;
            this.transform.position += mov;
        }
        */

    }


}
