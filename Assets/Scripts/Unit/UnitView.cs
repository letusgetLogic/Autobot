using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Managing Visual Components.
/// </summary>
public class UnitView : MonoBehaviour
{
    #region Variables

    [Header("Hide Objects")]
    [SerializeField] GameObject energyConsumptionComponent;
    [SerializeField]
    private GameObject[]
        hideObjectsDuringBattle,
        hideFullAttributes,
        hideObjectsByNoAbility,
        hideVisual;

    [Header("Sprites")]
    [Tooltip(("The shadow is showing, when sprite is being dragged out of the slot shop."))]
    [SerializeField] private SpriteRenderer dragSpriteRenderer;
    [SerializeField]
    private SpriteRenderer
        shadowSpriteRenderer,
        doAbilitySpriteRenderer,
        getAbilitySpriteRenderer,
        lockSpriteRenderer,
        damageSpriteRenderer,
        shutdownSpriteRenderer,
        temporaryItemSpriteRenderer;

    [Header("Description")]
    [SerializeField] private GameObject description;
    [SerializeField]
    private TextMeshProUGUI
        myName,
        energyConsumption,
        ability,
        fullAttack,
        fullHealth;

    [Header("Craft / Recycle Display")]
    [SerializeField] private GameObject nut;
    [SerializeField] private GameObject tool;
    [SerializeField]
    private TextMeshProUGUI
        craftText,
        nutValue,
        toolValue;

    [Header("Level Display")]
    [SerializeField] private GameObject levelDisplay;
    [SerializeField] private TextMeshProUGUI levelAmount;
    [SerializeField]
    private GameObject
        box1StepFilled,
        box2Step,
        step1Filled,
        step2Filled,
        box3Step,
        step3Filled,
        step4Filled,
        step5Filled;

    [Header("Attribute Display")]
    [SerializeField] private GameObject attributeGroup;
    [SerializeField] private GameObject energyIcon;
    [SerializeField]
    private TextMeshProUGUI
        health,
        attack,
        energy,
        damage,
        buffHealth,
        buffAttack,
        addEnergy,
        consumEnergy;

    [Header("Repair Display Health")]
    [SerializeField] private GameObject repairDisplayHp;
    [SerializeField]
    private GameObject
        repairPanelHp2,
        repairPanelHp3;
    [SerializeField]
    private GameObject
        repairStepFillHp1,
        repairStepFillHp2,
        repairStepFillHp3;

    [Header("Repair Display Attack")]
    [SerializeField] private GameObject repairDisplayAtk;
    [SerializeField]
    private GameObject
        repairPanelAtk2,
        repairPanelAtk3;
    [SerializeField]
    private GameObject
        repairStepFillAtk1,
        repairStepFillAtk2,
        repairStepFillAtk3;

    [Header("Settings")]
    [SerializeField] private Canvas canvasStats;
    [SerializeField] private SoUnitSettings settings;
    public SoUnitSettings Settings => settings;

    public float DelayUpdateLevel => settings.DelayUpdateLevel;
    public Vector3 OffsetMoveOverOther => settings.OffsetMoveOverOther;
    public Vector2 DragSpritePosition => dragSpriteRenderer.transform.position;
    private Vector3 originalScale;

    private int originalSortingOrder;

    #endregion


    private void OnValidate()
    {
        if (description) description.SetActive(false);
    }

    private void Awake()
    {
        // The unit in catalog sometimes doesn't need the components, so the components should be checked.
        if (description) description.SetActive(false);
        if (lockSpriteRenderer) lockSpriteRenderer.enabled = false;
        if (damageSpriteRenderer) damageSpriteRenderer.enabled = false;
        if (shutdownSpriteRenderer) shutdownSpriteRenderer.enabled = false;
        if (temporaryItemSpriteRenderer) temporaryItemSpriteRenderer.enabled = false;
        if (doAbilitySpriteRenderer) doAbilitySpriteRenderer.enabled = false;
        if (getAbilitySpriteRenderer) getAbilitySpriteRenderer.enabled = false;

    }

    private void Start()
    {
        originalScale = dragSpriteRenderer.gameObject.transform.localScale;
        originalSortingOrder = dragSpriteRenderer.sortingOrder;
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetData(Sprite _sprite, string _name, string _modelID, string _id)
    {
        // The unit in catalog sometimes doesn't need the components, so the components should be checked.
        if (dragSpriteRenderer) dragSpriteRenderer.sprite = _sprite;
        if (damageSpriteRenderer) damageSpriteRenderer.sprite = _sprite;
        if (shadowSpriteRenderer) shadowSpriteRenderer.sprite = _sprite;
        if (doAbilitySpriteRenderer) doAbilitySpriteRenderer.sprite = _sprite;
        if (getAbilitySpriteRenderer)getAbilitySpriteRenderer.sprite = _sprite;
        if (myName) myName.text = _name;
        gameObject.name = _id;
    }

    /// <summary>
    /// Sets the components view in slot shop.
    /// </summary>
    /// <param name="_value"></param>
    public void SetShopView(bool _value, bool _showLevel, bool _isLocked)
    {
        levelDisplay.SetActive(_showLevel);
        ShowFullAttributes(!_value);
        energyIcon.SetActive(!_value);
        shadowSpriteRenderer.enabled = _value;
        lockSpriteRenderer.enabled = _isLocked;
    }

    /// <summary>
    /// Set temporary buff sprite.
    /// </summary>
    /// <param name="_tempAttrib"></param>
    public void SetTemporaryItem(bool _enabled)
    {
        temporaryItemSpriteRenderer.enabled = _enabled;
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetAbility(string _description, int _energy)
    {
        if (_description != null)
        {
            ability.text = _description;

            if (_energy == 0)
                energyConsumptionComponent.SetActive(false);
            else
                energyConsumption.text = _energy.ToString();

            return;
        }

        // if it has an ability, hides the defined visuals
        foreach (var element in hideObjectsByNoAbility)
            element.SetActive(false);
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetBuyOrSell(Currency _cur, bool _isForBuying, UnitType _type)
    {
        craftText.text = _isForBuying ? (_type == UnitType.Item ? "Install" : "Craft") : "Recycle";

        if (_cur.Nut != 0)
        {
            string operation = _cur.Nut > 0 ? "+" : "";
            nutValue.text = operation + _cur.Nut.ToString();
            nut.SetActive(true);
        }
        else
        {
            nutValue.text = "";
            nut.SetActive(false);
        }

        if (_cur.Tool != 0)
        {
            string operation = _cur.Tool > 0 ? "+" : "";
            toolValue.text = operation + _cur.Tool.ToString();
            tool.SetActive(true);
        }
        else
        {
            toolValue.text = "";
            tool.SetActive(false);
        }
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetData(int _fullHp, int _fullAtk, int _hp, int _atk, int _energy)
    {
        if (fullHealth) fullHealth.text = _fullHp.ToString();
        if (fullAttack) fullAttack.text = _fullAtk.ToString();
        if (health) health.text = _hp.ToString();
        if (attack) attack.text = _atk.ToString();
        if (energy) energy.text = _energy.ToString();
    }

    /// <summary>
    /// Sets game object description active true/false.
    /// </summary>
    /// <param name="_value"></param>
    public void SetDescriptionActive(bool _value)
    {
        if (description) description.SetActive(_value);
    }

    #region Drag Event Behaviour

    /// <summary>
    /// Adjusts the drag sprite's scale and updates sorting orders when the drag operation begins.
    /// </summary>
    /// <param name="eventData">Pointer event data associated with the drag operation.</param>
    public void BeingAttached(PointerEventData eventData)
    {
        dragSpriteRenderer.gameObject.transform.localScale *= settings.DraggingScale;

        dragSpriteRenderer.sortingOrder = lockSpriteRenderer.sortingOrder + 10;
        canvasStats.sortingOrder = lockSpriteRenderer.sortingOrder + 10;
    }

    /// <summary>
    /// Updates the positions of the drag sprite and canvas statistics to follow the mouse pointer during a drag
    /// operation.
    /// </summary>
    /// <param name="eventData">Pointer event data containing the current mouse position.</param>
    public void BeingMovedOnMouse(PointerEventData eventData)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, 10f));

        dragSpriteRenderer.gameObject.transform.position = worldPosition + settings.OffsetDragOverOther;
        canvasStats.transform.position = worldPosition + settings.OffsetDragOverOther;
    }

    /// <summary>
    /// Resets the visual state and local position in response to a pointer release event.
    /// </summary>
    /// <param name="eventData">Event data associated with the pointer release.</param>
    public void BeingReleased(PointerEventData eventData)
    {
        SetVisualDefault();
        SetLocalPositionDefault();
    }

    #endregion


    /// <summary>
    /// Set the scale & sorting order default.
    /// </summary>
    public void SetVisualDefault()
    {
        dragSpriteRenderer.gameObject.transform.localScale = originalScale;
        dragSpriteRenderer.sortingOrder = originalSortingOrder;
        canvasStats.sortingOrder = originalSortingOrder;
    }

    /// <summary>
    /// Set the local position to Vector3.zero.
    /// </summary>
    public void SetLocalPositionDefault()
    {
        dragSpriteRenderer.gameObject.transform.localPosition = Vector3.zero;
        canvasStats.transform.localPosition = Vector3.zero;
    }


    /// <summary>
    /// Set the sprite renderer & stats canvas over other.
    /// </summary>
    public void SetSpriteOverOther()
    {
        dragSpriteRenderer.gameObject.transform.position += settings.OffsetMoveOverOther;
        canvasStats.transform.position += settings.OffsetMoveOverOther;
    }

    /// <summary>
    /// Sets the xp step components active.
    /// </summary>
    /// <param name="_box1"></param>
    /// <param name="_box2"></param>
    /// <param name="_step1"></param>
    /// <param name="_step2"></param>
    /// <param name="_box3"></param>
    /// <param name="_step3"></param>
    /// <param name="_step4"></param>
    /// <param name="_step5"></param>
    public void SetXpStepActive(
        string _level,
        bool _box1,
        bool _box2, bool _step1, bool _step2,
        bool _box3, bool _step3, bool _step4, bool _step5)
    {
        levelAmount.text = _level;
        box1StepFilled.SetActive(_box1);
        box2Step.SetActive(_box2);
        step1Filled.SetActive(_step1);
        step2Filled.SetActive(_step2);
        box3Step.SetActive(_box3);
        step3Filled.SetActive(_step3);
        step4Filled.SetActive(_step4);
        step5Filled.SetActive(_step5);
    }

    /// <summary>
    /// Sets the activity of repair display.
    /// </summary>
    /// <param name="_value"></param>
    public void SetRepairDisplayActive(bool _value)
    {
        repairDisplayHp.SetActive(_value);
        repairDisplayAtk.SetActive(_value);
    }

    /// <summary>
    /// Sets the activity of the repair panels / layers.
    /// </summary>
    /// <param name="_panel2"></param>
    /// <param name="_panel3"></param>
    public void SetRepairPanelActive(bool _panel2, bool _panel3)
    {
        if (repairPanelHp2) repairPanelHp2.SetActive(_panel2);
        if (repairPanelHp3) repairPanelHp3.SetActive(_panel3);

        if (repairPanelAtk2) repairPanelAtk2.SetActive(_panel2);
        if (repairPanelAtk3) repairPanelAtk3.SetActive(_panel3);
    }

    /// <summary>
    /// Sets the activity of the repair steps.
    /// </summary>
    /// <param name="_fill1"></param>
    /// <param name="_fill2"></param>
    /// <param name="_fill3"></param>
    public void SetRepairStepFillActive(bool _fill1, bool _fill2, bool _fill3)
    {
        if (repairStepFillHp1) repairStepFillHp1.SetActive(_fill1);
        if (repairStepFillHp2) repairStepFillHp2.SetActive(_fill2);
        if (repairStepFillHp3) repairStepFillHp3.SetActive(_fill3);

        if (repairStepFillAtk1) repairStepFillAtk1.SetActive(_fill1);
        if (repairStepFillAtk2) repairStepFillAtk2.SetActive(_fill2);
        if (repairStepFillAtk3) repairStepFillAtk3.SetActive(_fill3);
    }

    /// <summary>
    /// Flips the sprite and push the level display on the right side.
    /// </summary>
    public void SetRightSide()
    {
        dragSpriteRenderer.flipX = true;
        damageSpriteRenderer.flipX = true;
        temporaryItemSpriteRenderer.flipX = true;
        doAbilitySpriteRenderer.flipX = true;
        getAbilitySpriteRenderer.flipX = true;

        var pos = levelDisplay.transform.localPosition;
        levelDisplay.transform.localPosition = new Vector3(pos.x * -1, pos.y, pos.z);
    }

    /// <summary>
    /// Shows damage and update health.
    /// </summary>
    public void ShowDamage(int _damage, int _health)
    {
        health.text = _health.ToString();
        damage.enabled = true;
        damage.text = _damage.ToString();
        damageSpriteRenderer.enabled = true;
        StartCoroutine(HideDamage());
        StartCoroutine(HideDamageVisual());
    }

    /// <summary>
    /// Hides damage.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideDamage()
    {
        yield return new WaitForSeconds(settings.DurationShowDamage);

        damage.enabled = false;
    }

    /// <summary>
    /// Hides damage visual.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideDamageVisual()
    {
        yield return new WaitForSeconds(settings.DurationShowDamageVisual);

        damageSpriteRenderer.enabled = false;
    }

    /// <summary>
    /// Shows the buff.
    /// </summary>
    /// <param name="_health"></param>
    /// <param name="_attack"></param>
    /// <param name="_energy"></param>
    public void ShowBuff(Attribute _attribute)
    {
        buffHealth.text = _attribute.HP.ToString();
        buffHealth.enabled = _attribute.HP > 0;

        buffAttack.text = _attribute.ATK.ToString();
        buffAttack.enabled = _attribute.ATK > 0;

        addEnergy.text = _attribute.ENG.ToString();
        addEnergy.enabled = _attribute.ENG > 0;

        if (_attribute.IsGreaterThan0() == false)
            return;

        StartCoroutine(HideBuff());
    }

    /// <summary>
    /// Hides the buff.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideBuff()
    {
        yield return new WaitForSeconds(settings.DurationShowTemporaryValue);
        buffHealth.enabled = false;
        buffAttack.enabled = false;
        addEnergy.enabled = false;
    }

    /// <summary>
    /// Shows the consumed energy.
    /// </summary>
    /// <param name="_energy"></param>
    public void ShowConsume(int _energy)
    {
        consumEnergy.text = _energy.ToString();
        consumEnergy.enabled = _energy < 0;

        StartCoroutine(HideConsum());
    }

    /// <summary>
    /// Hides the consumed energy.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideConsum()
    {
        yield return new WaitForSeconds(settings.DurationShowTemporaryValue);
        consumEnergy.enabled = false;
    }

    /// <summary>
    /// Hides the objects during battle phase.
    /// </summary>
    public void HideObjectsDuringBattle()
    {
        foreach (var element in hideObjectsDuringBattle)
            element.SetActive(false);
    }

    /// <summary>
    /// Shows/Hides the full HP and ATK.
    /// </summary>
    public void ShowFullAttributes(bool _value)
    {
        foreach (var element in hideFullAttributes)
            element.SetActive(_value);
    }

    /// <summary>
    /// Hides attack, heart and energy icons.
    /// </summary>
    public void HideAttributes()
    {
        attributeGroup.SetActive(false);
    }

    /// <summary>
    /// Enables the shutdown indicator by activating its sprite renderer.
    /// </summary>
    /// <remarks>Call this method to visually indicate that the shutdown state is active. The shutdown
    /// indicator will become visible in the UI.</remarks>
    public void SetShutdown()
    {
        shutdownSpriteRenderer.enabled = true;
    }

    /// <summary>
    /// Hides only the visuals of unit.
    /// </summary>
    public void HideVisual()
    {
        foreach (var item in hideVisual)
            item.SetActive(false);
    }

    /// <summary>
    /// Shows or hides the execution of the ability.
    /// </summary>
    /// <param name="_enabled"></param>
    public void ShowAbility(bool _enabled)
    {
        doAbilitySpriteRenderer.enabled = _enabled;
    }

    /// <summary>
    /// Shows or hides the target of the ability.
    /// </summary>
    /// <param name="_enabled"></param>
    public void GetTargetedByAbility()
    {
        getAbilitySpriteRenderer.enabled = true;

        StartCoroutine(HideTarget());
    }

    /// <summary>
    /// Hides the buff.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideTarget()
    {
        yield return new WaitForSeconds(settings.DurationShowTemporaryValue);

        getAbilitySpriteRenderer.enabled = false;
    }

}
