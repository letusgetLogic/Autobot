using UnityEngine;
using UnityEngine.EventSystems;

public class EventDropSlotTeam : MonoBehaviour, IDropHandler
{
    private Slot slot { get; set; }
    private InputKey inputKey
    {
        get
        {
            if (slot.CompareTag("Slot Team"))
                return InputKey.DropSlotTeam;

            if (slot.CompareTag("Slot Charge"))
                return InputKey.DropSlotCharge;

            if (slot.CompareTag("Slot Random"))
                return InputKey.DropSlotTeamRandom;

            return InputKey.None;
        }
    }
    private void OnEnable()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    /// <summary>
    /// Mouse was released and hitted the collider.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        if (InputManager.Instance.IsBlockingInput(inputKey))
            return;

        if (eventData.pointerDrag == null)
            return;

        if (PhaseShopController.Instance.IsBlockingDropByItemRandomness(slot))
            return;

        var attached = PhaseShopController.Instance.AttachedController;
        if (attached == null || attached == slot.UnitController())
            return;

        // Prevent dropping items into charge slots
        if (slot.CompareTag("Slot Charge"))
        {
            if (attached.Model.Data.UnitType == UnitType.Item)
                return;
        }

        // EndDrag set later blocking input = false.
        InputManager.Instance.BlocksInput = true;

        PhaseShopController.Instance.ManageAttachedUnit(attached, slot, slot.UnitController());
        PhaseShopController.Instance.SetAttachedGameObject(null);
        PhaseShopController.Instance.SetItemRandomnessInactive();
    }
}
