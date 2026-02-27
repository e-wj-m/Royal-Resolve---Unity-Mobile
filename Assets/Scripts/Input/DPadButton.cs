using UnityEngine;
using UnityEngine.EventSystems;

public class DPadButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private SimplePlayerMovement player;
    [SerializeField] private Vector2 direction; // set in Inspector

    public void OnPointerDown(PointerEventData eventData)
    {
        if (player != null)
            player.SetMoveDir(direction);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (player != null)
            player.SetMoveDir(Vector2.zero);
    }
}