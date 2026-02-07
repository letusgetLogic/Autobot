using UnityEngine;

public class Slot : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The hint of attached object.")]
    [SerializeField] private SpriteRenderer indicator;
    [SerializeField] private EventHover eventHover;
    [SerializeField] private EventDragSlot eventDrag;
    [SerializeField] private GameObject lighting;

    [Tooltip("The hint of dropable slot.")]
    [SerializeField] private SpriteRenderer hintLight;
    [SerializeField] private LightenUpDown lightenUpDown;
    [SerializeField] private ScaleUpDown lightenScale;
    [SerializeField] private Color itemColor;
    [SerializeField] private Color fusionColor;
    [SerializeField] private Color swapColor;

    public LightenUpDown LightenUpDown => lightenUpDown;
    public ScaleUpDown LightenScale => lightenScale;

    private Color defaultColor;
    public int Index { get; set; }

    public bool IsDroppable => gameObject.CompareTag("Slot Team") || gameObject.CompareTag("Slot Charge");

    private void Start()
    {
        if (indicator != null)
            indicator.enabled = false;
    }

    private void OnEnable()
    {
        if (hintLight != null)
            defaultColor = hintLight.color;

        if (lighting != null)
            lighting.SetActive(false);

        eventHover.OnMouseOverEvent += ShowDescription;
        eventHover.OnMouseExitEvent += HideDescription;

    }

    private void OnDisable()
    {
        if (hintLight != null)
            hintLight.color = defaultColor;

        if (lighting != null)
            lighting.SetActive(false);

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
        if (GameManager.Instance.IsBlockingInput)
            return;

        if (UnitView() == null)
            return;

        UnitView().SetDescriptionActive(true);
    
        if (PhaseShopController.Instance &&
            PhaseShopController.Instance.IsBlockingInputsByItemRandomness(this))
            return;
     
        if (indicator != null)
            indicator.enabled = true;
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

        var attached = PhaseShopController.Instance.AttachedController;
        if (indicator.enabled &&
            (attached == null || attached != UnitController()))
        {
            indicator.enabled = false;
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

    /// <summary>
    /// Set indicator active or not.
    /// </summary>
    /// <param name="_isActive"></param>
    public void SetIndicatorActive(bool _isActive)
    {
        if (indicator != null)
            indicator.enabled = _isActive;
    }

    /// <summary>
    /// Sets the lighting for attached active or not.
    /// </summary>
    /// <param name="_isActive"></param>
    public void SetLightingActive(bool _isActive)
    {
        if (lighting != null)
            lighting.SetActive(_isActive);
    }
}
