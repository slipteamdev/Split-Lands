using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{

    //Este script asigna los controles (wasd, mando, flechas...) a cada jugador
    [SerializeField] private PlayerInput player1;
    [SerializeField] private PlayerInput player2;

    void Start()
    {
        int gamepadsConnected = Gamepad.all.Count;

        // Caso 1: Hay 2 o más mandos -> Ambos juegan con mando
        if (gamepadsConnected >= 2)
        {
            player1.SwitchCurrentControlScheme("GamePad", Gamepad.all[0]);
            player2.SwitchCurrentControlScheme("GamePad", Gamepad.all[1]);
        }
        // Caso 2: Hay solo 1 mando -> J1 usa Mando, J2 usa WASD en el teclado
        else if (gamepadsConnected == 1)
        {
            player1.SwitchCurrentControlScheme("GamePad", Gamepad.all[0]);
            player2.SwitchCurrentControlScheme("WASD", Keyboard.current);
        }
        // Caso 3: Cero mandos -> J1 usa WASD, J2 usa Flechas
        else
        {
            player1.SwitchCurrentControlScheme("WASD", Keyboard.current);
            player2.SwitchCurrentControlScheme("Arrows", Keyboard.current);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
