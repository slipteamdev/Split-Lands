using UnityEngine;


// Todo este script lo que hace es instanciar un boomerang, cuyo funcionamiento se encuentra en BoomerangProjectile
public class BoomerangSkill : Skill
{
    [SerializeField] private BoomerangProjectile boomerangPrefab;

    [Header("Atributos basicos")]
    [SerializeField] private float maxSpeed = 700f;          // Velocidad maxima
    [SerializeField] private float minSpeed = 40f;           // Velocidad minima
    [SerializeField] private float maxDistance = 350f;       // Alcance de la habilidad
    [SerializeField] private float catchRadius = 30f;
    [SerializeField] private Sprite destruction;

    [Header("Curvas de velocidad")]
    [SerializeField]
    private AnimationCurve outwardSpeedCurve = new AnimationCurve(
        //ignorar estos valores, los que importan son los asignados en editor
        new Keyframe(0f, 1f),
        new Keyframe(1f, 0f)
    );

    [SerializeField] private float returnTime = 1.2f;
    [SerializeField]
    private AnimationCurve returnSpeedCurve = new AnimationCurve(
        //ignorar estos valores, los que importan son los asignados en editor
        new Keyframe(0f, 0f),
        new Keyframe(1f, 1f)
    );

    private PlayerInputController player;
    private RectTransform playerRect;
    private Canvas parentCanvas;

    private void Awake()
    {
        player = GetComponent<PlayerInputController>();
        playerRect = GetComponent<RectTransform>();

        // Busca el Canvas padre para instanciar el proyectil dentro de su jerarquía
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public override void Execute()
    {
        if (boomerangPrefab == null || player == null || parentCanvas == null) return;

        Vector2 shootDirection = player.orientation;

        // Instanciamos el boomerang como hijo del Canvas
        BoomerangProjectile projectile = Instantiate(boomerangPrefab, parentCanvas.transform);

        // Posicion inicial
        RectTransform projectileRect = projectile.GetComponent<RectTransform>();
        projectileRect.anchoredPosition = playerRect.anchoredPosition;

        // Inicializamos su direccion, pasamos la referencia al jugador objetivo, y su equipo
        projectile.Initialize(
            playerRect,
            shootDirection,
            player.team,
            maxSpeed,
            minSpeed,
            maxDistance,
            catchRadius,
            returnTime,
            outwardSpeedCurve,
            returnSpeedCurve,
            destruction
        );

        Debug.Log($"{gameObject.name} lanzó un Boomerang.");
    }
}