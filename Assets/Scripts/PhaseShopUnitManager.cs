using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PhaseShopUnitManager : MonoBehaviour
{
    public static PhaseShopUnitManager Instance { get; private set; }

    [Header("Slots")]
    [SerializeField]
    private Slot[] battleSlots;
    [SerializeField]
    private Slot[] shopUnitSlots;
    public Slot[] BattleSlots => battleSlots;
    public Slot[] ShopUnitSlots => shopUnitSlots;

    [Header("Settings")]
    [Tooltip("Delay pushing other unit to make space")]
    [SerializeField]
    private float delayPushing = .1f;
    public float DelayPushing => delayPushing;

    [Tooltip("Delay pushing other unit to make space, while a fusion between 2 units is possible")]
    [SerializeField]
    private float delayPushingFusion = 1f;
    public float DelayPushingFusion => delayPushingFusion;
    public Player Player { get; private set; }
    public GameObject AttachedGameObject { get; set; } // save the reference of unit being dragged or clicked
    public bool PreventDragging { get; set; } = false;
    // to prevent dragging from a slot after an another unit is pushed there, while mouse is still holding down.

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        SetIndex(battleSlots);
        SetIndex(shopUnitSlots);
    }

    /// <summary>
    /// Set index depend on draw order.
    /// </summary>
    private void SetIndex(Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Index = i;
    }

    /// <summary>
    /// Initializes the slots.
    /// </summary>
    /// <param name="player"></param>
    public void Initialize(Player player)
    {
        Player = player;
        SpawnUnits();
    }


    #region Spawn objects

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
                if (unitModel == null)
                    continue;

                SpawnManager.Instance.Spawn(
                    PackManager.Instance.Units[unitModel.Index],
                    unitModel.Index,
                    unitModel,
                    unitModel.UnitState,
                    battleSlots[i].transform);

            }
        }

        if (Player.Data.ShopUnitModels != null)
        {
            for (int i = 0; i < Player.Data.ShopUnitModels.Length; i++)
            {
                var unitModel = Player.Data.ShopUnitModels[i];
                if (unitModel == null)
                    continue;

                SpawnManager.Instance.Spawn(
                    PackManager.Instance.Units[unitModel.Index],
                    unitModel.Index,
                    unitModel,
                    unitModel.UnitState,
                    shopUnitSlots[i].transform);
            }
        }
    }

    /// <summary>
    /// Spawns units in the shop slots.
    /// </summary>
    public void SpawnShopUnits()
    {
        for (int i = 0; i < shopUnitSlots.Length; i++)
        {
            var unitController = shopUnitSlots[i].UnitController();
            if (unitController != null)
            {
                if (unitController.Model.UnitState == UnitState.Freezed)
                    continue;

                Destroy(unitController.gameObject);
            }

            int rand = Random.Range(0, PackManager.Instance.Units.Count);
            var data = PackManager.Instance.Units[rand];

            SpawnManager.Instance.Spawn(
                data,
                rand,
                null,
                UnitState.InSlotShop,
                shopUnitSlots[i].transform);
        }

        GameManager.Instance.SetPhaseShop();
    }

    #endregion


    public void ManageAttachedObject(Slot slot)
    {
        if (AttachedGameObject == null)
            return;

        var attachedModel = AttachedGameObject.GetComponent<UnitController>().Model;

        if (attachedModel.UnitState == UnitState.InSlotShop)
        {
            PhaseShopUnitManager.Instance.Buy(slot);
        }
        else if (attachedModel.UnitState == UnitState.InSlotBattle)
        {
            PhaseShopUnitManager.Instance.TransportOrFusion(slot);
        }
    }


    public void Buy(Slot slot)
    {
        var unitOnSlot = slot.UnitController();
        var attachedController = AttachedGameObject.GetComponent<UnitController>();

        if (PhaseShopUI.Instance.Player.Data.Coins < attachedController.SoUnit.Cost.Value) // case: buy but not enough coins.
        {
            PhaseShopUI.Instance.HintNotEnoughCoins();
            return;
        }

        if (unitOnSlot != null)
        {
            if (PhaseShopUnitManager.Instance.IsFusible(unitOnSlot, attachedController)) // case: buy, units are fusible.
            {
                PhaseShopUI.Instance.UpdateCoin(-attachedController.SoUnit.Cost.Value);
                unitOnSlot.UpdateLevel(attachedController.Model, true);
                Destroy(AttachedGameObject);
            }
        }
        else // case: buy and place dragging unit on empty slot.
        {
            PhaseShopUI.Instance.UpdateCoin(-attachedController.SoUnit.Cost.Value);
            Transport(AttachedGameObject, slot.transform, true, true);
        }
    }

    public void TransportOrFusion(Slot slot)
    {
        var unitOnSlot = slot.UnitController();
        var attachedController = AttachedGameObject.GetComponent<UnitController>();

        if (unitOnSlot != null)
        {
            if (unitOnSlot == attachedController) // attached unit shouldn't be unit on the slot.
                return;

            if (PhaseShopUnitManager.Instance.IsFusible(unitOnSlot, attachedController)) // case: only fusion.
            {
                unitOnSlot.UpdateLevel(attachedController.Model, true);
                Destroy(AttachedGameObject);
            }
        }
        else // case: move dragging unit to empty slot.
        {
            PhaseShopUnitManager.Instance.Transport(AttachedGameObject, slot.transform, true, true);
        }
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
            attached.GetComponent<UnitController>().Model.UnitState == UnitState.Freezed)
            return;

        attached.transform.parent.GetComponent<Slot>().Border.enabled = false;
        attached.transform.SetParent(null);

        attached.transform.SetParent(dropSlot, false);
        attached.transform.localPosition = Vector3.zero;

        var unitView = attached.GetComponent<UnitView>();
        var controller = attached.GetComponent<UnitController>();

        if (mouseRelease)
            unitView.BeingReleased(null);

        if (disableShadow)
            unitView.Shadow.enabled = false;

        controller.Model.SetData(UnitState.InSlotBattle);
        controller.View.SetData(controller.CurrentLevel.Sell);

        Player.UpdateUnitData();
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

        while (search >= 0 && search < battleSlots.Length)
        {
            if (battleSlots[search].Unit() != null &&
                battleSlots[search].Unit() != AttachedGameObject) // slot is occupied
            {
                search += direction; // continue search for an emnpty space
            }
            else // search is on empty slot
            {
                for (int empty = search; empty != target; empty -= direction)
                {
                    int previous = empty - direction; // swap the previous slot index to the empty slot index

                    var movedUnit = battleSlots[previous].Unit();
                    if (movedUnit == null ||
                        movedUnit == AttachedGameObject) // unit being moved is null or self, break foe loop
                        break;
                    Transport(movedUnit, battleSlots[empty].transform, false, false);
                }

                if (AttachedGameObject.GetComponent<UnitController>().Model.UnitState
                    == UnitState.InSlotBattle)
                {
                    Transport(AttachedGameObject, battleSlots[target].transform, false, false);
                    AttachedGameObject.GetComponent<UnitView>().BeingReleased(null);
                    PhaseShopUnitManager.Instance.SetAttachedGameObject(null);

                    PreventDragging = true;
                }
                else if (AttachedGameObject.GetComponent<UnitController>().Model.UnitState
                    == UnitState.InSlotShop)
                {
                    battleSlots[target].Border.enabled = true;
                }

                return;
            }
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
        if (onSlot.IsMaxed || onDrag.IsMaxed)
            return false;

        if (onSlot.name == onDrag.name)
            return true;

        return false;
    }

    /// <summary>
    /// Sets attached game object being clicked or dragged.
    /// </summary>
    /// <param name="target"></param>
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
}
