using UnityEngine;

public class Slot : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The hint of attached object.")]
    [SerializeField] private SpriteRenderer border;
    [SerializeField] private EventHover eventHover;
    [SerializeField] private EventDragSlot eventDrag;

    [Tooltip("The hint of dropable slot.")]
    [SerializeField] private LightenUpDown lighten;
    [SerializeField] private ScaleUpDown lightenScale;

    public SpriteRenderer Border
    {
        get { return border; }
        set { border = value; }
    }

    public EventHover EventHover => eventHover;
    public EventDragSlot EventDrag => eventDrag;
    public LightenUpDown Lighten => lighten;
    public ScaleUpDown LightenScale => lightenScale;
    public int Index { get; set; }


    private void Start()
    {
        if (border != null)
            border.enabled = false;

        if (lighten != null)
            lighten.enabled = false;
        if (lightenScale != null)
            lightenScale.enabled = false;
    }

    private void OnEnable()
    {
        eventHover.OnMouseOverEvent += ShowDescription;
        eventHover.OnMouseExitEvent += HideDescription;

    }

    private void OnDisable()
    {
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

        if (PhaseShopUnitManager.Instance == null)
            return;

        var attached = PhaseShopUnitManager.Instance.AttachedGameObject;
        if (border.enabled &&
            (attached == null || attached != Unit()))
        {
            border.enabled = false;
        }
    }

    #endregion

}
