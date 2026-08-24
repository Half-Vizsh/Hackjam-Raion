using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    #region Variables
    private ItemStack[] _inventorySlots = new ItemStack[10];
    public event Action<int, ItemStack> OnInventoryChanged;
    private PlayerInput _inputSystem;
    [Header("Pick Up")]
    [SerializeField] private int _selectedPickupIdx;
    private List<ItemPickup> _itemsNearby = new List<ItemPickup>();
    [Header("Select Inventory")]
    [SerializeField] private int _selectedInventoryIdx;
    public event Action <int> OnSlotChanged;
    private int _currentSlotIdx;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _inputSystem = new PlayerInput();
    }
    private void Update()
    {
        if (_inputSystem.Player.Pickup.WasPressedThisFrame())
        {
            HandlePickup();
        }
        UpdatePickupHighlight();
    }
    private void OnEnable()
    {
        _inputSystem.Enable();
        _inputSystem.Player.Hotbar.performed += ChangeSelected;
    }
    private void OnDisable()
    {
        _inputSystem.Player.Hotbar.performed -= ChangeSelected;
        _inputSystem.Disable();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemPickup pickup = other.GetComponent<ItemPickup>();
        if (pickup == null) return;

        Debug.Log("[PlayerInventory] new Item in pick up range" + other.name);
        _itemsNearby.Add(pickup);
        if (_itemsNearby.Count == 1) _itemsNearby[_selectedPickupIdx].ActivateHighlight();
        else
        {
            foreach (ItemPickup item in _itemsNearby) item.DisableHighlight();
            pickup.ActivateHighlight();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        ItemPickup pickup = other.GetComponent<ItemPickup>();
        if (pickup == null) return;

        Debug.Log("[PlayerInventory] an item is removed from pick up range" + other.name);
        _itemsNearby.Remove(pickup);
        if (pickup.HighlightActive)
        {
            pickup.DisableHighlight();
            _selectedPickupIdx = 0;
            if (_selectedPickupIdx == _itemsNearby.Count) return;
            if (_itemsNearby[_selectedPickupIdx] != null)
            {
                _itemsNearby[_selectedPickupIdx].ActivateHighlight();
            }
        }
    }
    #endregion

    #region Private Methods
    private void HandlePickup()
    {
        if (_itemsNearby.Count == 0) return;
        Debug.Log("Ada item baru");
        ItemPickup targetItem = _itemsNearby[_selectedPickupIdx];
        if (targetItem.TryPickup(this))
        {
            _itemsNearby.Remove(targetItem);
        }
    }
    private void UpdatePickupHighlight()
    {
        //To update Highlight for selection 
        if (_itemsNearby.Count > 0)
        {
            _selectedPickupIdx = Math.Clamp(_selectedPickupIdx, 0, _itemsNearby.Count);
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (_selectedPickupIdx < _itemsNearby.Count - 1)
                {
                    foreach (ItemPickup item in _itemsNearby) item.DisableHighlight();
                    _selectedPickupIdx++;
                    _itemsNearby[_selectedPickupIdx].ActivateHighlight();
                    Debug.Log("[PlayerInventory] Index bertambah, current Index: " + _selectedPickupIdx);
                }
                else
                {
                    foreach (ItemPickup item in _itemsNearby) item.DisableHighlight();
                    _selectedPickupIdx = 0;
                    _itemsNearby[_selectedPickupIdx].ActivateHighlight();
                    Debug.Log("[PlayerInventory] Index bertambah hingga mentok, current Index: " + _selectedPickupIdx);
                }
            }
            else if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                if (_selectedPickupIdx > 0)
                {
                    foreach (ItemPickup item in _itemsNearby) item.DisableHighlight();
                    _selectedPickupIdx--;
                    _itemsNearby[_selectedPickupIdx].ActivateHighlight();
                    Debug.Log("[PlayerInventory] Index berkurang, current Index: " + _selectedPickupIdx);
                }
                else
                {
                    foreach (ItemPickup item in _itemsNearby) item.DisableHighlight();
                    _selectedPickupIdx = _itemsNearby.Count - 1;
                    _itemsNearby[_selectedPickupIdx].ActivateHighlight();
                    Debug.Log("[PlayerInventory] Index berkurang hingga mentok, current index: " + _selectedPickupIdx);
                }
            }
        }
    }
    private void ChangeSelected(InputAction.CallbackContext ctx)
    {
        String keyName = ctx.control.name;
        if (int.TryParse(keyName, out int parsingIdx))
        {
            if (parsingIdx == 0) _currentSlotIdx = _inventorySlots.Count() - 1; //Biar keyboard 0 return index 9
            else _currentSlotIdx = parsingIdx - 1;               
            OnSlotChanged?.Invoke(_currentSlotIdx);
        }
    }
    #endregion

    #region Public Methods
    public bool AddItem(ItemStack addedItem)
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (_inventorySlots[i] == null)
            {
                _inventorySlots[i] = addedItem;
                OnInventoryChanged?.Invoke(i, addedItem);
                Debug.Log("New Item acquired");
                return true;
            }
        }
        Debug.Log("Inventory Full and no Similar Item in Inventory");
        return false;
    }
    #endregion
}
