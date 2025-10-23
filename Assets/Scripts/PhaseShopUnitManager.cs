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
    public Player Player { get; private set; }
    public GameObject AttachedGameObject { get; set; }
    public bool StopDragging { get; set; } = false;

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
    public void Initialize(Player player)
    {
        Player = player;
        battleSlotScripts = InitializeArray(battleSlots);
        shopUnitSlotScripts = InitializeArray(shopUnitSlots);
        SpawnUnits();
    }

    /// <summary>
    /// Instantiate prefab and initialize it with data.
    /// </summary>
    private void SpawnUnits()
    {
        if (Player.Data.BattleUnitModels != null)
        {
            for (int i = 0; i < Player.Data.BattleUnitModels.Length; i++)
            {
                var unitModel = Player.Data.BattleUnitModels[i];
                if (unitModel.Index == -1)
                    continue;

                var unit = Instantiate(unitPrefab);
                unit.transform.SetParent(battleSlots[i].transform, false);

                var controller = unit.GetComponent<UnitController>();
                controller.Initialize(
                    StarterPack.Instance.Units[unitModel.Index], unitModel.Index, unitModel);

            }
        }
        
        if (Player.Data.ShopUnitModels != null)
        {
            for (int i = 0; i < Player.Data.ShopUnitModels.Length; i++)
            {
                var unitModel = Player.Data.ShopUnitModels[i];
                if (unitModel.Index == -1)
                    continue;

                var unit = Instantiate(unitPrefab);
                unit.transform.SetParent(shopUnitSlots[i].transform, false);

                var controller = unit.GetComponent<UnitController>();
                controller.Initialize(
                    StarterPack.Instance.Units[unitModel.Index], unitModel.Index, unitModel);
            }
        }
        else
        {
            for (int i = 0; i < shopUnitSlots.Length; i++)
            {
                var unit = Instantiate(unitPrefab);
                unit.transform.SetParent(battleSlots[i].transform, false);

                int rand = Random.Range(0, StarterPack.Instance.Units.Count);
                var data = StarterPack.Instance.Units[rand];

                var controller = unit.GetComponent<UnitController>();
                controller.Initialize(data, rand, null);
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
            int index = UnityEngine.Random.Range(0, StarterPack.Instance.Units.Count);
            var unitData = StarterPack.Instance.Units[index];

            GameObject unit = Instantiate(unitPrefab);
            //unit.GetComponent<UnitController>().Initialize(unitData, index);

            unit.transform.SetParent(shopUnitSlots[i].transform, false);
        }

        GameManager.Instance.SetPhaseShop();
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

            Player.UpdateUnitData();
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
        if (onSlot.IsMaxed ||
            onDrag.IsMaxed)
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
            if (battleSlotScripts[search].Unit() != null && 
                battleSlotScripts[search].Unit() != AttachedGameObject) // slot is occupied
            {
                search += direction; // continue search for an emnpty space
            }
            else // search is on empty slot
            {
                for (int empty = search; empty != target; empty -= direction)
                {
                    int previous = empty - direction; // swap the previous slot to the empty slot

                    var movedUnit = battleSlotScripts[previous].Unit();
                    if (movedUnit == null || 
                        movedUnit == AttachedGameObject)
                        break;
                    Transport(movedUnit, battleSlots[empty].transform, false, false);
                }

                if (AttachedGameObject.GetComponent<UnitController>().Model.ManageState
                    == UnitState.InSlotBattle)
                {
                    Transport(AttachedGameObject, battleSlots[target].transform, false, false);
                    AttachedGameObject.GetComponent<UnitView>().BeingReleased(null);

                    StopDragging = true;
                }
                return;
            }
        }
    }

}
