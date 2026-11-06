using System.Collections;
using UnityEngine;

public class BerserkSkill : Skill
{
    [SerializeField] private float duration = 10f;

    private DashNSplashSkill dash;
    private SlashSkill slash;
    void Start()
    {
        dash = GetComponent<DashNSplashSkill>();
        slash = GetComponent<SlashSkill>();
    }

    // Update is called once per frame
    public override void Execute()
    {
        StartCoroutine(BerserkRoutine());
    }

    private IEnumerator BerserkRoutine()
    {
        // 1. Activar modo Berserk
        if (dash != null) dash.BerserkMode();
        if (slash != null) slash.BerserkMode();

        yield return new WaitForSeconds(duration);

        if (dash != null) dash.NormalMode();
        if (slash != null) slash.NormalMode();

    }
}
