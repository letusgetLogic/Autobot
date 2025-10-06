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
        shopUnitSlots,
        extraShopUnitsSlots,
        shopItemsSlots,
        extraShopItemsSlots;

    private Slot[] battleSlotScripts { get; set; }
    private Slot[] shopUnitSlotScripts { get; set; }

    [SerializeField]
    private float delayPushing = 1f;
    public float DelayPushing => delayPushing;

    public GameObject AttachedGameObject { get; set; }
    public bool IsCheckingAttachedToDrop { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        battleSlotScripts = InitializeArray(battleSlots);
        shopUnitSlotScripts = InitializeArray(shopUnitSlots);
    }

    /// <summary>
    /// Initializes array.
    /// </summary>
    /// <param name="slotScripts"></param>
    /// <param name="slots"></param>
    private Slot[] InitializeArray( GameObject[] slots)
    {
        var slotScripts = new Slot[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            slotScripts[i] = slots[i].GetComponent<Slot>();
            slotScripts[i].Index = i;
        }
        return slotScripts;
    }

    /// <summary>
    /// Spawns units in the shop slots.
    /// </summary>
    public void SpawnUnits()
    {
        for (int i = 0; i < shopUnitSlots.Length; i++)
        {
            var existingUnit = shopUnitSlots[i].GetComponentInChildren<UnitController>();
            if (existingUnit != null)
            {
                Destroy(existingUnit.gameObject);
            }

            var unitData = GameManager.Instance.AvaiableUnits
                [UnityEngine.Random.Range(0, GameManager.Instance.AvaiableUnits.Count)];

            GameObject unit = Instantiate(unitPrefab);
            unit.GetComponent<UnitController>().Initialize(unitData);

            unit.transform.SetParent(shopUnitSlots[i].transform, false);

            shopUnitSlotScripts[i].GameObjectIsOnMe = unit;
        }

        GameManager.Instance.SetShopPhase();
    }

    public void TriggerStartOfTurn()
    {

    }

    /// <summary>
    /// Transports the attached game object to the drop slot.
    /// </summary>
    public void Transport(GameObject attached, Transform dropSlot, bool disableShadow, bool isAttachedByMouse)
    {
        var parent = attached.transform.parent;

        if (parent != null && parent.CompareTag("Slot Battle") || parent.CompareTag("Slot Shop"))
           parent.GetComponent<Slot>().GameObjectIsOnMe = null;

        attached.transform.SetParent(dropSlot, false);
        dropSlot.GetComponent<Slot>().GameObjectIsOnMe = attached;

        if (disableShadow && attached.CompareTag("Unit"))
        {
            attached.GetComponent<UnitView>().Shadow.enabled = false;
        }

        if (isAttachedByMouse)
            AttachedGameObject = null;
    }

    /// <summary>
    /// Checks fusible between 2 units.
    /// </summary>
    /// <param name="onSlot"></param>
    /// <param name="onDrag"></param>
    /// <returns></returns>
    public bool IsFusible(GameObject onSlot, GameObject onDrag)
    {
        if (onSlot.GetComponent<UnitController>().IsMaxed() ||
            onDrag.GetComponent<UnitController>().IsMaxed())
            return false;

        if (onSlot.name == onDrag.name)
            return true;

        return false;
    }

    /// <summary>
    /// Pushes the other units away.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="direction"></param>
    public void PushOtherAway(int target, int direction)
    {
        int search = target + direction;
        while (search >= 0 && search < battleSlotScripts.Length)
        {
            if (battleSlotScripts[search].GameObjectIsOnMe != null)
            {
                search += direction;
            }
            else
            {
                int previous = search - direction;

                if(previous == target)
                    break;

                Transport(battleSlotScripts[previous].GameObjectIsOnMe, battleSlots[search].transform, false, false);
                search -= direction;
            }
                
        }
    }

}
