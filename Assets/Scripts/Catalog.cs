using TMPro;
using UnityEngine;

public class Catalog : MonoBehaviour
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
    /// Sets the active state of all contained components.
    /// </summary>
    /// <param name="_isActive">true to activate the components; false to deactivate them.</param>
    public void SetActive(bool _isActive)
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
    public void SetData(SoUnit _data)
    {
        SetActive(true);

        myName.text = _data.name;
        attack.text = _data.Attack.ToString();
        health.text = _data.Health.ToString();
        ability1.text = _data.Levels[0].Description;
        ability2.text = _data.Levels[1].Description;
        ability3.text = _data.Levels[2].Description;

        if (_data.Levels[0].ConsumedEnergy.Value > 0)
        {
            consumedEnergy1.text = _data.Levels[0].ConsumedEnergy.ToString();
            energyIcon1.SetActive(true);
        }
        else
        {
            energyIcon1.SetActive(false);
        }

        if (_data.Levels[1].ConsumedEnergy.Value > 0)
        {
            consumedEnergy1.text = _data.Levels[1].ConsumedEnergy.ToString();
            energyIcon1.SetActive(true);
        }
        else
        {
            energyIcon1.SetActive(false);
        }

        if (_data.Levels[2].ConsumedEnergy.Value > 0)
        {
            consumedEnergy1.text = _data.Levels[2].ConsumedEnergy.ToString();
            energyIcon1.SetActive(true);
        }
        else
        {
            energyIcon1.SetActive(false);
        }
    }
}
