using UnityEngine;
using UnityEngine.EventSystems;

public class EventDragSlotBattle : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Slot slot { get; set; }
    private EventHover eventHover { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
        eventHover = slot.EventHover;
    }

    private void OnEnable()
    {
        eventHover.OnMouseExitEvent += SetAttached;
    }

    private void OnDisable()
    {
        eventHover.OnMouseExitEvent -= SetAttached;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //Debug.Log("PointerDown");

        var go = slot.GameObjectIsOnMe;

        if (go == null)
            return;

        PhaseShopUnitManager.Instance.AttachedGameObject = go;
        slot.GameObjectIsOnMe.transform.SetParent(null, false); 
        slot.GameObjectIsOnMe = null;

        if (go.CompareTag("Unit"))
        {
            go.GetComponent<UnitView>().BeingAttached(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //Debug.Log("Drag");

        var go = slot.GameObjectIsOnMe;

        if (go == null)
            return;

        if (go.CompareTag("Unit"))
        {
            go.GetComponent<UnitView>().BeingMovedOnMouse(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        //Debug.Log("PointerUp");

        var go = slot.GameObjectIsOnMe;

        if (go == null)
            return;

        if (go.CompareTag("Unit"))
        {
            go.GetComponent<UnitView>().BeingReleased(eventData);
        }

        if (PhaseShopUnitManager.Instance.IsCheckingAttachedToDrop == true)
            return;

        PhaseShopUnitManager.Instance.AttachedGameObject = null;
    }

    private void SetAttached()
    {
        if(eventHover.Data == null ||  eventHover.Data.pointerDrag == null)
            return;

        PhaseShopUnitManager.Instance.AttachedGameObject = slot.GameObjectIsOnMe;
    }
}