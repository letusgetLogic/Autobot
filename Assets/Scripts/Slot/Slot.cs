using System;
using System.Collections;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The hint of attached object.")]
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private EventHover eventHover;
    [SerializeField] private EventDragSlot eventDrag;

    [Tooltip("The hint of dropable slot.")]
    [SerializeField] private SpriteRenderer hintLight;
    [SerializeField] private LightenUpDown lighten;
    [SerializeField] private ScaleUpDown lightenScale;
    [SerializeField] private Color itemColor;
    [SerializeField] private Color fusionColor;
    [SerializeField] private Color swapColor;

    public SpriteRenderer Border
    {
        get { return border; }
        set { border = value; }
    }

    public LightenUpDown Lighten => lighten;
    private Color defaultColor;
    public ScaleUpDown LightenScale => lightenScale;
    public int Index { get; set; }

    public bool IsDroppable => gameObject.CompareTag("Slot Team") || gameObject.CompareTag("Slot Charge");

    private void Start()
    {
        if (border != null)
            border.enabled = false;

        if (lighten != null)
        {
            lighten.enabled = false;
        }
        if (lightenScale != null)
            lightenScale.enabled = false;
    }

    private void OnEnable()
    {
        if (hintLight != null)
            defaultColor = hintLight.color;

        eventHover.OnMouseOverEvent += ShowDescription;
        eventHover.OnMouseExitEvent += HideDescription;

    }

    private void OnDisable()
    {
        if (hintLight != null)
            hintLight.color = defaultColor;

        eventHover.OnMouseOverEvent -= ShowDescription;
        eventHover.OnMouseExitEvent -= HideDescription;
    }

    #region Returns Unit Components

    /// <summary>
    /// Returns the component UnitView.
    /// </summary>
    /// <returns></returns>
    public UnitView UnitView()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Unit"))
                return child.GetComponent<UnitView>();
        }
        return null;
    }

    /// <summary>
    /// Returns the component UnitController.
    /// </summary>
    /// <returns></returns>
    public UnitController UnitController()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Unit"))
                return child.GetComponent<UnitController>();
        }
        return null;
    }

    /// <summary>
    /// Returns the game object unit.
    /// </summary>
    /// <returns></returns>
    public GameObject Unit()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Unit"))
                return child.gameObject;
        }
        return null;
    }

    #endregion


    #region Hover Event - Show/Hide description

    /// <summary>
    /// Shows the description.
    /// </summary>
    private void ShowDescription()
    {
        if (UnitView() == null)
            return;

        UnitView().SetDescriptionActive(true);

        if (border != null)
            border.enabled = true;
    }

    /// <summary>
    /// Hides the description.
    /// </summary>
    public void HideDescription()
    {
        if (UnitView() != null)
            UnitView().SetDescriptionActive(false);

        if (PhaseShopController.Instance == null)
            return;

        var attached = PhaseShopController.Instance.AttachedGameObject;
        if (border.enabled &&
            (attached == null || attached != Unit()))
        {
            border.enabled = false;
        }
    }

    #endregion

    /// <summary>
    /// 0 = disable, 1 = default, 2 = item, 3 = fusion, 4 = swap
    /// </summary>
    /// <param name="_isfusion"></param>
    public void SetHintLight(int _state)
    {
        switch(_state)
        {
            case 0:
                break;

            case 1:
                hintLight.color = defaultColor;
                break;

            case 2:
                hintLight.color = itemColor;
                break;

            case 3:
                hintLight.color = fusionColor;
                break;

            case 4:
                hintLight.color = swapColor;
                break;
        }
    }
}
