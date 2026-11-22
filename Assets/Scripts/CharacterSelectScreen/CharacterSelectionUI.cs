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


    //Jugador2
    [SerializeField] private Image p2CharacterImage;
    [SerializeField] private Animator p2PreviousButton;
    [SerializeField] private Animator p2NextButton;
    [SerializeField] private Animator p2Animator;
    [SerializeField] private TMP_Text p2ReadyText;

    [Space]
    [SerializeField] private PlayerInput p1Input;
    [SerializeField] private TMP_Text p1LeftButtonText;
    [SerializeField] private TMP_Text p1RightButtonText;
    [SerializeField] private TMP_Text p1ReadyButtonText;

    [SerializeField] private PlayerInput p2Input;
    [SerializeField] private TMP_Text p2LeftButtonText;
    [SerializeField] private TMP_Text p2RightButtonText;
    [SerializeField] private TMP_Text p2ReadyButtonText;

    [SerializeField] private GameObject countdown;
    [SerializeField] private TMP_Text countdownText;

    //PJs que está visualizando cada jugador
    private int p1CurrentIndex = 0;
    private int p2CurrentIndex = 0;

    // -1 indica que el jugador aun no ha elegido
    public int p1PressedId = -1;
    public int p2PressedId = -1;

    //Animación Selección
    [SerializeField] private float scaleMultiplier = 1.15f;
    [SerializeField] private float animationDuration = 0.15f;

    private Coroutine p1SelectionAnimation;
    private Coroutine p2SelectionAnimation;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Start()
    {
        if (characterSprites == null || characterSprites.Length == 0)
        {
            Debug.LogError("No hay personajes asignados.");
            return;
        }



        // Mostrar personajes iniciales
        UpdateCharacterImage(p1CharacterImage, p1CurrentIndex);
        UpdateCharacterImage(p2CharacterImage, p2CurrentIndex);

        SetInputText(p1Input, p1LeftButtonText, p1RightButtonText, p1ReadyText);
        SetInputText(p2Input, p2LeftButtonText, p2RightButtonText, p2ReadyText);
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

    private void SetInputText(PlayerInput input, TMP_Text leftButton, TMP_Text rightButton, TMP_Text confirmButton)
    {
        if (input.currentControlScheme == "WASD")
        {
            leftButton.SetText("(A)");
            rightButton.SetText("(D)");
            confirmButton.SetText("(F)");

            leftButton.fontSize = 51;
            rightButton.fontSize = 51;
            confirmButton.fontSize = 51;
        }
        else if (input.currentControlScheme == "Arrows")
        {
            leftButton.SetText("Flecha\nIzq.");
            rightButton.SetText("Flecha\nDer.");
            confirmButton.SetText("(Ñ)");

            leftButton.fontSize = 26.4f;
            rightButton.fontSize = 26.4f;
            confirmButton.fontSize = 51;
        }
        else if (input.currentControlScheme == "GamePad")
        {
            leftButton.SetText("Izq.\n[Joystick]");
            rightButton.SetText("Der.\n[Joystick]");
            confirmButton.SetText("(Botón izquierdo [Mando])");

            leftButton.fontSize = 26.4f;
            rightButton.fontSize = 26.4f;
            confirmButton.fontSize = 26.4f;
        }
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
        p1ReadyButtonText.SetText("Preparandose...");
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
            p1ReadyButtonText.SetText("Preparandose...");
            Debug.Log("Ese personaje ya ha sido elegido por el jugador 2.");
            return;
        }

        if (p1PressedId != -1)
        {
            p1PressedId = -1;
            p1ReadyButtonText.SetText("Preparandose...");
            Debug.Log("Preparandose de nuevo.");
            return;
        }

        p1PressedId = p1CurrentIndex;

        if (p1SelectionAnimation != null)
            StopCoroutine(p1SelectionAnimation);

        p1SelectionAnimation = StartCoroutine(
            PlaySelectionAnimation(p1CharacterImage.rectTransform)
        );

        CheckCanStart();

        p1ReadyButtonText.SetText("LISTO");
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
        p2ReadyButtonText.SetText("Preparandose...");
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
            p2ReadyButtonText.SetText("Preparandose...");
            Debug.Log("Ese personaje ya ha sido elegido por el jugador 1.");
            return;
        }

        if (p2PressedId != -1)
        {
            p2PressedId = -1;
            p2ReadyButtonText.SetText("Preparandose...");
            Debug.Log("Preparandose de nuevo.");
            return;
        }

        p2PressedId = p2CurrentIndex;

        if (p2SelectionAnimation != null)
            StopCoroutine(p2SelectionAnimation);

        p2SelectionAnimation = StartCoroutine(
            PlaySelectionAnimation(p2CharacterImage.rectTransform)
        );

        CheckCanStart();

        p2ReadyButtonText.SetText("LISTO");
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




}