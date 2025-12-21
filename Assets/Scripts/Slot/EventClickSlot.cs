using UnityEngine;
using UnityEngine.EventSystems;

public class EventClickSlot : MonoBehaviour, IPointerClickHandler
{
    private Slot slot;

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }

    /// <summary>
    /// Button click calls.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (PhaseShopUnitManager.Instance.IsBlockingInput)
            return;

        if (slot.Unit() == null)
        {
            var attached = PhaseShopUnitManager.Instance.AttachedGameObject;

            // Transports unit per click, only to slot team.
            if (attached != null && slot.CompareTag("Slot Team"))
            {
                PhaseShopUnitManager.Instance.ManageAttachedObject(slot);
                PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
            }
        }
        else // An unit is on the slot.
        {
            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
            PhaseShopUnitManager.Instance.SetAttachedGameObject(slot.Unit());

            var model = slot.UnitController().Model;
            PhaseShopUI.Instance.SetButtonActive(model);
        }
    }
}
