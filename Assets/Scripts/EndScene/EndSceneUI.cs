
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSceneUI : MonoBehaviour
{
    [Header("Sprites de personajes")]
    [SerializeField] private Sprite[] characterSprites;

    [Header("GANADOR")]
    [SerializeField] private Image winnerImage;
    [SerializeField] private TextMeshProUGUI winnerTerritoryText;

    [Header("PERDEDOR")]
    [SerializeField] private Image loserImage;
    [SerializeField] private TextMeshProUGUI loserTerritoryText;

    void Start()
    {
        if (GameData.Instance == null)
        {
            Debug.LogError("No hay datos de partida disponibles.");
            return;
        }

        // Obtener porcentajes
        float p1Territory = GameData.Instance.gameResult;
        float p2Territory = 1f - p1Territory;

        // Determinar ganador
        bool p1Won = p1Territory >= p2Territory;

        int winnerIndex = p1Won ? 0 : 1;
        int loserIndex = p1Won ? 1 : 0;

        // Obtener datos de jugadores
        PlayerSelectionData winnerData =
            GameData.Instance.PlayersData[winnerIndex];

        PlayerSelectionData loserData =
            GameData.Instance.PlayersData[loserIndex];

        // Porcentajes
        float winnerPercentage =
            Mathf.Max(p1Territory, p2Territory) * 100f;

        float loserPercentage =
            Mathf.Min(p1Territory, p2Territory) * 100f;

        // Mostrar porcentajes
        winnerTerritoryText.text =
            $" {winnerPercentage:F1}%";

        loserTerritoryText.text =
            $" {loserPercentage:F1}%";

        // Mostrar personajes
        UpdateCharacterImage(winnerImage, winnerData.characterId);
        UpdateCharacterImage(loserImage, loserData.characterId);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private void UpdateCharacterImage(Image image, int characterId)
    {
        if (image == null) return;

        if (characterId < 0 || characterId >= characterSprites.Length)
        {
            Debug.LogError($"ID de personaje inválido: {characterId}");
            return;
        }

        image.sprite = characterSprites[characterId];
        image.preserveAspect = true;
    }
}
