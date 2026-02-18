using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhaseShopController : MonoBehaviour
{
    public static PhaseShopController Instance { get; private set; }

    [Header("References")]
    [Header("Slots")]
    [SerializeField] private Slot[] teamSlots;
    [SerializeField] private Slot chargeSlot;
    [SerializeField] private Slot[] shopBotSlots;
    [SerializeField] private Slot[] shopItemSlots;
    [SerializeField] private Slot itemRandomnessDropSlot;

    public Slot ChargeSlot => chargeSlot;

    [Header("Settings")]
    [SerializeField] private SoShopProcess process;
    public SoShopProcess Process => process;

    public Player Player { get; private set; }

    public UnitController AttachedController { get; private set; }

    /// <summary>
    /// Blocks hover and drop events in team slots and charge slots. 
    /// When an item has randomness ability and being attached, only the drop slot for it can being hovered.
    /// </summary>
    private bool isRandomnessItemAttached { get; set; } = false;

    /// <summary>
    /// To enable some actions when player is dragging something.
    /// To prevent pushing other away while an unit is attached by mouse click.
    /// To prevent end drag when set IsDragging = false while units are swapping.
    /// </summary>
    public bool IsDragging { get; set; } = false;

    private StartTurnState startTurn = StartTurnState.None;

    private UnityAction<AbilityBase, bool> OnAbility => (_ability, _isDestroyingUnit) =>
    {
        StartCoroutine(HandleAbility(_ability, _isDestroyingUnit));
        Debug.Log(EventManager.Instance.OnTriggerAbility + " sub");
    };

    private UnityAction<UnitController> onDestroyUnit => unit => Destroy(unit);

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


        GameManager.Instance.Switch(GameState.StartOfTurn);
    }

    private void OnEnable()
    {
        if (itemRandomnessDropSlot != null)
            itemRandomnessDropSlot.gameObject.SetActive(false);

        EventManager.Instance.OnTriggerAbility += OnAbility;
        EventManager.Instance.OnShutdown += onDestroyUnit;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnTriggerAbility -= OnAbility;
        EventManager.Instance.OnShutdown -= onDestroyUnit;
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
        StartCoroutine(SetHintClick());
    }

    /// <summary>
    /// Set the player name to the hint click.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetHintClick()
    {
        yield return new WaitUntil(() => CutScene.Instance != null);

        CutScene.Instance.SetHintClickClose(Player.Data.Name, false);
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

                PhaseShopUI.Instance.UpdateUI(Player);
                PhaseShopUI.Instance.SetChargingStationAt(Player.Data.Turn);
                SpawnSavedUnits();

                if (GameManager.Instance.IsReplay == false)
                {
                    PackManager.Instance.AssignList(Player.Data.Turn);
                    SpawnShopUnits();
                    SetStartTurn(StartTurnState.ChargeBot);
                }
                else
                {
                    SetStartTurn(StartTurnState.Done);
                }

                Player.UpdateUnitData();
                break;

            case StartTurnState.ShowingTurn:
                break;

            case StartTurnState.ChargeBot:
                StartCoroutine(DelayChargeBotsAtStartShop());
                break;

            case StartTurnState.Done:
                GameManager.Instance.Switch(GameState.ShopPhase);
                SetStartTurn(StartTurnState.None);
                break;

        }
    }

    // Spawn Objects
    #region Spawn objects

    /// <summary>
    /// Instantiate prefab and initialize it with data in team/charging/shop bots & shop items.
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

            int rand = UnityEngine.Random.Range(0, PackManager.Instance.Bots.Count);
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

            int rand = UnityEngine.Random.Range(0, PackManager.Instance.Items.Count);
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


    #region Charge Bot at start of Shop

    /// <summary>
    /// Delays charging bots at start of phase shop.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayChargeBotsAtStartShop()
    {
        float delayOpenScene = CutScene.Instance ? CutScene.Instance.DelayOpen : 0f;

        yield return new WaitForSeconds(delayOpenScene);

        if (ChargeSlot == null)
            yield break;

        yield return new WaitForSeconds(process.DelayChargingAtStart);

        var unit = ChargeSlot.UnitController();
        if (unit != null)
            unit.SetEnergy(PackManager.Instance.MyPack.ChargingEnergy.Value, true);

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
        bool isSomeoneThere = false;

        foreach (var slot in teamSlots)
        {
            if (slot.gameObject.activeSelf)
            {
                var unitController = slot.UnitController();
                if (unitController != null)
                {
                    unitController.SetEnergy(PackManager.Instance.MyPack.ChargingEnergyTeam.Value, false);
                    isSomeoneThere = true;
                }
            }
        }

        if (isSomeoneThere)
        {
            EventManager.Instance.OnBuff?.Invoke();
        }
    }

    #endregion

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
                _attached.Model.Cost.Nut, _attached.Model.Cost.Tool, true))
            {
                // then buy.
                Buy(_attached, _targetSlot, _target);
            }
        }
        // If unit is in the team and
        else if (attachedState == UnitState.InSlotTeam || attachedState == UnitState.InSlotCharge)
        {
            // unit is a robot,
            if (_attached.Model.IsRobot())
            {
                // then check if the slot is empty,
                if (_target == null)
                    // transport the unit to it,
                    Transport(_attached, _targetSlot.transform, true);
                else
                    // else fusion, if both are fusible.
                    if (IsFusible(_target, _attached))
                {
                    _target.UpdateLevel(_attached.Model, true);
                    Destroy(_attached.gameObject);
                }
            }
        }
        // Set drop slot inactive for randomness item.
        if (itemRandomnessDropSlot != null &&
            itemRandomnessDropSlot.gameObject.activeSelf)
        {
            itemRandomnessDropSlot.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Checks if it's purchasable, then buy the attached object.
    /// </summary>
    /// <param name="_targetedSlot"></param>
    private void Buy(UnitController _purchased, Slot _targetSlot, UnitController _target)
    {
        if (_target != null || _purchased.Model.IsItemDoRandomness) // on drop, not on click
        {
            // case: buy & destroy item
            if (_purchased.Model.Data.UnitType == UnitType.Item)
            {
                _purchased.Targets.Enqueue(_target);
                _purchased.TriggerCraft();

                PhaseShopUI.Instance.UpdateCurrency(
                     _purchased.Model.Cost.Nut, _purchased.Model.Cost.Tool);

                EventManager.Instance.OnCraft?.Invoke();

                Destroy(_purchased.gameObject);
            }
            // case: buy and bots are fusible.
            else if (IsFusible(_target, _purchased))
            {
                _target.UpdateLevel(_purchased.Model, true);
                _target.TriggerCraft();

                PhaseShopUI.Instance.UpdateCurrency(
                     _purchased.Model.Cost.Nut, _purchased.Model.Cost.Tool);

                EventManager.Instance.OnCraft?.Invoke();

                Destroy(_purchased.gameObject);
            }
        }
        else // case: buy and place dragging bot on empty slot.
        {
            if (_purchased.Model.IsRobot())
            {
                PhaseShopUI.Instance.UpdateCurrency(
                   _purchased.Model.Cost.Nut, _purchased.Model.Cost.Tool);

                Transport(_purchased, _targetSlot.transform, true);

                EventManager.Instance.OnCraft?.Invoke();

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
    public void Transport(UnitController _attached, Transform _dropSlot,
        bool _mouseRelease)
    {
        HideDescriptionByTransport();

        if (_attached == null)
            return;

        if (_attached.transform.parent)
            _attached.transform.parent.GetComponent<Slot>().SetIndicatorActive(false);

        _attached.transform.SetParent(_dropSlot, false);

        var model = _attached.Model;
        var view = _attached.View;

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

            if (model != null)
                view.SetBuyOrSell(model.Sell, false, model.Data.UnitType);
        }

        EventManager.Instance.OnDropUnit?.Invoke();
        Player.UpdateUnitData();
    }

    /// <summary>
    /// Move the targeted unit to the be dragged slot and the dragging unit to target slot.
    /// </summary>
    /// <param name="_unitTarget"></param>
    /// <param name="_slotDragged"></param>
    /// <param name="_unitDragged"></param>
    /// <param name="_slotTarget"></param>
    /// <returns></returns>
    public IEnumerator Swap(UnitController _unitTarget, Transform _slotDragged, UnitController _unitDragged, Transform _slotTarget)
    {
        HideDescriptionByTransport();
        bool a = GameManager.Instance.IsBlockingInput;
        var _unit1View = _unitTarget.GetComponent<UnitView>();

        float delay1 = default;
        float delay2 = default;

        if (_unitTarget != null && _slotDragged != null)
        {
            _unitTarget.transform.SetParent(null, true);
            _unit1View.SetSpriteOverOther();
            delay1 = _unitTarget.MoveToParent(_slotDragged.position, _slotDragged);
        }
        EventManager.Instance.OnSwap?.Invoke();

        yield return new WaitUntil(() => _unitTarget.transform.parent != null);

        Transport(_unitTarget, _slotDragged, false);

        if (_unitTarget != null)
            _unit1View.SetLocalPositionDefault();

        if (_unitDragged != null && _slotTarget != null)
        {
            _unitDragged.BeginSwap();
            delay2 = _unitDragged.MoveToParent(_slotTarget.position, _slotTarget);
        }
        EventManager.Instance.OnSwap?.Invoke();

        yield return new WaitUntil(() => _unitDragged.transform.parent != null);

        Transport(_unitDragged, _slotTarget, true);

        Player.UpdateUnitData();
        GameManager.Instance.IsBlockingInput = false;
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
            if (teamSlots[search].UnitController() != null &&
                teamSlots[search].UnitController() != AttachedController) // slot is occupied
            {
                search += _direction; // continue search for an emnpty space
            }
            else // search is on empty slot
            {
                for (int empty = search; empty != _target; empty -= _direction)
                {
                    int previous = empty - _direction; // swap the previous slot index to the empty slot index

                    var movedUnit = teamSlots[previous].UnitController();
                    if (movedUnit == null ||
                        movedUnit == AttachedController) // unit being moved is null or self, break foe loop
                        break;
                    Transport(movedUnit, teamSlots[empty].transform, false);
                }

                if (AttachedController.Model.Data.UnitState
                    == UnitState.InSlotTeam)
                {
                    Transport(AttachedController, teamSlots[_target].transform, true);
                    SetAttachedGameObject(null);
                }
                else if (AttachedController.Model.Data.UnitState
                    == UnitState.InSlotShop)
                {
                    teamSlots[_target].SetIndicatorActive(true);
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
        if (_onDrag == _onSlot)
            return false;

        if (_onSlot.Model.IsRobot() == false ||
            _onDrag.Model.IsRobot() == false)
            return false;

        if (_onSlot.Model.IsMaxed || _onDrag.Model.IsMaxed)
            return false;

        if (_onSlot.Model.SoUnit.Name == _onDrag.Model.SoUnit.Name)
            return true;

        return false;
    }

    /// <summary>
    /// Sets attached game object being clicked or dragged.
    /// </summary>
    /// <param name="_target"></param>
    public void SetAttachedGameObject(UnitController _target)
    {
        if (AttachedController != null)
            AttachedController.Slot.SetIndicatorActive(false);

        if (_target != null)
            _target.Slot.SetIndicatorActive(true);

        PhaseShopUI.Instance.SetButtonActive(_target != null ? _target.Model : null);
        AttachedController = _target;
        SetDropHint(_target != null);

        if (_target != null)
            EventManager.Instance.OnAttachedUnit?.Invoke(_target);
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
        if (AttachedController != null)
        {
            if (AttachedController.Model.IsInShop())
            {
                if (PhaseShopUI.Instance.HasEnoughCurrency(
                    AttachedController.Model.Cost.Nut, AttachedController.Model.Cost.Tool, false) == false)
                    return;

                if (AttachedController.Model.IsItemDoRandomness)
                {
                    if (itemRandomnessDropSlot != null)
                    {
                        isRandomnessItemAttached = true;
                        itemRandomnessDropSlot.gameObject.SetActive(true);
                        Debug.Log("Set drop hint true for randomness item");
                    }
                }
            }
        }

        if (_value == false)
        {
            if (itemRandomnessDropSlot != null)
            {
                if (!IsDragging)
                {
                    isRandomnessItemAttached = false;
                    itemRandomnessDropSlot.gameObject.SetActive(false);
                    Debug.Log("Set drop hint false");
                }
            }
        }

        //Debug.Log("Set drop hint: " + _value);

        int debugCount = -1;

        foreach (var slot in teamSlots)
        {
            debugCount++;
            if (!slot)
            {
                Debug.LogWarning($"slot {debugCount} in team is null");
                continue;
            }

            slot.SetLightingActive(false);
            slot.SetHintLight(AttachedController, _value);
        }

        // Not hint charging station
        if (AttachedController != null &&
            AttachedController.Model.Data.UnitType == UnitType.Item)
        {
            return;
        }

        // Charging station

        if (!chargeSlot)
        {
            Debug.LogWarning($"slot charge is null");
            return;
        }

        chargeSlot.SetLightingActive(false);
        chargeSlot.SetHintLight(AttachedController, _value);
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

    /// <summary>
    /// Is blocking inputs by randomness item being attached?
    /// </summary>
    /// <param name="_slot"></param>
    /// <returns></returns>
    public bool IsBlockingInputsByItemRandomness(Slot _slot)
    {
        if (AttachedController == null)
            return false;
        return isRandomnessItemAttached && (_slot != AttachedController.Slot && _slot != itemRandomnessDropSlot);
    }

    /// <summary>
    /// Is blocking drop by randomness item being attached?
    /// </summary>
    /// <param name="_slot"></param>
    /// <returns></returns>
    public bool IsBlockingDropByItemRandomness(Slot _slot)
    {
        return isRandomnessItemAttached && _slot != itemRandomnessDropSlot;
    }

    public void SetItemRandomnessInactive()
    {
        itemRandomnessDropSlot.gameObject.SetActive(false);
    }

    public void Destroy(UnitController _unit)
    {
        _unit.DestroyObject();
    }
}
