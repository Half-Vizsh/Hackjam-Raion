using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ItemStack
{
   [SerializeField] private ItemData _itemData;
   [SerializeField] private int _amount;

   public ItemData Item => _itemData;
   public int Amount => _amount;
    public void AddToStack()
    {
        this._amount++;
    }
    public void AddCustomAmount(int amount)
    {
        this._amount += amount;
    }
    public void RemoveFromStack()
    {
        this._amount--;
    }
}
