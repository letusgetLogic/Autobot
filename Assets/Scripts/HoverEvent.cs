using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityAction OnMouseOverEvent;
    public UnityAction OnMouseExitEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseOverEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExitEvent?.Invoke();
    }
}
