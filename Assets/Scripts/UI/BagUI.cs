using UnityEngine;
using UnityEngine.UI;
public class BagUI : MonoBehaviour
{
    #region Variables
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private Image fillImage;
    [Header("Colors")]
    [SerializeField] private Color greenColor = Color.green;
    [SerializeField] private Color yellowColor = Color.yellow;
    [SerializeField] private Color redColor = Color.red;
    [Header("Keyframe Percentages")]
    [SerializeField] private float greenEnd = 0.25f;  // hijau penuh sampai sini
    [SerializeField] private float yellowPoint = 0.5f; // kuning penuh di titik ini
    [SerializeField] private float redPoint = 0.75f;   // merah penuh mulai titik ini
    #endregion
    #region Unity Methods
    private void Awake()
    {
        playerInventory = FindAnyObjectByType<PlayerInventory>();
    }
    private void OnEnable()
    {
        playerInventory.OnWeightChanged += UpdateBar;
    }
    private void OnDisable()
    {
        playerInventory.OnWeightChanged -= UpdateBar;        
    }
    #endregion
    #region Private Methods
    private void UpdateBar(float percentage)
    {
        fillImage.fillAmount = percentage;
        fillImage.color = GetColorForPercentage(percentage);
    }
      private Color GetColorForPercentage(float percentage)
    {
        if (percentage <= greenEnd)
        {
            return greenColor;
        }
        else if (percentage <= yellowPoint)
        {
            float t = Mathf.InverseLerp(greenEnd, yellowPoint, percentage);
            return Color.Lerp(greenColor, yellowColor, t);
        }
        else if (percentage <= redPoint)
        {
            float t = Mathf.InverseLerp(yellowPoint, redPoint, percentage);
            return Color.Lerp(yellowColor, redColor, t);
        }
        else
        {
            return redColor;
        }
    }
    #endregion
}
