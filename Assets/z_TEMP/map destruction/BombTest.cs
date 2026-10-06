using UnityEngine;
using UnityEngine.InputSystem;

public class BombTest : MonoBehaviour
{
    [SerializeField] private InputActionReference click;
    [SerializeField] private InputActionReference click2;
    [SerializeField] private InputActionReference cursor;
    [Space]
    [SerializeField] private RectTransform hit;
    [SerializeField] private Sprite set;

    private void Update()
    {
        hit.localPosition = cursor.action.ReadValue<Vector2>() - new Vector2(Screen.width / 2, Screen.height / 2);

        if (click.action.WasPerformedThisFrame())
        {
            Map.Draw(hit.localPosition, set, Map.Team.Team1);
        }
        if (click2.action.WasPerformedThisFrame())
        {
            Map.Draw(hit.localPosition, set, Map.Team.Team2);
        }
    }
}
