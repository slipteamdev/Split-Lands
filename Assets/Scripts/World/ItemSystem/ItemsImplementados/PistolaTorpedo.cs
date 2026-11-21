using UnityEngine;

public class PistolaTorpedo : Item
{
    [SerializeField] private int BalasMaximas = 5;
    private int usos = 0;

    [SerializeField] private GameObject torpedo;

    [SerializeField] private float speed;
    [SerializeField] private Sprite destruction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        destructionAllowed = false;
    }
    public override void Execute()
    {
        if (PlayerOwner == null) return;
        if (usos + 1 >= BalasMaximas) destructionAllowed = true;
        playerContext = PlayerOwner.GetComponent<PlayerContext>();
        GameObject torpedoInit = Instantiate(torpedo, transform.position, Quaternion.identity, playerContext.canvas.transform);
        torpedoInit.GetComponent<Torpedo>().Init(playerContext.orientation, speed, destruction, playerContext.Team);
        usos++;
        if (usos >= BalasMaximas)
        {

            ItemsManager.RemoveObject(this.gameObject);
            Destroy(gameObject);
        }

    }
}
