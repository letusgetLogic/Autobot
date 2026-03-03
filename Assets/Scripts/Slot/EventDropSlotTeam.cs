using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventDropSlotTeam : MonoBehaviour, IDropHandler
{
    private Slot slot { get; set; }

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
        if (GameManager.Instance.IsBlockingInput)
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
        GameManager.Instance.IsBlockingInput = true;

        PhaseShopController.Instance.ManageAttachedUnit(attached, slot, slot.UnitController());
        PhaseShopController.Instance.SetAttachedGameObject(null);
        PhaseShopController.Instance.SetItemRandomnessInactive();


        //StartCoroutine(DelayEnableInput()); // End drag set blocking input = false
    }

    /// <summary>
    /// Delay enabling input.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayEnableInput()
    {
        yield return new WaitForEndOfFrame();

        GameManager.Instance.IsBlockingInput = false;
    }
}
