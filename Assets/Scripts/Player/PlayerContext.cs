using System.Collections;
using UnityEngine;

public class PlayerContext : MonoBehaviour
{
    [SerializeField] private Map.Team team;
    public Map.Team Team => team;

    public Vector2 orientation;
    public Vector2 Orientation => orientation;
    public RectTransform RectTransform { get; private set; }
    public Rigidbody Rb { get; private set; }

    public float CooldownSpeedMultiplier { get; private set; } = 1.0f;

    public void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Rb = this.gameObject.GetComponent<Rigidbody>();
        orientation = Vector2.right;
    }


    // Contadores internos de bloqueos activos. Usar un contador en vez de un bool permite que bloqueos de causas distintas coexistan, cada vez que algo bloquea, suma 1. Cada vez que algo libera, resta 1
    private int moveBlockCount = 0;
    private int moveInputBlockCount = 0;
    private int itemBlockCount = 0;
    private int skillBlockCount = 0;

    // Solo se permite la accion si no hay bloqueos (contador en 0)
    public bool MoveAllowed => moveBlockCount <= 0; // Controla si el personaje se queda congelado o no. Si se bloquea, se prohibe todo tipo de movimiento
    public bool MoveInputAllowed => moveInputBlockCount <= 0; // Se diferencia de MoveAllowed en que este solo bloquea el movimiento del input, pero permite que otras causas muevan al personaje (por ejemplo un dash)
    public bool ItemAllowed => itemBlockCount <= 0; // Controla si el jugador tiene permitido coger y usar objetos
    public bool SkillAllowed => skillBlockCount <= 0; // Controla si el jugador tiene permitido usar habilidades

    // Los metodos Allow liberan las acciones que hayan sido bloqueadas. Cada llamada reduce en 1 el contador de bloqueos
    public void AllowMove()
    {
        moveBlockCount = Mathf.Max(0, moveBlockCount - 1);
    }
    public void AllowMoveInput()
    {
        moveInputBlockCount = Mathf.Max(0, moveInputBlockCount - 1);
    }
    public void AllowItem()
    {
        itemBlockCount = Mathf.Max(0, itemBlockCount - 1);
    }
    public void AllowSkill()
    {
        skillBlockCount = Mathf.Max(0, skillBlockCount - 1);
    }

    // Los metodos block detienen la accion que controlan. Se pueden llamar sin parametros, con lo cual el bloqueo dura hasta que se quite manualmente,
    // o con un tiempo, lo que hara que el bloqueo se quite solo despues de ese tiempo
    public void BlockMove(float duration = 0f)
    {
        moveBlockCount++;

        if (duration > 0f)
        {
            StartCoroutine(ReleaseAfterDelay(duration, AllowMove));
        }
    }
    public void BlockMoveInput(float duration = 0f)
    {
        moveInputBlockCount++;

        if (duration > 0f)
        {
            StartCoroutine(ReleaseAfterDelay(duration, AllowMoveInput));
        }
    }

    public void BlockItem(float duration = 0f)
    {
        itemBlockCount++;

        if (duration > 0f)
        {
            StartCoroutine(ReleaseAfterDelay(duration, AllowItem));
        }
    }
    public void BlockSkill(float duration = 0f)
    {
        skillBlockCount++;

        if (duration > 0f)
        {
            StartCoroutine(ReleaseAfterDelay(duration, AllowSkill));
        }
    }

    // Recibe un tiempo y un metodo. Cuando haya pasado ese tiempo, activa el metodo
    private IEnumerator ReleaseAfterDelay(float delay, System.Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        onComplete?.Invoke();
    }


    public void SetOrientation(Vector2 orientation)
    {
        this.orientation = orientation;
    }
}
