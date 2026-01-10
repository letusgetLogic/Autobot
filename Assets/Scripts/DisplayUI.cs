using UnityEngine;

[RequireComponent(typeof(EventHover))]
public class DisplayUI : MonoBehaviour
{
    [SerializeField] private GameObject _textDisplay;

    private void OnEnable()
    {
        GetComponent<EventHover>().OnMouseOverEvent += () => _textDisplay.SetActive(true);
        GetComponent<EventHover>().OnMouseExitEvent += () => _textDisplay.SetActive(false);
    }

    private void OnDisable()
    {
        GetComponent<EventHover>().OnMouseOverEvent -= () => _textDisplay.SetActive(true);
        GetComponent<EventHover>().OnMouseExitEvent -= () => _textDisplay.SetActive(false);
    }
}

