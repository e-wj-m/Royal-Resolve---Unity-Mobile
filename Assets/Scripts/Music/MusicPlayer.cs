using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    [SerializeField] private AudioClip musicTrack;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicTrack;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        ApplyVolumeFromSettings();
        audioSource.Play();

        Debug.Log($"[MusicPlayer] Playing: {audioSource.isPlaying}, " +
          $"Volume: {audioSource.volume}, " +
          $"Clip: {(audioSource.clip != null ? audioSource.clip.name : "NULL")}, " +
          $"Mute: {audioSource.mute}, " +
          $"Listener Vol: {AudioListener.volume}, " +
          $"Listener Pause: {AudioListener.pause}");
    }

    private bool subscribed;

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Start()
    {
        TrySubscribe();   // second chance, after all Awakes have run
    }

    private void TrySubscribe()
    {
        if (subscribed) return;
        if (AudioSettingsManager.Instance == null) return;

        AudioSettingsManager.Instance.OnAudioSettingsChanged += ApplyVolumeFromSettings;
        ApplyVolumeFromSettings();
        subscribed = true;
        Debug.Log($"[MusicPlayer] Subscribed, settingsID={AudioSettingsManager.Instance.GetInstanceID()}");
    }

    private void OnDisable()
    {
        if (subscribed && AudioSettingsManager.Instance != null)
        {
            AudioSettingsManager.Instance.OnAudioSettingsChanged -= ApplyVolumeFromSettings;
            subscribed = false;
        }
    }

    private void ApplyVolumeFromSettings()
    {
        float v = 0.6f;
        if (AudioSettingsManager.Instance != null)
            v = AudioSettingsManager.Instance.GetEffectiveVolume(AudioChannel.Music);

        Debug.Log($"[MusicPlayer] ApplyVolumeFromSettings -> {v}");

        audioSource.volume = v;

        if (v <= 0.0001f)
        {
            if (audioSource.isPlaying) audioSource.Pause();
        }
        else
        {
            if (audioSource.clip != null) audioSource.UnPause();
        }
    }
}