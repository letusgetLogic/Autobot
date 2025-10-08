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

            // Transports unit per click, only to slot battle.
            if (attached != null && slot.CompareTag("Slot Shop") == false)
            {
                PhaseShopUnitManager.Instance.Transport(
                    PhaseShopUnitManager.Instance.AttachedGameObject,
                    slot.transform,
                    true,
                    true);

                PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
            }

            return;
        }

        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
        PhaseShopUnitManager.Instance.SetAttachedGameObject(slot.Unit());

        var state = slot.UnitController().Model.ManageState;
        PhaseShopUI.Instance.SetButtonActive(state);
    }
}
