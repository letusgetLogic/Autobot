using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PhaseShopUnitManager : MonoBehaviour
{
    public static PhaseShopUnitManager Instance { get; private set; }

    [Header("Slots")]
    [SerializeField]
    private Slot[] teamSlots;
    [SerializeField]
    private Slot[] shopUnitSlots;
    public Slot[] TeamSlots => teamSlots;
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

    public GameObject AttachedGameObject { get; set; }
    // Save the reference of unit being dragged or clicked
    public bool IsDragging { get; set; } = false;
    public bool PreventDragging { get; set; } = false;
    // to prevent dragging from a slot after an another unit is pushed in there,
    // while mouse is still holding down.

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        SetIndex(teamSlots);
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
        if (Player.Data.TeamUnitDatas != null)
        {
            for (int i = 0; i < Player.Data.TeamUnitDatas.Length; i++)
            {
                var unitData = Player.Data.TeamUnitDatas[i];
                if (unitData.HasReference != true)
                    continue;

                SpawnManager.Instance.Spawn(
                    PackManager.Instance.Units[unitData.Index],
                    unitData.Index,
                    unitData,
                    UnitState.InSlotTeam,
                    teamSlots[i].transform);

            }
        }

        if (Player.Data.ShopUnitDatas != null)
        {
            for (int i = 0; i < Player.Data.ShopUnitDatas.Length; i++)
            {
                var unitData = Player.Data.ShopUnitDatas[i];
                if (unitData.HasReference != true)
                    continue;

                SpawnManager.Instance.Spawn(
                    PackManager.Instance.Units[unitData.Index],
                    unitData.Index,
                    unitData,
                    unitData.UnitState,
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
                if (unitController.Model.Data.UnitState == UnitState.Freezed)
                    continue;

                Destroy(unitController.gameObject);
            }

            if (PackManager.Instance.Units.Count == 0)
                return;

            int rand = Random.Range(0, PackManager.Instance.Units.Count);
            var data = PackManager.Instance.Units[rand];

            SpawnManager.Instance.Spawn(
                data,
                rand,
                new(),
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

        if (AttachedGameObject.CompareTag("Unit"))
        {
            var unitState = AttachedGameObject.GetComponent<UnitController>().Model.Data.UnitState;
            if (unitState == UnitState.InSlotShop || unitState == UnitState.Freezed)
            {
                Buy(slot);
            }
            else if (unitState == UnitState.InSlotTeam)
            {
                TransportOrFusion(slot);
            }
        }
    }


    private void Buy(Slot slot)
    {
        if (AttachedGameObject.CompareTag("Unit"))
        {
            var unitOnSlot = slot.UnitController();
            var attachedController = AttachedGameObject.GetComponent<UnitController>();

            // case: buy but not enough currency.
            if (!PhaseShopUI.Instance.HasEnoughCurrency(
                attachedController.Model.Cost.Coin, attachedController.Model.Cost.Tool))
            {
                return;
            }

            if (unitOnSlot != null)
            {
                // case: buy, units are fusible.
                if (IsFusible(unitOnSlot, attachedController))
                {
                    PhaseShopUI.Instance.UpdateCurrency(
                        attachedController.Model.Cost.Coin, attachedController.Model.Cost.Tool);

                    unitOnSlot.UpdateLevel(attachedController.Model, true);
                    Destroy(AttachedGameObject);
                }
                return;
            }
            else // case: buy and place dragging unit on empty slot.
            {
                PhaseShopUI.Instance.UpdateCurrency(
                        attachedController.Model.Cost.Coin, attachedController.Model.Cost.Tool);

                Transport(AttachedGameObject, slot.transform, true, true);
            }

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

            if (IsFusible(unitOnSlot, attachedController)) // case: only fusion.
            {
                unitOnSlot.UpdateLevel(attachedController.Model, true);
                Destroy(AttachedGameObject);
            }
        }
        else // case: move dragging unit to empty slot.
        {
            Transport(AttachedGameObject, slot.transform, true, true);
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
        if (attached == null)
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

        controller.Model.SetData(UnitState.InSlotTeam);
        controller.View.SetBuyOrSell(controller.Model.Sell, false);

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

        while (search >= 0 && search < teamSlots.Length)
        {
            if (teamSlots[search].Unit() != null &&
                teamSlots[search].Unit() != AttachedGameObject) // slot is occupied
            {
                search += direction; // continue search for an emnpty space
            }
            else // search is on empty slot
            {
                for (int empty = search; empty != target; empty -= direction)
                {
                    int previous = empty - direction; // swap the previous slot index to the empty slot index

                    var movedUnit = teamSlots[previous].Unit();
                    if (movedUnit == null ||
                        movedUnit == AttachedGameObject) // unit being moved is null or self, break foe loop
                        break;
                    Transport(movedUnit, teamSlots[empty].transform, false, false);
                }

                if (AttachedGameObject.GetComponent<UnitController>().Model.Data.UnitState
                    == UnitState.InSlotTeam)
                {
                    Transport(AttachedGameObject, teamSlots[target].transform, false, false);
                    AttachedGameObject.GetComponent<UnitView>().BeingReleased(null);
                    SetAttachedGameObject(null);

                    PreventDragging = true;
                }
                else if (AttachedGameObject.GetComponent<UnitController>().Model.Data.UnitState
                    == UnitState.InSlotShop)
                {
                    teamSlots[target].Border.enabled = true;
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
        if (onSlot.Model.IsMaxed || onDrag.Model.IsMaxed)
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
