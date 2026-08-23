using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private Image _selectImage;
    [SerializeField] private Image _slotBase;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _itemAmount;
    public void UpdateItemInfo(Sprite icon, int amount)
    {
        Debug.Log("UI berhasil diubah");
        this._iconImage.sprite = icon;
        _itemAmount.text = amount+"X";
    }
}
