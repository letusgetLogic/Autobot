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
        if (GameManager.Instance.IsBlockingInput)
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
            PhaseShopUnitManager.Instance.HandleMouseDown(slot.Unit(), slot.UnitController().Model);
        }
    }
}
