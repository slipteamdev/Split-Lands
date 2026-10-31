using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private Vector2 moveInput;
    private Vector3 velocity;
    private Rigidbody rb;

    private void Start()
    {
        Screen.fullScreen = true;
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (moveInput.magnitude > 1)
        {
            moveInput = moveInput.normalized;
        }
        velocity = new Vector3(moveInput.x, moveInput.y, 0.0f) * speed;

        if (rb == null) //por si no tiene rigidbody que se mueva con el transform, ESTO IGUAL DA PROBLEMAS PERO DE MOMENTO LO DEJO
        {
            transform.position = transform.position + velocity * Time.deltaTime;
        }
        else
        {
            Debug.Log(velocity.sqrMagnitude);
            rb.linearVelocity = velocity;
        }
    }

    public void AssignInput(Vector2 input)
    {
        moveInput = input;
    }
}
