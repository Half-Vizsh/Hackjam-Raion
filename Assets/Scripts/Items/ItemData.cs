using System;
using UnityEngine;

public enum ItemType
{
    Throwable,
    Placeable    
}
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/Items", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int ItemID;
    public Sprite icon;
    public ItemType itemType;
    public int sellValue;
}
