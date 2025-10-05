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

        //Debug.Log("PointerDown");

        var go = slot.GameObjectIsOnMe;

        if (go == null)
            return;

        PhaseShopUnitManager.Instance.AttachedGameObject = go;
        
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
        Debug.Log(eventData.pointerDrag);
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
}