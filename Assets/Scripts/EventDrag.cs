using UnityEngine;
using UnityEngine.EventSystems;

public class EventDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Slot slot { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //var go = slot.GameObjectIsOnMe;

        if (slot.UnitView() == null)
            return;

        //PhaseShopUnitManager.Instance.AttachedGameObject = go;

        slot.UnitView().BeingAttached(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
       
        //var go = slot.GameObjectIsOnMe;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().GetComponent<UnitView>().BeingMovedOnMouse(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //var go = slot.GameObjectIsOnMe;

        if (slot.UnitView() == null)
            return;

        slot.UnitView().BeingReleased(eventData);

        if (PhaseShopUnitManager.Instance.IsCheckingAttachedToDrop == true)
            return;

        //PhaseShopUnitManager.Instance.AttachedGameObject = null;
       
    }
}