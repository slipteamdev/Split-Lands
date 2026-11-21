using UnityEngine;

public class ZumoKiwi : Item
{

    [SerializeField] private float duration;
    [SerializeField] private float multiplier;
    public override void Execute()
    {
        PlayerOwner.GetComponent<PlayerContext>().ApplyCooldownSpeedModifier(multiplier, duration);
        ItemsManager.RemoveObject(this.gameObject);
        Destroy(gameObject);
    }
}

