using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventDrop : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
    public UnityAction OnDropIsOccupied;
    public bool IsOccupied { get; private set; } = false;
    private Slot slot { get; set; }

    private void Start()
    {
        slot = transform.parent.GetComponent<Slot>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        PhaseShopUnitManager.Instance.IsCheckingAttachedToDrop = false;

        var go = PhaseShopUnitManager.Instance.AttachedGameObject;

        if (go == null || !go.GetComponent<UnitView>().DragSpriteRenderer.
            gameObject.CompareTag("Dropable"))
            return;

        if (IsOccupied)
            OnDropIsOccupied?.Invoke();
        else
        {
            PhaseShopUnitManager.Instance.TransportAttachedTo(transform.parent);
            IsOccupied = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var go = PhaseShopUnitManager.Instance.AttachedGameObject;

        if (go == null || !go.GetComponent<UnitView>().DragSpriteRenderer.
            gameObject.CompareTag("Dropable"))
            return;

        PhaseShopUnitManager.Instance.IsCheckingAttachedToDrop = true;

        slot.Border.enabled = true;
    }
}
