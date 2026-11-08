using TMPro;
using UnityEngine;
// TODO ESTE SCRIPT ES UNA CHAPUZA PLACEHOLDER PARA COMPROBAR SI LOS DATOS NECESARIOS SE PASAN ENTRE ESCENAS

public class ProvisionalUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    void Start()
    {
        if (resultText == null) return;

        if (GameData.Instance == null)
        {
            resultText.text = "No hay datos de partida disponibles (prueba arrancada directamente).";
            return;
        }

        float p1Territory = GameData.Instance.gameResult;
        float p2Territory = 1f - p1Territory;

        float p1Percentage = p1Territory * 100f;
        float p2Percentage = p2Territory * 100f;

        bool p1Won = p1Territory > p2Territory;
        int winnerIndex = p1Won ? 1 : 2;
        float winnerPercentage = p1Won ? p1Percentage : p2Percentage;

        PlayerSelectionData winnerData = GameData.Instance.PlayersData[p1Won ? 0 : 1];
        GameObject winnerPrefab = GameData.Instance.GetCharacterPrefab(winnerData.characterId);
        string characterName = winnerPrefab != null ? winnerPrefab.name : $"Personaje #{winnerData.characterId}";

        resultText.text = $"¡VICTORIA DEL JUGADOR {winnerIndex}!\n\n" +
                          $"Personaje: {characterName}\n" +
                          $"Terreno controlado: {winnerPercentage:F1}%";
    }

}
