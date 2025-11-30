using UnityEngine;
using UnityEngine.EventSystems;

public class EventDropSlotTeam : MonoBehaviour, IDropHandler
{
    private Slot slot { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (PhaseShopUnitManager.Instance.PreventDragging)
            return;

        if (eventData.pointerDrag == null)
            return;

        PhaseShopUnitManager.Instance.ManageAttachedObject(slot);
        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
    }
}
