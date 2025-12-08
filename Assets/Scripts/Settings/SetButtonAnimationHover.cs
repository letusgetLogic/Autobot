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
    [SerializeField] private GameObject rotatedSprite;
    [SerializeField] private GameObject rotatedSprite2;

    [Header("Settings")]
    [SerializeField] private string hexColorHover;

    private Color originalBackgroundColor;

    /// <summary>
    /// Start method.
    /// </summary>        
    private void Start()
    {
        if (background != null)
            originalBackgroundColor = background.color;

        SetAnimationActive(false);

        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                SetAnimationActive(false);

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

        SetAnimationActive(true);
    }

    /// <summary>
    /// OnPointerExit method to handle pointer exit events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (background != null)
            background.color = originalBackgroundColor;

        SetAnimationActive(false);
    }

    private void SetAnimationActive(bool value)
    {
        if (rotatedSprite != null)
            rotatedSprite.SetActive(value);

        if (rotatedSprite2 != null)
            rotatedSprite2.SetActive(value);
    }
}