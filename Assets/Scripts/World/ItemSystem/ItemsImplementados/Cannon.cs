using UnityEngine;

public class Cannon : Item
{
    [SerializeField] private Sprite destruction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Execute()
    {

        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos
        Vector2 direction = playerContext.orientation;
        Map.Draw(this.PlayerOwner.transform.localPosition, destruction, playerContext.Team, direction);
        Destroy(this.gameObject);
    }
}
