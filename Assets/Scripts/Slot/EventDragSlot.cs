using UnityEngine;
using UnityEngine.EventSystems;

public class EventDragSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Slot slot { get; set; }
    

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitController() == null)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().BeingAttached(eventData);

        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        PhaseShopUnitManager.Instance.SetAttachedGameObject(slot.Unit());
        PhaseShopUI.Instance.SetButtonActive(slot.UnitController().Model.Data);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PhaseShopUnitManager.Instance.PreventDragging)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;
       
        if (slot.UnitView() == null)
            return;

        PhaseShopUnitManager.Instance.IsDragging = true;
        slot.UnitView().BeingMovedOnMouse(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PhaseShopUnitManager.Instance.PreventDragging = false;
        PhaseShopUnitManager.Instance.IsDragging = false;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().BeingReleased(eventData);
        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
    }
}