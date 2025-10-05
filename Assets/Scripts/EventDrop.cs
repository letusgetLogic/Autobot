using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
    public UnityAction OnDropWhileOccupied { get; set; }
   
    private Slot slot { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        PhaseShopUnitManager.Instance.IsCheckingAttachedToDrop = false;

        var goOnDrag = PhaseShopUnitManager.Instance.AttachedGameObject;

        if (goOnDrag == null || !goOnDrag.GetComponent<UnitView>().DragSpriteRenderer.
            gameObject.CompareTag("Dropable"))
            return;

        if (slot.GameObjectIsOnMe != null)
            PhaseShopUnitManager.Instance.IsFusible(slot.GameObjectIsOnMe, goOnDrag.gameObject);
        else
        {
            PhaseShopUnitManager.Instance.TransportAttachedTo(transform.parent);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var go = PhaseShopUnitManager.Instance.AttachedGameObject;

        if (go == null || !go.GetComponent<UnitView>().DragSpriteRenderer.
            gameObject.CompareTag("Dropable"))
            return;

        PhaseShopUnitManager.Instance.IsCheckingAttachedToDrop = true;

        slot.Border.enabled = true;
    }
}
