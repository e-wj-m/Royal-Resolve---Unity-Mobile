using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController_InputSystem : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputActionReference moveAction; 
    [SerializeField] private VirtualMoveInput virtualMove;    

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform referenceFrame; 

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (virtualMove == null)
            virtualMove = FindFirstObjectByType<VirtualMoveInput>();
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.action.Disable();
    }

    private void Update()
    {
        Vector2 move = Vector2.zero;

        // Read keyboard/gamepad 
        if (moveAction != null)
            move += moveAction.action.ReadValue<Vector2>();

        if (virtualMove != null)
            move += virtualMove.Move;

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        Vector3 worldDir = GetWorldDirection(move);
        controller.Move(worldDir * moveSpeed * Time.deltaTime);
    }

    private Vector3 GetWorldDirection(Vector2 input)
    {
        if (input.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;

        if (referenceFrame != null)
        {
            forward = referenceFrame.forward;
            right = referenceFrame.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }

        return right * input.x + forward * input.y;
    }
}