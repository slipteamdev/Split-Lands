using System.Collections;
using UnityEngine;

public class Ratonera : Item
{
    [SerializeField] private float cooldown;
    [SerializeField] private float stunTime;
    [SerializeField] private float distancia;
    [SerializeField] private GameObject[] usos;
    private int cargas;

    private bool colocada = false;
    private void Start()
    {
        cargas = usos.Length;
    }

    public override void Execute()
    {
        if (PlayerOwner == null) return;
        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra

        this.transform.SetParent(canva.transform);
        ItemsManager.RemoveObject(this.gameObject);

        Vector2 direction = playerContext.Orientation;
        transform.position = playerContext.transform.position + new Vector3(direction.x * 2, direction.y * 2, 0);

        this.transform.GetChild(0).gameObject.SetActive(true);
        colocada = true;

    }

    private void Update()
    {
        if (colocada)
        {
            // detectar victimas y atraparlas

            foreach (GameObject animal in AnimalSpawner.allAnimals)
            {
                if (!animal.IsDestroying() && Vector2.Distance(animal.transform.position, this.transform.position) < distancia) // si pasa la victima
                {
                    Monkey m = animal.GetComponent<Monkey>();
                    if (m != null) // si es mono
                    {
                        m.Soltar();
                        m.totem.RemoveMonkey(animal);
                    }
                    else // si es cangrejo
                    {
                        AnimalMovement am = animal.GetComponent<AnimalMovement>();
                        am.Soltar();
                    }

                    AnimalSpawner.RemoveAnimal(animal);
                    Destroy(animal);

                    cargas--;

                    for (int i = 0; i < usos.Length; i++)
                    {
                        usos[i].SetActive(i < cargas);
                    }
                    // animacion de cerrarse

                    StartCoroutine(Espera());
                    colocada = false;
                    return;
                }
            }
            if (Vector2.Distance(playerContext.EnemyPlayer.transform.position, this.transform.position) < distancia)
            {
                playerContext.EnemyPlayer.gameObject.GetComponent<PlayerContext>().Stun(stunTime); // por ejemplo

                cargas--;
                for (int i = 0; i < usos.Length; i++)
                {
                    usos[i].SetActive(i < cargas);
                }
                // animacion de cerrarse

                StartCoroutine(Espera());
                colocada = false;
                return;
            }

            if (Vector2.Distance(playerContext.gameObject.transform.position, this.transform.position) < distancia)
            {
                playerContext.Stun(stunTime); // por ejemplo

                cargas--;
                for (int i = 0; i < usos.Length; i++)
                {
                    usos[i].SetActive(i < cargas);
                }
                // animacion de cerrarse

                StartCoroutine(Espera());
                colocada = false;
                return;
            }

        }
    }


    IEnumerator Espera()
    {
        yield return new WaitForSeconds(cooldown);

        colocada = true;
        if (cargas <= 0)
        {
            Destroy(this.gameObject);
        }
    }


}
