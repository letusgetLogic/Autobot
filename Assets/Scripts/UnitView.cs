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
        description,
        iceCube;

    public GameObject IceCube => iceCube;

    [SerializeField]
    private TextMeshProUGUI 
        myName,
        ability,
        coin,
        health,
        attack;

    [SerializeField]
    private float scale = 1.1f;

    [SerializeField]
    private Vector3 dragOverOther = Vector3.back; // offset while dragging over other

    private Camera mainCamera;
   
    private Vector3 originalScale;

    #endregion

    private void Awake()
    {
        description.SetActive(false);
        iceCube.gameObject.SetActive(false);
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
    }

    /// <summary>
    /// OnPointerUp calls this method.
    /// </summary>
    /// <param name="eventData"></param>
    public void BeingReleased(PointerEventData eventData)
    {
        dragSpriteRenderer.gameObject.transform.localPosition = Vector3.zero;
        dragSpriteRenderer.gameObject.transform.localScale = originalScale;

        health.enabled = true;
        attack.enabled = true;
    }

    #endregion
}
