using UnityEngine;
public class PitfallTrigger : MonoBehaviour
{
    [Header("Respawn")]
    [Tooltip("The Transform the player is teleported to.")]
    [SerializeField] private Transform respawnPoint;

    [Header("Damage")]
    [Tooltip("Damage dealt on each fall (1 = one pip).")]
    [SerializeField] private int damage = 1;

    [Header("Detection")]
    [Tooltip("Tag used to identify the player.")]
    [SerializeField] private string playerTag = "Player";
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        // Teleport the player to the respawn point.
        // CharacterController must be disabled briefly because it
        // overrides transform.position during Move() calls, which
        // can fight the teleport and snap the player back.
        CharacterController cc = other.GetComponent<CharacterController>();

        if (cc != null) cc.enabled = false;

        other.transform.position = respawnPoint.position;
        other.transform.rotation = respawnPoint.rotation;

        if (cc != null) cc.enabled = true;

        // Deal damage directly through PlayerHealth.
        PlayerHealth target = other.GetComponent<PlayerHealth>();

        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }
}