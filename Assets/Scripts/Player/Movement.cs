using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    [SerializeField] private GameObject CharacterSprite;
    [SerializeField] private GameObject invertedConfusion;
    [SerializeField] private GameObject stunConfusion;

    private Vector2 moveInput;
    private Vector3 velocity;
    private PlayerContext playerContext;
    private Rigidbody rb;

    Vector3 desfase;
    private void Awake()
    {
        playerContext = GetComponent<PlayerContext>();
    }
    private void Start()
    {
        rb = playerContext.Rb;
        desfase = CharacterSprite.transform.position - CharacterSprite.transform.parent.position;
    }

    void FixedUpdate()
    {
        if (stunConfusion != null) // para que se vea el objeto de la confusion
        {
            stunConfusion.SetActive(!playerContext.MoveInputAllowed);
        }
        if (invertedConfusion != null) // para que se vea el objeto de la confusion
        {
            invertedConfusion.SetActive(playerContext.InvertedControl);
        }


        if (!playerContext.MoveAllowed) // Si se prohibe movimiento, velocidad cero y acaba
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }


        if (!playerContext.MoveInputAllowed) return; // Si no se permite input, acaba el bucle

        if (moveInput.magnitude > 1)
        {
            moveInput = moveInput.normalized;
        }
        velocity = new Vector3(moveInput.x, moveInput.y, 0.0f) * speed * playerContext.MovementSpeedMultiplier;

        if (rb == null) //por si no tiene rigidbody que se mueva con el transform, ESTO IGUAL DA PROBLEMAS PERO DE MOMENTO LO DEJO
        {
            transform.position = transform.position + velocity * Time.deltaTime;
        }
        else
        {
            //Debug.Log(velocity.sqrMagnitude);
            rb.linearVelocity = velocity;
        }
    }

    public void AssignInput(Vector2 input)
    {
        moveInput = input;

        if (playerContext.InvertedControl)
        {
            moveInput.x = -input.x;
            moveInput.y = -input.y;
        }

        if (moveInput != Vector2.zero) playerContext.SetOrientation(moveInput.normalized);

        UpdateSpritePos();
    }

    public void UpdateSpritePos()
    {
        if (CharacterSprite != null)
        {
            CharacterSprite.transform.rotation = Quaternion.identity;
            CharacterSprite.transform.position = CharacterSprite.transform.parent.position + desfase;

        }
    }
}
