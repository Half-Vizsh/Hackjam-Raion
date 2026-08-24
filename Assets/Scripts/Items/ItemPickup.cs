using JetBrains.Annotations;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IPickupable
{
    #region Variables
    [SerializeField] private ItemStack _itemStack;
    public ItemStack Item => _itemStack;
    [SerializeField] private GameObject _highlightChild;
    [SerializeField] private bool _highlightActive = false;
    public bool HighlightActive => _highlightActive;
    #endregion

    #region Unity Method
    #endregion

    #region Private Methods
    #endregion

    #region  Public Methdos
    public bool TryPickup(PlayerInventory inventory)
    {
        // Debug.Log($"Inventory: {inventory}");
        // Debug.Log($"ItemStack: {_itemStack}");
        // Debug.Log($"ItemData: {_itemStack?.ItemData}");
        if (inventory.AddItem(_itemStack))
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
    public void ActivateHighlight()
    {
        this._highlightChild.SetActive(true);
        _highlightActive = true;
    }
    public void DisableHighlight()
    {
        this._highlightChild.SetActive(false);
        _highlightActive = false;
    }
    #endregion
}
