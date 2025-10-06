using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventHoverSlotBattle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private float offsetMoveOther = 0.3f;
    private Slot slot { get; set; }
    private Camera main { get; set; }
    private Coroutine couroutine { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
        main = Camera.main;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var goOnDrag = PhaseShopUnitManager.Instance.AttachedGameObject;

        if (goOnDrag == null || !goOnDrag.GetComponent<UnitView>().DragSpriteRenderer.
            gameObject.CompareTag("Dropable"))
            return;

        PhaseShopUnitManager.Instance.IsCheckingAttachedToDrop = true;

        slot.Border.enabled = true;
    }

    private void OnMouseOver()
    {
        if (PhaseShopUnitManager.Instance.AttachedGameObject == null)
            return;

        var goOnDrag = PhaseShopUnitManager.Instance.AttachedGameObject;
        Debug.Log(goOnDrag);
        if (goOnDrag.transform.parent == null ||
            !goOnDrag.transform.parent.CompareTag("Slot Battle"))
            return;

        if (slot.GameObjectIsOnMe == null)
            return;

        int direction = DirectionMoveOther();
        if (direction == 0)
            return;

        bool isFusible = PhaseShopUnitManager.Instance.IsFusible(
            slot.GameObjectIsOnMe, goOnDrag.gameObject);

        SetParentNull(goOnDrag);

        if (isFusible)
        {
            if(couroutine==null)
                couroutine = StartCoroutine(DelayPushing());
        }
        else
        {
            PhaseShopUnitManager.Instance.PushOtherAway(slot.Index, direction);
        }
    }

    /// <summary>
    /// Set the parent of dragged object null to make an empty space for pushing units.
    /// </summary>
    /// <param name="goOnDrag"></param>
    private void SetParentNull(GameObject goOnDrag)
    {
        goOnDrag.transform.parent.GetComponent<Slot>().GameObjectIsOnMe = null;
        goOnDrag.transform.SetParent(null);
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

    public void OnPointerExit(PointerEventData eventData)
    {
        PhaseShopUnitManager.Instance.IsCheckingAttachedToDrop = false; 
    }
}
