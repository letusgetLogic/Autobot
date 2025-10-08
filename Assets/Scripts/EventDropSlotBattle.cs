using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventDropSlotBattle : MonoBehaviour, IDropHandler
{
    public UnityAction OnDropWhileOccupied { get; set; }
   
    private Slot slot { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        var draggingUnit = eventData.pointerDrag.transform.parent.
            GetComponent<Slot>().Unit();

        if (slot.Unit() != null)
            PhaseShopUnitManager.Instance.IsFusible(slot.Unit(), draggingUnit);
        else
        {
            PhaseShopUnitManager.Instance.Transport(draggingUnit, transform.parent, true, true);
            draggingUnit.GetComponent<UnitController>().Model.ManageState = UnitState.InSlotBattle;
        }

        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
    }
}
