using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Catalog : MonoBehaviour, IPointerClickHandler
{
    public static Catalog Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject[] components;
    [SerializeField] private TextMeshProUGUI
        myName,
        attack,
        health,
        ability1,
        ability2,
        ability3,
        consumedEnergy1,
        consumedEnergy2,
        consumedEnergy3;
    [SerializeField] private GameObject energyIcon1;
    [SerializeField] private GameObject energyIcon2;
    [SerializeField] private GameObject energyIcon3;

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

    public void OnActivate()
    {
        GameManager.Instance.IsCatalogActive = true;
    }

    public void OnDeactivate()
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
    /// <param name="_data"></param>
    private void SetData(SoUnit _data)
    {
        SetActive(true);

        myName.text = _data.Name + " - Model " + _data.ModelID;
        attack.text = _data.Attack.ToString();
        health.text = _data.Health.ToString();
        ability1.text = _data.Levels[0].Description;
        ability2.text = _data.Levels[1].Description;
        ability3.text = _data.Levels[2].Description;

        if (_data.Levels[0].ConsumedEnergy && _data.Levels[0].ConsumedEnergy.Value < 0)
        {
            consumedEnergy1.text = _data.Levels[0].ConsumedEnergy.Value.ToString();
            energyIcon1.SetActive(true);
        }
        else
        {
            energyIcon1.SetActive(false);
        }

        if (_data.Levels[1].ConsumedEnergy && _data.Levels[1].ConsumedEnergy.Value < 0)
        {
            consumedEnergy2.text = _data.Levels[1].ConsumedEnergy.Value.ToString();
            energyIcon2.SetActive(true);
        }
        else
        {
            energyIcon2.SetActive(false);
        }

        if (_data.Levels[2].ConsumedEnergy && _data.Levels[2].ConsumedEnergy.Value < 0)
        {
            consumedEnergy3.text = _data.Levels[2].ConsumedEnergy.Value.ToString();
            energyIcon3.SetActive(true);
        }
        else
        {
            energyIcon3.SetActive(false);
        }
    }



}
