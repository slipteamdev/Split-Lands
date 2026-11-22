
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public static List<GameObject> allAnimals = new List<GameObject>();

    public List<GameObject> animals = new List<GameObject>();
    public List<Transform> puntosSpawn = new List<Transform>();

    private static GameObject animalsParent;

    public float tiempoEntreSpawns = 4f;
    public float margenDeDesviacion = 0.75f;

    private float tiempoReal;


    public static void AddAnimal(GameObject obj)
    {
        allAnimals.Add(obj);
    }
    public static void RemoveAnimal(GameObject obj)
    {
        allAnimals.Remove(obj);
    }
    public void IncreaseSpawnSpeed(float t)
    {
        tiempoEntreSpawns = Mathf.Max(0.01f, (tiempoEntreSpawns - t));
    }
    private void Start()
    {
        animalsParent = this.gameObject;
        allAnimals = new List<GameObject>();
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            tiempoReal = tiempoEntreSpawns + Random.Range(-margenDeDesviacion, margenDeDesviacion);
            yield return new WaitForSeconds(tiempoReal);

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

        if (Random.value < Map.normalicedProgress) // Probabilidad en funcion del resultado de la partida
        {
            numPunto = Random.Range(4, 8);
            numObjetive = Random.Range(0, 4);
        }
        else
        {
            numPunto = Random.Range(0, 4);
            numObjetive = Random.Range(4, 8);
        }
        Transform punto = puntosSpawn[numPunto];

        Vector2 direccion; //aqui les marco la direccion que deben seguir, luego esto se cambiara segun la dificultad de su comportamiento xq seguramente es mejor que vayan a un destino a que sigan una direccion
        if (numPunto < 4) direccion = Vector2.right;
        else direccion = Vector2.left;


        GameObject animal = SpawnSingleAnimal(animalPrefab, punto);
        animal.GetComponent<AnimalMovement>().AssignObjetive(puntosSpawn[numObjetive].position);

        punto = animal.transform.GetChild(0).transform;
        ItemsManager.SpawnRandomItem(punto.position, punto.rotation, animal.transform);

        // queda fijar su rumbo y que lleve un objeto aleatorio de la lista
        // y tambien que vaya actualizando la lista de objetos y mierdas de esas

    }


    public static GameObject SpawnSingleAnimal(GameObject animalPrefab, Transform punto)
    {
        GameObject animal = Instantiate(animalPrefab, punto.position, punto.rotation, animalsParent.transform);
        AddAnimal(animal);
        return animal;
    }


}
