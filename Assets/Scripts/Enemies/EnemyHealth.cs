using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyHealth : MonoBehaviour, IHittable
{
    [Header("Health")]
    [SerializeField] private int hitsToDie = 3;

    [Header("Animator")]
    [SerializeField] private Animator anim;
    [SerializeField] private float hitFlashTime = 0.12f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;     // optional, will auto-find
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioClip deathSfx;
    [SerializeField, Range(0f, 1f)] private float hitVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float deathVolume = 1f;

    private int hitsTaken;
    private bool isDead;

    private static readonly int IsHit = Animator.StringToHash("isHit");
    private static readonly int IsDead = Animator.StringToHash("isDead");

    private void Awake()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>(); // ok if still null
    }

    public void ReceiveHit()
    {
        if (isDead) return;

        hitsTaken++;

        // SFX: hit 
        float master = (AudioSettingsManager.Instance != null)
            ? AudioSettingsManager.Instance.GetEffectiveVolume(AudioChannel.EnemySfx)
            : 1f;

        PlayOneShot(hitSfx, master * hitVolume);

        // brief hit reaction
        if (anim != null)
        {
            anim.SetBool(IsHit, true);
            StopAllCoroutines();
            StartCoroutine(ClearHitBool());
        }

        if (hitsTaken >= hitsToDie)
            Die();
    }

    private IEnumerator ClearHitBool()
    {
        yield return new WaitForSeconds(hitFlashTime);
        if (anim != null) anim.SetBool(IsHit, false);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Make sure isHit doesn't fight death
        StopAllCoroutines();
        if (anim != null)
        {
            anim.SetBool(IsHit, false);
            anim.SetBool(IsDead, true);
        }

        // SFX: death 
        float master = (AudioSettingsManager.Instance != null)
            ? AudioSettingsManager.Instance.GetEffectiveVolume(AudioChannel.EnemySfx)
            : 1f;

        PlayOneShot(deathSfx, master * deathVolume);

        // Disable colliders 
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = false;

    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null) return;

        // Use audio source
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
            return;
        }

        AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }
}