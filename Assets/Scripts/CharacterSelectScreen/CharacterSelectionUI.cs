using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    [SerializeField] private Button[] p1CharacterButtons; // Botones de los jugadores terminados

    [SerializeField] private Button[] p2CharacterButtons; // Botones de los jugadores terminados

    [SerializeField] private Button startMatchButton;

    // -1 indica que el jugador aun no ha elegido
    private int p1SelectedId = -1;
    private int p2SelectedId = -1;

    private void Start()
    {
        // El boton de jugar arranca desactivado
        if (startMatchButton != null)
        {
            startMatchButton.interactable = false;
            startMatchButton.onClick.AddListener(OnStartMatchClicked);
        }

        // Suscribir eventos a los botones del Jugador 1
        for (int i = 0; i < p1CharacterButtons.Length; i++)
        {
            int characterIndex = i; // Copia local para evitar problemas con closures en delegates
            p1CharacterButtons[i].onClick.AddListener(() => SelectCharacterP1(characterIndex));
        }

        // Suscribir eventos a los botones del Jugador 2
        for (int i = 0; i < p2CharacterButtons.Length; i++)
        {
            int characterIndex = i;
            p2CharacterButtons[i].onClick.AddListener(() => SelectCharacterP2(characterIndex));
        }
    }

    public void SelectCharacterP1(int characterId)
    {
        p1SelectedId = characterId;
        UpdateButtonVisuals(p1CharacterButtons, p1SelectedId);
        CheckCanStart();
    }

    public void SelectCharacterP2(int characterId)
    {
        p2SelectedId = characterId;
        UpdateButtonVisuals(p2CharacterButtons, p2SelectedId);
        CheckCanStart();
    }

    private void CheckCanStart()
    {
        // Ambos jugadores deben haber seleccionado 
        bool ready = (p1SelectedId != -1) && (p2SelectedId != -1);

        if (startMatchButton != null)
        {
            startMatchButton.interactable = ready;
        }
    }


    private void UpdateButtonVisuals(Button[] buttons, int selectedIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = (i != selectedIndex);
        }
    }

    private void OnStartMatchClicked()
    {
        if (p1SelectedId == -1 || p2SelectedId == -1) return;

        // Construimos la lista de datos para pasar al gestor persistente
        List<PlayerSelectionData> matchSelections = new List<PlayerSelectionData>
        {
            new PlayerSelectionData
            {
                playerIndex = 0,
                characterId = p1SelectedId,
                team = Map.Team.Team1
            },
            new PlayerSelectionData
            {
                playerIndex = 1,
                characterId = p2SelectedId,
                team = Map.Team.Team2
            }
        };

        if (GameData.Instance != null)
        {
            // Guardamos la lista
            GameData.Instance.SetPlayersData(matchSelections);
        }
        else
        {
            Debug.LogWarning("[CharacterSelectionUI] CharacterSelectionData no encontrado en la escena");
        }

        // Cargar escena de partida
        SceneManager.LoadScene(gameSceneName);
    }
}