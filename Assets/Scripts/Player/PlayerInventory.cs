using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    #region Variables
    private ItemStack[] _inventorySlots = new ItemStack[9];
    [SerializeField] private int _selectedPickup;
    private PlayerInput _inputSystem;
    private List<ItemPickup> _itemsNearby = new List<ItemPickup>();
    public event Action<int, ItemStack> OnInventoryChanged;
    private ItemPickup _currentPickup;
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
        UpdatePickupSelection();
    }
    private void OnEnable()
    {
        _inputSystem.Enable();
    }
    private void OnDisable()
    {
        _inputSystem.Disable();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        ItemPickup pickup = other.GetComponent<ItemPickup>();
        if (pickup == null) return;
        
        Debug.Log("[PlayerInventory] new Item in pick up range"+other.name);
        _itemsNearby.Add(pickup);
        if (_itemsNearby.Count == 1) _itemsNearby[_selectedPickup].ActivateHighlight();
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
        
        Debug.Log("[PlayerInventory] an item is removed from pick up range"+other.name);
        _itemsNearby.Remove(pickup);
        if (pickup.HighlightActive)
        { 
            pickup.DisableHighlight();
            _selectedPickup = 0;
            if (_selectedPickup==_itemsNearby.Count) return;
            if (_itemsNearby[_selectedPickup] != null)
            {
                _itemsNearby[_selectedPickup].ActivateHighlight();
            }
        }
    }
    #endregion

    #region Private Methods
    private void HandlePickup()
    {
        if (_itemsNearby.Count == 0) return;
        Debug.Log("Ada item baru");
        ItemPickup targetItem = _itemsNearby[_selectedPickup];
        
        if (targetItem.TryPickup(this))
        {
            _itemsNearby.Remove(targetItem);
        }
    }
    private void UpdatePickupSelection()
    {
        if (_itemsNearby.Count > 0)
        {
            _selectedPickup = Math.Clamp(_selectedPickup, 0, _itemsNearby.Count);
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (_selectedPickup<_itemsNearby.Count-1) 
                {
                    foreach (ItemPickup item in _itemsNearby) item.DisableHighlight();
                    _selectedPickup++;
                    _itemsNearby[_selectedPickup].ActivateHighlight();
                    Debug.Log("[PlayerInventory] Index bertambah, current Index: "+_selectedPickup);
                } else {
                    foreach (ItemPickup item in _itemsNearby) item.DisableHighlight();
                    _selectedPickup = 0;
                    _itemsNearby[_selectedPickup].ActivateHighlight();
                    Debug.Log("[PlayerInventory] Index bertambah hingga mentok, current Index: "+_selectedPickup);
                }
            } else if (Keyboard.current.qKey.wasPressedThisFrame){
                if (_selectedPickup > 0)
                {
                    foreach (ItemPickup item in _itemsNearby) item.DisableHighlight();
                    _selectedPickup--; 
                    _itemsNearby[_selectedPickup].ActivateHighlight();
                    Debug.Log("[PlayerInventory] Index berkurang, current Index: "+_selectedPickup);
                } else {
                    foreach (ItemPickup item in _itemsNearby) item.DisableHighlight();
                    _selectedPickup = _itemsNearby.Count-1;
                    _itemsNearby[_selectedPickup].ActivateHighlight();
                  Debug.Log("[PlayerInventory] Index berkurang hingga mentok, current index: "+_selectedPickup);  
                } 
            }
        }
    }
    #endregion

    #region Public Methods
    public bool AddItem (ItemStack addedItem)
    {
        Debug.Log("Mencoba menambahkan item");
        Debug.Log($"Array null? {_inventorySlots == null}");
        Debug.Log($"Added item null? {addedItem == null}");

        for (int a = 0; a<_inventorySlots.Length; a++)
        {
            if (_inventorySlots[a] != null && _inventorySlots[a].ItemData.ItemID == addedItem.ItemData.ItemID)
            {
                //Trigger event UI
                _inventorySlots[a].AddToStack();
                OnInventoryChanged?.Invoke(a, _inventorySlots[a]);
                Debug.Log("Item added to the existing stack");
                return true;
            }
        }
        for (int i =0; i<_inventorySlots.Length; i++)
        {
            if (_inventorySlots[i]==null)
            {
                //Trigger event UI
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
