using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitView : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private GameObject[] hideVisuals;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;
    [SerializeField] private SpriteRenderer dragSpriteRenderer;
    [SerializeField] private SpriteRenderer iceCubeSpriteRenderer;
    public SpriteRenderer Shadow => shadowSpriteRenderer;
    public GameObject IceCube => iceCubeSpriteRenderer.gameObject;

    [Header("Description")]
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject[] hideDuringBattle;
    [SerializeField]
    private TextMeshProUGUI
        myName,
        energyConsumption,
        ability,
        fullAttack,
        fullHealth;

    [Header("Craft / Recycle")]
    [SerializeField] private GameObject coin;
    [SerializeField] private GameObject tool;
    [SerializeField]
    private TextMeshProUGUI
        craftText,
        coinValue,
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

    [Header("Stats")]
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
        buffAttack;

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

    private Camera mainCamera;

    private Vector3 originalScale;

    private int originalSortingOrder;

    #endregion

    private void Awake()
    {
        description.SetActive(false);
        iceCubeSpriteRenderer.gameObject.SetActive(false);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        originalScale = dragSpriteRenderer.gameObject.transform.localScale;
        originalSortingOrder = dragSpriteRenderer.sortingOrder;
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetData(Sprite _sprite, string _name)
    {
        dragSpriteRenderer.sprite = _sprite;
        shadowSpriteRenderer.sprite = _sprite;
        myName.text = _name;
        gameObject.name = _name;
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetAbility(string _description, int _energy)
    {
        ability.text = _description;
        energyConsumption.text = _energy.ToString();
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetBuyOrSell(Currency _cur, bool _isForBuying)
    {
        craftText.text = _isForBuying ? "craft" : "recycle";

        if (_cur.Coin != 0)
        {
            string operation = _cur.Coin > 0 ? "+" : "";
            coinValue.text = operation + _cur.Coin.ToString();
            coin.SetActive(true);
        }
        else
        {
            coinValue.text = "";
            coin.SetActive(false);
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
    /// <param name="value"></param>
    public void SetDescriptionActive(bool value)
    {
        description.SetActive(value);
    }

    public void SetRepairDisplayActive(bool value)
    {
        repairDisplayHp.SetActive(value);
        repairDisplayAtk.SetActive(value);
    }

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
            heartIcon.gameObject.SetActive(false);
            attackIcon.gameObject.SetActive(false);
            energyIcon.gameObject.SetActive(false);
            health.enabled = false;
            attack.enabled = false;
            energy.enabled = false;
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
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
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

        heartIcon.gameObject.SetActive(true);
        attackIcon.gameObject.SetActive(true);
        energyIcon.gameObject.SetActive(true);
        health.enabled = true;
        attack.enabled = true;
        energy.enabled = true;
    }

    #endregion


    /// <summary>
    /// Sets the xp step components active.
    /// </summary>
    /// <param name="box1"></param>
    /// <param name="box2"></param>
    /// <param name="step1"></param>
    /// <param name="step2"></param>
    /// <param name="box3"></param>
    /// <param name="step3"></param>
    /// <param name="step4"></param>
    /// <param name="step5"></param>
    public void SetXpStepActive(
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
    public void SetRepairStepActive(bool _panel2, bool _panel3)
    {
        repairPanelHp2.SetActive(_panel2);
        repairPanelHp3.SetActive(_panel3);

        repairPanelAtk2.SetActive(_panel2);
        repairPanelAtk3.SetActive(_panel3);
    }

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

    public void ShowBuff(int _health, int _attack)
    {
        buffHealth.text = _health.ToString();
        buffHealth.enabled = _health > 0;

        buffAttack.text = _attack.ToString();
        buffAttack.enabled = _attack > 0;

        StartCoroutine(HideBuff());
    }

    private IEnumerator HideBuff()
    {
        yield return new WaitForSeconds(unitSettings.DurationShowBuff);
        buffHealth.enabled = false;
        buffAttack.enabled = false;
    }

    public void HideVisuals()
    {
        foreach (var go in hideVisuals)
            go.SetActive(false);
    }


    public void HideDescriptionStats()
    {
        foreach (var element in hideDuringBattle)
            element.SetActive(false);
    }

}
