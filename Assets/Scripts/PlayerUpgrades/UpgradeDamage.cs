using UnityEngine;

public class UpgradeDamage : MonoBehaviour
{
    [SerializeField] private int bonusHitsToAdd = 1;

    public void Apply()
    {
        if (PlayerCombatStats.Instance == null) return;
        PlayerCombatStats.Instance.AddBonusHit(bonusHitsToAdd);
    }
}
