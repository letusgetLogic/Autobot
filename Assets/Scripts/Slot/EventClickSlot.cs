using UnityEngine;
using UnityEngine.EventSystems;

public class EventClickSlot : MonoBehaviour, IPointerClickHandler
{
    private Slot slot;

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
        if (GameManager.Instance.IsBlockingInput)
            return;

        // The slot is empty.
        if (slot.Unit() == null)
        {
            var attached = PhaseShopController.Instance.AttachedGameObject;
            if (attached == null)
                return;

            bool isAttachedUnit = attached.CompareTag("Unit");

            // Transports unit per click, only to slot team and slot charge.
            if (isAttachedUnit && slot.IsDroppable)
            {
                var controller = attached.GetComponent<UnitController>();
                PhaseShopController.Instance.ManageAttachedUnit(controller, slot, null);
                PhaseShopController.Instance.SetAttachedGameObject(null);
            }
        }
        else // An unit is on the slot, switch attached to it.
        {
            PhaseShopController.Instance.SwitchAttached(slot.Unit(), slot.UnitController().Model);
        }
    }
}
