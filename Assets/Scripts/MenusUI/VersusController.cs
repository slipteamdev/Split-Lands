
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class VersusController : MonoBehaviour
{
    [Header("Personajes")]
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private string[] characterNames;

    [Header("Jugador 1")]
    [SerializeField] private Image p1CharacterImage;
    [SerializeField] private TMP_Text p1CharacterName;

    [Header("Jugador 2")]
    [SerializeField] private Image p2CharacterImage;
    [SerializeField] private TMP_Text p2CharacterName;

    [Header("Transicion")]
    [SerializeField] private float waitTime = 4f;
    [SerializeField] private string gameSceneName = "Game";

    private void Start()
    {
        ShowSelectedCharacters();

        StartCoroutine(LoadGameAfterDelay());
    }

    private void ShowSelectedCharacters()
    {
        if (GameData.Instance == null)
        {
            Debug.LogError("No se ha encontrado GameData.");
            return;
        }

        List<PlayerSelectionData> players = GameData.Instance.PlayersData;

        if (players == null || players.Count < 2)
        {
            Debug.LogError("No se han encontrado los dos jugadores.");
            return;
        }

        foreach (PlayerSelectionData player in players)
        {
            int id = player.characterId;

            if (id < 0 || id >= characterSprites.Length ||
                id >= characterNames.Length)
            {
                Debug.LogError("ID de personaje inválido: " + id);
                return;
            }

            if (player.playerIndex == 0)
            {
                p1CharacterImage.sprite = characterSprites[id];
                p1CharacterImage.preserveAspect = true;
                p1CharacterName.text = characterNames[id];
            }
            else if (player.playerIndex == 1)
            {
                p2CharacterImage.sprite = characterSprites[id];
                p2CharacterImage.preserveAspect = true;
                p2CharacterName.text = characterNames[id];
            }
        }
    }

    private IEnumerator LoadGameAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(gameSceneName);
    }
}
