using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[RequireComponent(typeof(CharacterController))]
public class UiDpadFpsMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundedGravity = -2f;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    [Header("FMod Events")]
    [SerializeField] private EventReference footstepLoopEvent;

    private CharacterController controller;
    private float verticalVelocity;

    private EventInstance footstepInstance;
    private bool footstepsPlaying;

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

        footstepInstance = RuntimeManager.CreateInstance(footstepLoopEvent);
        RuntimeManager.AttachInstanceToGameObject(footstepInstance, gameObject);
        //footstepInstance.start();
    }

    private void Update()
    {
        if (controller == null || playerCamera == null)
            return;

        Vector3 horizontalDir = GetInputDirection();
        Vector3 horizontalVelocity = horizontalDir * moveSpeed;

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = groundedGravity;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = horizontalVelocity;
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);

        // Footstep loop: only react when actually moving on the ground
        bool isMoving = horizontalDir.sqrMagnitude > 0.0001f && controller.isGrounded;
        UpdateFootsteps(isMoving);
    }

    private void UpdateFootsteps(bool isMoving)
    {
        if (isMoving && !footstepsPlaying)
        {
            footstepInstance.start();
            footstepsPlaying = true;
        }
        else if (!isMoving && footstepsPlaying)
        {
            footstepInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            footstepsPlaying = false;
        }
    }

    private void OnDestroy()
    {
        footstepInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        footstepInstance.release();
    }

    private Vector3 GetInputDirection()
    {
        Vector3 result = Vector3.zero;
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

    public void ForwardDown() { forwardHeld = true; }
    public void ForwardUp() { forwardHeld = false; }
    public void BackDown() { backHeld = true; }
    public void BackUp() { backHeld = false; }
    public void LeftDown() { leftHeld = true; }
    public void LeftUp() { leftHeld = false; }
    public void RightDown() { rightHeld = true; }
    public void RightUp() { rightHeld = false; }
}