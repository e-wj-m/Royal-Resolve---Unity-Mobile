using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(Collider))]
public class CollectableCoin : MonoBehaviour
{
    //[Header("Audio")]
    //[SerializeField] private AudioClip pickupSfx;
    //[SerializeField, Range(0f, 1f)] private float pickupVolume = 1f;

    [Header("FMod Events")]
    [SerializeField] private EventReference coinPickupEvent;

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        //if (pickupSfx != null)
        //AudioSource.PlayClipAtPoint(pickupSfx, transform.position, pickupVolume);

        RuntimeManager.PlayOneShot(coinPickupEvent, transform.position);

        CollectableCoinInventory.Instance.AddCoin();
        Destroy(gameObject);
    }
}