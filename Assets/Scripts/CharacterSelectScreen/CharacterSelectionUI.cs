using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "VSBeforeGame";

    //[SerializeField] private Button[] p1CharacterButtons; // Botones de los jugadores terminados

    //[SerializeField] private Button[] p2CharacterButtons; // Botones de los jugadores terminados

    //PJ
    [SerializeField] private Sprite[] characterSprites;

    //Jugador1
    [SerializeField] private Image p1CharacterImage;
    [SerializeField] private Animator p1PreviousButton;
    [SerializeField] private Animator p1NextButton;
    [SerializeField] private Animator p1Animator;
    [SerializeField] private TMP_Text p1ReadyText;
    [SerializeField] private InputActionReference p1Move;
    [SerializeField] private InputActionReference p1Ready;

    //Jugador2
    [SerializeField] private Image p2CharacterImage;
    [SerializeField] private Animator p2PreviousButton;
    [SerializeField] private Animator p2NextButton;
    [SerializeField] private Animator p2Animator;
    [SerializeField] private TMP_Text p2ReadyText;
    [SerializeField] private InputActionReference p2Move;
    [SerializeField] private InputActionReference p2Move2;
    [SerializeField] private InputActionReference p2Ready;
    [SerializeField] private InputActionReference p2Ready2;

    [SerializeField] private GameObject countdown;
    [SerializeField] private TMP_Text countdownText;

    //PJs que está visualizando cada jugador
    private int p1CurrentIndex = 0;
    private int p2CurrentIndex = 0;

    // -1 indica que el jugador aun no ha elegido
    private int p1PressedId = -1;
    private int p2PressedId = -1;

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
        //      p1PreviousButton.onClick.AddListener(() => ChangeCharacterP1(-1));
        //      p1NextButton.onClick.AddListener(() => ChangeCharacterP1(1));
        //      
        //      // Flechas jugador 2
        //      p2PreviousButton.onClick.AddListener(() => ChangeCharacterP2(-1));
        //      p2NextButton.onClick.AddListener(() => ChangeCharacterP2(1));
        //      
        //      // Confirmar pulsando sobre el personaje
        //      p1CharacterButton.onClick.AddListener(ConfirmCharacterP1);
        //      p2CharacterButton.onClick.AddListener(ConfirmCharacterP2);

        // Mostrar personajes iniciales
        UpdateCharacterImage(p1CharacterImage, p1CurrentIndex);
        UpdateCharacterImage(p2CharacterImage, p2CurrentIndex);
    }

    private float matchCountdown;
    private void Update()
    {
        countdown.SetActive(startingMatch);

        if (startingMatch)
        {
            if (p1PressedId == -1 || p2PressedId == -1) startingMatch = false;

            matchCountdown -= Time.deltaTime;
            countdownText.SetText(Mathf.CeilToInt(matchCountdown).ToString());

            if (matchCountdown <= 0.1f) OnStartMatchClicked();
        }
        else matchCountdown = 5f;
    }

    //Jugador 1
    public void ChangeCharacterP1(InputAction.CallbackContext context)
    {
        if (context.action.ReadValue<Vector2>().x > 0.5 || context.action.ReadValue<Vector2>().x < -0.5)
        {
            ChangeCharacterP1(context.action.ReadValue<Vector2>().x < 0f ? -1 : 1);
        }
    }
    private bool changingP1 = false;
    private void ChangeCharacterP1(int direction)
    {
        if (changingP1) return;

        p1CurrentIndex = (p1CurrentIndex + direction +
                         characterSprites.Length) %
                         characterSprites.Length;

        p1PressedId = -1;
        p1ReadyText.SetText("Preparandose...");
        startingMatch = false;

        if (direction == -1) p1PreviousButton.SetTrigger("Pressed"); else p1NextButton.SetTrigger("Pressed");

        StartCoroutine(ChangeCharacterP1_EVENT());
        CheckCanStart();
    }
    private IEnumerator ChangeCharacterP1_EVENT()
    {
        p1Animator.SetTrigger("Switch");
        changingP1 = true;

        yield return new WaitForSeconds(0.34f);

        UpdateCharacterImage(p1CharacterImage, p1CurrentIndex);

        yield return new WaitForSeconds(0.69f);

        changingP1 = false;
    }

    public void ConfirmCharacterP1()
    {
        if (!IsCharacterAvailable(p1CurrentIndex, 1))
        {
            p1ReadyText.SetText("Preparandose...");
            Debug.Log("Ese personaje ya ha sido elegido por el jugador 2.");
            return;
        }

        p1PressedId = p1CurrentIndex;

        if (p1SelectionAnimation != null)
            StopCoroutine(p1SelectionAnimation);

        p1SelectionAnimation = StartCoroutine(
            PlaySelectionAnimation(p1CharacterImage.rectTransform)
        );

        CheckCanStart();

        p1ReadyText.SetText("LISTO");
        Debug.Log("Jugador 1 ha elegido: " + p1PressedId);
    }

    //Jugador 2
    public void ChangeCharacterP2(InputAction.CallbackContext context)
    {
        if (context.action.ReadValue<Vector2>().x > 0.5 || context.action.ReadValue<Vector2>().x < -0.5)
        {
            ChangeCharacterP2(context.action.ReadValue<Vector2>().x < 0f ? -1 : 1);
        }
    }
    private bool changingP2 = false;
    private void ChangeCharacterP2(int direction)
    {
        if (changingP2) return;

        p2CurrentIndex = (p2CurrentIndex + direction +
                         characterSprites.Length) %
                         characterSprites.Length;

        p2PressedId = -1;
        p2ReadyText.SetText("Preparandose...");
        startingMatch = false;

        if (direction == -1) p2PreviousButton.SetTrigger("Pressed"); else p2NextButton.SetTrigger("Pressed");

        StartCoroutine(ChangeCharacterP2_EVENT());
        CheckCanStart();
    }
    private IEnumerator ChangeCharacterP2_EVENT()
    {
        p2Animator.SetTrigger("Switch");
        changingP2 = true;

        yield return new WaitForSeconds(0.34f);

        UpdateCharacterImage(p2CharacterImage, p2CurrentIndex);

        yield return new WaitForSeconds(0.69f);

        changingP2 = false;
    }

    public void ConfirmCharacterP2()
    {
        if (!IsCharacterAvailable(p2CurrentIndex, 2))
        {
            p2ReadyText.SetText("Preparandose...");
            Debug.Log("Ese personaje ya ha sido elegido por el jugador 1.");
            return;
        }

        p2PressedId = p2CurrentIndex;

        if (p2SelectionAnimation != null)
            StopCoroutine(p2SelectionAnimation);

        p2SelectionAnimation = StartCoroutine(
            PlaySelectionAnimation(p2CharacterImage.rectTransform)
        );

        CheckCanStart();

        p2ReadyText.SetText("LISTO");
        Debug.Log("Jugador 2 ha elegido: " + p2PressedId);
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
        if (player == 1 && characterIndex == p2PressedId)
            return false;

        if (player == 2 && characterIndex == p1PressedId)
            return false;

        return true;
    }

    private bool startingMatch;
    private void CheckCanStart()
    {
        bool ready = p1PressedId != -1 &&
                     p2PressedId != -1 &&
                     p1PressedId != p2PressedId;

        startingMatch = ready;
    }

    //Comenzar partida
    private void OnStartMatchClicked()
    {
        if (p1PressedId == -1 || p2PressedId == -1)
            return;

        List<PlayerSelectionData> matchSelections =
            new List<PlayerSelectionData>
            {
            new PlayerSelectionData
            {
                playerIndex = 0,
                characterId = p1PressedId,
                team = Map.Team.Team1
            },

            new PlayerSelectionData
            {
                playerIndex = 1,
                characterId = p2PressedId,
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
        Vector3 originalScale = Vector3.one;
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
    //    p1PressedId = characterId;
    //    UpdateButtonVisuals(p1CharacterButtons, p1PressedId);
    //    CheckCanStart();
    //}

    //public void SelectCharacterP2(int characterId)
    //{
    //    p2PressedId = characterId;
    //    UpdateButtonVisuals(p2CharacterButtons, p2PressedId);
    //    CheckCanStart();
    //}

    //private void CheckCanStart()
    //{
    //    // Ambos jugadores deben haber seleccionado 
    //    bool ready = (p1PressedId != -1) && (p2PressedId != -1);

    //    if (startMatchButton != null)
    //    {
    //        startMatchButton.interactable = ready;
    //    }
    //}


    //private void UpdateButtonVisuals(Button[] buttons, int PressedIndex)
    //{
    //    for (int i = 0; i < buttons.Length; i++)
    //    {
    //        buttons[i].interactable = (i != PressedIndex);
    //    }
    //}

    //private void OnStartMatchClicked()
    //{
    //    if (p1PressedId == -1 || p2PressedId == -1) return;

    //    // Construimos la lista de datos para pasar al gestor persistente
    //    List<PlayerSelectionData> matchSelections = new List<PlayerSelectionData>
    //    {
    //        new PlayerSelectionData
    //        {
    //            playerIndex = 0,
    //            characterId = p1PressedId,
    //            team = Map.Team.Team1
    //        },
    //        new PlayerSelectionData
    //        {
    //            playerIndex = 1,
    //            characterId = p2PressedId,
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