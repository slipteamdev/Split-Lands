using UnityEngine;
using UnityEngine.InputSystem;

public class BombTest : MonoBehaviour
{
    [SerializeField] private InputActionReference click;
    [SerializeField] private InputActionReference click2;
    [SerializeField] private InputActionReference cursor;
    [Space]
    [SerializeField] private RectTransform hit;
    [SerializeField] private Sprite setteam1;
    [SerializeField] private Sprite setteam2;

    private void Update()
    {
        hit.localPosition = cursor.action.ReadValue<Vector2>() - new Vector2(Screen.width / 2, Screen.height / 2);

        if (click.action.WasPerformedThisFrame())
        {
            Map.Draw(hit.localPosition, setteam1, Map.Team.Team1);
        }
        if (click2.action.WasPerformedThisFrame())
        {
            Map.Draw(hit.localPosition, setteam2, Map.Team.Team2);
        }
    }
}
