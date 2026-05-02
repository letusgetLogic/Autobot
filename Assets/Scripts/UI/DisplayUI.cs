using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _textDisplay;

    /// <summary>
    /// OnPointerEnter method to handle pointer enter events. It activates the text display when the pointer enters the collider.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _textDisplay.SetActive(true);
    }

    /// <summary>
    /// OnPointerExit method to handle pointer exit events. It deactivates the text display when the pointer exits the collider.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        _textDisplay.SetActive(false);
    }
}

