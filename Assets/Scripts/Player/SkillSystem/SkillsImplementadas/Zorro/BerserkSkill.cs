using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BerserkSkill : Skill
{
    [SerializeField] private float duration = 10f;
    [SerializeField] private Image fuego;
    [SerializeField] private GameObject sprite;

    private DashNSplashSkill dash;
    private SlashSkill slash;
    void Start()
    {
        dash = GetComponent<DashNSplashSkill>();
        slash = GetComponent<SlashSkill>();

        fuego.enabled = false;
    }

    // Update is called once per frame
    public override void Execute()
    {
        StartCoroutine(BerserkRoutine());
    }

    private void Update()
    {
        if (fuego.enabled)
        {
            fuego.gameObject.transform.rotation = Quaternion.identity;
            fuego.gameObject.transform.localPosition = sprite.transform.localPosition;
        }
    }
    private IEnumerator BerserkRoutine()
    {
        // Activar modo Berserk
        if (dash != null) dash.BerserkMode();
        if (slash != null) slash.BerserkMode();
        playerContext.ApplyVisualCooldownSpeedModifier(1, duration);

        fuego.enabled = true;

        yield return new WaitForSeconds(duration);

        if (dash != null) dash.NormalMode();
        if (slash != null) slash.NormalMode();

        fuego.enabled = false;

    }
}
