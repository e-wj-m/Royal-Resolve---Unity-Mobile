using UnityEngine;
using UnityEngine.EventSystems;
using FMODUnity;

public class UIButtonSoundManager : MonoBehaviour, IPointerClickHandler
{
    [Header("FMod Events")]
    [SerializeField] private EventReference clickEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        RuntimeManager.PlayOneShot(clickEvent, transform.position);
    }
}
