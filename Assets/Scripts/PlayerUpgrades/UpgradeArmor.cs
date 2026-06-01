using UnityEngine;

public class UpgradeArmor : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 0.5f;

    public void Apply()
    {
        if (PlayerCombatStats.Instance == null) return;
        PlayerCombatStats.Instance.ApplyArmorUpgrade(damageMultiplier);
    }
}