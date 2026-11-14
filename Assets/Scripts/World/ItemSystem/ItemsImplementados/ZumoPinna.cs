using UnityEngine;

public class ZumoPinna : Item
{
    [SerializeField] private float duration;
    public override void Execute()
    {
        PlayerOwner.GetComponent<PlayerContext>().ApplyStunBuff(duration);
        Destroy(gameObject);
    }
}
