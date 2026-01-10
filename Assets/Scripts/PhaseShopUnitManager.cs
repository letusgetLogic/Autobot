using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhaseShopUnitManager : MonoBehaviour
{
    public static PhaseShopUnitManager Instance { get; private set; }

    public UnityAction OnSettingAttachedObject { get; set; }
    public UnityAction OnSettingNullObject { get; set; }

    [Header("Slots")]
    [SerializeField] private Slot[] teamSlots;
    [SerializeField] private Slot chargeSlot;
    [SerializeField] private Slot[] shopBotSlots;
    [SerializeField] private Slot[] shopItemSlots;
    public Slot ChargeSlot => chargeSlot;

    [Header("Settings")]
    [SerializeField] private SoShopProcess process;
    public SoShopProcess Process => process;

    public Player Player { get; private set; }

    public GameObject AttachedGameObject { get; set; }
    public UnitController TargetedController { get; set; }

    /// <summary>
    /// To block player's input while animation is running.
    /// </summary>
    public bool IsBlockingInput { get; set; } = false;

    /// <summary>
    /// To prevent pushing other away while an unit is attached by mouse click.
    /// </summary>
    public bool IsDragging { get; set; } = false;

    /// <summary>
    /// To prevent dragging from a slot after an another unit is pushed in there,
    /// while mouse is still holding down.
    /// </summary>
    public bool PreventDragging { get; set; } = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        Time.timeScale = 1f;

        SetIndex(teamSlots);
        SetIndex(shopBotSlots);
    }

    private void OnEnable()
    {
        OnSettingAttachedObject += () => SetDropHint(true);
        OnSettingNullObject += () => SetDropHint(false);
    }

    private void OnDisable()
    {
        OnSettingAttachedObject -= () => SetDropHint(true);
        OnSettingNullObject -= () => SetDropHint(false);
    }

    /// <summary>
    /// Set index depend on draw order.
    /// </summary>
    private void SetIndex(Slot[] _slots)
    {
        for (int i = 0; i < _slots.Length; i++)
            _slots[i].Index = i;
    }

    /// <summary>
    /// Initializes the slots.
    /// </summary>
    /// <param name="_player"></param>
    public void Initialize(Player _player)
    {
        Player = _player;
        IsBlockingInput = true;
    }

    // Spawn Objects
    #region Spawn objects

    /// <summary>
    /// Instantiate prefab and initialize it with data.
    /// </summary>
    public void SpawnSavedUnits()
    {
        // team bots
        if (Player.Data.TeamUnitDatas != null)
        {
            for (int i = 0; i < Player.Data.TeamUnitDatas.Length; i++)
            {
                var unitData = Player.Data.TeamUnitDatas[i];
                if (unitData.HasReference != true)
                    continue;
                Debug.Log("unitData.Index" + unitData.Index);
                SpawnManager.Instance.Spawn(
                    PackManager.Instance.GetSoUnit(unitData).soUnit,
                    PackManager.Instance.GetSoUnit(unitData).index,
                    unitData,
                    UnitState.InSlotTeam,
                    teamSlots[i].transform);
            }
        }

        // charging station bot
        var chargeUnitData = Player.Data.ChargeUnitData;
        if (chargeUnitData.HasReference)
        {
            SpawnManager.Instance.Spawn(
                PackManager.Instance.Bots[chargeUnitData.Index],
                chargeUnitData.Index,
                chargeUnitData,
                UnitState.InSlotCharge,
                chargeSlot.transform);
        }

        // shop bots
        if (Player.Data.ShopBotDatas != null)
        {
            for (int i = 0; i < Player.Data.ShopBotDatas.Length; i++)
            {
                var unitData = Player.Data.ShopBotDatas[i];
                if (unitData.HasReference != true)
                    continue;

                SpawnManager.Instance.Spawn(
                    PackManager.Instance.Bots[unitData.Index],
                    unitData.Index,
                    unitData,
                    unitData.UnitState,
                    shopBotSlots[i].transform);
            }
        }

        // shop items
        if (Player.Data.ShopItemDatas != null)
        {
            for (int i = 0; i < Player.Data.ShopItemDatas.Length; i++)
            {
                var unitData = Player.Data.ShopItemDatas[i];
                if (unitData.HasReference != true)
                    continue;

                SpawnManager.Instance.Spawn(
                    PackManager.Instance.Items[unitData.Index],
                    unitData.Index,
                    unitData,
                    unitData.UnitState,
                    shopItemSlots[i].transform);
            }
        }
    }

    /// <summary>
    /// Randomize scriptable objects and instantiate and initialize scripts of the unit.
    /// </summary>
    public void SpawnShopUnits()
    {
        // Spawn shop bots
        for (int i = 0; i < shopBotSlots.Length; i++)
        {
            if (shopBotSlots[i].gameObject.activeSelf == false)
            {
                continue;
            }

            var unitController = shopBotSlots[i].UnitController();
            if (unitController != null)
            {
                if (unitController.Model.Data.UnitState == UnitState.Freezed)
                    continue;

                Destroy(unitController.gameObject);
            }

            if (PackManager.Instance.Bots.Count == 0)
                return;

            int rand = Random.Range(0, PackManager.Instance.Bots.Count);
            var data = PackManager.Instance.Bots[rand];

            SpawnManager.Instance.Spawn(
                data,
                rand,
                new(),
                UnitState.InSlotShop,
                shopBotSlots[i].transform);
        }

        // Spwan shop items
        for (int i = 0; i < shopItemSlots.Length; i++)
        {
            if (shopItemSlots[i].gameObject.activeSelf == false)
            {
                continue;
            }

            var unitController = shopItemSlots[i].UnitController();
            if (unitController != null)
            {
                if (unitController.Model.Data.UnitState == UnitState.Freezed)
                    continue;

                Destroy(unitController.gameObject);
            }

            if (PackManager.Instance.Items.Count == 0)
                return;

            int rand = Random.Range(0, PackManager.Instance.Items.Count);
            var data = PackManager.Instance.Items[rand];

            SpawnManager.Instance.Spawn(
                data,
                rand,
                new(),
                UnitState.InSlotShop,
                shopItemSlots[i].transform);
        }
    }

    #endregion

    /// <summary>
    /// Delays charging of the bot on charging slot and enables player's input after delay.
    /// </summary>
    public void ChargeBotAtStartShop()
    {
        var unit = ChargeSlot.UnitController();
        if (unit == null)
            IsBlockingInput = false;
        else
            StartCoroutine(DelayChargeBotAtStartShop());
    }

    /// <summary>
    /// Delays charging bot at start of phase shop.
    /// </summary>
    /// <returns></returns>
    public IEnumerator DelayChargeBotAtStartShop()
    {
        yield return new WaitForSeconds(process.DelayChargingAtStart);

        var unit = ChargeSlot.UnitController();
        if (unit != null)
            unit.SetEnergy(PackManager.Instance.MyPack.ChargingEnergy.Value);

        IsBlockingInput = false;
    }

    /// <summary>
    /// Delays charging the bots at end of phase shop and returns the delay.
    /// </summary>
    /// <returns></returns>
    public float ChargeBotsAtEndShop()
    {
        foreach (var slot in teamSlots)
        {
            if (slot.gameObject.activeSelf)
            {
                var unitController = slot.UnitController();
                if (unitController != null)
                {
                    unitController.SetEnergy(PackManager.Instance.MyPack.ChargingEnergyTeam.Value);
                }
            }
        }

        return process.DurationCharging + process.DelayStartBattleAfterCharging;
    }

    /// <summary>
    /// Manages the attached object.
    /// </summary>
    /// <param name="_slot"></param>
    public void ManageAttachedObject(Slot _slot)
    {
        if (AttachedGameObject == null)
            return;

        if (AttachedGameObject.CompareTag("Unit"))
        {
            var attachedModel = AttachedGameObject.GetComponent<UnitController>().Model;
            var unitState = attachedModel.Data.UnitState;

            if (unitState == UnitState.InSlotShop || unitState == UnitState.Freezed)
            {
                // "not able to buy" - cases:

                // case: target is item.
                var targetedController = _slot.UnitController();
                if (targetedController != null &&
                    targetedController.Model.Data.UnitType == UnitType.Item)
                    return;

                // case: not enough currency.
                if (!PhaseShopUI.Instance.HasEnoughCurrency(
                    attachedModel.Cost.Nut, attachedModel.Cost.Tool))
                {
                    return;
                }
                //

                Buy(_slot);
            }
            else if (unitState == UnitState.InSlotTeam || unitState == UnitState.InSlotCharge)
            {
                if (attachedModel.Data.IsRobot())
                    TransportOrFusion(_slot);
            }
        }
    }

    /// <summary>
    /// Checks if it's purchasable, then buy the attached object.
    /// </summary>
    /// <param name="_targetedSlot"></param>
    private void Buy(Slot _targetedSlot)
    {
        if (AttachedGameObject.CompareTag("Unit"))
        {
            // unit on target slot, where the mouse was released.
            var targetedController = _targetedSlot.UnitController();

            var attachedController = AttachedGameObject.GetComponent<UnitController>();
            var attachedModel = attachedController.Model;

            if (targetedController != null)
            {
                // case: buy & destroy item
                if (attachedModel.Data.UnitType == UnitType.Item)
                {
                    PhaseShopUI.Instance.UpdateCurrency(
                       attachedModel.Cost.Nut, attachedModel.Cost.Tool);

                    TargetedController = targetedController;

                    attachedController.GetBought();

                    Destroy(AttachedGameObject);

                    TargetedController = null;
                    return;
                }

                // case: buy and bots are fusible.
                if (IsFusible(targetedController, attachedController))
                {
                    PhaseShopUI.Instance.UpdateCurrency(
                        attachedModel.Cost.Nut, attachedModel.Cost.Tool);

                    targetedController.UpdateLevel(attachedModel, true);

                    targetedController.GetBought();

                    Destroy(AttachedGameObject);
                    return;
                }

            }
            else // case: buy and place dragging bot on empty slot.
            {
                if (attachedModel.Data.IsRobot())
                {
                    PhaseShopUI.Instance.UpdateCurrency(
                       attachedModel.Cost.Nut, attachedModel.Cost.Tool);

                    Transport(AttachedGameObject, _targetedSlot.transform, true, true);

                    attachedController.GetBought();
                    return;
                }
            }

        }
    }

    /// <summary>
    /// Transports or merges the attached unit.
    /// </summary>
    /// <param name="_slot"></param>
    private void TransportOrFusion(Slot _slot)
    {
        var unitOnSlot = _slot.UnitController();
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
            Transport(AttachedGameObject, _slot.transform, true, true);
        }
    }

    /// <summary>
    /// Transports the attached game object to the drop slot.
    /// </summary>
    /// <param name="_attached"></param>
    /// <param name="_dropSlot"></param>
    /// <param name="_mouseRelease"> unitView.BeingReleased(null); </param>
    /// <param name="_disableShadow">  unitView.Shadow.enabled = false;</param>
    private void Transport(GameObject _attached, Transform _dropSlot,
        bool _mouseRelease, bool _disableShadow)
    {
        HideDescriptionByTransport();

        if (_attached == null)
            return;

        SoundManager.Instance.PlayFusionSound();

        _attached.transform.parent.GetComponent<Slot>().Border.enabled = false;
        _attached.transform.SetParent(null);

        _attached.transform.SetParent(_dropSlot, false);
        _attached.transform.localPosition = Vector3.zero;

        var unitView = _attached.GetComponent<UnitView>();
        var controller = _attached.GetComponent<UnitController>();

        if (_mouseRelease)
            unitView.BeingReleased(null);

        if (_disableShadow)
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
    public void PushOtherAway(int _target, int _direction)
    {
        if (_direction == 0)
            return;

        // Search index is on next slot in the defined direction.
        int search = _target + _direction;


        // Defines new length without inactive slots.
        int length = 0;
        for (int i = 1; i < teamSlots.Length; i++)
        {
            if (teamSlots[i].gameObject.activeSelf)
                length++;
        }


        // Search empty slot and push the other to it.
        while (search >= 0 && search < length)
        {
            if (teamSlots[search].Unit() != null &&
                teamSlots[search].Unit() != AttachedGameObject) // slot is occupied
            {
                search += _direction; // continue search for an emnpty space
            }
            else // search is on empty slot
            {
                for (int empty = search; empty != _target; empty -= _direction)
                {
                    int previous = empty - _direction; // swap the previous slot index to the empty slot index

                    var movedUnit = teamSlots[previous].Unit();
                    if (movedUnit == null ||
                        movedUnit == AttachedGameObject) // unit being moved is null or self, break foe loop
                        break;
                    Transport(movedUnit, teamSlots[empty].transform, false, false);
                }

                if (AttachedGameObject.GetComponent<UnitController>().Model.Data.UnitState
                    == UnitState.InSlotTeam)
                {
                    Transport(AttachedGameObject, teamSlots[_target].transform, false, false);
                    AttachedGameObject.GetComponent<UnitView>().BeingReleased(null);
                    SetAttachedGameObject(null);

                    PreventDragging = true;
                }
                else if (AttachedGameObject.GetComponent<UnitController>().Model.Data.UnitState
                    == UnitState.InSlotShop)
                {
                    teamSlots[_target].Border.enabled = true;
                }

                return;
            }
        }
    }

    /// <summary>
    /// Handles the ability coroutine.
    /// </summary>
    /// <param name="_ability"></param>
    /// <param name="_isDestroyingUnit"></param>
    /// <returns></returns>
    public IEnumerator HandleAbility(AbilityBase _ability, bool _isDestroyingUnit)
    {
        StartCoroutine(_ability.Handle(Process.DelayHideDescription, _isDestroyingUnit));

        yield return new WaitForSeconds(Process.DelayHideDescription);

        IsBlockingInput = false;
    }

    /// <summary>
    /// Checks fusible between 2 units.
    /// </summary>
    /// <param name="_onSlot"></param>
    /// <param name="_onDrag"></param>
    /// <returns></returns>
    public bool IsFusible(UnitController _onSlot, UnitController _onDrag)
    {
        if (_onSlot.Model.Data.IsRobot() == false ||
            _onDrag.Model.Data.IsRobot() == false)
            return false;

        if (_onSlot.Model.IsMaxed || _onDrag.Model.IsMaxed)
            return false;

        if (_onSlot.Model.SoUnit.Name == _onDrag.Model.SoUnit.Name)
            return true;

        return false;
    }

    public void HandleMouseDown(GameObject _unit, UnitModel _model)
    {
        SetAttachedGameObject(null);
        SetAttachedGameObject(_unit);
        PhaseShopUI.Instance.SetButtonActive(_model);
    }

    /// <summary>
    /// Sets attached game object being clicked or dragged.
    /// </summary>
    /// <param name="_target"></param>
    public void SetAttachedGameObject(GameObject _target)
    {Debug.Log("SetAttachedGameObject: " + _target);
        if (_target == null)
        {
            // previous attached object border disable
            if (AttachedGameObject != null)
                AttachedGameObject.transform.parent.
                    GetComponent<Slot>().Border.enabled = false;

            PhaseShopUI.Instance.SetButtonActive(null);
            AttachedGameObject = _target;
            OnSettingNullObject?.Invoke();
        }
        else
        {
            _target.transform.parent.GetComponent<Slot>().Border.enabled = true;
            AttachedGameObject = _target;
            OnSettingAttachedObject?.Invoke();
        }
    }

    /// <summary>
    /// Hides the description of units on team slots while transporting.
    /// </summary>
    private void HideDescriptionByTransport()
    {
        foreach (var slot in teamSlots)
        {
            slot.HideDescription();
        }
    }

    /// <summary>
    /// Sets the drop hint on team slots and empty charge slot.
    /// </summary>
    /// <param name="_value"></param>
    private void SetDropHint(bool _value)
    {
        bool isDroppable = false;
       
        foreach (var slot in teamSlots)
        {
            if (_value == true)
                isDroppable = IsDroppable(slot);

            slot.Lighten.enabled = isDroppable;
            slot.LightenScale.enabled = isDroppable;
        }

        chargeSlot.Lighten.enabled = _value && chargeSlot.Unit() == null;
        chargeSlot.LightenScale.enabled = _value && chargeSlot.Unit() == null;
    }

    /// <summary>
    /// Is the slot droppable for the attached object?
    /// </summary>
    /// <param name="_slot"></param>
    /// <returns></returns>
    private bool IsDroppable(Slot _slot)
    {
        bool isAttachedItem =
           AttachedGameObject &&
           AttachedGameObject.CompareTag("Unit") &&
           AttachedGameObject.GetComponent<UnitController>().Model.Data.UnitType == UnitType.Item;

        bool slotOccupied = _slot.Unit() != null;

        return isAttachedItem ? slotOccupied : true;
    }

    /// <summary>
    /// Returns only active shop bot slots.
    /// </summary>
    /// <returns></returns>
    public Slot[] ShopBotSlots()
    {
        List<Slot> activeSlots = new List<Slot>();
        foreach(var slot in shopBotSlots)
        {
            if (slot.gameObject.activeSelf)
                activeSlots.Add(slot);
        }
        return activeSlots.ToArray();
    }

    /// <summary>
    /// Returns only active shop item slots.
    /// </summary>
    /// <returns></returns>
    public Slot[] ShopItemSlots()
    {
        List<Slot> activeSlots = new List<Slot>();
        foreach (var slot in shopItemSlots)
        {
            if (slot.gameObject.activeSelf)
                activeSlots.Add(slot);
        }
        return activeSlots.ToArray();
    }

    /// <summary>
    /// Returns only active team slots.
    /// </summary>
    /// <returns></returns>
    public Slot[] TeamSlots()
    {
        List<Slot> activeSlots = new List<Slot>();
        foreach (var slot in teamSlots)
        {
            if (slot.gameObject.activeSelf)
                activeSlots.Add(slot);
        }
        return activeSlots.ToArray();
    }
}
