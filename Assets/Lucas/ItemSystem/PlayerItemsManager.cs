using UnityEngine;

public class PlayerItemsManager : MonoBehaviour
{
    [SerializeField] private float minDistance;

    private GameObject playerItem = null;


    public void TryGetOne()
    {
        playerItem = ItemsManager.TryGetOne(this.transform.position, minDistance);
    }
    public void UseItem()
    {
        Item item = playerItem.GetComponent<Item>();

        playerItem = null;
        item.Use();
    }
}
