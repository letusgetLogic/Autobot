using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventDropSlotTeam : MonoBehaviour, IDropHandler
{
    private Slot slot { get; set; }

    private void Start()
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

        var attached = PhaseShopController.Instance.AttachedGameObject;
        if (attached == null || attached == slot.Unit())
            return;

        GameManager.Instance.IsBlockingInput = true;

        bool isAttachedUnit = attached.CompareTag("Unit");

        if (isAttachedUnit)
        {
            var controller = attached.GetComponent<UnitController>();
            PhaseShopController.Instance.ManageAttachedUnit(controller, slot, slot.UnitController());
            PhaseShopController.Instance.SetAttachedGameObject(null);
        }

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
