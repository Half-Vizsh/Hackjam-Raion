using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StartingLoadoutManager : MonoBehaviour
{
 [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private StartingLoadoutPool _loadoutPool;

    private void Start()
    {
        GrantStartingItems();
    }

    private void GrantStartingItems()
    {
        if (_loadoutPool == null || _loadoutPool.possibleItems.Length == 0) return;

        List<StartingLoadoutPool.LoadoutEntry> pool = _loadoutPool.possibleItems.ToList();
        int typesToGive = Mathf.Min(_loadoutPool.itemTypesToGrant, pool.Count);

        for (int i = 0; i < typesToGive; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            StartingLoadoutPool.LoadoutEntry entry = pool[randomIndex];
            pool.RemoveAt(randomIndex); // biar item yg sama nggak kepilih dua kali

            int count = Random.Range(entry.minCount, entry.maxCount + 1);
            ItemStack stack = new ItemStack(entry.item, count);
            _inventory.AddItem(stack);
        }
    }
}
