using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PointerEventData Data { get; set; }
    private Slot slot { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().BeingAttached(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().GetComponent<UnitView>().BeingMovedOnMouse(eventData);

        Data = eventData;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().BeingReleased(eventData);

        Data = null;
    }
}