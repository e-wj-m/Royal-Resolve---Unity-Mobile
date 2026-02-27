using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameUIButtonsInput : MonoBehaviour
{
    [Header("Player To Control")]
    [SerializeField] private PlayerController player;  

    // UXML button names:
    private const string ForwardButtonName = "ForwardsButton";
    private const string BackwardButtonName = "BackwardsButton";
    private const string LeftButtonName = "LeftwardsButton";
    private const string RightButtonName = "RightwardsButton";

    private UIDocument uiDoc;

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        if (uiDoc == null)
            uiDoc = GetComponent<UIDocument>();

        var root = uiDoc.rootVisualElement;
        if (root == null || player == null)
        {
            Debug.LogWarning("[GameUIButtonsInput] Missing root or player reference.");
            return;
        }

        // Grab each button by its UXML name
        var forwardButton = root.Q<Button>(ForwardButtonName);
        var backwardButton = root.Q<Button>(BackwardButtonName);
        var leftButton = root.Q<Button>(LeftButtonName);
        var rightButton = root.Q<Button>(RightButtonName);

        if (forwardButton == null) Debug.LogWarning($"[GameUIButtonsInput] Could not find button '{ForwardButtonName}'");
        if (backwardButton == null) Debug.LogWarning($"[GameUIButtonsInput] Could not find button '{BackwardButtonName}'");
        if (leftButton == null) Debug.LogWarning($"[GameUIButtonsInput] Could not find button '{LeftButtonName}'");
        if (rightButton == null) Debug.LogWarning($"[GameUIButtonsInput] Could not find button '{RightButtonName}'");

        // Register press/hold handlers
        RegisterMoveButton(forwardButton, new Vector2(0f, 1f));  // forward
        RegisterMoveButton(backwardButton, new Vector2(0f, -1f));  // backward
        RegisterMoveButton(leftButton, new Vector2(-1f, 0f));  // left
        RegisterMoveButton(rightButton, new Vector2(1f, 0f));  // right
    }

    private void OnDisable()
    {
        // When this object disables, make sure to stop movement
        if (player != null)
            player.SetMoveInput(Vector2.zero);
    }

    private void RegisterMoveButton(Button button, Vector2 dir)
    {
        if (button == null) return;

        // PointerDown = start moving in that direction
        button.RegisterCallback<PointerDownEvent>(_ =>
        {
            if (player != null)
                player.SetMoveInput(dir);
        });

        // PointerUp/Leave/Cancel = stop moving
        button.RegisterCallback<PointerUpEvent>(_ => StopMoving());
        button.RegisterCallback<PointerLeaveEvent>(_ => StopMoving());
        button.RegisterCallback<PointerCancelEvent>(_ => StopMoving());
    }

    private void StopMoving()
    {
        if (player != null)
            player.SetMoveInput(Vector2.zero);
    }
    //private void RegisterMoveButton(Button button, Vector2 dir)
    //{
    //    if (button == null) return;

    //    // PointerDown = start moving in that direction
    //    button.RegisterCallback<PointerDownEvent>(_ =>
    //    {
    //        Debug.Log($"[GameUIButtonsInput] PointerDown on {button.name}, dir = {dir}");
    //        if (player != null)
    //            player.SetMoveInput(dir);
    //    });

    //    // PointerUp/Leave/Cancel = stop moving
    //    button.RegisterCallback<PointerUpEvent>(_ =>
    //    {
    //        Debug.Log($"[GameUIButtonsInput] PointerUp on {button.name}");
    //        StopMoving();
    //    });

    //    button.RegisterCallback<PointerLeaveEvent>(_ =>
    //    {
    //        Debug.Log($"[GameUIButtonsInput] PointerLeave on {button.name}");
    //        StopMoving();
    //    });

    //    button.RegisterCallback<PointerCancelEvent>(_ =>
    //    {
    //        Debug.Log($"[GameUIButtonsInput] PointerCancel on {button.name}");
    //        StopMoving();
    //    });
    //}
}