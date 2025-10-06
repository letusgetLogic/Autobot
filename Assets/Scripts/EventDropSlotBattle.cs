using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventDropSlotBattle : MonoBehaviour, IDropHandler
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
            PhaseShopUnitManager.Instance.Transport(goOnDrag, transform.parent, true);
        }

        PhaseShopUnitManager.Instance.AttachedGameObject = null;
    }
}
