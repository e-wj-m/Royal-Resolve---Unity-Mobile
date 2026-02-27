using UnityEngine;

public class DragCameraLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;   

    [Header("Sensitivity")]
    [SerializeField] private float yawSensitivity = 0.2f;   // horizontal look speed
    [SerializeField] private float pitchSensitivity = 0.2f; // vertical look speed

    [Header("Pitch Limits")]
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Screen Region For Look")]
    [Tooltip("Normalized X (0–1) where look region starts. 0.5 = right half only.")]
    [Range(0f, 1f)]
    [SerializeField] private float lookRegionMinX = 0.5f;

    private bool draggingLook = false;
    private Vector2 lastTouchPos;

    private float yaw;   // world-space Y rotation of player
    private float pitch; // local X rotation of camera

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Initialize yaw/pitch from current transforms
        yaw = transform.eulerAngles.y;

        if (playerCamera)
        {
            pitch = playerCamera.transform.localEulerAngles.x;
            if (pitch > 180f) pitch -= 360f; // convert 0–360 to -180..180
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTouchBegin += HandleTouchBegin;
            InputManager.Instance.OnTouchEnd += HandleTouchEnd;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnTouchBegin -= HandleTouchBegin;
            InputManager.Instance.OnTouchEnd -= HandleTouchEnd;
        }
    }

    private void HandleTouchBegin()
    {
        if (InputManager.Instance == null || playerCamera == null)
            return;

        Vector2 pos = InputManager.Instance.GetTouchScreenPosition();

        // Only start look-drag if touch begins in the "look" region
        float thresholdX = Screen.width * lookRegionMinX;
        if (pos.x >= thresholdX)
        {
            draggingLook = true;
            lastTouchPos = pos;
        }
        else
        {
            draggingLook = false; // touch is probably on D-pad / UI side
        }
    }

    private void HandleTouchEnd()
    {
        draggingLook = false;
    }

    private void Update()
    {
        if (!draggingLook || InputManager.Instance == null || playerCamera == null)
            return;

        Vector2 pos = InputManager.Instance.GetTouchScreenPosition();
        Vector2 delta = pos - lastTouchPos;
        lastTouchPos = pos;

        if (Screen.height <= 0)
            return;

        // Normalize deltas by screen height so it feels similar across resolutions
        float normX = delta.x / Screen.height;
        float normY = delta.y / Screen.height;

        // Yaw: rotate player around Y
        yaw += normX * yawSensitivity * 100f;

        // Pitch: rotate camera up/down (invert so dragging up looks up)
        pitch -= normY * pitchSensitivity * 100f;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        ApplyRotation();
    }

    private void ApplyRotation()
    {
        // Rotate player (yaw)
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Rotate camera (pitch)
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}