using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    private Vector2 moveInput;
    private Vector3 velocity;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        velocity = new Vector3(moveInput.x, moveInput.y, 0.0f).normalized * speed * Time.deltaTime;
        transform.position = transform.position + velocity;
    }

    public void AssignInput(Vector2 input)
    {
        moveInput = input;
    }
}
