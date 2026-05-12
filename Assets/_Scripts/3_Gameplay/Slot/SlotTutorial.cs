using UnityEngine;

public class SlotTutorial : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] GameObject energyConsumptionIndicator;
    [SerializeField] GameObject abilityIndicator;
    [SerializeField] GameObject energyIndicator;
    [SerializeField] GameObject hintArrow;


    private Slot slot;


    private void Start()
    {
        slot = GetComponent<Slot>();
    }

    public GameObject EnergyConsumptionIndicator
    {
        get
        {
            return energyConsumptionIndicator;
        }
        set
        {
            energyConsumptionIndicator = value;
        }
    }

    public GameObject AbilityIndicator
    {
        get
        {
            return abilityIndicator;
        }
        set
        {
            abilityIndicator = value;
        }
    }

    public GameObject EnergyIndicator
    {
        get
        {
            return energyIndicator;
        }
        set
        {
            energyIndicator = value;
        }
    }

    public GameObject HintArrow
    {
        get
        {
            return hintArrow;
        }
        set
        {
            hintArrow = value;
        }
    }

    public bool HasSmokingRobot()
    {
        var unit = slot.UnitController();
        if (unit != null)
        {
            var isRobot = unit.IsRobot(unit.Model.Data.UnitType);
            if (isRobot)
            {
                return unit.Model.Data.Cur.HP <= 0;
            }
        }

        return false;
    }
}
