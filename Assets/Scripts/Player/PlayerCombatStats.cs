using UnityEngine;

public class PlayerCombatStats : MonoBehaviour
{
    public static PlayerCombatStats Instance { get; private set; }

    [Header("Combat Settings")]
    [SerializeField] private int bonusHits = 0; // each point = 1 extra hit dealt

    [Header("Defense Settings")]
    [SerializeField] private float damageReduction = 1f; // 1 = full damage, 0.5 = half damage

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public int BonusHits => bonusHits;
    public float DamageReduction => damageReduction;

    public void AddBonusHit(int amount = 1)
    {
        bonusHits += amount;
        Debug.Log($"[PlayerCombatStats] Bonus hits: {bonusHits}");
    }

    public void ApplyArmorUpgrade(float multiplier = 0.5f)
    {
        damageReduction = multiplier;
        Debug.Log($"[PlayerCombatStats] Damage reduction set to {damageReduction}");
    }
}