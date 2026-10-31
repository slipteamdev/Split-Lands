using UnityEngine;

public abstract class Item : MonoBehaviour
{

    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon; // Icono para UI
    [SerializeField] private float baseCooldown = 3f; // Tiempo de recarga estándar

    private GameObject playerOwner = null;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public float BaseCooldown => baseCooldown;
    public GameObject PlayerOwner => playerOwner;


    public void Use()
    {
        //currentCooldown = BaseCooldown;
        Execute();
    }

    public void SetOwner(GameObject owner)
    {
        playerOwner = owner;
    }


    // Este es el método que hay que definir en los scripts individuales de cada objeto
    public abstract void Execute();
}
