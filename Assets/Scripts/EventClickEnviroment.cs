using UnityEngine;
using UnityEngine.EventSystems;

public class EventClickEnviroment : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        PhaseShopUnitManager.Instance.SetAttachedGameObject(null);
    }
}
