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

        GameManager.Instance.IsBlockingInput = true;

        if (eventData.pointerDrag == null)
            return;

        PhaseShopUnitManager.Instance.ManageAttachedObject(slot);
        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);

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
