using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

[RequireComponent(typeof(CanvasGroup))]
public class WorldSpaceButton : MonoBehaviour
{
    [Header("Distance Settings")]
    public float visibleDistance = 8f;
    public float interactableDistance = 4f;

    [Header("Fade Settings")]
    public float fadeSpeed = 3f;

    [Header("References")]
    public Transform playerTransform;
    public GestureDetector gestureDetector;   // assign in Inspector
    public Camera worldCamera;               // assign your main/game camera

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onPressed;  // hook up in Inspector like a normal Button

    [Header("Shop Settings")]
    public int coinCost = 1; // Cost in coins to interact with this button (if it's a shop button)

    [Header("FMod Events")]
    [SerializeField] private EventReference buttonActivationEvent;

    private CanvasGroup canvasGroup;
    private bool inRange = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        if (worldCamera == null)
            worldCamera = Camera.main;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void OnEnable()
    {
        if (gestureDetector != null)
            gestureDetector.OnTapDetected += HandleTap;
    }

    void OnDisable()
    {
        if (gestureDetector != null)
            gestureDetector.OnTapDetected -= HandleTap;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        float targetAlpha = dist <= visibleDistance ? 1f : 0f;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        if (canvasGroup.alpha < 0.01f) canvasGroup.alpha = 0f;

        inRange = dist <= interactableDistance;
    }

    private void HandleTap()
    {
        if (!inRange || canvasGroup.alpha < 0.5f) return;

        Vector2 tapPos = InputManager.Instance.GetTouchScreenPosition();

        if (RectTransformUtility.RectangleContainsScreenPoint(
                GetComponent<RectTransform>(), tapPos, worldCamera))
        {
            // Check affordability before doing anything
            if (!CollectableCoinInventory.Instance.CanAfford(coinCost))
            {
                Debug.Log($"[WorldSpaceButton] Can't afford {gameObject.name} — needs {coinCost} coins");
                return;
            }

            // Spend the chests
            CollectableCoinInventory.Instance.SpendCoins(coinCost);

            // Fire the upgrade event
            onPressed?.Invoke();

            RuntimeManager.PlayOneShot(buttonActivationEvent, transform.position);

            // Fade out and disable
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    private System.Collections.IEnumerator FadeOutAndDestroy()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        gameObject.SetActive(false); // or Destroy(gameObject) if you don't need it again
    }
}