using UnityEngine;
using System.Linq;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ItemData[] itemPool;
    [SerializeField] private ItemType typeToSpawn;
    [SerializeField] private Transform spawnPoint;

    [Header("Stack Count")]
    [SerializeField] private int minCount = 1;
    [SerializeField] private int maxCount = 3;

    public GameObject SpawnRandomItem()
    {
        ItemData[] filteredPool = itemPool.Where(item => item.itemType == typeToSpawn).ToArray();

        if (filteredPool.Length == 0)
        {
            Debug.LogWarning($"[ItemSpawner] Tidak ada item bertipe {typeToSpawn} di pool!");
            return null;
        }

        ItemData chosenItem = filteredPool[Random.Range(0, filteredPool.Length)];

        if (chosenItem.physicalPrefab == null)
        {
            Debug.LogError($"[ItemSpawner] '{chosenItem.itemName}' tidak punya physicalPrefab!");
            return null;
        }

        int count = Random.Range(minCount, maxCount + 1);

        GameObject spawned = Instantiate(chosenItem.physicalPrefab, spawnPoint.position, Quaternion.identity);

        if (spawned.TryGetComponent(out ItemPickup pickup))
        {
            pickup.Initialize(new ItemStack(chosenItem, count));
        }
        else
        {
            Debug.LogError($"[ItemSpawner] Prefab '{chosenItem.physicalPrefab.name}' tidak punya komponen ItemPickup!");
        }

        return spawned;
    }
}
