using System.Collections;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 3f;

    [Tooltip("Seconds after the attack anim starts before damage is actually applied.")]
    [SerializeField] private float damageDelay = 1f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("References")]
    [SerializeField] private Animator anim;

    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private float cooldownTimer;
    private bool isAttacking;
    private EnemyHealth enemyHealth;

    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");

    private void Awake()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
        
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        playerTransform = playerObj.transform;
        playerHealth = playerObj.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = playerObj.GetComponentInChildren<PlayerHealth>();
        if (playerHealth == null)
            playerHealth = playerObj.GetComponentInParent<PlayerHealth>();
    }

    private void Update()
    {
        if (playerTransform == null || playerHealth == null) return;
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            if (isAttacking)
            {
                StopAllCoroutines();
                isAttacking = false;

                if (anim != null)
                    anim.SetBool(IsAttacking, false);
            }
            return;
        }
        if (isAttacking) return;

        cooldownTimer -= Time.deltaTime;

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distToPlayer <= attackRange && cooldownTimer <= 0f)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        cooldownTimer = attackCooldown;

        if (anim != null)
            anim.SetBool(IsAttacking, true);

        yield return new WaitForSeconds(damageDelay);

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= attackRange)
        {
            float reduction = PlayerCombatStats.Instance != null ? PlayerCombatStats.Instance.DamageReduction : 1f;
            playerHealth.TakeDamage(attackDamage * reduction);
        }

        yield return new WaitForSeconds(0.25f);

        if (anim != null)
            anim.SetBool(IsAttacking, false);

        isAttacking = false;
    }
}