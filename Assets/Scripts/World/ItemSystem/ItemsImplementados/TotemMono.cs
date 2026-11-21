using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class TotemMono : Item
{

    [SerializeField] private float desvanecimiento;

    [SerializeField] private float duracion; // cuanto tiempo duran los monos
    [SerializeField] private float radio = 2f; // distancia a la que salen monos
    [SerializeField] private int numMonkeys = 5; // cuantos monos quieres

    public GameObject macacoPrefab;

    private List<GameObject> monkeyList = new List<GameObject>();

    private Image im;
    private void Start()
    {
        im = GetComponent<Image>();
    }

    public void AddMonkey(GameObject obj)
    {
        monkeyList.Add(obj);
    }
    public void RemoveMonkey(GameObject obj)
    {
        monkeyList.Remove(obj);
    }
    public override void Execute()
    {
        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra

        this.transform.SetParent(canva.transform);
        ItemsManager.RemoveObject(this.gameObject);

        transform.position = playerContext.transform.position + new Vector3(0, 2, 0);

        for (int i = 0; i < numMonkeys; i++) {
            float angulo = (i * 360 / numMonkeys) * Mathf.Deg2Rad;

            float x = - Mathf.Sin(angulo) * radio;
            float y = Mathf.Cos(angulo) * radio;

            Transform punto = this.transform;
            punto.localPosition = this.transform.localPosition + new Vector3(x, y, 0);
            GameObject mono = AnimalSpawner.SpawnSingleAnimal(macacoPrefab, punto);
            monkeyList.Add(mono);
            AnimalMovement am = mono.GetComponent<AnimalMovement>();
            if (am != null)
            {
                am.dieWhenObjetive = false;
            }
            Monkey monkey = mono.GetComponent<Monkey>();
            if (monkey != null)
            {
                monkey.playerContext = playerContext;
                monkey.FindObjetive();
                monkey.totem = this;
            }
        }
        StartCoroutine(Desvanecimiento());

        StartCoroutine(MonkeysDying());

    }

    public void FindNewObjetive(GameObject monkey)
    {
        foreach (GameObject m in AnimalSpawner.allAnimals)
        {
            if (m.GetComponent<Monkey>() != null)
            {
                if (!m.gameObject.IsDestroying() && m != monkey)
                {
                    m.GetComponent<Monkey>().FindObjetive();
                }
            }
        }
    }

    IEnumerator MonkeysDying()
    {
        yield return new WaitForSeconds(duracion);

        foreach (GameObject monkey in monkeyList)
        {
            monkey.GetComponent<Monkey>().Soltar();
            AnimalSpawner.RemoveAnimal(monkey);
            Destroy(monkey);
        }
        Destroy(this.gameObject);
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

    }


}
