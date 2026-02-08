using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetRectLocalScaleClick : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Button button;

    [Header("Settings")]
    [SerializeField] private float scaleFactor;

    private bool isHeld = false;
    private Vector3 originalScale;

    /// <summary>
    /// Start method.
    /// </summary>
    private void OnEnable()
    {
        originalScale = GetComponent<RectTransform>().localScale;
        GameManager.Instance.IsBlockingInput = false;
    }

    /// <summary>
    /// OnPointerDown method to handle button press events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;

        if(button != null && button.interactable == false)
            return;

        GetComponent<RectTransform>().localScale = originalScale * scaleFactor;
    }

    /// <summary>
    /// OnPointerUp method to handle button release events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;
        GetComponent<RectTransform>().localScale = originalScale;
    }

    private void Update()
    {
        if (isHeld)
        {
            // Do something while the button is held.
        }
    }
}