using UnityEngine;

public enum ItemType
{
    Draw,    // Objetos que se usan principalmente para pintar (bombas)
    Damage,  // Objetos cuyo objetivo es principalmente aturdir
    Buff,    // Objetos que mejoran al usuario
    Debuff,       // Objetos que perjudican al rival
    Other        // Objetos con efectos variados y mas caoticos
}

public enum ItemTier
{
    Common,
    Uncommon,
    Rare
}

public abstract class Item : MonoBehaviour
{

    [SerializeField] private string itemName;

    public ItemType type;
    public ItemTier tier;

    private GameObject playerOwner = null;

    public string ItemName => itemName;
    public GameObject PlayerOwner => playerOwner;
    public PlayerContext playerContext;


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
