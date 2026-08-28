using UnityEngine;
using System.Linq;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private StartingLoadoutPool _pool;
    [SerializeField] private ItemType typeToSpawn;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private int itemCount = 3;
    [SerializeField] private float spreadRadius = 1f;
    private void Start()
    {
        SpawnItems();
    }

    public GameObject SpawnRandomItem(Vector3 Spawnpoint)
    {
        var filteredPool = _pool.possibleItems
            .Where(entry => entry.item.itemType == typeToSpawn)
            .ToArray();

        if (filteredPool.Length == 0)
        {
            Debug.LogWarning($"[ItemSpawner] Tidak ada item bertipe {typeToSpawn} di pool '{_pool.name}'!");
            return null;
        }

        StartingLoadoutPool.LoadoutEntry chosenEntry = filteredPool[Random.Range(0, filteredPool.Length)];

        if (chosenEntry.item.physicalPrefab == null)
        {
            Debug.LogError($"[ItemSpawner] '{chosenEntry.item.itemName}' tidak punya physicalPrefab!");
            return null;
        }

        int count = Random.Range(chosenEntry.minCount, chosenEntry.maxCount + 1);

        GameObject spawned = Instantiate(chosenEntry.item.physicalPrefab, spawnPoint.position, Quaternion.identity);

        if (spawned.TryGetComponent(out ItemPickup pickup))
        {
            pickup.Initialize(new ItemStack(chosenEntry.item, count));
        }
        else
        {
            Debug.LogError($"[ItemSpawner] Prefab '{chosenEntry.item.physicalPrefab.name}' tidak punya komponen ItemPickup!");
        }

        return spawned;
    }
    public void SpawnItems()
{
    for (int i = 0; i < itemCount; i++)
    {
        Vector2 offset = Random.insideUnitCircle * spreadRadius;
        Vector3 spawnPos = centerPoint.position + new Vector3(offset.x, offset.y, 0f);
        SpawnRandomItem(spawnPos); // ganti parameter jadi Vector3 langsung, bukan Transform
    }
}
}
