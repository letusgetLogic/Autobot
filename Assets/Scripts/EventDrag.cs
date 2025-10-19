using UnityEngine;
using UnityEngine.EventSystems;

public class EventDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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

        if (slot.UnitController() == null ||
            slot.UnitController().Model.ManageState == UnitState.Freezed)
            return;

        if (slot.UnitView() == null)
            return;

        PhaseShopUnitManager.Instance.IsDragging = true;

        slot.UnitView().BeingAttached(eventData);

        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        PhaseShopUnitManager.Instance.SetAttachedGameObject(
            eventData.pointerDrag.transform.parent.GetComponent<Slot>().Unit());
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PhaseShopUnitManager.Instance.IsDragging == false)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;
       
        if (slot.UnitController() == null ||
            slot.UnitController().Model.ManageState == UnitState.Freezed)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().GetComponent<UnitView>().BeingMovedOnMouse(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitController() == null ||
            slot.UnitController().Model.ManageState == UnitState.Freezed)
            return;

        if (slot.UnitView() == null)
            return;

        PhaseShopUnitManager.Instance.IsDragging = false;

        slot.UnitView().BeingReleased(eventData);
        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
    }
}