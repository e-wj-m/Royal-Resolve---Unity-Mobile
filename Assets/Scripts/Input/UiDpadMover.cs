using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class UiDpadFpsMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundedGravity = -2f; // small downward force to keep CC grounded

    [Header("Camera")]
    [SerializeField] private Camera playerCamera; 

    private CharacterController controller;
    private float verticalVelocity;

    // Held states from the D-pad
    private bool forwardHeld;
    private bool backHeld;
    private bool leftHeld;
    private bool rightHeld;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (controller == null || playerCamera == null)
            return;

        // Horizontal movement 
        Vector3 horizontalDir = GetInputDirection();      // normalized on XZ
        Vector3 horizontalVelocity = horizontalDir * moveSpeed;

        // Gravity / vertical motion
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = groundedGravity;
        }
        else
        {
            // accelerate downward
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Combine horizontal + vertical and move
        Vector3 move = horizontalVelocity;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private Vector3 GetInputDirection()
    {
        Vector3 result = Vector3.zero;

        // Camera-relative forward/right on the ground plane
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        if (forwardHeld) result += forward;
        if (backHeld) result -= forward;
        if (leftHeld) result -= right;
        if (rightHeld) result += right;

        if (result.sqrMagnitude > 0.0001f)
            result.Normalize();

        return result;
    }

    // Called by the UI buttons via EventTrigger 

    public void ForwardDown() { forwardHeld = true; }
    public void ForwardUp() { forwardHeld = false; }

    public void BackDown() { backHeld = true; }
    public void BackUp() { backHeld = false; }

    public void LeftDown() { leftHeld = true; }
    public void LeftUp() { leftHeld = false; }

    public void RightDown() { rightHeld = true; }
    public void RightUp() { rightHeld = false; }
}