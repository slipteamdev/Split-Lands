
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public List<GameObject> animals = new List<GameObject>();
    public List<Transform> puntosSpawn = new List<Transform>();
    public List<Item> objetos = new List<Item>();

    public Transform destino;

    public float tiempoEntreSpawns = 3f;


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

        if (destino == null)
            return;

        GameObject animalPrefab = animals[Random.Range(0, animals.Count)]; //elige un animal aleatorio
        Transform punto = puntosSpawn[Random.Range(0, puntosSpawn.Count)]; //le dice donde aparece
        GameObject animal = Instantiate(animalPrefab, punto.position, punto.rotation); 
        // queda fijar su rumbo y que lleve un objeto aleatorio de la lista
        // y tambien que vaya actualizando la lista de objetos y mierdas de esas
        // y tambien alterar las probabilidades para que haya mas igualdad entre jugadores

    }

}
