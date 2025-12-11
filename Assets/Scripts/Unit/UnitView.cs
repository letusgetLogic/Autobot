using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitView : MonoBehaviour
{
    #region Variables

    [Header("Sprites")]
    [Tooltip(("The shadow is showing, when sprite is being dragged out of the slot shop."))]
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;
    [SerializeField] private SpriteRenderer dragSpriteRenderer;
    [SerializeField] private SpriteRenderer iceCubeSpriteRenderer;
    public SpriteRenderer Shadow => shadowSpriteRenderer;
    public GameObject IceCube => iceCubeSpriteRenderer.gameObject;

    [SerializeField] 
    private GameObject[] 
        hideObjectsDuringBattle,
        hideFullAttributes,
        hideObjectsByNoAbility;

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
    [SerializeField] private GameObject heartIcon;
    [SerializeField] private GameObject attackIcon;
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
    [SerializeField] private Canvas canvas;
    [SerializeField] private SoUnitSettings unitSettings;

    [Header("Color")]
    [SerializeField] private Color damageColor;
    [SerializeField] private Color buffColor;


    public float DelayUpdateLevel => unitSettings.DelayUpdateLevel;

    private Vector3 originalScale;

    private int originalSortingOrder;

    #endregion

    private void Awake()
    {
        description.SetActive(false);
        iceCubeSpriteRenderer.gameObject.SetActive(false);
        SetRepairDisplayActive(false);
    }

    private void Start()
    {
        originalScale = dragSpriteRenderer.gameObject.transform.localScale;
        originalSortingOrder = dragSpriteRenderer.sortingOrder;
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetData(Sprite _sprite, string _name, string _id)
    {
        dragSpriteRenderer.sprite = _sprite;
        shadowSpriteRenderer.sprite = _sprite;
        myName.text = _name;
        gameObject.name = _id;
    }

    /// <summary>
    /// Sets the components view in slot shop.
    /// </summary>
    /// <param name="_value"></param>
    public void SetShopView(bool _value, bool _showLevel)
    {
        levelDisplay.SetActive(_showLevel);
        ShowFullAttributes(!_value);
        energyIcon.SetActive(!_value);
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetAbility(string _description, int _energy)
    {
        if (_description != null)
        {
            ability.text = _description;
            energyConsumption.text = _energy.ToString();
            return;
        }

        foreach (var element in hideObjectsByNoAbility)
            element.SetActive(false);
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetBuyOrSell(Currency _cur, bool _isForBuying)
    {
        craftText.text = _isForBuying ? "craft" : "recycle";

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
        fullHealth.text = _fullHp.ToString();
        fullAttack.text = _fullAtk.ToString();
        health.text = _hp.ToString();
        attack.text = _atk.ToString();
        energy.text = _energy.ToString();
    }

    /// <summary>
    /// Sets game object description active true/false.
    /// </summary>
    /// <param name="_value"></param>
    public void SetDescriptionActive(bool _value)
    {
        description.SetActive(_value);
    }

    // Drag Event
    #region Drag Event 

    /// <summary>
    /// OnPointerDown calls this method.
    /// </summary>
    /// <param name="eventData"></param>
    public void BeingAttached(PointerEventData eventData)
    {
        dragSpriteRenderer.gameObject.transform.localScale *= unitSettings.DraggingScale;

        if (shadowSpriteRenderer.enabled == false)
        {
            //heartIcon.gameObject.SetActive(false);
            //attackIcon.gameObject.SetActive(false);
            //energyIcon.gameObject.SetActive(false);
            //health.enabled = false;
            //attack.enabled = false;
            //energy.enabled = false;
        }

        dragSpriteRenderer.sortingOrder = iceCubeSpriteRenderer.sortingOrder + 10;
        canvas.sortingOrder = iceCubeSpriteRenderer.sortingOrder + 10;
    }

    /// <summary>
    /// OnDrag calls this method.
    /// </summary>
    /// <param name="eventData"></param>
    public void BeingMovedOnMouse(PointerEventData eventData)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, 10f));

        dragSpriteRenderer.gameObject.transform.position = worldPosition + unitSettings.OffsetDragOverOther;
        canvas.transform.position = worldPosition + unitSettings.OffsetDragOverOther;
    }

    /// <summary>
    /// OnPointerUp calls this method.
    /// </summary>
    /// <param name="eventData"></param>
    public void BeingReleased(PointerEventData eventData)
    {
        dragSpriteRenderer.gameObject.transform.localPosition = Vector3.zero;
        dragSpriteRenderer.gameObject.transform.localScale = originalScale;
        dragSpriteRenderer.sortingOrder = originalSortingOrder;
        canvas.sortingOrder = originalSortingOrder;
        canvas.transform.localPosition = Vector3.zero;

        //heartIcon.gameObject.SetActive(true);
        //attackIcon.gameObject.SetActive(true);
        //energyIcon.gameObject.SetActive(true);
        //health.enabled = true;
        //attack.enabled = true;
        //energy.enabled = true;
    }

    #endregion


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
    /// Sets the activity of the repair panels.
    /// </summary>
    /// <param name="_panel2"></param>
    /// <param name="_panel3"></param>
    public void SetRepairPanelActive(bool _panel2, bool _panel3)
    {
        repairPanelHp2.SetActive(_panel2);
        repairPanelHp3.SetActive(_panel3);

        repairPanelAtk2.SetActive(_panel2);
        repairPanelAtk3.SetActive(_panel3);
    }

    /// <summary>
    /// Sets the activity of the repair steps.
    /// </summary>
    /// <param name="_fill1"></param>
    /// <param name="_fill2"></param>
    /// <param name="_fill3"></param>
    public void SetRepairStepFillActive(bool _fill1, bool _fill2, bool _fill3)
    {
        repairStepFillHp1.SetActive(_fill1);
        repairStepFillHp2.SetActive(_fill2);
        repairStepFillHp3.SetActive(_fill3);

        repairStepFillAtk1.SetActive(_fill1);
        repairStepFillAtk2.SetActive(_fill2);
        repairStepFillAtk3.SetActive(_fill3);
    }

    /// <summary>
    /// Flips the sprite and push the level display on the right side.
    /// </summary>
    public void SetRightSide()
    {
        dragSpriteRenderer.flipX = true;

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
        StartCoroutine(HideDamage());
    }

    /// <summary>
    /// Hides damage.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideDamage()
    {
        yield return new WaitForSeconds(unitSettings.DurationShowDamage);

        damage.enabled = false;
    }

    /// <summary>
    /// Shows the buff.
    /// </summary>
    /// <param name="_health"></param>
    /// <param name="_attack"></param>
    /// <param name="_energy"></param>
    public void ShowBuff(int _health, int _attack, int _energy)
    {
        buffHealth.text = _health.ToString();
        buffHealth.enabled = _health > 0;

        buffAttack.text = _attack.ToString();
        buffAttack.enabled = _attack > 0;

        addEnergy.text = _energy.ToString();
        addEnergy.enabled = _energy > 0;

        StartCoroutine(HideBuff());
    }

    /// <summary>
    /// Hides the buff.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideBuff()
    {
        yield return new WaitForSeconds(unitSettings.DurationShowTemporaryValue);
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
        yield return new WaitForSeconds(unitSettings.DurationShowTemporaryValue);
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

}
