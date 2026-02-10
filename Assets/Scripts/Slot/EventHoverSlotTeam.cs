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

    private UnitController unitOnSlot { get; set; }
    private UnitController unitDragged { get; set; }
    private Slot draggedSlot { get; set; }


    private void OnEnable()
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

        if (PhaseShopController.Instance.IsBlockingInputsByItemRandomness(slot)) 
            return;

        var attached = PhaseShopController.Instance.AttachedController;
        var dragged = eventData.pointerDrag;

        if (IsInteractable(attached, dragged))
        {
            slot.SetIndicatorActive(true);
        }
    }

    /// <summary>
    /// Is the hovered slot interactable?
    /// </summary>
    /// <param name="_attached"></param>
    /// <param name="_dragged"></param>
    /// <returns></returns>
    private bool IsInteractable(UnitController _attached, GameObject _dragged)
    {
        if (_attached == null)
            return false;

        if (_attached.Model.IsInShop())
        {
            if (PhaseShopUI.Instance.HasEnoughCurrency(
                   _attached.Model.Cost.Nut, _attached.Model.Cost.Tool, false) == false)
            return false;

            if (_attached.Model.IsItemDoRandomness)
            {
                if (slot.CompareTag("Slot Random"))
                    return true;
            }
        }

        //  attached is clicked and slot is empty
        if (_attached.IsRobot(_attached.Model.SoUnit.UnitType) && slot.Unit() == null)
            return true;

        if (_dragged == null || _dragged.CompareTag("Unit") == false)
            return false;

        var controller = _dragged.GetComponent<UnitController>();
        if (controller == null)
            return false;

        // dragged is a robot
        if (controller.IsRobot(controller.Model.SoUnit.UnitType))
            return true;

        // dragged is an item and slot is team slot
        if (controller.CanItemBeDropped())
            return true;

        return false;
    }

    private void OnMouseOver()
    {
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (PhaseShopController.Instance.IsBlockingInputsByItemRandomness(slot))
            return;

        if (PhaseShopController.Instance.IsDragging && CanPushOther())
        {
            if (!isCounting)
            {
                //Initialize references
                unitOnSlot = slot.UnitController();
                unitDragged = PhaseShopController.Instance.AttachedController;
                draggedSlot = unitDragged.GetComponent<UnitController>().Slot;

                // Check if unit on slot is fusible with attached.
                bool isFusible = PhaseShopController.Instance.IsFusible(
                    slot.UnitController(),
                    unitDragged);

                // Set countdown based on fusibility.
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
                    unitOnSlot, draggedSlot.transform,
                    unitDragged, slot.transform));

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
        var attached = PhaseShopController.Instance.AttachedController;

        // if attached game object is null, return false.
        if (attached == null)
            return false;

        // if no game object is on the slot or it is self, return false.
        if (slot.UnitController() == null ||
            slot.UnitController() == attached)
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
