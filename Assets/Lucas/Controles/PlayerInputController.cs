using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Movement movement;
    [SerializeField] private SkillManager skillManager;


    [SerializeField] public Map.Team team;
    public Vector2 orientation;

    Vector2 moveInput;
    void Start()
    {
        //movement = GetComponent<Movement>();
        //skillManager = GetComponent<SkillManager>();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.magnitude <= 0.01) { moveInput = Vector2.zero; } // Si el movimiento de joystick es muy bajo, ignorarlo (ayuda con el drifting)
        movement.AssignInput(moveInput);

        if (moveInput != Vector2.zero) { orientation = moveInput; }
    }

    public void OnBasicButton(InputAction.CallbackContext context)
    {
        if (context.performed && context.ReadValueAsButton())
        {
            //if (itemManager.Interactable==true){
            //   itemManager.BasicPressed();
            //} else
            skillManager.BasicPressed();
        }


    }

    public void OnSpecialButton(InputAction.CallbackContext context)
    {
        if (context.performed && context.ReadValueAsButton())
        {
            skillManager.SpecialPressed();
        }

    }

    public void OnUltimateButton(InputAction.CallbackContext context)
    {
        if (context.performed && context.ReadValueAsButton())
        {
            skillManager.UltimatePressed();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
