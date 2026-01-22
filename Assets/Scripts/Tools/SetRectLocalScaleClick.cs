using UnityEngine;
using UnityEngine.EventSystems;

public class SetRectLocalScaleClick : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private RectTransform rotatedSprite;
    [SerializeField] private RectTransform rotatedSprite2;

    [Header("Settings")]
    [SerializeField] private float scaleFactor;
    [SerializeField] private float rotateSpeed;

    private bool isHeld = false;
    private Vector3 originalScale;
    private Quaternion originalRotation;

    /// <summary>
    /// Start method.
    /// </summary>
    private void Start()
    {
        originalScale = GetComponent<RectTransform>().localScale;

        if (rotatedSprite != null)
        {
            rotatedSprite.gameObject.SetActive(false);
            originalRotation = rotatedSprite.localRotation;
        }

        if (rotatedSprite2 != null)
        {
            rotatedSprite2.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// OnPointerDown method to handle button press events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;
        GetComponent<RectTransform>().localScale = originalScale * scaleFactor;

        if (rotatedSprite != null)
            rotatedSprite.gameObject.SetActive(true);

        if (rotatedSprite2 != null)
        {
            rotatedSprite2.gameObject.SetActive(true);
            rotatedSprite2.localRotation = originalRotation;
        }
    }

    /// <summary>
    /// OnPointerUp method to handle button release events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;
        GetComponent<RectTransform>().localScale = originalScale;

        if (rotatedSprite != null)
            rotatedSprite.localRotation = originalRotation;

        if (rotatedSprite2 != null)
        {
            rotatedSprite2.localRotation = originalRotation;
        }
    }

    private void Update()
    {
        if (isHeld)
        {
            if (rotatedSprite != null)
            {
                Quaternion rotate = new Quaternion(
              rotatedSprite.localRotation.x,
              rotatedSprite.localRotation.y,
              rotatedSprite.localRotation.z - rotateSpeed * Time.deltaTime,
              rotatedSprite.localRotation.w
              );
                rotatedSprite.localRotation = rotate;

                if (rotatedSprite2 != null)
                    rotatedSprite2.localRotation = rotate;
            }

        }
    }
}