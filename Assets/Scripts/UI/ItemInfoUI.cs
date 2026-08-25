using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using System.Diagnostics.Tracing;

public class ItemInfoUI : MonoBehaviour
{
   #region Variables
   [SerializeField] private CanvasGroup _canvasGroup;
   [SerializeField] private InventoryUI _inventoryUI;
   [SerializeField] private TextMeshProUGUI _itemName;
   [SerializeField] private TextMeshProUGUI _itemWeight;
   [SerializeField] private float _fadeInSpeed;
   [SerializeField] private float _lingeringDur;
   [SerializeField] private float _fadeOutSpeed;
    private float _lingeringTime;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        _inventoryUI.OnChoosenSlotChange+=ChangeInfo;
        _inventoryUI.OnNewItemAdded+=ChangeInfo;
    }
    private void OnDisable()
    {
        _inventoryUI.OnChoosenSlotChange-=ChangeInfo;
        _inventoryUI.OnNewItemAdded-=ChangeInfo;
    }
    private void Update()
    {
        if (_lingeringTime>0)
        {
            ShowInfo();
            _lingeringTime -= Time.deltaTime;
        } else
        {
            HideInfo();
        }
    }
    #endregion
    #region Private Methods
    private void ShowInfo()
    {
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, 1, _fadeInSpeed*Time.deltaTime);
    }
    private void HideInfo()
    {
        _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, 0, _fadeOutSpeed*Time.deltaTime);
    }
    #endregion
    #region Public Methods
    public void ChangeInfo(ItemStack itemStack)
    {
        if (itemStack==null) return; 
        _itemName.text = itemStack.ItemData.itemName;
        String weight = itemStack.ItemData.itemWeight.ToString()+" Kg";
        _itemWeight.text = weight;
        _lingeringTime = _lingeringDur;
    } 
   #endregion
}
