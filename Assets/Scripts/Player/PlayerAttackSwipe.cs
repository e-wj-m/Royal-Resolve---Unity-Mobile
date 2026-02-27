using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class PlayerAttackSwipe : MonoBehaviour
{
    [Header("Attack Gate")]
    [SerializeField] private UISwipeZoneGate swipeGate; 
    [SerializeField] private float minAttackSwipeScreenDistance = 60f; 

    [Header("Attack Rules")]
    [SerializeField] private float attackCooldown = 0.25f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // optional, will auto-find
    [SerializeField] private AudioClip swipeSfx;
    [SerializeField, Range(0f, 1f)] private float swipeVolume = 1f;

    private readonly HashSet<IHittable> hittablesInRange = new HashSet<IHittable>();

    private Vector2 touchStart;
    private bool touchStartedInZone;
    private float lastAttackTime;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>(); // ok if still null
    }

    private void Reset()
    {
        // Sensible defaults for the trigger bubble
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 1.25f;
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTouchBegin += OnTouchBegin;
            InputManager.Instance.OnTouchEnd += OnTouchEnd;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTouchBegin -= OnTouchBegin;
            InputManager.Instance.OnTouchEnd -= OnTouchEnd;
        }
    }

    private void OnTouchBegin()
    {
        if (InputManager.Instance == null) return;

        touchStart = InputManager.Instance.GetTouchScreenPosition();

        // Only allow attacks if the touch started inside the center zone
        touchStartedInZone = (swipeGate == null) || swipeGate.IsAllowed(touchStart);
    }

    private void OnTouchEnd()
    {
        if (InputManager.Instance == null) return;
        if (!touchStartedInZone) return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        Vector2 touchEnd = InputManager.Instance.GetTouchScreenPosition();
        float swipeDist = Vector2.Distance(touchStart, touchEnd);

        // Require a minimum swipe distance so taps don’t count as attacks
        if (swipeDist < minAttackSwipeScreenDistance)
            return;

        lastAttackTime = Time.time;

        float v = (AudioSettingsManager.Instance != null)
            ? AudioSettingsManager.Instance.GetEffectiveVolume(AudioChannel.PlayerSfx) * swipeVolume
            : swipeVolume;

        PlayOneShot(swipeSfx, v);

        PerformAttack();
    }

    private void PerformAttack()
    {
        foreach (var h in hittablesInRange)
        {
            if (h == null) continue;
            h.ReceiveHit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IHittable h = other.GetComponentInParent<IHittable>();
        if (h != null) hittablesInRange.Add(h);
    }

    private void OnTriggerExit(Collider other)
    {
        IHittable h = other.GetComponentInParent<IHittable>();
        if (h != null) hittablesInRange.Remove(h);
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null) return;

        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
            return;
        }

        AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }
}

public interface IHittable
{
    void ReceiveHit();
}