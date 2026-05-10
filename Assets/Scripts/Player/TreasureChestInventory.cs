using UnityEngine;
using TMPro;

public class TreasureChestInventory : MonoBehaviour
{
    public static TreasureChestInventory Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject chestUIRoot;
    [SerializeField] private TextMeshProUGUI chestCountText;

    private int chestCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddChest()
    {
        chestCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (chestUIRoot != null)
            chestUIRoot.SetActive(chestCount > 0);

        if (chestCountText != null)
            chestCountText.text = chestCount.ToString();
    }
}