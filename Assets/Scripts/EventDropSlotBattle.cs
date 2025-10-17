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

        var draggingUnit = eventData.pointerDrag.transform.parent.GetComponent<Slot>().Unit();
        var draggingUnitController = draggingUnit.GetComponent<UnitController>();
        var model = draggingUnitController.Model;

        var unitOnSlot = slot.UnitController();
        if (unitOnSlot != null)
        {
            if (model.ManageState == UnitState.InSlotShop)
                PhaseShopUI.Instance.UpdateCoin(-model.Data.Cost);

            if (PhaseShopUnitManager.Instance.IsFusible(unitOnSlot, draggingUnitController))
            {
                unitOnSlot.UpdateLevel(
                    model.XP,
                    model.BattleHealth - model.Data.Health,
                    model.BattleAttack - model.Data.Attack
                    );

                Destroy(draggingUnit);
            }
        }
        else
        {
            if (model.ManageState == UnitState.InSlotShop)
                PhaseShopUI.Instance.UpdateCoin(-model.Data.Cost);

            PhaseShopUnitManager.Instance.Transport(draggingUnit, transform.parent, true, true);
        }
       
        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
    }
}
