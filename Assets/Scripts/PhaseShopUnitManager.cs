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
