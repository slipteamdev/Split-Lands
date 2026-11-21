using UnityEngine;

public class TormentaNieve : Item
{
    [SerializeField] private float duration;
    [SerializeField] private float movementSpeedMultiplier;
    [SerializeField] private float cooldownSpeedMultiplier;
    public override void Execute()
    {
        PlayerContext other = PlayerOwner.GetComponent<PlayerContext>().EnemyPlayer.GetComponent<PlayerContext>();
        other.ApplyMovementSpeedModifier(movementSpeedMultiplier, duration);
        other.ApplyCooldownSpeedModifier(cooldownSpeedMultiplier, duration);

        ItemsManager.RemoveObject(this.gameObject);
        Destroy(gameObject);
    }
}
