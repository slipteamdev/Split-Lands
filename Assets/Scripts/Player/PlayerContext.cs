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

    public Sprite background;
    public GameObject EnemyPlayer;

    public GameObject[] UIEfects;
    public GameObject[] tormenta = new GameObject[2];



    public Canvas canvas { get; private set; }

    public float CooldownSpeedMultiplier { get; private set; } = 1.0f;
    public float MovementSpeedMultiplier { get; private set; } = 1.0f;
    public float StunDurationMultiplier { get; private set; } = 1.0f;

    public void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        Rb = this.gameObject.GetComponent<Rigidbody>();
        orientation = Vector2.right;
        canvas = this.transform.GetComponentInParent<Canvas>();
    }

    public void Init(Map.Team team, Vector2 orientation)
    {
        this.team = team;
        this.orientation = orientation;
    }


    // Contadores internos de bloqueos activos. Usar un contador en vez de un bool permite que bloqueos de causas distintas coexistan, cada vez que algo bloquea, suma 1. Cada vez que algo libera, resta 1
    private int moveBlockCount = 0;
    private int moveInputBlockCount = 0;
    private int itemBlockCount = 0;
    private int skillBlockCount = 0;

    // Contadores internos de buffos
    private int stunBuffCount = 0;
    private int movementSpeedBuffCount = 0;
    private int cooldownSpeedBuffCount = 0;

    // Contadores internos de debuffos
    private int invertedControlCount = 0;

    private int stunDebuffCount = 0;
    private int movementSpeedDebuffCount = 0;
    private int cooldownSpeedDebuffCount = 0;


    // Solo se permite la accion si no hay bloqueos (contador en 0)
    public bool MoveAllowed => moveBlockCount <= 0; // Controla si el personaje se queda congelado o no. Si se bloquea, se prohibe todo tipo de movimiento
    public bool MoveInputAllowed => moveInputBlockCount <= 0; // Se diferencia de MoveAllowed en que este solo bloquea el movimiento del input, pero permite que otras causas muevan al personaje (por ejemplo un dash)
    public bool ItemAllowed => itemBlockCount <= 0; // Controla si el jugador tiene permitido coger y usar objetos
    public bool SkillAllowed => skillBlockCount <= 0; // Controla si el jugador tiene permitido usar habilidades


    // BOOLEANOS USADOS PARA DETECTAR BUFFOS Y DEBUFFOS, PARA APLICAR UI
    public bool StunBuff => stunBuffCount > 0;
    public bool MovementSpeedBuff => movementSpeedBuffCount > 0;
    public bool CooldownSpeedBuff => cooldownSpeedBuffCount > 0;

    public bool InvertedControl => invertedControlCount > 0;
    public bool StunDebuff => stunDebuffCount > 0;
    public bool MovementSpeedDebuff => movementSpeedDebuffCount > 0;
    public bool CooldownSpeedDebuff => cooldownSpeedDebuffCount > 0;
    // FIN BOOLEANOS


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

    public void Update()
    {
        if (UIEfects != null) // ESTO ESTARIA GUAY NO PONERLO EN EL UPDATE, PERO DE MOMENTO VA
        {
            UIEfects[0].SetActive(StunBuff);
            UIEfects[1].SetActive(MovementSpeedBuff);
            UIEfects[2].SetActive(MovementSpeedDebuff);
            UIEfects[3].SetActive(CooldownSpeedBuff);
            UIEfects[4].SetActive(CooldownSpeedDebuff);
            UIEfects[5].SetActive(!MoveInputAllowed);
            UIEfects[6].SetActive(InvertedControl);
        }
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

    public void Stun(float duration = 1f)
    {
        moveBlockCount++;
        moveInputBlockCount++;
        itemBlockCount++;
        skillBlockCount++;
        duration *= StunDurationMultiplier;

        StartCoroutine(ReleaseAfterDelay(duration, AllowMove));
        StartCoroutine(ReleaseAfterDelay(duration, AllowMoveInput));
        StartCoroutine(ReleaseAfterDelay(duration, AllowItem));
        StartCoroutine(ReleaseAfterDelay(duration, AllowSkill));

    }



    public void ApplyMovementSpeedModifier(float multiplier, float duration = 0f)
    {
        if (multiplier > 1f)
        {
            movementSpeedBuffCount++;
        }
        else if (multiplier < 1f)
        {
            movementSpeedDebuffCount++;
        }
        MovementSpeedMultiplier *= multiplier;
        StartCoroutine(ReleaseAfterDelay(duration, () =>
        {
            MovementSpeedMultiplier /= multiplier;
            if (multiplier > 1f)
            {
                movementSpeedBuffCount--;
            }
            else if (multiplier < 1f)
            {
                movementSpeedDebuffCount--;
            }
        }));
    }

    public void ApplyCooldownSpeedModifier(float multiplier, float duration = 0f)
    {
        if (multiplier > 1f)
        {
            cooldownSpeedBuffCount++;
        }
        else if (multiplier < 1f)
        {
            cooldownSpeedDebuffCount++;
        }
        CooldownSpeedMultiplier *= multiplier;
        StartCoroutine(ReleaseAfterDelay(duration, () =>
        {
            CooldownSpeedMultiplier /= multiplier;
            if (multiplier > 1f)
            {
                cooldownSpeedBuffCount--;
            }
            else if (multiplier < 1f)
            {
                cooldownSpeedDebuffCount--;
            }
        }));
    }

    public void ApplyVisualCooldownSpeedModifier(float multiplier, float duration = 0f) // ESTE METODO NO MODIFICA LA VELOCIDAD, SOLO SIRVE PARA INDICAR A LOS CONTADORES QUE SE ESTA APLICANDO UN MODIFICADOR DE COOLDOWN DIRECTAMENTE A UNA HABILIDAD CONCRETA (PREGUNTAR ANTES DE USAR)
    {
        if (multiplier > 1f)
        {
            cooldownSpeedBuffCount++;
        }
        else if (multiplier < 1f)
        {
            cooldownSpeedDebuffCount++;
        }
        StartCoroutine(ReleaseAfterDelay(duration, () =>
        {
            if (multiplier > 1f)
            {
                cooldownSpeedBuffCount--;
            }
            else if (multiplier < 1f)
            {
                cooldownSpeedDebuffCount--;
            }
        }));
    }

    public void ApplyStunDurationModifier(float multiplier, float duration = 0f)
    {
        StunDurationMultiplier *= multiplier;
        if (multiplier < 1f)
        {
            stunBuffCount++;
        }
        else if (multiplier > 1f)
        {
            stunDebuffCount++;
        }
        StartCoroutine(ReleaseAfterDelay(duration, () =>
        {
            StunDurationMultiplier /= multiplier;
            if (multiplier < 1f)
            {
                stunBuffCount--;
            }
            else if (multiplier > 1f)
            {
                stunDebuffCount--;
            }
        }));
    }

    public void ApplyInvertedControl(float duration = 0f)
    {
        invertedControlCount++;
        StartCoroutine(ReleaseAfterDelay(duration, () => invertedControlCount--));
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
