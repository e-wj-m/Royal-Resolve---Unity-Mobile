using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class VirtualMoveInput : MonoBehaviour
{
    public Vector2 Move { get; private set; }  // read-only 

    private UIDocument doc;

    private bool holdUp, holdDown, holdLeft, holdRight;

    private void Awake()
    {
        doc = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        var root = doc.rootVisualElement;

        var up = root.Q<Button>("ForwardsButton");
        var down = root.Q<Button>("BackwardsButton");
        var left = root.Q<Button>("LeftwardsButton");
        var right = root.Q<Button>("RightwardsButton");

        if (up == null || down == null || left == null || right == null)
        {
            Debug.LogError("VirtualMoveInput: Missing one or more buttons. Check UXML 'name' fields.");
            enabled = false;
            return;
        }

        BindHold(up, () => holdUp = true, () => holdUp = false);
        BindHold(down, () => holdDown = true, () => holdDown = false);
        BindHold(left, () => holdLeft = true, () => holdLeft = false);
        BindHold(right, () => holdRight = true, () => holdRight = false);
    }

    private void Update()
    {
        float x = (holdRight ? 1f : 0f) - (holdLeft ? 1f : 0f);
        float y = (holdUp ? 1f : 0f) - (holdDown ? 1f : 0f);

        Vector2 v = new Vector2(x, y);
        if (v.sqrMagnitude > 1f) v.Normalize();

        Move = v;
    }

    private static void BindHold(Button button, System.Action down, System.Action up)
    {
        button.RegisterCallback<PointerDownEvent>(evt =>
        {
            down?.Invoke();
            button.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        });

        button.RegisterCallback<PointerUpEvent>(evt =>
        {
            up?.Invoke();
            if (button.HasPointerCapture(evt.pointerId))
                button.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        });

        button.RegisterCallback<PointerCancelEvent>(evt =>
        {
            up?.Invoke();
            if (button.HasPointerCapture(evt.pointerId))
                button.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        });
    }
}