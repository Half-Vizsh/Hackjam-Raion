using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    #region Variables
    private ItemStack[] _inventorySlots = new ItemStack[2];
    [SerializeField] private int selectedSlot;
    private PlayerInput _inputSystem;
    private ItemPickup _itemNearby;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _inputSystem = new PlayerInput();
        // _inventorySlots = new ItemStack[2];
        Debug.Log($"Inventory array: {_inventorySlots}");
        Debug.Log($"Inventory size: {_inventorySlots.Length}");
    }
    private void Update()
    {
        if (_inputSystem.Player.Pickup.WasPressedThisFrame())
        {
            Interact();
        }
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
        _itemNearby = pickup;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _itemNearby = null;
    }
    #endregion

    #region Private Methods
    private void Interact()
    {
        if (_itemNearby == null) return;
        Debug.Log("Ada item baru");
        _itemNearby.TryPickup(this);
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
            if (_inventorySlots[a] != null && _inventorySlots[a].Item.ItemID == addedItem.Item.ItemID)
            {
                _inventorySlots[a].AddToStack();
                Debug.Log("Item added to the existing stack");
                return true;
            }
        }
        for (int i =0; i<_inventorySlots.Length; i++)
        {
            if (_inventorySlots[i]==null)
            {
                //Trigger event?
                _inventorySlots[i] = addedItem;
                Debug.Log("New Item acquired");
                return true;
            } 
        }
        Debug.Log("Inventory Full and no Similar Item in Inventory");
        return false;
    }
    #endregion
}
