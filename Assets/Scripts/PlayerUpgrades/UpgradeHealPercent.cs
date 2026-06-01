using UnityEngine;

public class UpgradeHealPercent : MonoBehaviour
{
    [SerializeField] private float upgradedHealPercent = 50f;

    public void Apply()
    {
        if (PotionInventory.Instance == null) return;
        PotionInventory.Instance.UpgradeHealPercent(upgradedHealPercent);
    }
}
