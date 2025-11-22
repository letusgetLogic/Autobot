using UnityEngine;
using UnityEngine.EventSystems;

public class SetRectLocalScaleClick : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleFactor;

    //private bool isHeld = false;
    private Vector3 originalScale;

    /// <summary>
    /// Start method.
    /// </summary>
    private void Start()
    {
        originalScale = GetComponent<RectTransform>().localScale;
    }

    /// <summary>
    /// OnPointerDown method to handle button press events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        //isHeld = true;
        GetComponent<RectTransform>().localScale = originalScale * scaleFactor;
    }

    /// <summary>
    /// OnPointerUp method to handle button release events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        //isHeld = false;
        GetComponent<RectTransform>().localScale = originalScale;
    }

    /// <summary>
    /// OnPointerEnter method to handle pointer enter events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = originalScale * scaleFactor;
    }

    /// <summary>
    /// OnPointerExit method to handle pointer exit events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        //isHeld = false;
        GetComponent<RectTransform>().localScale = originalScale;
    }

    //private void Update()
    //{
    //    if (isHeld)
    //    {

    //    }
    //}
}