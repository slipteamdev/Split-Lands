using System.Collections.Generic;
using UnityEngine;

public class Monkey : MonoBehaviour
{
    public PlayerContext playerContext; // el jugador para el que juegas

    private GameObject objetivo = null;
    private AnimalMovement am;
    public TotemMono totem;

    void Start()
    {
        am = this.gameObject.GetComponent<AnimalMovement>();
    }

    void Update()
    {
        //Debug.Log(transform.childCount);
        if (transform.childCount <= 1) // si no tiene objeto
        {
            if (objetivo == null || objetivo.IsDestroying() || !ItemsManager.Contains(objetivo)) // si no tiene objetivo o si el objetivo se esta muriendo
            {
                FindObjetive();
            }
            else
            {
                if (Vector2.Distance(objetivo.transform.position, this.transform.position) < 1.5f) // si ha llegado al objetivo
                {
                        // robar
                        if (objetivo.GetComponent<Item>() != null) //si el objeto esta solito
                        {
                            objetivo.transform.SetParent(this.gameObject.transform);
                            objetivo.transform.localPosition = this.gameObject.transform.GetChild(0).transform.localPosition;
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
                                GameObject g = objetivo.transform.GetChild(1).gameObject;

                                if (g != null)
                                {
                                    g.transform.SetParent(this.gameObject.transform);
                                    g.transform.localPosition = this.gameObject.transform.GetChild(0).transform.localPosition;
                                    this.transform.localRotation = Quaternion.identity;
                                    g.transform.rotation = Quaternion.identity;

                                    g.GetComponent<Item>().SetOwner(null);
                                    ItemsManager.RemoveObject(g);
                                }

                            }
                        }

                    totem.FindNewObjetive(this.gameObject);
                    objetivo = playerContext.gameObject;
                }
                else // si no ha llegado
                {
                    am.AssignObjetive(objetivo.transform.position);
                }
            }
        }
        else // si si tiene objeto
        {
            if (Vector2.Distance(objetivo.transform.position, this.transform.position) < 1.25f) // si tiene objeto, lo suelta
            {
                // soltar
                Soltar();

                FindObjetive();
            }
            else // si no ha llegado
            {
                am.AssignObjetive(objetivo.transform.position);
            }
        }

    }

    public void Soltar()
    {
        if (transform.childCount <= 1) // si no tiene objeto
        {
            return;
        }
        GameObject obj = this.gameObject.transform.GetChild(1).gameObject;


        obj.transform.SetParent(playerContext.canvas.gameObject.transform);
        this.transform.localRotation = Quaternion.identity;
        obj.transform.rotation = Quaternion.identity;

        ItemsManager.AddObject(obj); //lo devuelve al mundo

    }

    public void FindObjetive()
    {
        if (transform.childCount > 1) // si tiene objeto
        {
            return;
        }
        List<GameObject> enemyItems = new List<GameObject>();
        foreach (GameObject item in ItemsManager.allItems)
        {
            Map.Team current_pixel_team;

            if (!item.transform.parent.gameObject.TryGetComponent<AnimalMovement>(out AnimalMovement a) && !item.transform.parent.gameObject.TryGetComponent<PlayerItemsManager>(out PlayerItemsManager m)) // esta linea peta
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
        else
        {
            am = this.gameObject.GetComponent<AnimalMovement>();
            if (am != null)
            {

                am.AssignObjetive(this.transform.position);
            }
            objetivo = null;
        }

    }


}
