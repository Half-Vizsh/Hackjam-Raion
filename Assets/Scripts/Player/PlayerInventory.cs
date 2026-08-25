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
    private ItemPickup _currentClosest;
    public event Action <int, ItemStack> OnSlotChanged;
    private int _currentSlotIdx =0;
    
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _inputSystem = new PlayerInput();
    }
    private void Start()
    {
        OnSlotChanged?.Invoke(_currentSlotIdx, _inventorySlots[_currentSlotIdx]);
    }
    private void Update()
    {
        if (_inputSystem.Player.Collect.WasPressedThisFrame())
        {
            HandlePickup();
        }
        UpdateClosest();
    }
    private void OnEnable()
    {
        _inputSystem.Enable();
        _inputSystem.Player.Hotbar.performed += ChangeSelected;
        _inputSystem.Player.HotbarScroll.performed += HandleHotbarScroll;
    }
    private void OnDisable()
    {
        _inputSystem.Player.HotbarScroll.performed -= HandleHotbarScroll;
        _inputSystem.Player.Hotbar.performed -= ChangeSelected;
        _inputSystem.Disable();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemPickup pickup = other.GetComponent<ItemPickup>();
        if (pickup == null) return;

        Debug.Log("[PlayerInventory] new Item in pick up range" + other.name);
        _itemsNearby.Add(pickup);
        _currentClosest = pickup;
        pickup.ActivateHighlight();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        ItemPickup pickup = other.GetComponent<ItemPickup>();
        if (pickup == null) return;

        Debug.Log("[PlayerInventory] an item is removed from pick up range" + other.name);
        _itemsNearby.Remove(pickup);
        pickup.DisableHighlight();
        _selectedPickupIdx = 0;
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
    private void UpdateClosest() 
    {
        if (_currentClosest == null) return;
        Vector3 playerPost = this.transform.position;
        foreach (ItemPickup item in _itemsNearby) 
        {
            float currentClosestDistance = Vector2.SqrMagnitude(playerPost - _currentClosest.transform.position);
            float distance = Vector2.SqrMagnitude(playerPost - item.transform.position);                
            if (distance < currentClosestDistance)
            {
                _currentClosest.DisableHighlight();
                _currentClosest = item;
                _selectedPickupIdx = _itemsNearby.IndexOf(_currentClosest);
                _currentClosest.ActivateHighlight();
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
            OnSlotChanged?.Invoke(_currentSlotIdx, _inventorySlots[_currentSlotIdx]);
        }
    }
    private void HandleHotbarScroll(InputAction.CallbackContext ctx)
    {
        Vector2 scroll = ctx.ReadValue<Vector2>();
        if (scroll.y > 0)
        {
            if (_currentSlotIdx < _inventorySlots.Count()-1) _currentSlotIdx++;
            else _currentSlotIdx = 0;
        } else if (scroll.y < 0)
        {
            if (_currentSlotIdx > 0) _currentSlotIdx--;
            else _currentSlotIdx = _inventorySlots.Count()-1;
        }
        OnSlotChanged?.Invoke(_currentSlotIdx, _inventorySlots[_currentSlotIdx]);
    }
    private void TryUseItem(ItemPickup usedItem)
    {
        
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
        Debug.Log("[PlayerInventory] Inventory Full!");
        return false;
    }
    #endregion
}
