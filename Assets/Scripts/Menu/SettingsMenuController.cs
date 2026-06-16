using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SettingsMenuController : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(SetupNextFrame());
    }

    private IEnumerator SetupNextFrame()
    {
        yield return null;
        SetupControls();
    }

    private void SetupControls()
    {
        var doc = GetComponent<UIDocument>();
        if (doc == null)
        {
            Debug.LogError("[Settings] No UIDocument on this object");
            return;
        }

        var root = doc.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[Settings] rootVisualElement is null");
            return;
        }

        var musicToggle = root.Q<Toggle>("MusicToggle");
        var musicSlider = root.Q<SliderInt>("MusicSlider");

        var settings = AudioSettingsManager.Instance;

        Debug.Log($"[Settings] toggle={(musicToggle != null)}, " +
                  $"slider={(musicSlider != null)}, " +
                  $"settings={(settings != null)}, " +
                  $"settingsID={(settings != null ? settings.GetInstanceID() : 0)}");

        if (settings == null)
        {
            Debug.LogError("[Settings] AudioSettingsManager.Instance is null");
            return;
        }

        if (musicToggle == null || musicSlider == null)
        {
            Debug.LogError("[Settings] Could not find MusicToggle or MusicSlider — check name attributes");
            return;
        }

        // Initialize controls FROM saved settings (no notify, so we don't stomp saved data)
        musicToggle.SetValueWithoutNotify(settings.GetEnabled(AudioChannel.Music));
        musicSlider.SetValueWithoutNotify(Mathf.RoundToInt(settings.GetVolume(AudioChannel.Music) * 100f));

        // Push UI changes INTO settings
        musicToggle.RegisterValueChangedCallback(evt =>
        {
            Debug.Log($"[Settings] toggle changed to {evt.newValue}");
            settings.SetEnabled(AudioChannel.Music, evt.newValue);
        });

        // SliderInt is 0-100; manager stores 0-1, so scale down
        musicSlider.RegisterValueChangedCallback(evt =>
        {
            Debug.Log($"[Settings] slider changed to {evt.newValue}");
            settings.SetVolume(AudioChannel.Music, evt.newValue / 100f);
        });
    }
}