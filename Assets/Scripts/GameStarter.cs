using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private UIManager UIManager;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Canvas canvas;

    private List<GameObject> playerList = new List<GameObject>();

    void Start()
    {
        if (GameData.Instance == null || GameData.Instance.PlayersData.Count == 0)
        {
            Debug.LogWarning("[PlayerSpawner] No hay datos de selección previos. ¿Arrancaste directamente desde la escena de juego?");
            // TODO: Annadir inicializacion por defecto
            return;
        }

        // Coge las elecciones de jugador desde la instancia
        var players = GameData.Instance.PlayersData;

        for (int i = 0; i < players.Count; i++)
        {
            PlayerSelectionData data = players[i];

            // Spawnea los personajes en el lugar que les toque
            Transform spawn = (i < spawnPoints.Length) ? spawnPoints[i] : transform;

            // Sacamos el prefab a partir de la instancia
            GameObject prefab = GameData.Instance.GetCharacterPrefab(data.characterId);

            // Instanciamos el prefab como hijo del canvas
            GameObject playerObj = Instantiate(prefab, spawn.position, spawn.rotation, canvas.transform);

            // Inicializamos los datos del jugador (por ahora el equipo)
            PlayerContext context = playerObj.GetComponent<PlayerContext>();
            if (context != null)
            {
                context.Init(data.team, Vector2.right);
            }

            // Annadimos el gameobject a al lista para los siguientes pasos
            playerList.Add(playerObj);
        }

        for (int i = 0; i < playerList.Count; i++)
        {
            PlayerContext context = playerList[i].GetComponent<PlayerContext>();
            if (context != null)
            {
                context.EnemyPlayer = playerList[(i + 1) % 2];
            }

        }

        // Asignamos input
        PlayerInputManagerInitialicer.Init(playerList[0], playerList[1]);

        // Inicializamos la UI
        UIManager.Init(playerList[0], playerList[1]);



    }

}
