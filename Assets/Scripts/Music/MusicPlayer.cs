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
    }

    private void OnEnable()
    {
        if (AudioSettingsManager.Instance != null)
            AudioSettingsManager.Instance.OnAudioSettingsChanged += ApplyVolumeFromSettings;
    }

    private void OnDisable()
    {
        if (AudioSettingsManager.Instance != null)
            AudioSettingsManager.Instance.OnAudioSettingsChanged -= ApplyVolumeFromSettings;
    }

    private void ApplyVolumeFromSettings()
    {
        float v = 0.6f;
        if (AudioSettingsManager.Instance != null)
            v = AudioSettingsManager.Instance.GetEffectiveVolume(AudioChannel.Music);

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