using UnityEngine;

public class WhirlpoolSkill : Skill
{
    [SerializeField] private WhirlpoolProjectile projectilePrefab;
    [SerializeField] private Sprite destruction;

    [Header("Atributos basicos")]
    [SerializeField] private int projectileCount = 4;
    [SerializeField] private float orbitRadius = 120f;
    [SerializeField] private float orbitSpeed = 180f;
    [SerializeField] private float duration = 5f;

    private PlayerInputController player;
    private RectTransform playerRect;
    private Canvas parentCanvas;

    private void Awake()
    {
        player = GetComponent<PlayerInputController>();
        playerRect = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public override void Execute()
    {
        if (projectilePrefab == null || player == null || parentCanvas == null) return;

        float angleStep = 360f / Mathf.Max(1, projectileCount);

        for (int i = 0; i < projectileCount; i++)
        {
            float startingAngle = i * angleStep;

            WhirlpoolProjectile projectile = Instantiate(projectilePrefab, parentCanvas.transform);

            projectile.Initialize(
                playerRect,
                startingAngle,
                player.team,
                orbitRadius,
                orbitSpeed,
                duration,
                destruction
            );
        }

        Debug.Log($"{gameObject.name} activó {SkillName} ({projectileCount} orbes).");
    }
}