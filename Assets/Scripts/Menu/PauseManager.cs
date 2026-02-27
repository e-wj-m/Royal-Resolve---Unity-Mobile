using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("UI References (UGUI)")]
    [SerializeField] private GameObject pauseUI;   // Pause overlay root (Canvas or Panel)
    [SerializeField] private Button pauseButton;   // Button on GameUI
    [SerializeField] private Button resumeButton;  // Button on PauseUI
    [SerializeField] private Button quitButton;    // Button on PauseUI

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MenuScene";

    private bool isPaused;

    private void Awake()
    {
        // Ensure correct startup state
        SetPaused(false);
    }

    private void OnEnable()
    {
        if (pauseButton != null) pauseButton.onClick.AddListener(Pause);
        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (quitButton != null) quitButton.onClick.AddListener(QuitToMainMenu);
    }

    private void OnDisable()
    {
        if (pauseButton != null) pauseButton.onClick.RemoveListener(Pause);
        if (resumeButton != null) resumeButton.onClick.RemoveListener(Resume);
        if (quitButton != null) quitButton.onClick.RemoveListener(QuitToMainMenu);
    }

    public void Pause()
    {
        SetPaused(true);
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void QuitToMainMenu()
    {
        // Important: unpause before loading a new scene
        Time.timeScale = 1f;
        isPaused = false;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pauseUI != null)
            pauseUI.SetActive(paused);

        Time.timeScale = paused ? 0f : 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}