using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Movement movement;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private PlayerItemsManager itemsManager;



    Vector2 moveInput;

    public void OnMovement(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.sqrMagnitude <= 0.01) { moveInput = Vector2.zero; } // Si el movimiento de joystick es muy bajo, ignorarlo (ayuda con el drifting)
        movement.AssignInput(moveInput);


    }

    public void OnBasicButton(InputAction.CallbackContext context)
    {
        if (context.performed && context.ReadValueAsButton())
        {
            if (itemsManager.ItemInReach() || itemsManager.hasItem())
            {
                // usar item
                itemsManager.BasicPressed();
            }
            else
            {
                skillManager.BasicPressed(); // usa la habilidad
            }
            //if (itemManager.Interactable==true){
            //   itemManager.BasicPressed();
            //} else
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
}
