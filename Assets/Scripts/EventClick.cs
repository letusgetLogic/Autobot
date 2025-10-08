using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerClickHandler
{
    private Slot slot;

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot.Unit() == null)
            return;

        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        PhaseShopUnitManager.Instance.SetAttachedGameObject(slot.Unit());

        var state = slot.UnitController().Model.ManageState;
        PhaseShopUI.Instance.SetButtonActive(state);
    }
}
