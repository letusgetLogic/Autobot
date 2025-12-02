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
        if (PhaseShopUnitManager.Instance.AttachedGameObject == null &&
            (eventData.pointerDrag == null ||
            eventData.pointerDrag.transform.parent.GetComponent<Slot>().Unit() == null))
            return;

        slot.Border.enabled = true;
    }

    /// <summary>
    /// Calls, whether mouse is in the collider.
    /// </summary>
    private void OnMouseOver()
    {
        if (CanPushOther())
        {
            if (!isCounting)
            {
                var attached = PhaseShopUnitManager.Instance.AttachedGameObject;
                bool isFusible = PhaseShopUnitManager.Instance.IsFusible(
                slot.UnitController(),
                attached.GetComponent<UnitController>());

                countDown = isFusible ?
                PhaseShopUnitManager.Instance.DelayPushingFusion :
                PhaseShopUnitManager.Instance.DelayPushing;

                direction = DirectionMoveOther();
                isCounting = true;
            }

            if (isCounting && DirectionMoveOther() == direction)
                countDown -= Time.deltaTime;
            else
                isCounting = false;

            if (countDown <= 0)
            {
                PhaseShopUnitManager.Instance.PushOtherAway(slot.Index, DirectionMoveOther());
                isCounting = false;
            }
        }

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
        var attached = PhaseShopUnitManager.Instance.AttachedGameObject;

        // if attached game object is null, return false.
        if (attached == null)
            return false;

        // if no game object is on the slot or it is self, return false.
        if (slot.Unit() == null ||
            slot.Unit() == attached)
            return false;

        if (attached.CompareTag("Unit"))
        {
            var unitModel = attached.GetComponent<UnitController>().Model;

            // if the unit is in the shop, freezed, or in the charge slot, return true.
            if (unitModel.Data.UnitState == UnitState.InSlotShop ||
                unitModel.Data.UnitState == UnitState.Freezed ||
                unitModel.Data.UnitState == UnitState.InSlotCharge)
            {
                return true;
            }

            // if the unit is in the team slot and only is dragging, return true.
            if (unitModel.Data.UnitState == UnitState.InSlotTeam &&
                PhaseShopUnitManager.Instance.IsDragging)
            {
                return true;
            }
        }

        return false;
    }
}
