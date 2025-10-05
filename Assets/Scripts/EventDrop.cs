using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventDrop : MonoBehaviour, IDropHandler
{
    public UnityAction OnDropIsOccupied;
    public bool IsOccupied { get; private set; } = false;

    public void OnDrop(PointerEventData eventData)
    {
        var go = eventData.pointerDrag;

        if (!go.CompareTag("Dropable"))
            return;

        if (IsOccupied)
            OnDropIsOccupied?.Invoke();
        else
        {
            go.transform.parent.SetParent(transform, false);
            IsOccupied = true;
        }
    }
}
