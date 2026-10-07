using UnityEngine;

// Categorías de habilidades
public enum SkillType
{
    Basic,
    Special,
    Ultimate
}
public abstract class Skill : MonoBehaviour
{
    #region Atributos Básicos
    [SerializeField] private string skillName;
    [SerializeField] private Sprite icon; // Icono para UI
    [SerializeField] private float baseCooldown = 3f; // Tiempo de recarga estándar
    [SerializeField] SkillType type; // Categoría (Básica, Especial, Definitiva)

    public string SkillName => skillName;
    public Sprite Icon => icon;
    public float BaseCooldown => baseCooldown;
    public SkillType Type => type;


    #endregion

    #region Cooldowns
    // Contador de segundos restantes para que el cooldown termine (0 = lista)
    private float currentCooldown;

    // Multiplicador de velocidad de recarga (para buffs y debuffs)
    // Actualmente siempre es 1, pero se deja aquí para cuando se implementen alteraciones de velocidad de recarga
    public float CooldownSpeedMultiplier { get; set; } = 1f;

    // Comprueba si la habilidad está lista para ser ejecutada
    public bool IsReady => currentCooldown <= 0f;

    // Segundos nominales brutos que faltan por descontar (el número no cambia aunque lo haga la velocidad de recarga)
    // En la gran mayoría de casos es mejor usar RemainingRealSeconds
    public float RawRemainingCooldown => Mathf.Max(0f, currentCooldown);

    // Segundos reales que tardará la habilidad en estar lista al ritmo actual (cambia en función de la velocidad de recarga)
    // Útil para UI
    public float RemainingRealSeconds
    {
        get
        {
            if (currentCooldown <= 0f) return 0f;

            // Evitamos división por cero si el multiplicador es 0
            if (CooldownSpeedMultiplier <= 0f) return Mathf.Infinity;

            return currentCooldown / CooldownSpeedMultiplier;
        }
    }

    // Progreso del cooldown normalizado de 0 a 1 (1 = recién usada, 0 = lista)
    // Diseñado para asignarlo directamente a Image.fillAmount en la UI
    // Si se multiplica por 100, conseguimos el porcentaje de progreso
    public float CooldownNormalized
    {
        get
        {
            if (BaseCooldown <= 0f || IsReady) return 0f;
            return Mathf.Clamp01(currentCooldown / BaseCooldown);
        }
    }


    // Hace avanzar el reloj del cooldown. Se le debe pasar el tiempo desde la última vez que se ejecutó
    public void Tick(float deltaTime)
    {
        if (currentCooldown > 0f)
        {
            // Resta el tiempo escalándolo según el multiplicador de velocidad
            currentCooldown -= deltaTime * CooldownSpeedMultiplier;

            // Evitamos que queden números negativos residuales
            if (currentCooldown < 0f)
            {
                currentCooldown = 0f;
            }
        }
    }


    // Aplica el cooldown y utiliza la habilidad
    public void Use()
    {
        currentCooldown = BaseCooldown;
        Execute();
    }

    #endregion

    // Este es el método que hay que definir en los scripts individuales de cada habilidad
    public abstract void Execute();

}
