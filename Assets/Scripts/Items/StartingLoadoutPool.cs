using UnityEngine;

[CreateAssetMenu(menuName = "Items/Starting Loadout Pool")]
public class StartingLoadoutPool : ScriptableObject
{
    [System.Serializable]
    public class LoadoutEntry
    {
        public ItemData item;
        public int minCount = 1;
        public int maxCount = 1;
    }

    public LoadoutEntry[] possibleItems;
    [Tooltip("Berapa banyak jenis item berbeda yang akan didapat player di awal")]
    public int itemTypesToGrant = 3;
}
