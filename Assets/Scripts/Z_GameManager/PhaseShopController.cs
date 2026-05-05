using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhaseShopController : MonoBehaviour
{
    public static PhaseShopController Instance { get; private set; }

    [Header("References")]
    [Header("Slots")]
    [SerializeField] private List<Slot> teamSlots;
    [SerializeField] private Slot chargeSlot;
    [SerializeField] private List<Slot> shopBotSlots;
    [SerializeField] private List<Slot> shopItemSlots;
    [SerializeField] private Slot itemRandomnessDropSlot;

    public Slot ChargeSlot => chargeSlot;
    public List<Slot> TeamSlots => teamSlots.Where(x => x.gameObject.activeSelf).ToList();
    public List<Slot> ShopBotSlots => shopBotSlots.Where(x => x.gameObject.activeSelf).ToList();
    public List<Slot> ShopItemSlots => shopItemSlots.Where(x => x.gameObject.activeSelf).ToList();

    [Header("Panels")]
    [SerializeField] private GameObject coverPanelPreventButtonClick;

    [Header("Settings")]
    [SerializeField] private SoShopProcess process;
    [SerializeField] private SoLerpMovementSettings unitSwapSettings;

    public SoShopProcess Process => process;
    public Player Player { get; private set; }
    public UnitController AttachedController { get; private set; }

    /// <summary>
    /// Blocks hover and drop events in team slots and charge slots. 
    /// When an item has randomness ability and being attached, only the drop slot for it can being hovered.
    /// </summary>
    private bool isRandomnessItemAttached = false;

    /// <summary>
    /// To enable some actions when player is dragging something.
    /// To prevent pushing other away while an unit is attached by mouse click.
    /// To prevent end drag when set IsDragging = false while units are swapping.
    /// </summary>
    public bool IsDragging { get; set; } = false;
    public bool IsSwapping { get; private set; } = false;

    public bool HasAnyBotInShop => ShopBotSlots.Any(slot => slot.UnitController() != null);

    public bool IsAnyRobotDamaged => TeamSlots.Any(x =>
    {
        var unitController = x.UnitController();
        return unitController != null ? !unitController.Model.IsFullDurability() : false;
    }) // charge slot
        || (ChargeSlot != null && ChargeSlot.UnitController() != null && !ChargeSlot.UnitController().Model.IsFullDurability());

    private StartTurnState startTurn = StartTurnState.None;
    private InputManager input => InputManager.Instance;

    private List<AbilityBase> abilities = new List<AbilityBase>();

    private void Awake()
    {
        Debug.Log(this.name + ".Awake()");

        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        Time.timeScale = 1f;
    }

    private void Start()
    {
        TeamSlots.ForEach(x => x.Index = TeamSlots.IndexOf(x));
        ShopBotSlots.ForEach(x => x.Index = ShopBotSlots.IndexOf(x));
        GameManager.Instance.Switch(GameState.StartOfTurn);
    }

    private void OnEnable()
    {
        if (itemRandomnessDropSlot != null)
            itemRandomnessDropSlot.gameObject.SetActive(false);

        EventManager.Instance.OnTriggerAbility += TriggerAbility;
        EventManager.Instance.OnShutdown += DestroyUnit;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnTriggerAbility -= TriggerAbility;
        EventManager.Instance.OnShutdown -= DestroyUnit;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// Initializes the template of player.
    /// </summary>
    /// <param name="_player"></param>
    public void Initialize(Player _player)
    {
        Player = _player;
        SetStartTurn(StartTurnState.Init);

        if (IsTurnAI() == false)
        {
            StartCoroutine(SetHintClick());
        }
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
    public void SetStartTurn(StartTurnState _state)
    {
        startTurn = _state;
        switch (startTurn)
        {
            case StartTurnState.None:
                break;

            case StartTurnState.Init:
                bool isTutorial = GameManager.Instance.Mode == GameMode.Tutorial;
                PhaseShopUI.Instance.UpdateUI(isTutorial, Player);
                PhaseShopUI.Instance.SetChargingStationAt(Player.Data.Turn);
                SpawnSavedUnits();

                if (GameManager.Instance.Replay == null)
                {
                    PackManager.Instance.AssignList(Player.Data.Turn);

                    if (ShouldFixedUnitsSpawned)
                        SpawnFixedUnits();
                    else
                        SpawnShopUnits();

                    SetStartTurn(StartTurnState.WaitingOpenScene);
                }
                else SetStartTurn(StartTurnState.Done);

                Player.UpdateUnitData();
                break;



            case StartTurnState.WaitingOpenScene:
                break;



            case StartTurnState.OpenSceneEnd:
                coverPanelPreventButtonClick.SetActive(false);

                if (GameManager.Instance.Replay != null)
                    return;

                (bool isUnlocking, int index) = PackManager.Instance.IsUnlockingTier(Player.Data.Turn);

                bool showUnlock = isUnlocking && index > 1;
                PhaseShopUI.Instance.SetUnlockedTier(showUnlock, index);

                if (showUnlock)
                {
                    EventManager.Instance.OnPopUpSound?.Invoke();
                    SetStartTurn(StartTurnState.WaitingClick);
                }
                else
                    SetStartTurn(StartTurnState.ChargeBot);
                break;




            case StartTurnState.WaitingClick:
                // wait for clicking unlock panel
                break;



            case StartTurnState.ClickPanelUnlock:
                PhaseShopUI.Instance.SetUnlockedTier(false, 0);
                SetStartTurn(StartTurnState.ChargeBot);
                break;



            case StartTurnState.ChargeBot:

                if (IsTurnAI())
                {
                    ChargeBotAtStartShop();
                }
                else
                {
                    input.BlocksInput = false;
                    StartCoroutine(DelayChargeBotsAtStartShop());
                }
                break;



            case StartTurnState.Done:

                if (IsTurnAI())
                {
                    gameObject.AddComponent<AI>();
                }
                SetStartTurn(StartTurnState.None);
                break;

        }
    }

    private bool ShouldFixedUnitsSpawned => Player.Data.Turn == 1 && GameManager.Instance.IsTutorialRunning;

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
                if (unitData == null)
                    continue;
                Debug.Log("unitData.Index" + unitData.Index);
                SpawnManager.Instance.Spawn(
                    PackManager.Instance.GetSoUnit(unitData),
                    unitData.Index,
                    unitData,
                    UnitState.InSlotTeam,
                    teamSlots[i].transform);
            }
        }

        // charging station bot
        var chargeUnitData = Player.Data.ChargeUnitData;
        if (chargeUnitData != null)
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
                if (unitData == null)
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
                if (unitData == null)
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
        var shopBotSlots = ShopBotSlots;
        for (int i = 0; i < shopBotSlots.Count; i++)
        {
            var unitController = shopBotSlots[i].UnitController();
            if (unitController != null)
            {
                if (unitController.Model.Data.UnitState == UnitState.Freezed)
                    continue;

                Destroy(unitController.gameObject);
            }

            if (PackManager.Instance.Bots.Count == 0)
                return;

            int randomNumber = Random.Range(0, PackManager.Instance.Bots.Count);
            var soUnit = PackManager.Instance.Bots[randomNumber];

            SpawnManager.Instance.Spawn(
                soUnit,
                randomNumber,
                null,
                UnitState.InSlotShop,
                shopBotSlots[i].transform);
        }

        var shopItemSlots = ShopItemSlots;
        for (int i = 0; i < shopItemSlots.Count; i++)
        {
            var unitController = shopItemSlots[i].UnitController();
            if (unitController != null)
            {
                if (unitController.Model.Data.UnitState == UnitState.Freezed)
                    continue;

                Destroy(unitController.gameObject);
            }

            if (PackManager.Instance.Items.Count == 0)
                return;

            int randomNumber = Random.Range(0, PackManager.Instance.Items.Count);
            var soUnit = PackManager.Instance.Items[randomNumber];

            SpawnManager.Instance.Spawn(
                soUnit,
                randomNumber,
                null,
                UnitState.InSlotShop,
                shopItemSlots[i].transform);
        }
    }

    private void SpawnFixedUnits()
    {
        var shopBotSlots = ShopBotSlots;
        for (int i = 0; i < shopBotSlots.Count; i++)
        {
            var units = TutorialManager.Instance.BotsTurn1;
            if (i >= units.Length)
                continue;

            var bots = PackManager.Instance.Bots;
            SpawnManager.Instance.Spawn(
                units[i],
                bots.IndexOf(bots.FirstOrDefault(bot => bot == units[i])),
                null,
                UnitState.InSlotShop,
                shopBotSlots[i].transform);
        }

        var shopItemSlots = ShopItemSlots;
        for (int i = 0; i < shopItemSlots.Count; i++)
        {
            var units = TutorialManager.Instance.ItemsTurn1;
            if (i >= units.Length)
                continue;

            var items = PackManager.Instance.Items;
            SpawnManager.Instance.Spawn(
                units[i],
                items.IndexOf(items.FirstOrDefault(item => item == units[i])),
                null,
                UnitState.InSlotShop,
                shopItemSlots[i].transform);
        }
    }

    #endregion

    #region Charge Bot

    private void ChargeBotAtStartShop()
    {
        if (ChargeSlot == null)
            return;

        var unit = ChargeSlot.UnitController();
        if (unit != null)
            unit.AddEnergy(PackManager.Instance.MyPack.ChargingEnergy.Value, true, true);

        SetStartTurn(StartTurnState.Done);
    }

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

        float durationCharge = 0f;

        var unit = ChargeSlot.UnitController();
        if (unit != null)
            durationCharge = unit.AddEnergy(PackManager.Instance.MyPack.ChargingEnergy.Value, true, true);

        yield return new WaitForSeconds(durationCharge);

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
                    unitController.AddEnergy(PackManager.Instance.MyPack.ChargingEnergyTeam.Value, false, true);
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

    private Coroutine buyCoroutine;

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
                buyCoroutine = StartCoroutine(Buy(_attached, _targetSlot, _target));
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
                        _target.UpdateLevel(_attached.Model.Data, true);
                        Destroy(_attached.gameObject);
                    }
                    else StartCoroutine(Swap(_target, _attached.Slot.transform, _attached, _targetSlot.transform));
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
    private IEnumerator Buy(UnitController _purchased, Slot _targetSlot, UnitController _target)
    {
        if (_target != null || _purchased.Model.IsItemDoRandomness) // on drop, not on click
        {
            if (_purchased.Model.Data.UnitType == UnitType.Item)
            {
                // case: virus wouldn't trigger ability
                if (IsBuyingItemNotUseful(_purchased, _target))
                {
                    var panel = PhaseShopUI.Instance.PanelShutdownNotTrigger;
                    if (panel != null)
                    {
                        panel.gameObject.SetActive(true);
                        _target.View.SetDescriptionActive(true);
                        yield return new WaitUntil(() =>
                            panel.MyResult == PanelConfirmation.Result.Confirmed ||
                            panel.MyResult == PanelConfirmation.Result.Declined);
                      
                        if (panel.MyResult == PanelConfirmation.Result.Declined)
                        {
                            _target.View.SetDescriptionActive(false);
                            input.BlocksInput = false;
                            buyCoroutine = null;
                            yield break;
                        }
                    }
                }

                // case: buy & destroy item
                _purchased.Targets.Enqueue(_target);
                _purchased.TriggerCraft();

                PhaseShopUI.Instance.UpdateCurrency(
                     _purchased.Model.Cost.Nut, _purchased.Model.Cost.Tool);

                EventManager.Instance.OnCraft?.Invoke(InputKey.DropSlotTeam);

                Destroy(_purchased.gameObject);
            }
            // case: buy and bots are fusible.
            else if (IsFusible(_target, _purchased))
            {
                _target.UpdateLevel(_purchased.Model.Data, true);
                _target.TriggerCraft();

                PhaseShopUI.Instance.UpdateCurrency(
                     _purchased.Model.Cost.Nut, _purchased.Model.Cost.Tool);

                EventManager.Instance.OnCraft?.Invoke(InputKey.DropSlotTeam);

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

                EventManager.Instance.OnCraft?.Invoke(InputKey.DropSlotTeam);

                _purchased.TriggerCraft();
            }
        }
        input.BlocksInput = false;
    }


    #region Transport Unit

    /// <summary>
    /// Transports the attached game object to the drop slot in Phase Shop.
    /// </summary>
    /// <param name="_attached"></param>
    /// <param name="_dropSlot"></param>
    /// <param name="_mouseRelease"> unitView.BeingReleased(null); </param>
    /// <param name="_disableShadow">  unitView.Shadow.enabled = false;</param>
    public void Transport(UnitController _attached, Transform _dropSlot, bool _mouseRelease)
    {
        HideDescriptionOfUnits();

        if (_attached == null)
            return;

        if (_attached.transform.parent)
            _attached.transform.parent.GetComponent<Slot>().SetIndicatorActive(false);

        _attached.transform.SetParent(_dropSlot, false);

        var model = _attached.Model;
        var view = _attached.View;

        if (model != null)
        {
            model.SetDataView(_dropSlot.CompareTag("Slot Charge")
                ? UnitState.InSlotCharge
                : UnitState.InSlotTeam,
                true);
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
    public IEnumerator Swap(
        UnitController _unitTarget, Transform _slotDragged,
        UnitController _unitDragged, Transform _slotTarget)
    {
        IsSwapping = true;

        HideDescriptionOfUnits();

        var _unitTargetView = _unitTarget.GetComponent<UnitView>();

        float delay1 = default;
        float delay2 = default;

        if (_unitTarget != null && _slotDragged != null)
        {
            _unitTarget.transform.SetParent(null, true);
            _unitTargetView.SetSpriteOverOther();
            delay1 = _unitTarget.SwapMoveToParent(_slotDragged.position, _slotDragged, unitSwapSettings);
            EventManager.Instance.OnSwap?.Invoke();
        }

        yield return new WaitForSeconds(delay1);
        yield return new WaitUntil(() => _unitTarget.transform.parent != null);

        Transport(_unitTarget, _slotDragged, false);

        if (_unitTarget != null)
            _unitTargetView.SetLocalPositionDefault();

        if (_unitDragged != null && _slotTarget != null)
        {
            _unitDragged.BeginSwap();
            delay2 = _unitDragged.SwapMoveToParent(_slotTarget.position, _slotTarget, unitSwapSettings);
            EventManager.Instance.OnSwap?.Invoke();
        }

        yield return new WaitForSeconds(delay2);
        yield return new WaitUntil(() => _unitDragged.transform.parent != null);

        Transport(_unitDragged, _slotTarget, true);

        IsSwapping = false;
        input.BlocksInput = false;
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

        var teamSlots = TeamSlots;

        // Search empty slot and push the other to it.
        while (search >= 0 && search < teamSlots.Count)
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

    #endregion

    /// <summary>
    /// Starts the coroutine of handling the ability.
    /// </summary>
    /// <param name="_ability"></param>
    /// <param name="_isDestroyingUnit"></param>
    private void TriggerAbility(AbilityBase _ability, bool _isDestroyingUnit)
    {
        abilities.Add(_ability);
        StartCoroutine(HandleAbility(_ability, _isDestroyingUnit));
        Debug.Log(EventManager.Instance.OnTriggerAbility + " sub");
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

        input.BlocksInput = false;
        abilities.Remove(_ability);
    }

    #region End shop

    /// <summary>
    /// Ends the phase shop.
    /// </summary>
    public void EndShop()
    {
        EventManager.Instance.OnEndTurnAccepted?.Invoke();
        SetAttachedGameObject(null);
        coverPanelPreventButtonClick.SetActive(true);

        ChargeTeamBots();
        EventManager.Instance.OnEndTurnCharged?.Invoke();
        float delay = Process.DurationCharging + Process.DelayStartBattleAfterEndTurn;
        endShopCoroutine = StartCoroutine(DelayEndShop(delay));
    }
    private Coroutine endShopCoroutine;

    /// <summary>
    /// Delays ending the shop phase for charging units at turn 1.
    /// </summary>
    /// <param name="_delay"></param>
    /// <returns></returns>
    private IEnumerator DelayEndShop(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        // If any ability is not null and not done, wait.
        yield return new WaitUntil(() => abilities.Any(x => x != null && x.IsDone == false) == false);

        Player.UpdateUnitData();

        GameManager.Instance.Switch(GameState.EndOfTurn);

        endShopCoroutine = null;
    }

    #endregion

    /// <summary>
    /// Sets attached game object being clicked or dragged.
    /// </summary>
    /// <param name="_target"></param>
    public void SetAttachedGameObject(UnitController _target)
    {
        if (AttachedController && AttachedController.Slot)
            AttachedController.Slot.SetIndicatorActive(false);

        if (_target && _target.Slot)
        {
            _target.Slot.SetIndicatorActive(true);
        }

        AttachedController = _target;
        SetDropHint(_target != null);

        EventManager.Instance.OnAttachedUnit?.Invoke(_target);
    }

    /// <summary>
    /// Hides the description of units on team slots while transporting.
    /// </summary>
    public void HideDescriptionOfUnits()
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

    /// <summary>
    /// Sets the drop slot for randomness item inactive.
    /// </summary>
    public void SetItemRandomnessInactive()
    {
        itemRandomnessDropSlot.gameObject.SetActive(false);
    }

    /// <summary>
    /// Destroys the unit.
    /// </summary>
    /// <param name="_unit"></param>
    public void DestroyUnit(UnitController _unit)
    {
        _unit.DestroyObject();
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

        if (_onSlot.Model.IsFullDurability() == false)
            return false;

        if (_onSlot.Model.SoUnit.Name == _onDrag.Model.SoUnit.Name &&
            _onSlot.Model.IsFullDurability() && _onDrag.Model.IsFullDurability())
            return true;

        return false;
    }

    public bool IsTurnAI()
    {
        var game = GameManager.Instance.CurrentGame;
        var player = GameManager.Instance.CurrentPlayer;

        return game == null ? false : game.Mode == GameMode.AI &&
            player == null ? false : player.Data.IsAI;
    }

    public List<UnitController> GetRandomShopBots()
    {
        List<UnitController> controllers = new();
        for (int i = 0; i < ShopBotSlots.Count; i++)
        {
            int randomNumber = Random.Range(0, PackManager.Instance.Bots.Count);
            var soUnit = PackManager.Instance.Bots[randomNumber];
            var unit = AddUnitController(soUnit, randomNumber, null, UnitState.InSlotShop);
            controllers.Add(unit);
        }
        return controllers;
    }

    public UnitController AddUnitController(SoUnit _soUnit, int _index, SaveUnitData _unitData, UnitState _unitState)
    {
        var unit = gameObject.AddComponent<UnitController>();
        unit.Initialize(_soUnit, _index, _unitData, UnitState.InSlotShop, true);
        return unit;
    }

    public bool HasAllFullRobots()
    {
        int robots = 0;
        int fullRobots = 0;

        foreach (var slot in TeamSlots)
        {
            var unitController = slot.UnitController();
            if (unitController != null)
            {
                robots++;
                fullRobots += unitController.Model.IsFullDurability() ? 1 : 0;
            }
        }

        return fullRobots == robots;
    }

    public bool IsBuyingItemNotUseful(UnitController _item, UnitController _target)
    {
        if (_item.Model.CurrentLevel.DoType == DoType.ShutDown &&
            _target.Model.CurrentLevel.TriggerType == TriggerType.Shutdown)
        {
            var consum = _target.Model.CurrentLevel.ConsumedEnergy;
            int consumENG = consum != null ? Mathf.Abs(consum.Value) : 0;

            if (_target.Model.Data.Cur.ENG < consumENG)
                return true;
        }

        return false;
    }

    public bool CanRecycleTrigger(UnitController _unit)
    {
        if (_unit.Model.CurrentLevel.TriggerType == TriggerType.Recycle)
        {
            var consum = _unit.Model.CurrentLevel.ConsumedEnergy;
            int consumENG = consum != null ? Mathf.Abs(consum.Value) : 0;

            if (_unit.Model.Data.Cur.ENG >= consumENG)
                return true;
        }

        return false;
    }
}