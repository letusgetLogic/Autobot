using UnityEngine;
using UnityEngine.EventSystems;

public class EventClickSlot : MonoBehaviour, IPointerClickHandler
{
    private Slot slot;
    private InputKey inputKey
    {
        get
        {
            if (slot.CompareTag("Slot Team"))
                return InputKey.ClickSlotTeam;

            if (slot.CompareTag("Slot Charge"))
                return InputKey.ClickSlotCharge;
            
            if (slot.CompareTag("Slot Shop"))
            {
                var unit = slot.UnitController();
                if (unit && unit.IsRobot(unit.Model.SoUnit.UnitType))
                    return InputKey.ClickSlotShopBot;
                else
                    return InputKey.ClickSlotShopItem;
            }

            return InputKey.None;
        }
    }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    /// <summary>
    /// Button click calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (InputManager.Instance.IsBlockingInput(inputKey))
            return;

        var unit = slot.UnitController();

        // The slot is empty.
        if (unit == null)
        {
            var attached = PhaseShopController.Instance.AttachedController;
            if (attached == null)
                return;

            // Transports unit per click, only to slot team and slot charge.
            if (slot.IsDroppable)
            {
                PhaseShopController.Instance.ManageAttachedUnit(attached, slot, null);
                PhaseShopController.Instance.SetAttachedGameObject(null);
            }
        }
        else // An unit is on the slot, switch attached to it.
        {
            PhaseShopController.Instance.SetAttachedGameObject(unit);
        }
    }
}
