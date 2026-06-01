using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotionInventory : MonoBehaviour
{
    public static PotionInventory Instance { get; private set; }

    [Header("Potion Settings")]
    [Tooltip("Percentage of max health restored per potion.")]
    [SerializeField] private float healPercent = 25f;

    [Header("UI References")]
    [SerializeField] private GameObject potionUIRoot;
    [SerializeField] private Button potionButton;
    [SerializeField] private TextMeshProUGUI potionCountText;

    [Header("Audio")]
    [SerializeField] private AudioClip useSfx;
    [SerializeField, Range(0f, 1f)] private float useVolume = 1f;

    private PlayerHealth playerHealth;
    private int potionCount;

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
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerHealth = playerObj.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                playerHealth = playerObj.GetComponentInChildren<PlayerHealth>();
        }

        if (potionButton != null)
            potionButton.onClick.AddListener(UsePotion);

        UpdateUI();
    }

    public void AddPotion()
    {
        potionCount++;
        UpdateUI();
    }

    public void UsePotion()
    {
        if (potionCount <= 0) return;
        if (playerHealth == null) return;
        if (playerHealth.currentHealth >= playerHealth.maxHealth) return;
        if (useSfx != null)
            AudioSource.PlayClipAtPoint(useSfx, Camera.main.transform.position, useVolume);

        float healAmount = playerHealth.maxHealth * (healPercent / 100f);
        playerHealth.Heal(healAmount);
        potionCount--;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (potionUIRoot != null)
            potionUIRoot.SetActive(potionCount > 0);

        if (potionCountText != null)
            potionCountText.text = potionCount.ToString();
    }

    public void UpgradeHealPercent(float newHealPercent)
    {
        healPercent = newHealPercent;
        Debug.Log($"[PotionInventory] Heal upgraded to {healPercent}%");
    }

}