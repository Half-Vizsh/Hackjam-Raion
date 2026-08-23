using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    #region Variables
    [SerializeField]
    SlotUI[] uiSlots = new SlotUI [9];
    [SerializeField] private PlayerInventory _playerInventory;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _playerInventory = FindFirstObjectByType<PlayerInventory>();
    }
    private void Start()
    {
        _playerInventory.OnInventoryChanged += ChangeSlotUI;
    }
    private void OnDisable()
    {
        _playerInventory.OnInventoryChanged -= ChangeSlotUI;
    }
    #endregion

    #region Private Methods
    private void ChangeSlotUI(int slotIdx, ItemStack item)
    {
        uiSlots[slotIdx].UpdateItemInfo(item.ItemData.icon, item.Amount);
    }
    #endregion

    #region Public Methods
    #endregion
}
