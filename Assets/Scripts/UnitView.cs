using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitView : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private SpriteRenderer shadowSpriteRenderer;
    public SpriteRenderer Shadow => shadowSpriteRenderer;

    [SerializeField]
    private SpriteRenderer
        dragSpriteRenderer;

    [SerializeField]
    private GameObject
        canvas,
        description,
        heartIcon,
        attackIcon,
        iceCube,
        level,
        box1StepFilled,
        box2Step,
        step1Filled,
        step2Filled,
        box3Step,
        step3Filled,
        step4Filled,
        step5Filled;

    public GameObject IceCube => iceCube;

    [SerializeField]
    private TextMeshProUGUI
        myName,
        ability,
        coin,
        health,
        attack,
        levelAmount;

    [SerializeField]
    private float scale = 1.1f;

    [SerializeField]
    private float delayUpdateLevel = .5f;
    public float DelayUpdateLevel => delayUpdateLevel;

    [SerializeField]
    private Vector3 dragOverOther = Vector3.back; // offset while dragging over other

    private Camera mainCamera;

    private Vector3 originalScale;
    private Vector3 levelPosition;

    #endregion

    private void Awake()
    {
        description.SetActive(false);
        iceCube.gameObject.SetActive(false);
        levelPosition = level.transform.localPosition;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        originalScale = dragSpriteRenderer.gameObject.transform.localScale;
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_description"></param>
    /// <param name="_cost"></param>
    /// <param name="_health"></param>
    /// <param name="_attack"></param>
    public void SetData(Sprite _sprite, string _name, string _description,
        int _cost, int _health, int _attack)
    {
        dragSpriteRenderer.sprite = _sprite;
        shadowSpriteRenderer.sprite = _sprite;
        myName.text = _name;
        gameObject.name = _name;
        ability.text = _description;
        coin.text = _cost.ToString();
        health.text = _health.ToString();
        attack.text = _attack.ToString();
    }

    /// <summary>
    /// Sets game object description active true/false.
    /// </summary>
    /// <param name="value"></param>
    public void SetDescriptionActive(bool value)
    {
        description.SetActive(value);
    }

    #region Drag Event

    /// <summary>
    /// OnPointerDown calls this method.
    /// </summary>
    /// <param name="eventData"></param>
    public void BeingAttached(PointerEventData eventData)
    {
        dragSpriteRenderer.gameObject.transform.localScale *= scale;

        if (shadowSpriteRenderer.enabled == false)
        {
            heartIcon.gameObject.SetActive(false);
            attackIcon.gameObject.SetActive(false);
            health.enabled = false;
            attack.enabled = false;
        }
    }

    /// <summary>
    /// OnDrag calls this method.
    /// </summary>
    /// <param name="eventData"></param>
    public void BeingMovedOnMouse(PointerEventData eventData)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, 10f));

        dragSpriteRenderer.gameObject.transform.position = worldPosition + dragOverOther;
        canvas.transform.position = worldPosition + dragOverOther;
    }

    /// <summary>
    /// OnPointerUp calls this method.
    /// </summary>
    /// <param name="eventData"></param>
    public void BeingReleased(PointerEventData eventData)
    {
        dragSpriteRenderer.gameObject.transform.localPosition = Vector3.zero;
        dragSpriteRenderer.gameObject.transform.localScale = originalScale;
        canvas.transform.localPosition = Vector3.zero;

        heartIcon.gameObject.SetActive(true);
        attackIcon.gameObject.SetActive(true);
        health.enabled = true;
        attack.enabled = true;
    }

    #endregion

    /// <summary>
    /// Sets the step components active.
    /// </summary>
    /// <param name="box1"></param>
    /// <param name="box2"></param>
    /// <param name="step1"></param>
    /// <param name="step2"></param>
    /// <param name="box3"></param>
    /// <param name="step3"></param>
    /// <param name="step4"></param>
    /// <param name="step5"></param>
    public void SetStepActive(
        string level,
        bool box1,
        bool box2, bool step1, bool step2,
        bool box3, bool step3, bool step4, bool step5)
    {
        levelAmount.text = level;
        box1StepFilled.SetActive(box1);
        box2Step.SetActive(box2);
        step1Filled.SetActive(step1);
        step2Filled.SetActive(step2);
        box3Step.SetActive(box3);
        step3Filled.SetActive(step3);
        step4Filled.SetActive(step4);
        step5Filled.SetActive(step5);
    }
}
