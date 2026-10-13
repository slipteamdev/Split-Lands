using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemsManager : MonoBehaviour
{
    private static List<GameObject> allItems = new List<GameObject>();


    // esto es para probarlo
    [SerializeField] private GameObject[] array;
    public void Start()
    {
        foreach (GameObject item in array)
        {
            AddObject(item);
        }
    }

    public static void AddObject(GameObject obj)
    {
        allItems.Add(obj);
    }

    public static GameObject TryGetOne(Vector2 pos, float minDistance)
    {
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (GameObject item in allItems)
        {
            if (item == null) continue;

            RectTransform rect = item.GetComponent<RectTransform>();
            if (rect == null) continue;
            float distance = Vector2.Distance(rect.anchoredPosition, pos);

            if (distance < minDist)
            {
                minDist = distance;
                nearest = item;
            }
        }

        if (nearest != null && minDist < minDistance)
        {

            allItems.Remove(nearest);

            return nearest;
        }

        return null;
    }
}


