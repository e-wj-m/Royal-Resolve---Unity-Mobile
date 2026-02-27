using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Optional Input System (fallback)")]
    [SerializeField] private InputActionReference moveAction; // Vector2

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private Camera cameraOverride; 

    private CharacterController cc;
    private Camera cam;

    // UI input
    private Vector2 uiMoveInput;
    private bool uiMoveActive;

    // final input used this frame
    private Vector2 moveInput;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = cameraOverride ? cameraOverride : Camera.main;
    }

    private void OnEnable()
    {
        if (moveAction && moveAction.action != null)
            moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction && moveAction.action != null)
            moveAction.action.Disable();
    }

    private void Update()
    {
        if (cc == null) return;

        ReadInputs();

        Vector3 moveDir = ProjectMoveDirection(moveInput);

        // actually move the character
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            cc.Move(moveDir * moveSpeed * Time.deltaTime);

            // face movement direction
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void ReadInputs()
    {
        // fallback keyboard / gamepad 
        Vector2 fallbackMove = Vector2.zero;
        if (moveAction && moveAction.action != null)
            fallbackMove = moveAction.action.ReadValue<Vector2>();

        // UI has priority when active
        moveInput = uiMoveActive ? uiMoveInput : fallbackMove;
    }

    private Vector3 ProjectMoveDirection(Vector2 input)
    {
        if (input.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        if (cam == null)
            return new Vector3(input.x, 0f, input.y).normalized;

        Vector3 fwd = cam.transform.forward;
        Vector3 right = cam.transform.right;
        fwd.y = 0f;
        right.y = 0f;
        fwd.Normalize();
        right.Normalize();

        Vector3 world = fwd * input.y + right * input.x;
        return world.normalized;
    }

    public void SetMoveInput(Vector2 move)
    {
        uiMoveInput = move;
        uiMoveActive = move.sqrMagnitude > 0.0001f;

        Debug.Log($"[PlayerController] SetMoveInput: {move}");
    }
}