using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Lira : Item
{

    [SerializeField] private float desvanecimiento;

    [SerializeField] private float duracion; // cuanto tiempo duran la musica

    private Image im;

    public override void Start()
    {
        base.Start();

        im = GetComponent<Image>();
    }
    public override void Execute()
    {
        if (PlayerOwner == null) return;
        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos

        // GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra
        // this.transform.SetParent(canva.transform);
        ItemsManager.RemoveObject(this.gameObject);

        transform.position = playerContext.transform.position + new Vector3(0, 2, 0);

        AnimalSpawner.AddLira(this.PlayerOwner.gameObject);

        StartCoroutine(Desvanecimiento());

    }


    IEnumerator Desvanecimiento()
    {
        Color colorInicial = im.color;
        Color colorFinal = colorInicial;
        colorFinal.a = 0f;

        Vector3 escalaInicial = im.transform.localScale;
        Vector3 escalaFinal = escalaInicial * 2.5f;

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

        AnimalSpawner.RemoveLira(this.PlayerOwner.gameObject);

        Destroy(this.gameObject);
    }

}
