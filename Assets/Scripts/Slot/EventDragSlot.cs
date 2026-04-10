using UnityEngine;
using UnityEngine.EventSystems;

public class EventDragSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Slot slot { get; set; }

    private InputKey inputKey
    {
        get
        {
            if (slot.CompareTag("Slot Team"))
                return InputKey.DragSlotTeam;

            if (slot.CompareTag("Slot Charge"))
                return InputKey.DragSlotCharge;

            if (slot.CompareTag("Slot Shop"))
            {
                var unit = slot.UnitController();
                if (unit && unit.IsRobot(unit.Model.SoUnit.UnitType))
                    return InputKey.DragSlotShopBot;
                else
                    return InputKey.DragSlotShopItem;
            }

            return InputKey.None;
        }
    }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    /// <summary>
    /// Beginning of dragging object calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InputManager.Instance.IsBlockingInput(inputKey))
            return;

        if (PhaseShopController.Instance.IsSwapping)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitController() == null)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().BeingAttached(eventData);

        PhaseShopController.Instance.SetAttachedGameObject(slot.UnitController());
    }

    /// <summary>
    /// Dragging objects calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (InputManager.Instance.IsBlockingInput(inputKey))
            return;

        if (PhaseShopController.Instance.IsSwapping)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitView() == null)
            return;

        PhaseShopController.Instance.IsDragging = true;
        slot.UnitView().BeingMovedOnMouse(eventData);
    }

    /// <summary>
    /// Ending of dragging object calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (PhaseShopController.Instance.IsDragging == false)
            return;

        if (PhaseShopController.Instance.IsSwapping)
            return;

        InputManager.Instance.BlocksInput = false;
        PhaseShopController.Instance.IsDragging = false;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().BeingReleased(eventData);

        PhaseShopController.Instance.SetAttachedGameObject(null);
    }
}