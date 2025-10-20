using UnityEngine;
using UnityEngine.Rendering;

public class PhaseShopUnitManager : MonoBehaviour
{
    public static PhaseShopUnitManager Instance { get; private set; }

    [SerializeField]
    private GameObject unitPrefab;

    [Tooltip("Slots")]
    [SerializeField]
    private GameObject[]
        battleSlots,
        shopUnitSlots,
        extraShopUnitsSlots,
        shopItemsSlots,
        extraShopItemsSlots;

    private Slot[] battleSlotScripts { get; set; }
    public Slot[] BattleSlots => battleSlotScripts;
    private Slot[] shopUnitSlotScripts { get; set; }
    public Slot[] ShopUnitSlots => shopUnitSlotScripts;

    [Tooltip("Delay pushing other unit to make space")]
    [SerializeField]
    private float delayPushing = .5f;
    public float DelayPushing => delayPushing;

    [Tooltip("Delay pushing other unit to make space, while a fusion between 2 units is possible")]
    [SerializeField]
    private float delayPushingFusion = 1f;
    public float DelayPushingFusion => delayPushingFusion;

    public GameObject AttachedGameObject { get; set; }

    public bool IsDragging { get; set; } = false;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    /// <summary>
    /// Initializes the slots.
    /// </summary>
    /// <param name="player"></param>
    public void Initialize(PlayerData player)
    {
       battleSlotScripts = InitializeArray( battleSlots);
       shopUnitSlotScripts = InitializeArray(shopUnitSlots);

        if (player.BattleUnits != null)
        {
            for (int i = 0; i < player.BattleUnits.Length; i++)
            {
                var unit = player.BattleUnits[i];
                if (unit != null)
                {
                    unit.transform.SetParent(battleSlots[i].transform, false);
                    unit.transform.localPosition = Vector3.zero;
                }
            }
        }
        if (player.FreezedUnits != null)
        {
            for (int i = 0; i < player.FreezedUnits.Length; i++)
            {
                var unit = player.FreezedUnits[i];
                if (unit != null)
                {
                    unit.transform.SetParent(shopUnitSlots[i].transform, false);
                    unit.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    /// <summary>
    /// Initializes array.
    /// </summary>
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
    public void SpawnShopUnits()
    {
        for (int i = 0; i < shopUnitSlots.Length; i++)
        {
            var unitController = shopUnitSlotScripts[i].UnitController();
            if (unitController != null)
            {
                if (unitController.Model.ManageState == UnitState.Freezed)
                    continue;

                Destroy(unitController.gameObject);
            }

            var unitData = GameManager.Instance.CurrentGame.AvaiableUnits
                [UnityEngine.Random.Range(0, GameManager.Instance.CurrentGame.AvaiableUnits.Count)];

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

            PhaseShopUI.Instance.SetButtonActive(UnitState.None);
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
    /// <param name="attached"></param>
    /// <param name="dropSlot"></param>
    /// <param name="mouseRelease"> unitView.BeingReleased(null); </param>
    /// <param name="disableShadow">  unitView.Shadow.enabled = false;</param>
    public void Transport(GameObject attached, Transform dropSlot,
        bool mouseRelease, bool disableShadow)
    {
        if (attached == null || 
            attached.GetComponent<UnitController>().Model.ManageState == UnitState.Freezed)
            return;

        attached.transform.parent.GetComponent<Slot>().Border.enabled = false;
        attached.transform.SetParent(null);

        attached.transform.SetParent(dropSlot, false);
        attached.transform.localPosition = Vector3.zero;

        if (attached.CompareTag("Unit"))
        {
            var unitView = attached.GetComponent<UnitView>();
            var controller = attached.GetComponent<UnitController>();

            if (mouseRelease)
                unitView.BeingReleased(null);

            if (disableShadow)
                unitView.Shadow.enabled = false;

            controller.Model.ManageState = UnitState.InSlotBattle;
            controller.UpdateData(false);
        }
    }

    /// <summary>
    /// Checks fusible between 2 units.
    /// </summary>
    /// <param name="onSlot"></param>
    /// <param name="onDrag"></param>
    /// <returns></returns>
    public bool IsFusible(UnitController onSlot, UnitController onDrag)
    {
        if (onSlot.Model.IsMaxed ||
            onDrag.Model.IsMaxed)
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
