using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Catalog : MonoBehaviour, IPointerClickHandler
{
    public static Catalog Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject[] components;
    [SerializeField]
    private TextMeshProUGUI
        myName,
        modelID,
        attack,
        health;

    [SerializeField] private GameObject stats;

    [SerializeField] private GameObject[] myLevels;

    [SerializeField] private TextMeshProUGUI[] abilitys;

    [SerializeField] private GameObject[] energyIcons;

    [SerializeField] private TextMeshProUGUI[] consumedEnergys;

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
    /// <param name="_data"></param>
    private void SetData(SoUnit _data)
    {
        myName.text = _data.Name;
        modelID.text = _data.ModelID == ""
            ? ""
            : "- Model " + _data.ModelID + " -";

        if (_data.Attack <= 0 && _data.Health <= 0)
        {
            stats.SetActive(false);
        }
        else
        {
            stats.SetActive(true);
            attack.text = _data.Attack.ToString();
            health.text = _data.Health.ToString();
        }

        if (_data.Levels.Length > 0)
        {
            for (int i = 0; i < myLevels.Length; i++)
            {
                if (i >= _data.Levels.Length)
                {
                    myLevels[i].SetActive(false);
                    continue;
                }

                myLevels[i].SetActive(true);
                abilitys[i].text = _data.Levels[i].Description;

                if (_data.Levels[i].ConsumedEnergy && _data.Levels[i].ConsumedEnergy.Value < 0)
                {
                    energyIcons[i].SetActive(true);
                    consumedEnergys[i].text = _data.Levels[i].ConsumedEnergy.Value.ToString();
                }
                else
                {
                    energyIcons[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < myLevels.Length; i++)
            {
                myLevels[i].SetActive(false);
            }
        }
    }
}
