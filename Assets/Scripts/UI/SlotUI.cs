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
    public void UpdateItemInfo(Sprite icon, int amount)
    {
        Debug.Log("[SlotUI] UI berhasil diubah, current amount = "+amount);
        this._iconImage.sprite = icon;
        _itemAmount.text = amount+"X";
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
