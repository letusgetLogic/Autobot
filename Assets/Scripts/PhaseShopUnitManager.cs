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

    public GameObject AttachedGameObject { get; set; }
    public bool IsCheckingAttachedToDrop {  get; set; }

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

            shopUnitsSlots[i].GetComponent<Slot>().GameObjectIsOnMe = unit;
        }

        GameManager.Instance.SetShopPhase();
    }

    /// <summary>
    /// Transports the attached game object to the drop slot.
    /// </summary>
    /// <param name="dropParent"></param>
    public void TransportAttachedTo(Transform dropParent)
    {
        var slot = AttachedGameObject.transform.parent;
        slot.GetComponent<Slot>().GameObjectIsOnMe = null;

        AttachedGameObject.transform.SetParent(dropParent, false);

        AttachedGameObject = null;
    }
    public void TriggerStartOfTurn()
    {

    }
}
