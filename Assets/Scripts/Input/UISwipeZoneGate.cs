using UnityEngine;
using UnityEngine.EventSystems;

public class UISwipeZoneGate : MonoBehaviour
{
    [Header("Swipe Zone")]
    [SerializeField] private RectTransform swipeZone; // If null, uses this object's RectTransform
    [SerializeField] private Canvas canvas;           // Auto-finds if null

    [Header("Optional Filters")]
    [SerializeField] private bool blockWhenOverUI = false; 

    private void Awake()
    {
        if (swipeZone == null)
            swipeZone = GetComponent<RectTransform>();

        if (canvas == null && swipeZone != null)
            canvas = swipeZone.GetComponentInParent<Canvas>();
    }

    public bool IsAllowed(Vector2 screenPos)
    {
        if (swipeZone == null) return true;

        if (blockWhenOverUI && EventSystem.current != null)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (EventSystem.current.IsPointerOverGameObject())
                return false;
#else
            // Touch pointer on device (fingerId 0 is usually fine for single-touch)
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return false;
#endif
        }

        Camera uiCam = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCam = canvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(swipeZone, screenPos, uiCam);
    }
}