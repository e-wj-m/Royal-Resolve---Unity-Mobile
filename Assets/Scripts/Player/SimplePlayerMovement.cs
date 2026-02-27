using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Camera cameraOverride;  

    private CharacterController cc;
    private Vector2 moveInput;  // x = left/right, y = forward/back

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 move = GetWorldMoveVector(moveInput);

        if (move.sqrMagnitude > 0.0001f)
        {
            cc.Move(move * moveSpeed * Time.deltaTime);

            // face movement direction
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    // Called by the DPad buttons
    public void SetMoveDir(Vector2 dir)
    {
        moveInput = dir;
    }

    private Vector3 GetWorldMoveVector(Vector2 input)
    {
        if (input.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        Camera cam = cameraOverride != null ? cameraOverride : Camera.main;

        if (cam == null)
        {
            // world-space fallback: X = left/right, Z = forward/back
            return new Vector3(input.x, 0f, input.y).normalized;
        }

        // camera-relative movement: up = where the camera looks
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 world = forward * input.y + right * input.x;
        return world.normalized;
    }
}