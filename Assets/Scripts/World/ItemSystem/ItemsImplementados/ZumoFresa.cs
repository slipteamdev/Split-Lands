using UnityEngine;

public class ZumoFresa : Item
{
    [SerializeField] private float duration;
    [SerializeField] private float multiplier;
    public override void Execute()
    {
        PlayerOwner.GetComponent<PlayerContext>().ApplyMovementSpeedModifier(multiplier, duration);
        Destroy(gameObject);
    }
}
