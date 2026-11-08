using UnityEngine;
using static ItemsManager;

public class PlayerItemsManager : MonoBehaviour
{
    [SerializeField] private float minDistance;
    [SerializeField] private GameObject topOfMyHead;

    private PlayerContext playerContext;

    private GameObject playerItem = null;

    public void Start()
    {
        playerContext = GetComponent<PlayerContext>();
    }

    public bool hasItem()
    {
        return (playerItem != null);
    }
    public bool ItemInReach()
    {
        Vector2 posicionJugador;
        RectTransformUtility.ScreenPointToLocalPointInRectangle( // HAY QUE PONER ESTA MIERDA PORQUE SINO NO COMPARA BIEN LAS POSICIONES
            playerContext.canvas.gameObject.GetComponent<RectTransform>(),
            this.transform.position,
            null,
            out posicionJugador
        );
        return ItemNear(posicionJugador, minDistance);
    }

    public void BasicPressed()
    {
        if (!playerContext.ItemAllowed) return; // Si no se permite uso de objetos, se ignora
        if (hasItem())
        {
            UseItem();
        }
        else
        {
            TryGetOne();
        }
    }

    public void TryGetOne()
    {
        if (playerItem != null) return; // si tiene un objeto no coges mas

        Vector2 posicionJugador;
        RectTransformUtility.ScreenPointToLocalPointInRectangle( // HAY QUE PONER ESTA MIERDA PORQUE SINO NO COMPARA BIEN LAS POSICIONES
            playerContext.canvas.gameObject.GetComponent<RectTransform>(),
            this.transform.position,
            null,
            out posicionJugador
        );
        playerItem = ItemsManager.TryGetOne(posicionJugador, minDistance);

        //playerItem = ItemsManager.TryGetOne(this.transform.position, minDistance);

        if (playerItem == null) return;

        playerItem.GetComponent<Item>().SetOwner(this.gameObject); //le dice "soy tu dueño 

        // colocar como hijo del jugador para que se muevan juntos
        playerItem.transform.SetParent(topOfMyHead.transform);
        playerItem.transform.localPosition = Vector3.zero;

        // ESTA MIERDA HAY QUE PONERLA XQ POR ALGUNA RAZON SE EMPIEZAN A GIRAR SOLAS LAS COSAS AL JUGAR CON LOS PADRES
        this.gameObject.GetComponent<Movement>().UpdateSpritePos();
        this.transform.localRotation = Quaternion.identity;
        playerItem.transform.localRotation = Quaternion.identity;

        this.gameObject.GetComponent<Movement>().UpdateSpritePos();

    }
    public void UseItem()
    {
        Item item = playerItem.GetComponent<Item>();

        playerItem = null;
        item.Use();
    }
}
