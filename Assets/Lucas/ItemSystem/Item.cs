using UnityEngine;

public abstract class Item : MonoBehaviour
{

    [SerializeField] private string skillName;
    [SerializeField] private Sprite icon; // Icono para UI
    [SerializeField] private float baseCooldown = 3f; // Tiempo de recarga estándar

    public string SkillName => skillName;
    public Sprite Icon => icon;
    public float BaseCooldown => baseCooldown;


    // Aplica el cooldown y utiliza la habilidad
    public void Use()
    {
        //currentCooldown = BaseCooldown;
        Execute();
    }


    // Este es el método que hay que definir en los scripts individuales de cada habilidad
    public abstract void Execute();
}
