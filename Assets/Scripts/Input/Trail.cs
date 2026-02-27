using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class Trail : MonoBehaviour
{
    [Header("Where the trail lives")]
    [SerializeField] private Camera cam;
    [SerializeField] private float distanceInFrontOfCamera = 1.5f; 
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private float followLerp = 35f;

    [Header("Swipe Zone (Center Gate)")]
    [SerializeField] private UISwipeZoneGate swipeGate;          
    [SerializeField] private bool stopDrawingIfLeaveZone = true; // If finger leaves center, stop drawing

    private TrailRenderer tr;
    private bool isDrawing;

    private Plane drawPlane; // plane always facing the camera

    private void Awake()
    {
        tr = GetComponent<TrailRenderer>();
        tr.enabled = false;
        tr.Clear();

        if (cam == null) cam = Camera.main;
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTouchBegin += BeginSwipe;
            InputManager.Instance.OnTouchEnd += EndSwipe;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTouchBegin -= BeginSwipe;
            InputManager.Instance.OnTouchEnd -= EndSwipe;
        }
    }

    private void BeginSwipe()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null || InputManager.Instance == null) return;

        Vector2 screenPos = InputManager.Instance.GetTouchScreenPosition();

        // Only begin drawing if touch starts inside the center SwipeZone
        if (swipeGate != null && !swipeGate.IsAllowed(screenPos))
            return;

        // Plane origin is in front of camera; normal points back toward camera
        Vector3 planePoint = cam.transform.position + cam.transform.forward * distanceInFrontOfCamera;
        drawPlane = new Plane(-cam.transform.forward, planePoint);

        // Snap to finger position on that plane
        Vector3 p = ScreenToPlanePoint(screenPos);
        transform.position = p;

        tr.Clear();
        tr.enabled = true;
        isDrawing = true;
    }

    private void EndSwipe()
    {
        isDrawing = false;
        tr.enabled = false;
    }

    private void Update()
    {
        if (!isDrawing || cam == null || InputManager.Instance == null) return;

        Vector2 screenPos = InputManager.Instance.GetTouchScreenPosition();

        // If finger leaves the center zone, stop drawing
        if (swipeGate != null && !swipeGate.IsAllowed(screenPos))
        {
            if (stopDrawingIfLeaveZone)
                EndSwipe();

            return;
        }

        // Keep the plane moving with camera 
        Vector3 planePoint = cam.transform.position + cam.transform.forward * distanceInFrontOfCamera;
        drawPlane.SetNormalAndPosition(-cam.transform.forward, planePoint);

        Vector3 target = ScreenToPlanePoint(screenPos);

        if (smoothFollow)
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followLerp);
        else
            transform.position = target;
    }

    private Vector3 ScreenToPlanePoint(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (drawPlane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return transform.position;
    }
}