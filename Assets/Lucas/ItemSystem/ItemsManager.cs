using UnityEngine;
using UnityEngine.UIElements;

public class ItemsManager : MonoBehaviour
{
    // lo hago serializefield para probarlo
    [SerializeField] private static GameObject[] allItems;

    public static GameObject TryGetOne(Vector2 pos, float minDistance)
    {
        GameObject nearest = null;
        foreach (var item in allItems)
        {
            if (nearest == null)
            {
                nearest = item;
            }
            else if (Vector2.Distance(nearest.transform.position, pos) > Vector2.Distance(item.transform.position, pos))
            {
                nearest = item;
            }
        }
        if (nearest != null)
        {
            if (Vector2.Distance(nearest.transform.position, pos) < minDistance)
            {

                // aqui lo sacaba de la lista

                return nearest;
            }
        }
        return null;
    }

}
