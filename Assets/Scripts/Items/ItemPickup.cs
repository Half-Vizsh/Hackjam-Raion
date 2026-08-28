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
    private bool _isFragile = false;
    #endregion

    #region Unity Method
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isFragile) return;
        Destroy(gameObject);
    }
    #endregion

    #region Private Methods
    #endregion

    #region  Public Methdos
    public bool TryPickup(PlayerInventory inventory)
    {
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
    public void MakeFragile()
    {
        _isFragile = true;
    }
    public void Initialize(ItemStack stack)
    {
    _itemStack = stack;
    }
    #endregion
}
