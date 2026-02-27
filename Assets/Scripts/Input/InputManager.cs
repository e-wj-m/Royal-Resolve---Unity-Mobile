using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public event System.Action OnTouchBegin;
    public event System.Action OnTouchEnd;
    public event System.Action<Vector3> OnPhoneTilt;

    private PlayerControls input;
    private bool hooked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        input = new PlayerControls();

        HookInputEventsOnce();
    }

    private void OnEnable()
    {
        if (input == null)
            input = new PlayerControls();

        input.Enable();
    }

    private void OnDisable()
    {
        if (input != null)
            input.Disable();
    }

    private void OnDestroy()
    {
        if (Instance != this) return;

        UnhookInputEvents();
        Instance = null;
    }

    private void HookInputEventsOnce()
    {
        if (hooked || input == null) return;

        input.Gameplay.Touch.started += TouchStarted;
        input.Gameplay.Touch.canceled += TouchCanceled;
        input.Gameplay.Accelerometer.performed += AccelerometerPerformed;

        hooked = true;
    }

    private void UnhookInputEvents()
    {
        if (!hooked || input == null) return;

        input.Gameplay.Touch.started -= TouchStarted;
        input.Gameplay.Touch.canceled -= TouchCanceled;
        input.Gameplay.Accelerometer.performed -= AccelerometerPerformed;

        hooked = false;
    }

    private void TouchStarted(InputAction.CallbackContext ctx) => OnTouchBegin?.Invoke();
    private void TouchCanceled(InputAction.CallbackContext ctx) => OnTouchEnd?.Invoke();
    private void AccelerometerPerformed(InputAction.CallbackContext ctx) => OnPhoneTilt?.Invoke(ctx.ReadValue<Vector3>());

    public Vector2 GetTouchScreenPosition()
    {
        if (input == null) return Vector2.zero;
        return input.Gameplay.PrimaryPosition.ReadValue<Vector2>();
    }

    public Vector3 GetTouchWorldPosition(Camera cameraToUse = null, float worldZ = 0f)
    {
        if (cameraToUse == null)
            cameraToUse = Camera.main;

        if (cameraToUse == null)
        {
            Debug.LogError("GetTouchWorldPosition: No camera available (Camera.main is null).");
            return Vector3.zero;
        }

        Vector2 screenPos = GetTouchScreenPosition();
        float zDistFromCamera = worldZ - cameraToUse.transform.position.z;

        Vector3 worldPos = cameraToUse.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistFromCamera));
        worldPos.z = worldZ;
        return worldPos;
    }
}