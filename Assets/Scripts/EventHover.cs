using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PointerEventData Data { get; private set; }
    public UnityAction OnMouseOverEvent { get; set; }
    public UnityAction OnMouseExitEvent { get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Data = eventData;
        OnMouseOverEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Data = null;
        OnMouseExitEvent?.Invoke();
    }
}
