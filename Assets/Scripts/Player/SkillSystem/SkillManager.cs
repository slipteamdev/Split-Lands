using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //Referencias a las habilidades para asignar desde el editor
    [SerializeField] private Skill basicSkill;
    [SerializeField] private Skill specialSkill;
    [SerializeField] private Skill ultimateSkill;

    public Skill BasicSlot { get; private set; }
    public Skill SpecialSlot { get; private set; }
    public Skill UltimateSlot { get; private set; }

    // Array con las habilidades, se usa para bucles
    private Skill[] allSlots;


    void Awake()
    {
        BasicSlot = basicSkill;
        SpecialSlot = specialSkill;
        UltimateSlot = ultimateSkill;

        allSlots = new Skill[] { BasicSlot, SpecialSlot, UltimateSlot };
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        UpdateSlots(dt);
    }

    // Recorre el array de habilidades para avanzar sus contadores internos.
    public void UpdateSlots(float dt)
    {
        for (int i = 0; i < allSlots.Length; i++)
        {
            if (allSlots[i] != null)
            {
                allSlots[i].Tick(dt);
            }
        }
    }

    //Estos métodos se llaman desde PlayerInputController cuando se pulsa el botón correspondiente
    public void BasicPressed() => TryUse(0);
    public void SpecialPressed() => TryUse(1);
    public void UltimatePressed() => TryUse(2);


    // Pregunta a las habilidades si su cooldown ha acabado, y si lo ha hecho, las activa
    private void TryUse(int id)
    {
        if (allSlots[id] == null)
        {
            return;
        }
        if (allSlots[id].IsReady)
        {
            allSlots[id].Use();
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] No puede usar [{allSlots[id].SkillName}]. " +
            $"Cooldown restante: {allSlots[id].RemainingRealSeconds:F1}s | Porcentaje de carga: {allSlots[id].CooldownNormalized * 100:F0}%");
        }
    }




}
