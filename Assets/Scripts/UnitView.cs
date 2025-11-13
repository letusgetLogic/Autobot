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
    [SerializeField] private GameObject iceCube;
    public SpriteRenderer Shadow => shadowSpriteRenderer;
    public GameObject IceCube => iceCube;

    [Header("Description")]
    [SerializeField] private GameObject description;
    [SerializeField] private TextMeshProUGUI
        myName,
        ability,
        coin,
        energyConsumption;

    [Header("Level Display")]
    [SerializeField] private GameObject levelDisplay;
    [SerializeField] private TextMeshProUGUI levelAmount;
    [SerializeField] private GameObject
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
    [SerializeField] private TextMeshProUGUI
        health,
        attack,
        energy,
        damage,
        buffHealth,
        buffAttack;

    [Header("Repair Display")]
    [SerializeField] private GameObject repairDisplay;
    [SerializeField] private GameObject 
        repairStep1,
        repairStep2,
        repairStep3;

    [Header("Settings")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private SoUnitSettings unitSettings;

    [Header("Color")]
    [SerializeField] private Color substractColor;
    [SerializeField] private Color addColor;
    public float DelayUpdateLevel => unitSettings.DelayUpdateLevel;

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
    public void SetData(string _description)
    {
        ability.text = "          " + _description;
        energyConsumption.text = PackManager.Instance.MyPack.
            EnergyConsumption.Value.ToString();
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetData(int _coin, bool _isForBuying)
    {
        coin.text = _coin.ToString();
        coin.color = _isForBuying ? substractColor : addColor;
    }

    /// <summary>
    /// Sets the data for the unit view.
    /// </summary>
    public void SetData(int _health, int _attack, int _energy)
    {
        health.text = _health.ToString();
        attack.text = _attack.ToString();
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
         repairDisplay.SetActive(value);
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
    public void SetRepairStepActive(bool step1, bool step2,bool step3)
    {
        repairStep1.SetActive(step1);
        repairStep2.SetActive(step2);
        repairStep3.SetActive(step3);
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
}
