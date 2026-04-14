
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [Tooltip("The hint of attached object.")]
    [SerializeField] private SpriteRenderer indicator;

    [SerializeField] private EventDragSlot eventDrag;
    [SerializeField] private GameObject lighting;
    [SerializeField] private SoSlotSettings settings;

    [Tooltip("The hint of dropable slot.")]
    [SerializeField] private SpriteRenderer hintLight;
    [SerializeField] private LightenUpDown lightenUpDown;
    [SerializeField] private ScaleUpDown lightenScale;

    public LightenUpDown LightenUpDown => lightenUpDown;
    public ScaleUpDown LightenScale => lightenScale;

    private Color defaultColor;
    public int Index { get; set; }

    public bool IsDroppable =>
        gameObject.CompareTag("Slot Team") ||
        gameObject.CompareTag("Slot Charge") ||
        gameObject.CompareTag("Slot Random");

    private InputKey inputKey
    {
        get
        {
            if (CompareTag("Slot Team"))
                return InputKey.HoverSlotTeam;

            if (CompareTag("Slot Charge"))
                return InputKey.HoverSlotCharge;

            if (CompareTag("Slot Shop"))
            {
                var unit = UnitController();
                if (unit && unit.IsRobot(unit.Model.SoUnit.UnitType))
                    return InputKey.HoverSlotShopBot;
                else
                    return InputKey.HoverSlotShopItem;
            }

            if (CompareTag("Slot Random"))
                return InputKey.HoverSlotTeamRandom;

            if (CompareTag("Slot Battle"))
                return InputKey.HoverSlotBattle;

            return InputKey.None;
        }
    }

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
    }

    private void OnDisable()
    {
        if (hintLight != null)
            hintLight.color = defaultColor;

        if (lighting != null)
            lighting.SetActive(false);
    }

    /// <summary>
    /// Mouse entered the collider.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InputManager.Instance.IsBlockingInput(inputKey))
            return;

        if (GameManager.Instance.IsCatalogActive == false)
            ShowDescription();
    }

    /// <summary>
    /// Mouse exited the collider.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (InputManager.Instance.IsBlockingInput(inputKey))
            return;

        if (GameManager.Instance.IsCatalogActive == false)
            HideDescription();
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
    /// 0 = disable, 1 = default, 2 = item, 3 = fusion, 4 = swap, 5 = shutdown
    /// </summary>
    /// <param name="_isfusion"></param>
    public void SetHintLight(UnitController _attached, bool _isEnabled)
    {
        int colorIndex = ColorIndex(_attached);
        switch (colorIndex)
        {
            case 0:
                break;

            case 1:
                hintLight.color = defaultColor;
                break;

            case 2:
                hintLight.color = settings.ItemColor;
                break;

            case 3:
                hintLight.color = settings.FusionColor;
                break;

            case 4:
                hintLight.color = settings.SwapColor;
                break;

            case 5:
                hintLight.color = settings.ShutdownColor;
                break;
        }

        lightenUpDown.enabled = colorIndex == 0 ? false : _isEnabled;
        lightenScale.enabled = colorIndex == 0 ? false : _isEnabled;


        if (_attached == UnitController())
        {
            SetLightingActive(_isEnabled);
        }
    }

    /// <summary>
    /// Returns the color index.
    /// </summary>
    /// <param name="_slot"></param>
    /// <returns></returns>
    private int ColorIndex(UnitController _attached)
    {
        if (_attached == null || _attached == UnitController())
            return 0;

        bool slotEmpty = Unit() == null;

        bool isFusible = slotEmpty ? false : PhaseShopController.Instance.IsFusible(UnitController(), _attached);

        bool isAttachedItem = _attached.Model.Data.UnitType == UnitType.Item;
        bool isShutdown = false;
        if (isAttachedItem)
            isShutdown = _attached.Model.CurrentLevel.DoType == DoType.ShutDown;

        bool isAttachedBotInShop = _attached.Model.IsRobotInShop();

        // is attached item?
        //              yes ->  (slot empty?     no -> shutdown?) 
        return isAttachedItem ? (slotEmpty ? 0 : (isShutdown ? 5 : 2)) :
            //          no ->
            // (is slot empty?   no -> (is fusible between both units?   no -> is attached robot in shop?))
            (slotEmpty ? 1 : (isFusible ? 3 : isAttachedBotInShop ? 0 : 4));
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
