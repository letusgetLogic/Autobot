using UnityEngine;
using UnityEngine.EventSystems;

public class EventDropButton : MonoBehaviour, IDropHandler
{
    /// <summary>
    /// Mouse was released and hitted the collider.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        if (transform.parent.CompareTag("Repair Button"))
        {
            PhaseShopUI.Instance.OnRepair();
        }
        else if (transform.parent.CompareTag("Recycle Button"))
        {
            PhaseShopUI.Instance.OnRecycle();
        }
        else if (transform.parent.CompareTag("Lock Button"))
        {
            PhaseShopUI.Instance.OnLock();
        }
        else if(transform.parent.CompareTag("Unlock Button"))
        {
            PhaseShopUI.Instance.OnUnlock();
        }
    }
}

