using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventHoverSlotBattle : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private float offsetMoveOther = 0.3f;

    private Slot slot { get; set; }
    private Coroutine couroutine { get; set; }

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
        if (PhaseShopUnitManager.Instance.AttachedGameObject == null ||
            PhaseShopUnitManager.Instance.AttachedGameObject.
            GetComponent<UnitController>().Model.ManageState == UnitState.Freezed)
            return;

        if (slot.Unit() == null ||
            slot.Unit() == PhaseShopUnitManager.Instance.AttachedGameObject)
            return;

        if (couroutine == null)
            couroutine = StartCoroutine(DelayPushing());
    }

    /// <summary>
    /// Delays pushing.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayPushing()
    {
        bool isFusible = PhaseShopUnitManager.Instance.IsFusible(
            slot.Unit(), PhaseShopUnitManager.Instance.AttachedGameObject);

        yield return new WaitForSeconds(isFusible ?
            PhaseShopUnitManager.Instance.DelayPushingFusion :
            PhaseShopUnitManager.Instance.DelayPushing);

        PhaseShopUnitManager.Instance.PushOtherAway(slot.Index, DirectionMoveOther());
        couroutine = null;
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
