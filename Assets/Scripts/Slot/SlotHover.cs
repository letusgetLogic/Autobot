using System;
using UnityEngine;

[RequireComponent(typeof(EventHover))]
public class SlotHover : MonoBehaviour
{
    private EventHover eventHover;

    private void Start()
    {
        eventHover = GetComponent<EventHover>();
    }

    private void OnEnable()
    {
        eventHover.OnMouseOverEvent += ShowDescription;
        eventHover.OnMouseExitEvent += HideDescription;
    }

    private void OnDisable()
    {
        eventHover.OnMouseOverEvent -= ShowDescription;
        eventHover.OnMouseExitEvent -= HideDescription;
    }

    private void ShowDescription()
    {
        throw new NotImplementedException();
    }
    private void HideDescription()
    {
        throw new NotImplementedException();
    }

}
