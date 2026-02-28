using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _textDisplay;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _textDisplay.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _textDisplay.SetActive(false);
    }
}

