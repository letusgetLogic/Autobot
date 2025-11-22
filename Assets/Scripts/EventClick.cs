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

            var controller = slot.UnitController();
            var state = controller.Model.Data.UnitState;
            PhaseShopUI.Instance.SetButtonActive(state);

            if (controller.Model.Data.Durability < PackManager.Instance.MyPack.
                CurrencyData.HealthPortion)
            {
                PhaseShopUI.Instance.SetRepairButtonActiv();
            }
        }
    }
}
