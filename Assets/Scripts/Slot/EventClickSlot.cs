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
            PhaseShopController.Instance.SwitchAttached(slot.UnitController());
        }
    }
}
