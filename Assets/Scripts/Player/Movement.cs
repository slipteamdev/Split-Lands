using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;

    [SerializeField] private GameObject CharacterSprite;
    [SerializeField] private Animator animator;

    private Vector2 moveInput;
    private Vector3 velocity;
    private PlayerContext playerContext;
    private Rigidbody rb;

    Vector3 desfase;

    private void Start()
    {
        Screen.fullScreen = true;

        playerContext = GetComponent<PlayerContext>();
        rb = playerContext.Rb;
        desfase = CharacterSprite.transform.position - CharacterSprite.transform.parent.position;
    }

    void FixedUpdate()
    {
        if (!playerContext.MoveInputAllowed) return; // Si no se permite input, acaba el bucle

        if (!playerContext.MoveAllowed) // Si se prohibe movimiento, velocidad cero y acaba
        {
            rb.linearVelocity = Vector3.zero;
            if (animator != null) animator.SetBool("AndarDerecha", false);
            return;
        }


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
            //Debug.Log(velocity.sqrMagnitude);
            rb.linearVelocity = velocity;
        }
        if (animator != null)
        {
            if (rb.linearVelocity == Vector3.zero)
            {
                if (animator.GetBool("AndarDerecha") == true) animator.SetBool("AndarDerecha", false);
            }
            else
            {
                if (animator.GetBool("AndarDerecha") == false) animator.SetBool("AndarDerecha", true);
            }
        }
    }

    public void AssignInput(Vector2 input)
    {
        moveInput = input;
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
