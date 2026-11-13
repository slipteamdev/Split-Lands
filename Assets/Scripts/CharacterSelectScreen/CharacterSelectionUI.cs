using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    //[SerializeField] private Button[] p1CharacterButtons; // Botones de los jugadores terminados

    //[SerializeField] private Button[] p2CharacterButtons; // Botones de los jugadores terminados

    //PJ
    [SerializeField] private Sprite[] characterSprites;

    //Jugador1
    [SerializeField] private Image p1CharacterImage;
    [SerializeField] private Button p1PreviousButton;
    [SerializeField] private Button p1NextButton;
    [SerializeField] private Button p1CharacterButton;

    //Jugador2
    [SerializeField] private Image p2CharacterImage;
    [SerializeField] private Button p2PreviousButton;
    [SerializeField] private Button p2NextButton;
    [SerializeField] private Button p2CharacterButton;
    
    [SerializeField] private Button startMatchButton;

    //PJs que está visualizando cada jugador
    private int p1CurrentIndex = 0;
    private int p2CurrentIndex = 0;

    // -1 indica que el jugador aun no ha elegido
    private int p1SelectedId = -1;
    private int p2SelectedId = -1;

    //Animación Selección
    [SerializeField] private float scaleMultiplier = 1.15f;
    [SerializeField] private float animationDuration = 0.15f;

    private Coroutine p1SelectionAnimation;
    private Coroutine p2SelectionAnimation;

    private void Start()
    {
        if (characterSprites == null || characterSprites.Length == 0)
        {
            Debug.LogError("No hay personajes asignados.");
            return;
        }

        //// El boton de jugar arranca desactivado
        //if (startMatchButton != null)
        //{
        //    startMatchButton.interactable = false;
        //    startMatchButton.onClick.AddListener(OnStartMatchClicked);
        //}

        //// Suscribir eventos a los botones del Jugador 1
        //for (int i = 0; i < p1CharacterButtons.Length; i++)
        //{
        //    int characterIndex = i; // Copia local para evitar problemas con closures en delegates
        //    p1CharacterButtons[i].onClick.AddListener(() => SelectCharacterP1(characterIndex));
        //}

        //// Suscribir eventos a los botones del Jugador 2
        //for (int i = 0; i < p2CharacterButtons.Length; i++)
        //{
        //    int characterIndex = i;
        //    p2CharacterButtons[i].onClick.AddListener(() => SelectCharacterP2(characterIndex));
        //}

        // Flechas jugador 1
        p1PreviousButton.onClick.AddListener(() => ChangeCharacterP1(-1));
        p1NextButton.onClick.AddListener(() => ChangeCharacterP1(1));

        // Flechas jugador 2
        p2PreviousButton.onClick.AddListener(() => ChangeCharacterP2(-1));
        p2NextButton.onClick.AddListener(() => ChangeCharacterP2(1));

        // Confirmar pulsando sobre el personaje
        p1CharacterButton.onClick.AddListener(ConfirmCharacterP1);
        p2CharacterButton.onClick.AddListener(ConfirmCharacterP2);

        // Botón de jugar
        startMatchButton.interactable = false;
        startMatchButton.onClick.AddListener(OnStartMatchClicked);

        // Mostrar personajes iniciales
        UpdateCharacterImage(p1CharacterImage, p1CurrentIndex);
        UpdateCharacterImage(p2CharacterImage, p2CurrentIndex);
    }

    //Jugador 1
    private void ChangeCharacterP1(int direction)
    {
        p1CurrentIndex = (p1CurrentIndex + direction +
                         characterSprites.Length) %
                         characterSprites.Length;

        p1SelectedId = -1;

        UpdateCharacterImage(p1CharacterImage, p1CurrentIndex);
        CheckCanStart();
    }

    private void ConfirmCharacterP1()
    {
        if (!IsCharacterAvailable(p1CurrentIndex, 1))
        {
            Debug.Log("Ese personaje ya ha sido elegido por el jugador 2.");
            return;
        }

        p1SelectedId = p1CurrentIndex;

        if (p1SelectionAnimation != null)
            StopCoroutine(p1SelectionAnimation);

        p1SelectionAnimation = StartCoroutine(
            PlaySelectionAnimation(p1CharacterImage.rectTransform)
        );

        CheckCanStart();

        Debug.Log("Jugador 1 ha elegido: " + p1SelectedId);
    }

    //Jugador 2
    private void ChangeCharacterP2(int direction)
    {
        p2CurrentIndex = (p2CurrentIndex + direction +
                         characterSprites.Length) %
                         characterSprites.Length;

        p2SelectedId = -1;

        UpdateCharacterImage(p2CharacterImage, p2CurrentIndex);
        CheckCanStart();
    }

    private void ConfirmCharacterP2()
    {
        if (!IsCharacterAvailable(p2CurrentIndex, 2))
        {
            Debug.Log("Ese personaje ya ha sido elegido por el jugador 1.");
            return;
        }

        p2SelectedId = p2CurrentIndex;

        if (p2SelectionAnimation != null)
            StopCoroutine(p2SelectionAnimation);

        p2SelectionAnimation = StartCoroutine(
            PlaySelectionAnimation(p2CharacterImage.rectTransform)
        );

        CheckCanStart();

        Debug.Log("Jugador 2 ha elegido: " + p2SelectedId);
    }

    //Actualizar imágenes PJ
    private void UpdateCharacterImage(Image image, int index)
    {
        image.sprite = characterSprites[index];
        image.preserveAspect = true;
    }

    //Comprobar Slección
    private bool IsCharacterAvailable(int characterIndex, int player)
    {
        if (player == 1 && characterIndex == p2SelectedId)
            return false;

        if (player == 2 && characterIndex == p1SelectedId)
            return false;

        return true;
    }

    private void CheckCanStart()
    {
        bool ready = p1SelectedId != -1 &&
                     p2SelectedId != -1 &&
                     p1SelectedId != p2SelectedId;

        startMatchButton.interactable = ready;
    }

    //Comenzar partida
    private void OnStartMatchClicked()
    {
        if (p1SelectedId == -1 || p2SelectedId == -1)
            return;

        List<PlayerSelectionData> matchSelections =
            new List<PlayerSelectionData>
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
            GameData.Instance.SetPlayersData(matchSelections);
        }
        else
        {
            Debug.LogWarning(
                "[CharacterSelectionUI] GameData no encontrado"
            );
        }

        SceneManager.LoadScene(gameSceneName);
    }

    private IEnumerator PlaySelectionAnimation(RectTransform image)
    {
        Vector3 originalScale = image.localScale;
        Vector3 enlargedScale = originalScale * scaleMultiplier;

        float elapsed = 0f;

        // Aumentar tamaño
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / animationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            image.localScale = Vector3.Lerp(
                originalScale,
                enlargedScale,
                t
            );

            yield return null;
        }

        elapsed = 0f;

        // Volver al tamaño original
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / animationDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            image.localScale = Vector3.Lerp(
                enlargedScale,
                originalScale,
                t
            );

            yield return null;
        }

        image.localScale = originalScale;
    }



    //public void SelectCharacterP1(int characterId)
    //{
    //    p1SelectedId = characterId;
    //    UpdateButtonVisuals(p1CharacterButtons, p1SelectedId);
    //    CheckCanStart();
    //}

    //public void SelectCharacterP2(int characterId)
    //{
    //    p2SelectedId = characterId;
    //    UpdateButtonVisuals(p2CharacterButtons, p2SelectedId);
    //    CheckCanStart();
    //}

    //private void CheckCanStart()
    //{
    //    // Ambos jugadores deben haber seleccionado 
    //    bool ready = (p1SelectedId != -1) && (p2SelectedId != -1);

    //    if (startMatchButton != null)
    //    {
    //        startMatchButton.interactable = ready;
    //    }
    //}


    //private void UpdateButtonVisuals(Button[] buttons, int selectedIndex)
    //{
    //    for (int i = 0; i < buttons.Length; i++)
    //    {
    //        buttons[i].interactable = (i != selectedIndex);
    //    }
    //}

    //private void OnStartMatchClicked()
    //{
    //    if (p1SelectedId == -1 || p2SelectedId == -1) return;

    //    // Construimos la lista de datos para pasar al gestor persistente
    //    List<PlayerSelectionData> matchSelections = new List<PlayerSelectionData>
    //    {
    //        new PlayerSelectionData
    //        {
    //            playerIndex = 0,
    //            characterId = p1SelectedId,
    //            team = Map.Team.Team1
    //        },
    //        new PlayerSelectionData
    //        {
    //            playerIndex = 1,
    //            characterId = p2SelectedId,
    //            team = Map.Team.Team2
    //        }
    //    };

    //    if (GameData.Instance != null)
    //    {
    //        // Guardamos la lista
    //        GameData.Instance.SetPlayersData(matchSelections);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[CharacterSelectionUI] CharacterSelectionData no encontrado en la escena");
    //    }

    //    // Cargar escena de partida
    //    SceneManager.LoadScene(gameSceneName);
    //}
}