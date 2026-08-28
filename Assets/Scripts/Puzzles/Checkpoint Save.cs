using System.Collections.Generic;
using UnityEngine;

public class CheckpointSave : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<GameObject> puzzleStored;
    [SerializeField] private Transform respawnPos;
    public Transform RespawnPos => respawnPos;

    private ItemStack[] _savedInventorySnapshot;
    private bool _hasSaved = false;
    private PlayerInventory inventory;

    public static CheckpointSave ActiveCheckpoint { get; private set; }
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
            ActiveCheckpoint = this; // checkpoint ini jadi yang aktif
        }
    }

    public void RestoreSegment()
    {
        inventory.RestoreFromSnapshot(_savedInventorySnapshot);
        foreach (GameObject puzzle in puzzleStored)
        {
            if (puzzle.TryGetComponent(out IResetable resettable))
            {
                resettable.ResetTrap();
            }
        }
    }

    public void SaveInventory(PlayerInventory inventory)
    {
        _savedInventorySnapshot = inventory.GetInventorySnapshot();
    }
}