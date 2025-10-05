using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PointerEventData Data { get; private set; }
    public UnityAction OnMouseOverEvent;
    public UnityAction OnMouseExitEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Data = eventData;
        OnMouseOverEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Data = eventData;
        OnMouseExitEvent?.Invoke();
    }
}
