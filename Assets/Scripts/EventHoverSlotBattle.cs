using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventHoverSlotBattle : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private float offsetMoveOther = 0.3f;
    private Slot slot { get; set; }
    private Camera main { get; set; }
    private Collider2D collider2D;

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
        main = Camera.main;
        collider2D = GetComponent<Collider2D>();
    }
    void Update()
    {
        Vector3 mousePosition = main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);

            if (collider2D == targetObject)
            {
                OnMouseOverCollider();
            }
        }
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

    private void OnMouseOverCollider()
    {
        var goOnDrag = PhaseShopUnitManager.Instance.AttachedGameObject;

        if (goOnDrag == null || !goOnDrag.GetComponent<UnitView>().DragSpriteRenderer.
            gameObject.CompareTag("Dropable"))
            return;

        if (slot.GameObjectIsOnMe == null)
            return;

        if (PhaseShopUnitManager.Instance.IsFusible(slot.GameObjectIsOnMe, goOnDrag.gameObject))
        {
            StartCoroutine(DelayMoving());
        }
        else
        {
            PhaseShopUnitManager.Instance.PushOtherAway(slot.Index, DirectionMoveOther());
        }
    }

    private IEnumerator DelayMoving()
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

        if (slot.transform.position.x < worldPosition.x)
        {
            return -1;
        }
        else if (slot.transform.position.x > worldPosition.x)
        {
            return 1;
        }

        return 0;
    }
}
