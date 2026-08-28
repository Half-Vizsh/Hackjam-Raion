using NUnit.Framework;
using TMPro;
using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    private float totalScore;
    public bool isWin = false;
    public static Finish instance;
    public void Awake()
    {
        if (instance!=this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
               PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
               totalScore = inventory.CalculateInventoryValue();
               isWin = true;
               ShowWinPanel();
        }
    }
    private void ShowWinPanel()
    {
        WinPanel.SetActive(true);
        scoreText.text = "$"+totalScore;
    }
}
