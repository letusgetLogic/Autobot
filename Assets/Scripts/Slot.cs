using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer border;
    public SpriteRenderer Border
    {
        get { return border; }
        set { border = value; }
    }

    [SerializeField]
    private EventHover eventHover;
    public EventHover EventHover => eventHover;

    [SerializeField]
    private EventDrag eventDrag;
    public EventDrag EventDrag => eventDrag;

    public int Index { get; set; }


    private void Start()
    {
        if (border != null)
            border.enabled = false;
    }

    private void OnEnable()
    {
        eventHover.OnMouseOverEvent += ShowStats;
        eventHover.OnMouseExitEvent += HideStats;

    }

    private void OnDisable()
    {
        eventHover.OnMouseOverEvent -= ShowStats;
        eventHover.OnMouseExitEvent -= HideStats;
    }

    #region Returns Unit

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


    #region Hover Event - Show description

    /// <summary>
    /// Shows the description.
    /// </summary>
    private void ShowStats()
    {
        if (eventHover.Data != null && eventHover.Data.pointerDrag != null)
            return;

        if (UnitController() == null ||
            UnitController().Model.UnitState == UnitState.Freezed)
            return;

        if (UnitView() == null)
            return;

        UnitView().SetDescriptionActive(true);

        if (border != null)
            border.enabled = true;
    }

    /// <summary>
    /// Hides the description.
    /// </summary>
    private void HideStats()
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
