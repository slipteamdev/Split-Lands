using UnityEngine;
public class WindGeneratorSkill : Skill
{
    [SerializeField] private ItemTier[] itemTiersToGenerate;
    [SerializeField] private float spawnRadius = 5f;

    public override void Execute()
    {
        if (itemTiersToGenerate == null || itemTiersToGenerate.Length == 0) return;
        float angleStep = 360f / Mathf.Max(1, itemTiersToGenerate.Length);
        ItemTier[] tierFilter = new ItemTier[1];
        float charAngle = Mathf.Atan2(playerContext.orientation.y, playerContext.orientation.x);
        for (int i = 0; i < itemTiersToGenerate.Length; i++)
        {
            float baseAngle = i * angleStep * Mathf.Deg2Rad;

            float finalAngle = baseAngle + charAngle;
            tierFilter[0] = itemTiersToGenerate[i];
            Vector3 spawnPosition = new Vector3(transform.position.x + (spawnRadius * Mathf.Cos(finalAngle)), transform.position.y + (spawnRadius * Mathf.Sin(finalAngle)), transform.position.z);
            ItemsManager.SpawnRandomItem(spawnPosition, Quaternion.identity, playerContext.canvas.transform, allowedTiers: tierFilter);
        }
    }
}
