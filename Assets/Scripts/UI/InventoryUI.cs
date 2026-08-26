using System;
using System.ComponentModel;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    #region Variables
    [SerializeField]
    SlotUI[] uiSlots = new SlotUI [10];
    [SerializeField] private PlayerInventory _playerInventory;
    public event Action <ItemStack> OnChoosenSlotChange;
    public event Action <ItemStack> OnNewItemAdded;
    private int _currentSelectedIdx;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _playerInventory = FindFirstObjectByType<PlayerInventory>();
        _playerInventory.OnInventoryChanged += ChangeSlotUI;
        _playerInventory.OnSlotChanged += ChangeChoosenSlot;
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
        uiSlots[slotIdx].UpdateItemInfo(item);
        OnNewItemAdded?.Invoke(item);
    }
    private void ChangeChoosenSlot(int newIdx, ItemStack item)
    {
        uiSlots[_currentSelectedIdx].DisableSelectedSlot();
        uiSlots[newIdx].ActivateSelectedSlot();
        _currentSelectedIdx = newIdx;
        OnChoosenSlotChange?.Invoke(item);
    }
    #endregion

    #region Public Methods
    #endregion
}
