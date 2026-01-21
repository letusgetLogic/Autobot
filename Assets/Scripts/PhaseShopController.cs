using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseShopController : MonoBehaviour
{
    public static PhaseShopController Instance { get; private set; }

    [Header("References")]
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

    public GameObject AttachedGameObject { get; private set; }

    /// <summary>
    /// To prevent pushing other away while an unit is attached by mouse click and
    /// to prevent end drag when set IsDragging = false while units are swapping.
    /// </summary>
    public bool IsDragging { get; set; } = false;

    private StartTurnState startTurn = StartTurnState.None;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }
        Instance = this;

        Time.timeScale = 1f;

        SetIndex(teamSlots);
        SetIndex(shopBotSlots);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnSettingAttachedObject += () => SetDropHint(true);
        EventManager.Instance.OnSettingNullObject += () => SetDropHint(false);
    }

    private void OnDisable()
    {
        EventManager.Instance.OnSettingAttachedObject -= () => SetDropHint(true);
        EventManager.Instance.OnSettingNullObject -= () => SetDropHint(false);
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
    /// Initializes the template of player.
    /// </summary>
    /// <param name="_player"></param>
    public void Initialize(Player _player)
    {
        Player = _player;
        SetStartTurn(StartTurnState.Init);
    }

    /// <summary>
    /// Set start turn state.
    /// </summary>
    /// <param name="_state"></param>
    private void SetStartTurn(StartTurnState _state)
    {
        startTurn = _state;
        switch (startTurn)
        {
            case StartTurnState.None:
                break;

            case StartTurnState.Init:
                PackManager.Instance.AssignList(Player.Data.Turn);
                PhaseShopUI.Instance.UpdateUI(Player);
                SpawnSavedUnits();
                SpawnShopUnits();
                Player.UpdateUnitData();
                PhaseShopUI.Instance.SetChargingStationAt(Player.Data.Turn);
                SetStartTurn(StartTurnState.ChargeBot);
                break;

            case StartTurnState.ShowingTurn:
                break;

            case StartTurnState.ChargeBot:
                StartCoroutine(DelayChargeBotsAtStartShop());
                break;

            case StartTurnState.Done:
                GameManager.Instance.Switch(GameState.ShopPhase, null);
                SetStartTurn(StartTurnState.None);
                break;

        }
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
    /// Delays charging bots at start of phase shop.
    /// </summary>
    /// <returns></returns>
    public IEnumerator DelayChargeBotsAtStartShop()
    {
        float delayOpenScene = CutScene.Instance ? CutScene.Instance.DelayOpen : 0f;

        yield return new WaitForSeconds(delayOpenScene + process.DelayChargingAtStart);

        var unit = ChargeSlot.UnitController();
        if (unit != null)
            unit.SetEnergy(PackManager.Instance.MyPack.ChargingEnergy.Value);

        //if (Player.Data.Turn > 1)
        //    ChargeTeamBots();

        SetStartTurn(StartTurnState.Done);
    }

    /// <summary>
    /// Charge team bots.
    /// </summary>
    /// <returns></returns>
    public void ChargeTeamBots()
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
    }

    /// <summary>
    ///  Manages the attached unit.
    /// </summary>
    /// <param name="_attachedController"></param>
    /// <param name="_targetSlot"></param>
    public void ManageAttachedUnit(UnitController _attached, Slot _targetSlot, UnitController _target)
    {
        var attachedState = _attached.Model.Data.UnitState;

        // If unit is in the shop and
        if (attachedState == UnitState.InSlotShop || attachedState == UnitState.Freezed)
        {
            // player has enough currency for buying unit,
            if (PhaseShopUI.Instance.HasEnoughCurrency(
                _attached.Model.Cost.Nut, _attached.Model.Cost.Tool))
            {
                // then buy.
                Buy(_attached, _targetSlot, _target);
            }
        }
        // If unit is in the team and
        else if (attachedState == UnitState.InSlotTeam || attachedState == UnitState.InSlotCharge)
        {
            // unit is a robot,
            if (_attached.Model.Data.IsRobot())
            {
                // then check if the slot is empty,
                if (_target == null)
                    // transport the unit to it,
                    Transport(_attached.gameObject, _targetSlot.transform, true, true);
                else
                    // else fusion, if both are fusible.
                    if (IsFusible(_target, _attached)) 
                {
                    _target.UpdateLevel(_attached.Model, true);
                    Destroy(_attached.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Checks if it's purchasable, then buy the attached object.
    /// </summary>
    /// <param name="_targetedSlot"></param>
    private void Buy(UnitController _purchased, Slot _targetSlot, UnitController _target)
    {
        if (_target != null) // on drop, not on click
        {
            // case: buy & destroy item
            if (_purchased.Model.Data.UnitType == UnitType.Item)
            {
                _purchased.TargetedByItem = _target;
                _purchased.TriggerCraft();

                PhaseShopUI.Instance.UpdateCurrency(
                     _purchased.Model.Cost.Nut, _purchased.Model.Cost.Tool);

                Destroy(_purchased.gameObject);
            }
            // case: buy and bots are fusible.
            else if (IsFusible(_target, _purchased))
            {
                _target.UpdateLevel(_purchased.Model, true);
                _target.TriggerCraft();

                PhaseShopUI.Instance.UpdateCurrency(
                     _purchased.Model.Cost.Nut, _purchased.Model.Cost.Tool);

                Destroy(_purchased.gameObject);
            }

        }
        else // case: buy and place dragging bot on empty slot.
        {
            if (_purchased.Model.Data.IsRobot())
            {
                PhaseShopUI.Instance.UpdateCurrency(
                   _purchased.Model.Cost.Nut, _purchased.Model.Cost.Tool);

                Transport(AttachedGameObject, _targetSlot.transform, true, true);

                _purchased.TriggerCraft();
            }
        }
    }

    /// <summary>
    /// Transports the attached game object to the drop slot in Phase Shop.
    /// </summary>
    /// <param name="_attached"></param>
    /// <param name="_dropSlot"></param>
    /// <param name="_mouseRelease"> unitView.BeingReleased(null); </param>
    /// <param name="_disableShadow">  unitView.Shadow.enabled = false;</param>
    public void Transport(GameObject _attached, Transform _dropSlot,
        bool _mouseRelease, bool _disableShadow)
    {
        HideDescriptionByTransport();

        if (_attached == null)
            return;

        _attached.transform.parent.GetComponent<Slot>().Border.enabled = false;
        _attached.transform.SetParent(_dropSlot, false);

        var controller = _attached.GetComponent<UnitController>();
        var model = controller.Model;
        var view = controller.View;

        if (model != null)
        {
            model.SetData(_dropSlot.CompareTag("Slot Charge") ?
                UnitState.InSlotCharge :
                UnitState.InSlotTeam);
        }

        if (view != null)
        {
            if (_mouseRelease)
                view.BeingReleased(null);

            if (_disableShadow)
                view.Shadow.enabled = false;

            if (model != null)
                view.SetBuyOrSell(model.Sell, false, model.Data.UnitType);
        }

        EventManager.Instance.OnTransportUnit?.Invoke();
        Player.UpdateUnitData();
    }

    public IEnumerator Swap(UnitController _unitTarget, Transform _slotTarget, UnitController _unitDragged, Transform _slotDragged)
    {
        HideDescriptionByTransport();

        var _unit1View = _unitTarget.GetComponent<UnitView>();

        float delay1 = default;
        float delay2 = default;

        if (_unitTarget != null && _slotTarget != null)
        {
            _unitTarget.transform.SetParent(null, true);
            _unit1View.SetSpriteOverOther();
            delay1 = _unitTarget.MoveToParent(_slotTarget.position, _slotTarget);
        }
        yield return new WaitForSeconds(delay1);

        if (_unitTarget != null)
            _unit1View.SetLocalPositionDefault();

        if (_unitDragged != null && _slotDragged != null)
        {
            _unitDragged.BeginSwap();
            delay2 = _unitDragged.MoveToParent(_slotDragged.position, _slotDragged);
        }
        yield return new WaitForSeconds(delay2);

        if (_unitDragged != null)
            _unitDragged.EndSwap();

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

        var teamSlots = TeamSlots();

        // Search empty slot and push the other to it.
        while (search >= 0 && search < teamSlots.Length)
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
                    Transport(AttachedGameObject, teamSlots[_target].transform, true, false);
                    SetAttachedGameObject(null);

                    //PreventDragging = true;
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

        GameManager.Instance.IsBlockingInput = false;
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

    public void SwitchAttached(GameObject _unit, UnitModel _model)
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
    {
        if (_target == null)
        {
            // previous attached object border disable
            if (AttachedGameObject != null)
                AttachedGameObject.transform.parent.
                    GetComponent<Slot>().Border.enabled = false;

            PhaseShopUI.Instance.SetButtonActive(null);
            AttachedGameObject = _target;
            EventManager.Instance.OnSettingNullObject?.Invoke();
        }
        else
        {
            _target.transform.parent.GetComponent<Slot>().Border.enabled = true;
            AttachedGameObject = _target;
            EventManager.Instance.OnSettingAttachedObject?.Invoke();
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

        return isAttachedItem ? true : slotOccupied;
    }

    /// <summary>
    /// Returns only active shop bot slots.
    /// </summary>
    /// <returns></returns>
    public Slot[] ShopBotSlots()
    {
        List<Slot> activeSlots = new List<Slot>();
        foreach (var slot in shopBotSlots)
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
