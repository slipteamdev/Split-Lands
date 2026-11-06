using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PlayerSelectionData
{
    public int playerIndex;
    public int characterId;
    public Map.Team team;
}
public class CharacterSelectionData : MonoBehaviour
{

    public static CharacterSelectionData Instance { get; private set; }

    [SerializeField] private GameObject[] characterPrefabs;

    public List<PlayerSelectionData> PlayersData { get; private set; } = new List<PlayerSelectionData>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Recibe una lista con las elecciones definitivas y las guarda para el cambio de escena
    public void SetPlayersData(List<PlayerSelectionData> selections)
    {
        PlayersData = new List<PlayerSelectionData>(selections);
    }

    // Devuelve el prefab asociado al indice dado
    public GameObject GetCharacterPrefab(int characterId)
    {
        if (characterId >= 0 && characterId < characterPrefabs.Length)
        {
            return characterPrefabs[characterId];
        }

        Debug.LogError($"[GameMatchSettings] ID de personaje inválido: {characterId}");
        return characterPrefabs[0];
    }

}
