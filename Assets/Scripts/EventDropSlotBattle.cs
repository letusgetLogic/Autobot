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
        if (PhaseShopUnitManager.Instance.StopDragging)
            return;

        if (eventData.pointerDrag == null)
            return;

        var draggingUnit = eventData.pointerDrag.transform.parent.GetComponent<Slot>().Unit();
        if (draggingUnit == null)
            return;

        var draggingController = draggingUnit.GetComponent<UnitController>();
        var draggingModel = draggingController.Model;

        var unitOnSlot = slot.UnitController();

        if (draggingModel.UnitState == UnitState.InSlotShop)
        {
            int cost = draggingController.Data.Cost.Value;
            if (PhaseShopUI.Instance.Player.Data.Coins < cost) // case: buy but not enough coins.
            {
                PhaseShopUI.Instance.HintNotEnoughCoins();
                return;
            }

            if (unitOnSlot != null) 
            {
                if (PhaseShopUnitManager.Instance.IsFusible(unitOnSlot, draggingController)) // case: buy, units are fusible.
                {
                    PhaseShopUI.Instance.UpdateCoin(-draggingController.Data.Cost.Value);
                    unitOnSlot.UpdateLevel(draggingModel, true);
                    Destroy(draggingUnit);
                }
            }
            else // case: buy and place dragging unit on empty slot.
            {
                PhaseShopUI.Instance.UpdateCoin(-draggingController.Data.Cost.Value);
                PhaseShopUnitManager.Instance.Transport(draggingUnit, transform.parent, true, true);
            }
        }
        else if (draggingModel.UnitState == UnitState.InSlotBattle)
        {
            if (unitOnSlot != null)
            {
                if (PhaseShopUnitManager.Instance.IsFusible(unitOnSlot, draggingController)) // case: only fusion.
                {
                    unitOnSlot.UpdateLevel(draggingModel, true);
                    Destroy(draggingUnit);
                }
            }
            else // case: move dragging unit to empty slot.
            {
                PhaseShopUnitManager.Instance.Transport(draggingUnit, transform.parent, true, true);
            }
        }

        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
    }
}
