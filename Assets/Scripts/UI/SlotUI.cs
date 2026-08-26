using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private GameObject _selectImage;
    [SerializeField] private Image _slotBase;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _itemAmount;
    public void UpdateItemInfo(ItemStack item)
    {
        if (item!=null) this._iconImage.sprite = item.ItemData.icon;
        else this._iconImage.sprite = null;
    }
    public void ActivateSelectedSlot()
    {
        _selectImage.SetActive(true);
    }
    public void DisableSelectedSlot()
    {
        _selectImage.SetActive(false);
    }
}
