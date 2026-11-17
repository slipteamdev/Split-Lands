using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.Progress;

public class Monkey : MonoBehaviour
{
    public PlayerContext playerContext; // el jugador para el que juegas

    private GameObject objetivo = null;
    private AnimalMovement am;

    void Start()
    {
        am = this.gameObject.GetComponent<AnimalMovement>();
    }

    void Update()
    {
        if (objetivo == null) // si no tiene objetivo
        {
            am.AssignObjetive(this.transform.position);
        }
        else
        {
            if (Vector2.Distance(objetivo.transform.localPosition, this.transform.localPosition) < 1.25f) // si ha llegado al objetivo
            {
                if (transform.childCount > 1) // si no tiene objeto, lo roba
                {
                    // robar
                    if (objetivo.GetComponent<Item>()  != null) //si el objeto esta solito
                    {
                        objetivo.transform.SetParent(this.gameObject.transform.GetChild(0).transform);
                        objetivo.transform.localPosition = Vector3.zero;
                        this.transform.localRotation = Quaternion.identity;
                        objetivo.transform.rotation = Quaternion.identity;

                        objetivo.GetComponent<Item>().SetOwner(null);
                        ItemsManager.RemoveObject(objetivo);
                    }
                    else // si tiene papa
                    {
                        PlayerItemsManager p = objetivo.GetComponent<PlayerItemsManager>();
                        if (p != null) // es jugador
                        {
                            p.MeRoban(this.gameObject);
                        }
                        else // es cangrejo
                        {
                            Item[] g = objetivo.GetComponentsInChildren<Item>(); // mira, no estoy pa pensar, asi se va con un array mu guarro pero me la pela
                            foreach (Item item in g)
                            {
                                if (item != null)
                                {
                                    item.gameObject.transform.SetParent(this.gameObject.transform.GetChild(0).transform);
                                    item.gameObject.transform.localPosition = Vector3.zero;
                                    this.transform.localRotation = Quaternion.identity;
                                    item.gameObject.transform.rotation = Quaternion.identity;

                                    item.gameObject.GetComponent<Item>().SetOwner(null);
                                    ItemsManager.RemoveObject(item.gameObject);
                                }

                            }
                        }
                    }


                        objetivo = playerContext.gameObject;
                }
                else // si tiene objeto, lo suelta
                {
                    // soltar
                    Soltar();

                    FindObjetive();
                }
            }
            else // si no ha llegado
            {
                am.AssignObjetive(objetivo.transform.position);
            }
        }
    }

    public void Soltar()
    {
        GameObject obj = null;
        Item[] g = objetivo.GetComponentsInChildren<Item>(); // mira, no estoy pa pensar, asi se va con un array mu guarro pero me la pela
        foreach (Item item in g)
        {
            if (item != null)
            {
                obj = item.gameObject;
            }
        }

        if (obj == null) return;

        obj.transform.SetParent(playerContext.canvas.gameObject.transform);
        this.transform.localRotation = Quaternion.identity;
        obj.transform.rotation = Quaternion.identity;

        ItemsManager.AddObject(obj); //lo devuelve al mundo

    }

    public void FindObjetive()
    {
        List<GameObject> enemyItems = new List<GameObject>();
        foreach (GameObject item in ItemsManager.allItems)
        {
            Map.Team current_pixel_team;

            if (item.transform.parent.gameObject.GetComponent<AnimalMovement>() == null && item.transform.parent.gameObject.GetComponent<PlayerItemsManager>() == null)
            {
                current_pixel_team = Map.ReadPixelAt(item.transform.localPosition.x, item.transform.localPosition.y);
            }
            else // si tiene papa
            {
                if (item.transform.parent.gameObject.GetComponent<Monkey>() != null) // si es otro mono nos da igual, SIMIO RESPETA SIMIO
                {
                    continue;
                }
                current_pixel_team = Map.ReadPixelAt(item.transform.parent.transform.localPosition.x, item.transform.parent.transform.localPosition.y);
            }

            if (current_pixel_team != playerContext.Team && current_pixel_team != Map.Team.None) // si esta en terreno enemigo
            {
                enemyItems.Add(item);
            }
        }

        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (GameObject item in enemyItems)
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

            float distance = Vector2.Distance(posicionObjeto, this.transform.localPosition);
            if (distance < minDist)
            {
                minDist = distance;
                nearest = item;
            }
        }

        // nearest es el que mas cerca esta
        if (nearest != null)
        {
            objetivo = nearest;
        }

    }


}
