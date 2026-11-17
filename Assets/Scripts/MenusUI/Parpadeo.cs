using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.MaterialProperty;
using static UnityEngine.EventSystems.EventTrigger;

public class Parpadeo : MonoBehaviour
{

    [SerializeField] private float desvanecimiento;

    private TextMeshProUGUI texto;
    private void Start()
    {
        texto = GetComponent<TextMeshProUGUI>();
        StartCoroutine(Desvanecimiento());
    }

    IEnumerator Desvanecimiento()
    {
        Color colorInicial = texto.color;
        Color colorFinal = colorInicial;
        colorFinal.a = 0.25f;


        float tiempo = 0f;
        while (tiempo < desvanecimiento)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / desvanecimiento;

            texto.color = Color.Lerp(colorInicial, colorFinal, t);

            yield return null;
        }
        texto.color = colorFinal;

        StartCoroutine(Aparecimiento());
    }

    IEnumerator Aparecimiento()
    {
        Color colorInicial = texto.color;
        Color colorFinal = colorInicial;
        colorFinal.a = 1f;

        Vector3 escalaInicial = texto.transform.localScale;

        float tiempo = 0f;
        while (tiempo < desvanecimiento)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / desvanecimiento;

            texto.color = Color.Lerp(colorInicial, colorFinal, t);

            yield return null;
        }
        texto.color = colorFinal;

        StartCoroutine(Desvanecimiento());
    }
}
