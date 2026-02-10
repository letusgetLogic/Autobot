using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonInteractedAnimation : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private RectTransform rotatedSprite;
    [SerializeField] private RectTransform rotatedSprite2;
    [SerializeField] private GameObject layoutToDeactivate;
    [SerializeField] private GameObject layoutToActivate;
    [SerializeField] private UnityEvent doSomethingAfterClick;

    [Header("Settings")]
    [SerializeField] private float degreesPerSecond;
    [SerializeField] private float rotateTime;

    private Quaternion originalRotation;
    private float countdown = 0f;
    private bool isRunning = false;

    private void OnEnable()
    {
        if (rotatedSprite != null)
        {
            rotatedSprite.gameObject.SetActive(false);
            originalRotation = rotatedSprite.localRotation;
        }

        if (rotatedSprite2 != null)
        {
            rotatedSprite2.gameObject.SetActive(false);
            rotatedSprite2.localRotation = originalRotation;
        }

        GameManager.Instance.IsBlockingInput = false;
    }

    /// <summary>
    /// OnPointerEnter method to handle pointer enter events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rotatedSprite != null)
            rotatedSprite.gameObject.SetActive(true);

        if (rotatedSprite2 != null)
            rotatedSprite2.gameObject.SetActive(true);
    }

    /// <summary>
    /// OnPointerExit method to handle pointer exit events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isRunning)
            return;

        if (rotatedSprite != null)
            rotatedSprite.gameObject.SetActive(false);

        if (rotatedSprite2 != null)
            rotatedSprite2.gameObject.SetActive(false);

        GameManager.Instance.IsBlockingInput = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.IsBlockingInput || isRunning)
            return;

        GameManager.Instance.IsBlockingInput = true;

        isRunning = true;
        countdown = rotateTime;
    }

    /// <summary>
    /// OnPointerDown method to handle button press events.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
       
    }

    private void Update()
    {
        if (isRunning)
        {
            if (countdown <= 0f)
            {
                countdown = 0f;

                if (rotatedSprite != null)
                {
                    rotatedSprite.localRotation = originalRotation;
                    rotatedSprite.gameObject.SetActive(false);
                }

                if (rotatedSprite2 != null)
                {
                    rotatedSprite2.localRotation = originalRotation;
                    rotatedSprite2.gameObject.SetActive(false);
                }

                GameManager.Instance.IsBlockingInput = false;
                isRunning = false;
                return;
            }

            countdown -= Time.deltaTime;

            if (rotatedSprite != null)
            {
                rotatedSprite.Rotate(0f, 0f, -degreesPerSecond * Time.deltaTime);

                if (rotatedSprite2 != null)
                    rotatedSprite2.Rotate(0f, 0f, -degreesPerSecond * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Switches the layout after a specified delay.
    /// </summary>
    /// <remarks>This method starts a coroutine to perform the layout switch. It is typically called in
    /// response to a user action or event that requires changing the current layout.</remarks>
    public void OnSwitchLayout()
    {
        StartCoroutine(Switch(rotateTime));
    }

    /// <summary>
    /// Switches the layout after a specified delay.
    /// </summary>
    /// <param name="_delay"></param>
    /// <returns></returns>
    private IEnumerator Switch(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        if (layoutToDeactivate != null)
            layoutToDeactivate.SetActive(false);

        if (layoutToActivate != null)
            layoutToActivate.SetActive(true);

        if (doSomethingAfterClick != null)
            doSomethingAfterClick.Invoke();

    }

}
