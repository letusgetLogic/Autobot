using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class UnitMouseEvent : MonoBehaviour
{
    public UnityAction OnMouseOverEvent;
    public UnityAction OnMouseExitEvent;

    public void OnMouseOver()
    {
        OnMouseOverEvent?.Invoke();
    }

    public void OnMouseExit()
    {
        OnMouseExitEvent?.Invoke();
    }
}
