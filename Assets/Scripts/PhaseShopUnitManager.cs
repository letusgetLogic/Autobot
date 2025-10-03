using System;
using UnityEngine;

public class PhaseShopUnitManager : MonoBehaviour
{
    public static PhaseShopUnitManager Instance { get; private set; }

    [SerializeField]
    private GameObject unitPrefab;

    [SerializeField]
    private GameObject[] 
        battleSlots,
        shopUnitsSlots,
        extraShopUnitsSlots,
        shopItemsSlots,
        extraShopItemsSlots; 

    private Vector2[] 
        battleSlotPos,
        shopUnitsSlotsPos,
        extraShopUnitsSlotsPos,
        shopItemsSlotsPos,
        extraShopItemsSlotsPos;

    /// <summary>
    /// Awake method.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
             Destroy(Instance.gameObject);
        }
        Instance = this;

        Initialize();
    }

    /// <summary>
    /// Initializes the positions once.
    /// </summary>
    private void Initialize()
    {
        battleSlotPos = new Vector2[battleSlots.Length];
        for (int i = 0; i < battleSlots.Length; i++)
            battleSlotPos[i] = battleSlots[i].transform.position;

        shopUnitsSlotsPos = new Vector2[shopUnitsSlots.Length];
        for (int i = 0; i < shopUnitsSlots.Length; i++)
            shopUnitsSlotsPos[i] = shopUnitsSlots[i].transform.position;

        extraShopUnitsSlotsPos = new Vector2[extraShopUnitsSlots.Length];
        for (int i = 0; i < extraShopUnitsSlots.Length; i++)
            extraShopUnitsSlotsPos[i] = extraShopUnitsSlots[i].transform.position;

        shopItemsSlotsPos = new Vector2[shopItemsSlots.Length];
        for (int i = 0; i < shopItemsSlots.Length; i++)
            shopItemsSlotsPos[i] = shopItemsSlots[i].transform.position;

        extraShopItemsSlotsPos = new Vector2[extraShopItemsSlots.Length];
        for (int i = 0; i < extraShopItemsSlots.Length; i++)
            extraShopItemsSlotsPos[i] = extraShopItemsSlots[i].transform.position;
    }

    /// <summary>
    /// Spawns units in the shop slots.
    /// </summary>
    public void SpawnUnits()
        {
        for (int i = 0; i < shopUnitsSlots.Length; i++)
        {
            var existingUnit = shopUnitsSlots[i].GetComponentInChildren<UnitController>();
            if (existingUnit != null)
            {
                Destroy(existingUnit.gameObject);
            }

            var unitData = GameManager.Instance.AvaiableUnits
                [UnityEngine.Random.Range(0, GameManager.Instance.AvaiableUnits.Count)];

            GameObject unit = Instantiate(unitPrefab);

            unit.transform.SetParent(shopUnitsSlots[i].transform, false);

            unit.GetComponent<UnitController>().Initialize(unitData);
        }

        GameManager.Instance.SetShopPhase();
    }
    public void TriggerStartOfTurn()
    {

    }
}
