using System.ComponentModel;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    #region Variables
    [SerializeField]
    SlotUI[] uiSlots = new SlotUI [10];
    [SerializeField] private PlayerInventory _playerInventory;
    private int _currentSelectedIdx;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _playerInventory = FindFirstObjectByType<PlayerInventory>();
        _playerInventory.OnInventoryChanged += ChangeSlotUI;
        _playerInventory.OnSlotChanged += ChangeChoosenSlot;
    }
    private void Start()
    {
    }
    private void OnDisable()
    {
        _playerInventory.OnInventoryChanged -= ChangeSlotUI;
        _playerInventory.OnSlotChanged -= ChangeChoosenSlot;
    }
    #endregion

    #region Private Methods
    private void ChangeSlotUI(int slotIdx, ItemStack item)
    {
        uiSlots[slotIdx].UpdateItemInfo(item.ItemData.icon, item.Amount);
    }
    private void ChangeChoosenSlot(int newIdx)
    {
        uiSlots[_currentSelectedIdx].DisableSelectedSlot();
        uiSlots[newIdx].ActivateSelectedSlot();
        _currentSelectedIdx = newIdx;
    }
    #endregion

    #region Public Methods
    #endregion
}
