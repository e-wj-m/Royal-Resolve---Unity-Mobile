using UnityEngine;
using TMPro;

public class CollectableCoinInventory : MonoBehaviour
{
    public static CollectableCoinInventory Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject coinUIRoot;
    [SerializeField] private TextMeshProUGUI coinCountText;

    private int coinCount;

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

    public void AddCoin()
    {
        coinCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinUIRoot != null)
            coinUIRoot.SetActive(coinCount > 0);

        if (coinCountText != null)
            coinCountText.text = coinCount.ToString();
    }

    public bool CanAfford(int cost)
    {
        return coinCount >= cost;
    }

    public bool SpendCoins(int cost)
    {
        if (!CanAfford(cost)) return false;

        coinCount -= cost;
        UpdateUI();
        return true;
    }

}