using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class ItemStack
{
   [SerializeField] private ItemData _itemData;
   [SerializeField] private int _amount;

   public ItemData ItemData => _itemData;
   public int Amount => _amount;
    public ItemStack(ItemData item, int count)
    {
        _itemData = item;
        _amount = count;
    }
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
    public ItemStack Clone()
    {
        return new ItemStack(this._itemData, this._amount);
    }
}
