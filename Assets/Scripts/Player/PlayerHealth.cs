using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Player Health Bar")]
    [SerializeField] private Image healthFillImage;

    [Header("Player Health Colors")]
    [SerializeField] private Gradient healthGradient;

    [Header("Health Bar Animation")]
    [Tooltip("How quickly the bar catches up. Higher = faster.")]
    [SerializeField] private float drainSpeed = 5f;

    [Header("Death Overlay")]
    [SerializeField] private GameObject deathOverlay;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private float deathOverlayDelay = 1.5f;

    [Header("Player Death Conditions")]
    [Tooltip("Player Will 'Die' On The Following Conditions:")]
    public string deathTriggerName = "Die";
    public float restartDelay = 2f;

    private bool isDead = false;
    private float targetFill;
    private float currentFill;

    private void Start()
    {
        currentHealth = maxHealth;
        targetFill = 1f;
        currentFill = 1f;
        UpdateHealthBar(true);

        if (deathOverlay != null)
            deathOverlay.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartScene);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    private void Update()
    {
        if (healthFillImage == null) return;
        if (Mathf.Approximately(currentFill, targetFill)) return;

        currentFill = Mathf.MoveTowards(currentFill, targetFill, drainSpeed * Time.deltaTime);
        healthFillImage.fillAmount = currentFill;
        healthFillImage.color = healthGradient.Evaluate(currentFill);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        targetFill = currentHealth / maxHealth;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        targetFill = currentHealth / maxHealth;
    }

    private void UpdateHealthBar(bool instant)
    {
        if (healthFillImage == null) return;

        float normalized = currentHealth / maxHealth;
        targetFill = normalized;

        if (instant)
        {
            currentFill = normalized;
            healthFillImage.fillAmount = currentFill;
        }

        healthFillImage.color = healthGradient.Evaluate(normalized);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Disable player input
        if (InputManager.Instance != null)
            InputManager.Instance.enabled = false;

        Invoke(nameof(ShowDeathOverlay), deathOverlayDelay);
    }

    private void ShowDeathOverlay()
    {
        if (deathOverlay != null)
            deathOverlay.SetActive(true);

        Time.timeScale = 0f;
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;

        if (InputManager.Instance != null)
            InputManager.Instance.enabled = true;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}