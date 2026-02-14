using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetButtonAnimationHover : MonoBehaviour,
     IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private GameObject DeactivatedParent;
    [SerializeField] private Image background;

    [Header("Settings")]
    [SerializeField] private string hexColorHover;

    private Color originalBackgroundColor;

    private void OnEnable()
    {
        if (background != null)
            originalBackgroundColor = background.color;

        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (gameObject.activeSelf == false)
                {
                    if (background != null)
                        background.color = originalBackgroundColor;
                }
                else if (DeactivatedParent != null && DeactivatedParent.activeSelf == false)
                {
                    if (background != null)
                        background.color = originalBackgroundColor;
                }
            });
        }
    }

    private void OnDisable()
    {
        if (background != null)
            background.color = originalBackgroundColor;
    }

    /// <summary>
    /// OnPointerEnter method to handle pointer enter events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (background != null)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hexColorHover, out color))
                background.color = color;
        }
    }

    /// <summary>
    /// OnPointerExit method to handle pointer exit events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (background != null)
            background.color = originalBackgroundColor;
    }
}