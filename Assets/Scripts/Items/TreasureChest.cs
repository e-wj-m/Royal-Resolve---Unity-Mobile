using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TreasureChest : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSfx;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 1f;

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        if (pickupSfx != null)
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position, pickupVolume);

        TreasureChestInventory.Instance.AddChest();
        Destroy(gameObject);
    }
}