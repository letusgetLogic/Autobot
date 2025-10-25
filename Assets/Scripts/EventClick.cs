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
            //var attached = PhaseShopUnitManager.Instance.AttachedGameObject;

            //// Transports unit per click, only to slot battle.
            //if (attached != null && slot.CompareTag("Slot Shop") == false)
            //{
            //    if (PhaseShopUI.Instance.Player.Coins <= 0)
            //    {
            //        PhaseShopUI.Instance.HintNotEnoughCoins();
            //        return;
            //    }

            //    PhaseShopUI.Instance.UpdateCoin(-attached.GetComponent<UnitController>().Model.Data.Cost);

            //    PhaseShopUnitManager.Instance.Transport(
            //        PhaseShopUnitManager.Instance.AttachedGameObject,
            //        slot.transform,
            //        true,
            //        true);

            //    PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
            //}
        }
        else
        {
            PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
            PhaseShopUnitManager.Instance.SetAttachedGameObject(slot.Unit());

            var state = slot.UnitController().Model.UnitState;
            PhaseShopUI.Instance.SetButtonActive(state);
        }
    }
}
