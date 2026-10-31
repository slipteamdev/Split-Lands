using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class WhirlpoolProjectile : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform targetPlayerRect;
    private Map.Team team;

    private float orbitRadius;
    private float orbitSpeed;
    [SerializeField] private float selfRotationSpeed = 360f;
    private float lifeTimer;
    private Sprite destruction;

    private float currentAngle;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(
        RectTransform playerRect,
        float startingAngleDeg,
        Map.Team originalTeam,
        float newOrbitRadius,
        float newOrbitSpeed,
        float newDuration,
        Sprite newDestruction)
    {
        targetPlayerRect = playerRect;
        currentAngle = startingAngleDeg;
        team = originalTeam;
        orbitRadius = newOrbitRadius;
        orbitSpeed = newOrbitSpeed;
        lifeTimer = newDuration;
        destruction = newDestruction;

        UpdateOrbitPosition();
    }

    private void Update()
    {
        if (targetPlayerRect == null)
        {
            Destroy(gameObject);
            return;
        }

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        UpdateOrbitPosition();

        rectTransform.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);

        if (destruction != null)
        {
            Map.Draw(transform.localPosition, destruction, team);
        }
    }

    private void UpdateOrbitPosition()
    {
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;
        rectTransform.anchoredPosition = targetPlayerRect.anchoredPosition + offset;
    }
}
