using UnityEngine;

public class Pala : Item
{
    [SerializeField] private ItemTier SpawnTier = ItemTier.Rare;
    // Update is called once per frame
    public override void Execute()
    {
        if (PlayerOwner == null) return;
        playerContext = PlayerOwner.GetComponent<PlayerContext>();
        Vector3 spawnPosition = new Vector3(PlayerOwner.transform.position.x + playerContext.Orientation.x, PlayerOwner.transform.position.y + playerContext.Orientation.x, PlayerOwner.transform.position.z);
        ItemsManager.SpawnRandomItem(spawnPosition, Quaternion.identity, playerContext.canvas.transform, allowedTiers: new[] { SpawnTier });

        Destroy(gameObject);
    }
}
