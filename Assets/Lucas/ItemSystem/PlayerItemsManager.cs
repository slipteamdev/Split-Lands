using System;
using UnityEngine;
using UnityEngine.UIElements;
using static ItemsManager;

public class PlayerItemsManager : MonoBehaviour
{
    [SerializeField] private float minDistance;
    [SerializeField] private GameObject topOfMyHead;

    private GameObject playerItem = null;

    public bool hasItem()
    {
        return (playerItem != null);
    }
    public void TryGetOne()
    {
        if (playerItem != null) return; // si tiene un objeto no coges mas


        playerItem = ItemsManager.TryGetOne(this.GetComponent<RectTransform>().anchoredPosition, minDistance);
        //playerItem = ItemsManager.TryGetOne(this.transform.position, minDistance);

        if (playerItem == null) return;

        playerItem.GetComponent<Item>().SetOwner(this.gameObject); //le dice "soy tu dueño 

        // colocar como hijo del jugador para que se muevan juntos
        playerItem.transform.SetParent(topOfMyHead.transform);
        playerItem.transform.localPosition = Vector3.zero;

        // ESTA MIERDA HAY QUE PONERLA XQ POR ALGUNA RAZON SE EMPIEZAN A GIRAR SOLAS LAS COSAS AL JUGAR CON LOS PADRES
        this.transform.localRotation = Quaternion.identity;
        playerItem.transform.localRotation = Quaternion.identity;

    }
    public void UseItem()
    {
        Item item = playerItem.GetComponent<Item>();

        playerItem = null;
        item.Use();
    }
}
