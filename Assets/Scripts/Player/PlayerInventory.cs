using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    #region Variables
    private ItemStack[] _inventorySlots = new ItemStack[10];
    public event Action<int, ItemStack> OnInventoryChanged;
    private PlayerStateMachine playerSM;
    private PlayerInput _inputSystem;
    [Header("Pick Up")]
    [SerializeField] private int _selectedPickupIdx;
    private List<ItemPickup> _itemsNearby = new List<ItemPickup>();
    private ItemPickup _currentClosest;
    [Header("Select Inventory")]
    [SerializeField] private int _currentSlotIdx = 0;
    public event Action <int, ItemStack> OnSlotChanged;
    private ItemStack _currentSelected;
    public ItemStack GetCurrentSelected () => _currentSelected;
    [Header("Delete Item From Inventory")]
    private PlayerUseController _useController;
    [Header("Weight System")]
    [SerializeField] private float _maxWeight;
    private float _currentWeight;
    public event Action <float> OnWeightChanged;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _inputSystem = new PlayerInput();
        _useController = GetComponent<PlayerUseController>();
        playerSM = GetComponent<PlayerStateMachine>();
    }
    private void Start()
    {
        OnSlotChanged?.Invoke(_currentSlotIdx, _inventorySlots[_currentSlotIdx]);
        RecalculateWeight();
    }
    private void Update()
    {
        if (PauseController.instance.IsPause) return;            
        if (playerSM.CurrentState == PlayerState.Dead) return;

        if (_inputSystem.Player.Collect.WasPressedThisFrame())
        {
            HandlePickup();
        }
        UpdateClosest();
        _currentSelected = _inventorySlots[_currentSlotIdx];
    }
    private void OnEnable()
    {
        _inputSystem.Enable();
        _inputSystem.Player.Hotbar.performed += ChangeSelected;
        _inputSystem.Player.HotbarScroll.performed += HandleHotbarScroll;
        _useController.OnItemUsed += HandleItemUsed;
    }
    private void OnDisable()
    {
        _inputSystem.Player.HotbarScroll.performed -= HandleHotbarScroll;
        _inputSystem.Player.Hotbar.performed -= ChangeSelected;
        _useController.OnItemUsed -= HandleItemUsed;
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
            RecalculateWeight();
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
        if (PauseController.instance.IsPause) return;
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
        if (PauseController.instance.IsPause) return;
        
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
    private void HandleItemUsed()
    {
        _inventorySlots[_currentSlotIdx] = null;
        OnInventoryChanged?.Invoke(_currentSlotIdx, null);
        RecalculateWeight();
    }
    private void RecalculateWeight()
    {
        _currentWeight = 0;
        foreach(ItemStack item in _inventorySlots)
        {
            if (item == null) continue;
            _currentWeight += item.ItemData.itemWeight;
        }
        OnWeightChanged?.Invoke(_currentWeight/_maxWeight);
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
    public float CalculateInventoryValue()
    {
        float total = 0;
        foreach(ItemStack item in _inventorySlots)
        {
            if (item!=null) total += item.ItemData.sellValue;
        }
        return total;
    }
    public ItemStack[] GetInventorySnapshot()
    {
        ItemStack[] snapshot = new ItemStack[_inventorySlots.Length];
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            snapshot[i] = _inventorySlots[i] != null ? _inventorySlots[i].Clone() : null;
        }
        return snapshot;
    }

    public void RestoreFromSnapshot(ItemStack[] snapshot)
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            _inventorySlots[i] = snapshot[i] != null ? snapshot[i].Clone() : null;
            OnInventoryChanged?.Invoke(i, _inventorySlots[i]);
        }
    }
    #endregion
}
