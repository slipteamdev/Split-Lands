
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    private static List<GameObject> allAnimals = new List<GameObject>();

    public List<GameObject> animals = new List<GameObject>();
    public List<Transform> puntosSpawn = new List<Transform>();
    public List<GameObject> objects = new List<GameObject>();

    [SerializeField] private GameObject animalsParent;

    public float tiempoEntreSpawns = 3f;


    public static void AddAnimal(GameObject obj)
    {
        allAnimals.Add(obj);
    }
    public static void RemoveAnimal(GameObject obj)
    {
        allAnimals.Remove(obj);
    }
    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntreSpawns);

            SpawnAnimal();
        }
    }

    public void SpawnAnimal()
    {
        if (animals.Count == 0 || puntosSpawn.Count == 0)
            return;


        GameObject animalPrefab = animals[Random.Range(0, animals.Count)]; //elige un animal aleatorio

        int numPunto;
        int numObjetive;
        if (Map.normalicedProgress < 0.45) //le dice donde aparece en funcion de quien vaya ganagdo
        {
            numPunto = Random.Range(0, 4);
            numObjetive = Random.Range(4, 8);
        }
        else if (Map.normalicedProgress > 0.55)
        {
            numPunto = Random.Range(4, 8);
            numObjetive = Random.Range(0, 4);
        }
        else
        {
            numPunto = Random.Range(0, puntosSpawn.Count);
            numObjetive = numPunto < 4 ? Random.Range(4, 8) : Random.Range(0, 4);
        }
        Transform punto = puntosSpawn[numPunto];

        Vector2 direccion; //aqui les marco la direccion que deben seguir, luego esto se cambiara segun la dificultad de su comportamiento xq seguramente es mejor que vayan a un destino a que sigan una direccion
        if (numPunto < 4) direccion = Vector2.right;
        else direccion = Vector2.left;


        GameObject animal = Instantiate(animalPrefab, punto.position, punto.rotation, animalsParent.transform);
        animal.GetComponent<AnimalMovement>().AssignObjetive(puntosSpawn[numObjetive].position);
        AddAnimal(animal);

        GameObject itemPrefab = objects[Random.Range(0, objects.Count)].gameObject; //elige un objeto aleatorio, luego habra que marcar rarezas entre ellos
        punto = animal.transform.GetChild(0).transform;
        GameObject obj = Instantiate(itemPrefab, punto.position, punto.rotation, animal.transform); //le meto como hijo pa que se muevan juntos
        ItemsManager.AddObject(obj);

        // queda fijar su rumbo y que lleve un objeto aleatorio de la lista
        // y tambien que vaya actualizando la lista de objetos y mierdas de esas

    }

}
