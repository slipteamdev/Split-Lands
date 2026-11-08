using System;
using System.Collections.Generic;
using UnityEngine;

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
    public static void RemoveObject(GameObject obj)
    {
        allItems.Remove(obj);
    }

    public static bool ItemNear(Vector2 pos, float minDistance)
    {
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (GameObject item in allItems)
        {
            if (item == null) continue;

            Vector2 posicionObjeto;
            RectTransform rect = item.gameObject.transform.GetComponentInParent<Canvas>().gameObject.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle( // HAY QUE PONER ESTA MIERDA PORQUE SINO NO COMPARA BIEN LAS POSICIONES
                rect,
                item.transform.position,
                null,
                out posicionObjeto
            );
            float distance = Vector2.Distance(posicionObjeto, pos);

            if (distance < minDist)
            {
                minDist = distance;
                nearest = item;
            }
        }

        if (nearest != null && minDist < minDistance)
        {
            return true;
        }

        return false;
    }

    public static GameObject TryGetOne(Vector2 pos, float minDistance)
    {
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (GameObject item in allItems)
        {
            if (item == null) continue;

            Vector2 posicionObjeto;
            RectTransform rect = item.gameObject.transform.GetComponentInParent<Canvas>().gameObject.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle( // HAY QUE PONER ESTA MIERDA PORQUE SINO NO COMPARA BIEN LAS POSICIONES
                rect,
                item.transform.position,
                null,
                out posicionObjeto
            );
            float distance = Vector2.Distance(posicionObjeto, pos);

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


