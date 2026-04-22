using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Catalog : MonoBehaviour, IPointerClickHandler
{
    public static Catalog Instance { get; private set; }

    [Header("Pack")]
    [SerializeField] private SoPack pack;

    [Header("Description References")]
    [SerializeField] private GameObject[] components;
    [SerializeField] private GameObject[] hideByItems;
    [SerializeField] private GameObject stats;
    [SerializeField] private GameObject cost;
    [SerializeField] private GameObject costTool;
    [SerializeField] private GameObject costNut;
    [SerializeField]
    private TextMeshProUGUI
        myName,
        modelID,
        attack,
        health,
        itemAbility,
        itemNut,
        costToolText,
        costNutText;


    [Header("Level References")]
    [SerializeField] private CatalogDescriptionLevel[] myLevels;

    private UnitController attachedController;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        SetActive(false);
    }

    /// <summary>
    /// OnActivate method to set the catalog active in the GameManager when the catalog is activated.
    /// </summary>
    private void OnEnable()
    {
        GameManager.Instance.IsCatalogActive = true;
    }

    /// <summary>
    /// OnDeactivate method to set the catalog inactive in the GameManager when the catalog is deactivated.
    /// </summary>
    private void OnDisable()
    {
        GameManager.Instance.IsCatalogActive = false;
    }

    /// <summary>
    /// Clicks on the catalog background to set the attached game object to null.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        SetAttachedGameObject(null);
    }

    /// <summary>
    /// Sets attached game object being clicked and shows/hides the description.
    /// </summary>
    /// <param name="_target"></param>
    public void SetAttachedGameObject(UnitController _target)
    {
        if (attachedController && attachedController.Slot)
            attachedController.Slot.SetIndicatorActive(false);

        if (_target && _target.enabled && _target.Slot)
            _target.Slot.SetIndicatorActive(true);

        attachedController = _target;

        if (_target && _target.enabled && _target.DefinedUnit)
        {
            SetActive(true);
            SetData(_target.DefinedUnit);
        }
        else
        {
            SetActive(false);
        }
    }

    /// <summary>
    /// Sets the active state of all contained components.
    /// </summary>
    /// <param name="_isActive">true to activate the components; false to deactivate them.</param>
    private void SetActive(bool _isActive)
    {
        foreach (var component in components)
        {
            component.SetActive(_isActive);
        }
    }


    /// <summary>
    /// Pass the data of the unit over.
    /// </summary>
    /// <param name="_soUnit"></param>
    private void SetData(SoUnit _soUnit)
    {
        myName.text = _soUnit.Name;
        modelID.text = _soUnit.ModelID == ""
            ? ""
            : "- Model " + _soUnit.ModelID + " -";

        // Special settings for items, as they don't have attack and health but have a unique ability and cost.
        if (_soUnit.UnitType == UnitType.Item)
        {
            foreach (var hide in hideByItems)
            {
                hide.SetActive(false);
            }

            itemAbility.gameObject.SetActive(true);
            itemAbility.text = _soUnit.Levels.Length > 0 ? _soUnit.Levels[0].Description : "";
            itemNut.text = _soUnit.UniqueCostNuts != null ? _soUnit.UniqueCostNuts.Value.ToString() : "";
            return;
        }

        // Settings for bots.

        itemAbility.gameObject.SetActive(false);

        foreach (var hide in hideByItems)
        {
            hide.SetActive(true);
        }

        if (_soUnit.Attack <= 0 && _soUnit.Health <= 0)
        {
            stats.SetActive(false);
        }
        else
        {
            stats.SetActive(true);
            attack.text = _soUnit.Attack.ToString();
            health.text = _soUnit.Health.ToString();
        }

        var unitCost = pack.CurrencyData.UnitCost;
        if (unitCost.SoTool == 0 && unitCost.Nut == 0)
        {
            cost.SetActive(false);
        }
        else
        {
            cost.SetActive(true);
            costTool.SetActive(unitCost.SoTool < 0);
            costToolText.text = unitCost.SoTool.ToString();
            costNut.SetActive(unitCost.Nut < 0);
            costNutText.text = unitCost.Nut.ToString();
        }

        // Settings for levels.
        if (_soUnit.Levels.Length > 0)
        {
            for (int i = 0; i < myLevels.Length; i++)
            {
                if (i >= _soUnit.Levels.Length)
                {
                    myLevels[i].gameObject.SetActive(false);
                    continue;
                }

                myLevels[i].gameObject.SetActive(true);
                myLevels[i].SetData(_soUnit, pack.CurrencyData, i);
            }
        }
        else
        {
            for (int i = 0; i < myLevels.Length; i++)
            {
                myLevels[i].gameObject.SetActive(false);
            }
        }
    }
}
