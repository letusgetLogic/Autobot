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
        if (draggingUnit == null) 
            return;

        var draggingController = draggingUnit.GetComponent<UnitController>();
        var draggingModel = draggingController.Model;

        var unitOnSlot = slot.UnitController();

        if (PhaseShopUI.Instance.Player.Coins <= 0)
        {
            PhaseShopUI.Instance.HintNotEnoughCoins();
            return;
        }

        if (unitOnSlot != null)
        {
            if (PhaseShopUnitManager.Instance.IsFusible(unitOnSlot, draggingController))
            {
                if (draggingModel.ManageState == UnitState.InSlotShop)
                    PhaseShopUI.Instance.UpdateCoin(-draggingModel.Data.Cost);

                unitOnSlot.UpdateLevel(
                    draggingModel.XP,
                    draggingModel.BattleHealth - draggingModel.Data.Health,
                    draggingModel.BattleAttack - draggingModel.Data.Attack
                    );

                Destroy(draggingUnit);
            }
        }
        else
        {
            if (draggingModel.ManageState == UnitState.InSlotShop)
                PhaseShopUI.Instance.UpdateCoin(-draggingModel.Data.Cost);

            PhaseShopUnitManager.Instance.Transport(draggingUnit, transform.parent, true, true);
        }
       
        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
    }
}
