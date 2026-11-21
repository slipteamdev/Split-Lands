using UnityEngine;

public class ZumoFeliz : Item
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float invertedTime = 8f;

    private Vector2 direction;
    private Rigidbody rb;

    [SerializeField] private Collider solidCollider;
    [SerializeField] private Collider triggerCollider;

    private bool moving = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Execute()
    {
        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra

        this.transform.SetParent(canva.transform);
        ItemsManager.RemoveObject(this.gameObject);

        direction = playerContext.Orientation;
        transform.position = playerContext.transform.position + new Vector3(direction.x * 2, direction.y * 2, 0);

        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.constraints =
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = false;

        solidCollider.enabled = true;
        triggerCollider.enabled = true;
        IgnoreSpecificCollisions();


        rb.linearVelocity = direction * speed;
        // Mensaje identificador
        Debug.Log($" ha usado el objeto " + $"{ItemName} .");

        moving = true;

    }


    private void IgnoreSpecificCollisions() // Ignora colisiones con ciertos tipos de objeto para no tocarlos o moverlos
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            Collider[] playerCols = p.GetComponentsInChildren<Collider>();
            foreach (Collider col in playerCols)
            {
                Physics.IgnoreCollision(solidCollider, col, true);
            }
        }

        GameObject[] rays = GameObject.FindGameObjectsWithTag("Ray");
        foreach (GameObject rayObj in rays)
        {
            Collider[] rayColliders = rayObj.GetComponentsInChildren<Collider>();
            foreach (Collider col in rayColliders)
            {
                Physics.IgnoreCollision(solidCollider, col, true);
            }
        }
    }

    private void FixedUpdate()
    {
        // Asegura que tras cada rebote mantenga la velocidad constante sin perder energia por rebotes o amortiguamiento
        if (moving && rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!moving) return;
        //if (collision.gameObject.CompareTag("MapEdge"))
        //{
        Destroy(gameObject);
        //}
    }

    private void OnTriggerEnter(Collider other) // Esto solo se activa con el segundo collider de la concha, y se usa para interactuar con jugadores sin moverlos por fisicas
    {
        if (!moving) return;
        if (other.gameObject == PlayerOwner) return; // Si se descomenta esta linea, solo reaccionara al chocar contra el otro jugador

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Impacto contra: {other.gameObject.name}");
            other.GetComponent<PlayerContext>().ApplyInvertedControl(invertedTime);
            Destroy(gameObject);
        }
    }
}
