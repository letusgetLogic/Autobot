using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventHoverSlotBattle : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private float offsetMoveOther = 0.3f;

    private EventDrag eventDrag { get; set; }
    private Slot slot { get; set; }
    private Coroutine couroutine { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
        eventDrag = GetComponent<EventDrag>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || 
            eventData.pointerDrag.transform.parent.GetComponent<Slot>().Unit() == null)
            return;

        slot.Border.enabled = true;
    }

    private void OnMouseOver()
    {
        GameObject draggingUnit;

        if (eventDrag.Data == null ||
            eventDrag.Data.pointerDrag == null ||
            eventDrag.Data.pointerDrag.transform.parent.GetComponent<Slot>().Unit() == null)
            return;
        else
            draggingUnit = eventDrag.Data.pointerDrag.transform.parent.GetComponent<Slot>().Unit();

        if (slot.Unit() == null||
            slot.Unit() == draggingUnit)
            return;

        if (PhaseShopUnitManager.Instance.IsFusible(slot.Unit(), draggingUnit))
        {
            if (couroutine == null)
                couroutine = StartCoroutine(DelayPushing());
        }
        else
        {
            PhaseShopUnitManager.Instance.PushOtherAway(slot.Index, DirectionMoveOther());
        }
    }

    /// <summary>
    /// Delays pushing.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayPushing()
    {
        yield return new WaitForSeconds(PhaseShopUnitManager.Instance.DelayPushing);

        PhaseShopUnitManager.Instance.PushOtherAway(slot.Index, DirectionMoveOther());
    }

    /// <summary>
    /// Returns an integer, which represents the direction to push other away.
    /// </summary>
    /// <returns></returns>
    private int DirectionMoveOther()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if (worldPosition.x - offsetMoveOther > slot.transform.position.x)
        {
            return 1;
        }
        else if (worldPosition.x + offsetMoveOther < slot.transform.position.x)
        {
            return -1;
        }

        return 0;
    }
}
