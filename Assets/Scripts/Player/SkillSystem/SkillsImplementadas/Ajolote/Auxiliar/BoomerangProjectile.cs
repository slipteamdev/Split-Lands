using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class BoomerangProjectile : MonoBehaviour
{
    private enum State { GoingOut, Returning }
    private State currentState = State.GoingOut;
    private Map.Team team;


    private float maxSpeed;                                      // Velocidad maxima
    private float minSpeed;                                      // Velocidad minima
    private float maxDistance;                                   // Alcance de la habilidad
    [SerializeField] private float selfRotationSpeed = 720f;         // Velocidad de rotacion del sprite, unicamente visual
    private float catchRadius;
    private Sprite destruction;



    private AnimationCurve outwardSpeedCurve;
    private float returnTime;
    private AnimationCurve returnSpeedCurve;




    private RectTransform rectTransform;
    private RectTransform targetPlayerRect;
    private Vector2 startPosition;
    private Vector2 travelDirection;

    private float returnTimer = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(
        RectTransform playerRect,
        Vector2 direction,
        Map.Team originalTeam,
        float newMaxSpeed,
        float newMinSpeed,
        float newMaxDistance,
        float newCatchRadius,
        float newReturnTime,
        AnimationCurve newOutwardSpeedCurve,
        AnimationCurve newReturnSpeedCurve,
        Sprite newDestruction)
    {
        targetPlayerRect = playerRect;
        travelDirection = direction.normalized;
        startPosition = rectTransform.anchoredPosition;
        currentState = State.GoingOut;
        team = originalTeam;
        returnTimer = 0f;

        maxSpeed = newMaxSpeed;
        minSpeed = newMinSpeed;
        maxDistance = newMaxDistance;
        catchRadius = newCatchRadius;
        returnTime = newReturnTime;
        outwardSpeedCurve = newOutwardSpeedCurve;
        returnSpeedCurve = newReturnSpeedCurve;
        destruction = newDestruction;
    }

    private void Update()
    {

        rectTransform.Rotate(0f, 0f, selfRotationSpeed * Time.deltaTime);
        Map.Draw(this.transform.localPosition, destruction, team);

        // maquina de estados
        if (currentState == State.GoingOut)
        {
            MoveOutward();
        }
        else
        {
            ReturnToPlayer();
        }
    }

    private void MoveOutward()
    {
        float distanceTraveled = Vector2.Distance(startPosition, rectTransform.anchoredPosition);

        // t es la variable que se usa para las curvas. En este caso se va moviendo desde 0 hasta 1 en función de lo cerca que esté de alcanzar la distancia maxima
        float t = Mathf.Clamp01(distanceTraveled / maxDistance);

        float curveFactor = outwardSpeedCurve.Evaluate(t);
        float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, curveFactor); //devuelve la velocidad correspondiente al valor de t actual

        rectTransform.anchoredPosition += travelDirection * (currentSpeed * Time.deltaTime);

        // Si ha recorrido lo suficiente, empieza a volver
        if (distanceTraveled >= maxDistance)
        {
            currentState = State.Returning;
            returnTimer = 0f;
        }
    }


    private void ReturnToPlayer()
    {
        if (targetPlayerRect == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 targetPos = targetPlayerRect.anchoredPosition;
        float currentDistanceToPlayer = Vector2.Distance(currentPos, targetPos);

        // Si entra en el radio de recogida, se destruye
        if (currentDistanceToPlayer <= catchRadius)
        {
            Destroy(gameObject);
            return;
        }

        returnTimer += Time.deltaTime; // El boomerang va mas rapido con el tiempo, para garantizar que acabe alcanzando al jugador


        float duration = Mathf.Max(0.0001f, returnTime); //dividir entre cero es malo

        // Esta vez t depende del tiempo
        float t = Mathf.Clamp01(returnTimer / duration);

        float curveFactor = returnSpeedCurve.Evaluate(t);
        float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, curveFactor); //devuelve la velocidad correspondiente al valor de t actual

        // Direccion hacia la posicion actual del jugador
        Vector2 directionToPlayer = (targetPos - currentPos).normalized;
        rectTransform.anchoredPosition += directionToPlayer * (currentSpeed * Time.deltaTime);
    }

}