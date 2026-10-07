using UnityEngine;

//Habilidad Dummy
public class BasicSkill : Skill
{
    public override void Execute()
    {
        // Mensaje identificador
        Debug.Log($"{gameObject.name} ha usado la habilidad " +
                  $"{SkillName} (Tipo: {Type}).");
    }
}
