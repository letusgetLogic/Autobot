using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Sprite Sprite;

    public TextMeshProUGUI 
        Name,
        Description,
        Cost,
        Health,
        Damage;

    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
