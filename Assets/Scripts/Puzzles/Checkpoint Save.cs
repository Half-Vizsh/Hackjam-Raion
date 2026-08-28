using System.Collections.Generic;
using UnityEngine;

public class CheckpointSave : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<GameObject> puzzleStored;
    private ItemStack[] _savedInventorySnapshot;
    [SerializeField] private Transform respawnPos;
    private bool _hasSaved = false;
    private PlayerInventory inventory;
    #endregion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasSaved) return;
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Inventory Saved");
            _hasSaved = true;
            inventory = collision.GetComponent<PlayerInventory>();
            SaveInventory(inventory);
        }
    }
    public void RestoreSegment()
    {
        inventory.RestoreFromSnapshot(_savedInventorySnapshot);
        foreach(GameObject puzzle in puzzleStored)
        {
            if (puzzle.GetComponent<IResetable>() != null)
            {
                puzzle.GetComponent<IResetable>().ResetTrap();
            }
        }
    }
    public void SaveInventory(PlayerInventory inventory)
    {
        _savedInventorySnapshot = inventory.GetInventorySnapshot();
    }
}
