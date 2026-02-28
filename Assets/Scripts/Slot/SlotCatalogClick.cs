using UnityEngine;
using UnityEngine.EventSystems;

public class SlotCatalogClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Slot slot;

    /// <summary>
    /// Clicks on the slot to set the attached game object to the unit on the slot,
    /// and shows its description on the catalog.
    /// </summary>
    /// <param name="eventData"></param>
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Catalog.Instance.SetAttachedGameObject(slot.UnitController());
        EventManager.Instance.OnAttachedUnitCatalog?.Invoke(slot.UnitController());
    }
}
