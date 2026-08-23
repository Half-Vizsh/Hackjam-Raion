using JetBrains.Annotations;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IPickupable
{
    #region Variables
    [SerializeField] private ItemStack _itemStack;
    public ItemStack Item => _itemStack;
    #endregion

    #region Unity Method
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
    #endregion
}
