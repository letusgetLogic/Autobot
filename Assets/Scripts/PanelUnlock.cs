using UnityEngine;
using UnityEngine.EventSystems;

public class PanelUnlock : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        PhaseShopUI.Instance.SetUnlockedTier(false, 0);
    }
}
