using UnityEngine;

public class Caracol : Item
{

    [SerializeField] private Sprite destruction;
    private bool moving = false;
    private AnimalMovement am;


    public override void Execute()
    {
        playerContext = this.PlayerOwner.GetComponent<PlayerContext>(); // Nota de Lucas: Esta asignacion intentaria mirar si se puede hacer en el script base, para no tener que ponerla en todos los objetos

        GameObject canva = GetComponentInParent<Canvas>().gameObject; //busca en el padre y en los padres del padre y asi hasta que lo encuentra

        this.transform.SetParent(canva.transform);

        Vector2 direction = playerContext.Orientation;
        transform.position = playerContext.transform.position + new Vector3(direction.x, direction.y, 0);

        am = this.gameObject.GetComponent<AnimalMovement>();
        if (am != null)
        {
            am.enabled = true;
            am.AssignObjetive(playerContext.EnemyPlayer.transform.localPosition);
        }

        Map.Draw(this.transform.localPosition, destruction, playerContext.Team, Vector2.right);
        moving = true;
    }

    Vector2 obj;
    private void Update()
    {
        if (!moving) return;

        Map.Team current_pixel_team = Map.ReadPixelAt(this.transform.localPosition.x, this.transform.localPosition.y);
        if (current_pixel_team != playerContext.Team && current_pixel_team != Map.Team.None)
        {
            am.Die();
        }

        obj = Map.FindNearestEnemyPixel(this.transform.localPosition.x, this.transform.localPosition.y, playerContext.Team);
        Debug.Log(obj);

        if (obj == Vector2.zero)
        {
            am.AssignObjetive(playerContext.EnemyPlayer.transform.localPosition);
        }
        else
        {
            // Vector3 ob = new Vector3(obj.x, obj.y, 0);
            // am.AssignObjetive(transform.localPosition + ob);
            am.AssignObjetive(obj);

        }
        Map.Draw(this.transform.localPosition, destruction, playerContext.Team, Vector2.right);

    }

}
