using UnityEngine;
using UnityEngine.EventSystems;

public class EventDropButton : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        if (transform.parent.CompareTag("Repair Button"))
        {
            PhaseShopUI.Instance.Repair();
        }
        else if (transform.parent.CompareTag("Freeze Button"))
        {
            PhaseShopUI.Instance.Freeze();
        }
        else if(transform.parent.CompareTag("Unfreeze Button"))
        {
            PhaseShopUI.Instance.Unfreeze();
        }
        else if(transform.parent.CompareTag("Sell Button"))
        {
            PhaseShopUI.Instance.Sell();
        }
    }
}

