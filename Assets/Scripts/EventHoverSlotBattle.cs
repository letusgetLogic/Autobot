using UnityEngine;
using UnityEngine.EventSystems;

public class EventHoverSlotBattle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PhaseShopUnitManager.Instance.AttachedGameObject == null &&
            (eventData.pointerDrag == null ||
            eventData.pointerDrag.transform.parent.GetComponent<Slot>().Unit() == null))
            return;

        slot.Border.enabled = true;
    }

    private void OnMouseOver()
    {
        var attached = PhaseShopUnitManager.Instance.AttachedGameObject;

        // if attached game object is null or it is being frezzed, return.
        if (attached == null ||
            attached.GetComponent<UnitController>().Model.Data.UnitState == 
            UnitState.Freezed)
            return;

        // if no game object is on the slot or it is self, return.
        if (slot.Unit() == null ||
            slot.Unit() == attached)
            return;

        if (!isCounting && PhaseShopUnitManager.Instance.IsDragging)
        {
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

    public void OnPointerExit(PointerEventData eventData)
    {
        isCounting = false;
    }
}
