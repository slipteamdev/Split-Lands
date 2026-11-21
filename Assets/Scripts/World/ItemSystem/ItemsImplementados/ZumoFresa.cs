using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ZumoFresa : Item
{
    [SerializeField] private float duration;
    [SerializeField] private float multiplier;

    [SerializeField] private float desvanecimiento = 2f;
    private Image im;

    private void Start()
    {
        im = GetComponent<Image>();
    }

    public override void Execute()
    {
        if (PlayerOwner == null) return;
        PlayerOwner.GetComponent<PlayerContext>().ApplyMovementSpeedModifier(multiplier, duration);
        ItemsManager.RemoveObject(this.gameObject);

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra
        this.transform.SetParent(canva.transform);

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

        Destroy(gameObject);

    }
}
