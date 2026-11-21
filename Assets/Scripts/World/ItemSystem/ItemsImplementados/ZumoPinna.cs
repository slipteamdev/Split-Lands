using UnityEngine;

public class ZumoPinna : Item
{
    [SerializeField] private float duration;
    [SerializeField] private float multiplier;
    public override void Execute()
    {
        PlayerOwner.GetComponent<PlayerContext>().ApplyStunDurationModifier(multiplier, duration);
        ItemsManager.RemoveObject(this.gameObject);
        Destroy(gameObject);
    }
}
