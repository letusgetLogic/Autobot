using UnityEngine;
using UnityEngine.EventSystems;

public class EventDragSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Slot slot { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    /// <summary>
    /// Beginning of dragging object calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitController() == null)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().BeingAttached(eventData);

        PhaseShopController.Instance.SwitchAttached(slot.Unit(), slot.UnitController().Model);
    }

    /// <summary>
    /// Dragging objects calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;
       
        if (slot.UnitView() == null)
            return;

        PhaseShopController.Instance.IsDragging = true;
        slot.UnitView().BeingMovedOnMouse(eventData);
    }

    /// <summary>
    /// Ending of dragging object calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        GameManager.Instance.IsBlockingInput = false;

        if (PhaseShopController.Instance.IsDragging == false)
            return;

        PhaseShopController.Instance.IsDragging = false;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().BeingReleased(eventData);
        PhaseShopController.Instance.SetAttachedGameObject(null);
    }
}