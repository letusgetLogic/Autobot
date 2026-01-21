using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventHoverSlotTeam : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float offsetMoveOther = 0.3f;

    private Slot slot { get; set; }
    private float countDown { get; set; }
    private bool isCounting { get; set; }
    private int direction { get; set; }

    private GameObject unitOnSlot { get; set; }
    private GameObject unitDragged { get; set; }
    private Slot draggedSlot { get; set; }
    private bool isFusible { get; set; }


    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    /// <summary>
    /// Mouse entered the collider.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (PhaseShopController.Instance.AttachedGameObject == null &&
            (eventData.pointerDrag == null ||
            eventData.pointerDrag.transform.parent.GetComponent<Slot>().Unit() == null))
            return;

        slot.Border.enabled = true;
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (PhaseShopController.Instance.IsDragging && CanPushOther())
        {
            if (!isCounting)
            {
               unitOnSlot = slot.Unit();
               unitDragged = PhaseShopController.Instance.AttachedGameObject;
               draggedSlot = unitDragged.GetComponent<UnitController>().Slot;

               isFusible = PhaseShopController.Instance.IsFusible(
                    slot.UnitController(),
                    unitDragged.GetComponent<UnitController>());

                countDown = isFusible ?
                PhaseShopController.Instance.Process.DelayPushingFusion :
                PhaseShopController.Instance.Process.DelayPushing;

                //direction = DirectionMoveOther();
                isCounting = true;
            }

            if (isCounting/* && DirectionMoveOther() == direction*/)
                countDown -= Time.deltaTime;
            else
                isCounting = false;

            if (countDown <= 0)
            {
                //PhaseShopUnitManager.Instance.PushOtherAway(slot.Index, DirectionMoveOther());
                GameManager.Instance.IsBlockingInput = true;
                PhaseShopController.Instance.IsDragging = false;
                PhaseShopController.Instance.SetAttachedGameObject(null);

                StartCoroutine(PhaseShopController.Instance.Swap(
                    unitOnSlot.GetComponent<UnitController>(), draggedSlot.transform,
                    unitDragged.GetComponent<UnitController>(), slot.transform));

                //PhaseShopUnitManager.Instance.Transport(unitOnSlot, draggedSlot.transform, true, false);
                //PhaseShopUnitManager.Instance.Transport(unitDragged, slot.transform, true, false);

                SetDefault();

                isCounting = false;
            }
        }
    }

    private void SetDefault()
    {
        unitOnSlot = default;
        unitDragged = default;
        draggedSlot = default;
        isFusible = default;
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

    /// <summary>
    /// Mouse exited the collider.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        isCounting = false;
    }

    /// <summary>
    /// Can push other units?
    /// </summary>
    /// <returns></returns>
    private bool CanPushOther()
    {
        var attached = PhaseShopController.Instance.AttachedGameObject;

        // if attached game object is null, return false.
        if (attached == null)
            return false;

        // if no game object is on the slot or it is self, return false.
        if (slot.Unit() == null ||
            slot.Unit() == attached)
            return false;

        if (attached.CompareTag("Unit"))
        {
            var attachedModel = attached.GetComponent<UnitController>().Model;

            // if attached game object is item, return false.
            if (attachedModel.Data.UnitType == UnitType.Item)
                return false;


            // if the attached unit is in the shop or freezed slot, return false.
            if (attachedModel.Data.UnitState == UnitState.InSlotShop ||
                attachedModel.Data.UnitState == UnitState.Freezed)
            {
                return false;
            }

            // if the unit is in the team or charging slot and only is dragging, return true.
            if (attachedModel.Data.UnitState == UnitState.InSlotTeam ||
                attachedModel.Data.UnitState == UnitState.InSlotCharge &&
                PhaseShopController.Instance.IsDragging)
            {
                return true;
            }
        }

        return false;
    }
}
