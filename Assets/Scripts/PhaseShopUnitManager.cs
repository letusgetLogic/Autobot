using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    private Slot[] InitializeArray(GameObject[] slots)
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
        }

        GameManager.Instance.SetShopPhase();
    }

    public void SetAttachedGameObject(GameObject target)
    {
        if (target == null)
        {
            if (AttachedGameObject != null)
                AttachedGameObject.transform.parent.
                    GetComponent<Slot>().Border.enabled = false;
        }
        else
        {
            target.transform.parent.GetComponent<Slot>().Border.enabled = true;
        }

        AttachedGameObject = target;
    }

    public void TriggerStartOfTurn()
    {

    }

    /// <summary>
    /// Transports the attached game object to the drop slot.
    /// </summary>
    public void Transport(GameObject attached, Transform dropSlot,
        bool mouseRelease, bool disableShadow)
    {
        if (attached == null)
            return;

        attached.transform.SetParent(null);

        attached.transform.SetParent(dropSlot, false);
        attached.transform.localPosition = Vector3.zero;

        if (attached.CompareTag("Unit"))
        {
            var unitView = attached.GetComponent<UnitView>();

            if (mouseRelease)
                unitView.BeingReleased(null);

            if (disableShadow)
                unitView.Shadow.enabled = false;
        }
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


    ///summary>
    /// Pushes the other units away.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="direction"></param>
    public void PushOtherAway(int target, int direction)
    {
        if (direction == 0)
            return;

        int search = target + direction;

        while (search >= 0 && search < battleSlotScripts.Length)
        {
            if (battleSlotScripts[search].Unit() != null) // slot is occupied
            {
                search += direction; // search for an emnpty space
            }
            else // slot is empty
            {
                for (int i = search; i != target; i -= direction)
                {
                    int previous = i - direction; // swap the previous slot to the empty slot

                    var attached = battleSlotScripts[previous].Unit();
                    if (attached == null)
                        break;
                    Transport(attached, battleSlots[i].transform, false, false);
                }
                return;
            }
        }
    }

}
