using System;
using UnityEngine;

public enum AudioChannel
{
    Music,
    PlayerSfx,
    EnemySfx
}

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance { get; private set; }

    public event Action OnAudioSettingsChanged;

    private const string KeyMusicEnabled = "audio_music_enabled";
    private const string KeyMusicVolume = "audio_music_volume";
    private const string KeyPlayerEnabled = "audio_player_enabled";
    private const string KeyPlayerVolume = "audio_player_volume";
    private const string KeyEnemyEnabled = "audio_enemy_enabled";
    private const string KeyEnemyVolume = "audio_enemy_volume";

    [Header("Defaults (used if no PlayerPrefs yet)")]
    [SerializeField] private bool musicEnabledDefault = true;
    [SerializeField, Range(0f, 1f)] private float musicVolumeDefault = 0.6f;

    [SerializeField] private bool playerSfxEnabledDefault = true;
    [SerializeField, Range(0f, 1f)] private float playerSfxVolumeDefault = 1f;

    [SerializeField] private bool enemySfxEnabledDefault = true;
    [SerializeField, Range(0f, 1f)] private float enemySfxVolumeDefault = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SeedIfMissing(KeyMusicEnabled, musicEnabledDefault ? 1 : 0);
        SeedIfMissing(KeyMusicVolume, musicVolumeDefault);

        SeedIfMissing(KeyPlayerEnabled, playerSfxEnabledDefault ? 1 : 0);
        SeedIfMissing(KeyPlayerVolume, playerSfxVolumeDefault);

        SeedIfMissing(KeyEnemyEnabled, enemySfxEnabledDefault ? 1 : 0);
        SeedIfMissing(KeyEnemyVolume, enemySfxVolumeDefault);
    }

    private void SeedIfMissing(string key, int value)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetInt(key, value);
    }

    private void SeedIfMissing(string key, float value)
    {
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetFloat(key, value);
    }

    public bool GetEnabled(AudioChannel channel)
    {
        return channel switch
        {
            AudioChannel.Music => PlayerPrefs.GetInt(KeyMusicEnabled, 1) == 1,
            AudioChannel.PlayerSfx => PlayerPrefs.GetInt(KeyPlayerEnabled, 1) == 1,
            AudioChannel.EnemySfx => PlayerPrefs.GetInt(KeyEnemyEnabled, 1) == 1,
            _ => true
        };
    }

    public float GetVolume(AudioChannel channel)
    {
        float v = channel switch
        {
            AudioChannel.Music => PlayerPrefs.GetFloat(KeyMusicVolume, 0.6f),
            AudioChannel.PlayerSfx => PlayerPrefs.GetFloat(KeyPlayerVolume, 1f),
            AudioChannel.EnemySfx => PlayerPrefs.GetFloat(KeyEnemyVolume, 1f),
            _ => 1f
        };

        return Mathf.Clamp01(v);
    }

    // Returns 0 if disabled.
    public float GetEffectiveVolume(AudioChannel channel)
    {
        return GetEnabled(channel) ? GetVolume(channel) : 0f;
    }

    public void SetEnabled(AudioChannel channel, bool enabled)
    {
        switch (channel)
        {
            case AudioChannel.Music: PlayerPrefs.SetInt(KeyMusicEnabled, enabled ? 1 : 0); break;
            case AudioChannel.PlayerSfx: PlayerPrefs.SetInt(KeyPlayerEnabled, enabled ? 1 : 0); break;
            case AudioChannel.EnemySfx: PlayerPrefs.SetInt(KeyEnemyEnabled, enabled ? 1 : 0); break;
        }

        PlayerPrefs.Save();
        OnAudioSettingsChanged?.Invoke();
    }

    public void SetVolume(AudioChannel channel, float volume01)
    {
        float v = Mathf.Clamp01(volume01);

        switch (channel)
        {
            case AudioChannel.Music: PlayerPrefs.SetFloat(KeyMusicVolume, v); break;
            case AudioChannel.PlayerSfx: PlayerPrefs.SetFloat(KeyPlayerVolume, v); break;
            case AudioChannel.EnemySfx: PlayerPrefs.SetFloat(KeyEnemyVolume, v); break;
        }

        PlayerPrefs.Save();
        OnAudioSettingsChanged?.Invoke();
    }
}