using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseBattleController : MonoBehaviour, I_FSM_Battle
{
    public static PhaseBattleController Instance { get; private set; }

    [Header("Duration of each state")]
    [SerializeField]
    private SoBattleProcess process;
    public SoBattleProcess Process => process;

    [Header("Slots")]
    [SerializeField]
    private Slot[] slots1;
    [SerializeField]
    private Slot[] slots2;

    private StateBaseBattle state { get; set; }

    /// <summary>
    /// This sub state is used to run another states without breaking the current base state.
    /// </summary>
    public StateBaseBattle SubState { get; set; }

    public UnitController AttackingUnit1 => slots1[0].UnitController();
    public UnitController AttackingUnit2 => slots2[0].UnitController();

    /// <summary>
    /// Enqueue the unit abilities, and this queue will be executed later.
    /// </summary>
    public Queue<AbilityBase> UnitAbilities
    {
        get
        {
            if (unitAbilities == null)
                unitAbilities = new Queue<AbilityBase>();
            return unitAbilities;
        }
    }
    private Queue<AbilityBase> unitAbilities;

    /// <summary>
    /// Enqueue the units that will be shutdown, and this queue will be executed later.
    /// </summary>
    public Queue<UnitController> ShutdownUnits
    {
        get
        {
            if (shutdownUnits == null)
                shutdownUnits = new Queue<UnitController>();
            return shutdownUnits;
        }
    }
    private Queue<UnitController> shutdownUnits;

    public bool IsStopped { get; set; } = false;
    public float IsRunning { get; set; } = 1f;  // 1 = running, 0 = stopped


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        Time.timeScale = 1f;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning(this.name + ".Awake: GameManager instance not found.");
            return;
        }

        if (GameManager.Instance.Replay != null)
            GameManager.Instance.Replay.Switch(GameState.StartOfBattle);
        else
            GameManager.Instance.Switch(GameState.StartOfBattle);

        StartCoroutine(SetHintClick());
    }

    /// <summary>
    /// Set the player name to the hint click.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetHintClick()
    {
        yield return new WaitUntil(() => CutScene.Instance != null);

        CutScene.Instance.SetHintClickClose("", true);
    }

    private void Start()
    {
        PhaseBattleView.Instance.SetRunningButton();
        SetIndex(slots1);
        SetIndex(slots2);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnTriggerAbility += EnqueueAbility;
        EventManager.Instance.OnShutdown += EnqueueShutdown;
        EventManager.Instance.OnBattleDelayHintClick += DelayHintClick;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnTriggerAbility -= EnqueueAbility;
        EventManager.Instance.OnShutdown -= EnqueueShutdown;
        EventManager.Instance.OnBattleDelayHintClick -= DelayHintClick;
    }

    /// <summary>
    /// Set index depend on draw order.
    /// </summary>
    private void SetIndex(Slot[] _slots)
    {
        for (int i = 0; i < _slots.Length; i++)
            _slots[i].Index = i;
    }


    #region Finite State Machine

    public void Update()
    {
        if (Slots1() != null && Slots1()[0].UnitController())
        {
            var unit = Slots1()[0].UnitController();
            if (unit.Model != null)
            {
                Debug.Log(unit.name + " is left " + unit.Model.Data.IsTeamLeft);
            }
        }


        if (state == null)
            return;

        float speed = IsRunning * Time.deltaTime/* * GameManager.Instance.CurrentSpeedMultiplier*/;
        state.OnUpdate(this, speed);

        if (SubState != null)
            SubState.OnUpdate(this, speed);
    }

    /// <summary>
    /// Set the state of the battle.
    /// </summary>
    /// <param name="_state"></param>
    public void SetState(StateBaseBattle _state)
    {
        if (state != null)
            state.OnExit(this);

        state = _state;

        if (_state == null)
            return;

        state.OnEnter(this);
    }

    /// <summary>
    /// Set the sub state of the battle, and this sub state will run without breaking the current base state.
    /// </summary>
    /// <param name="_state"></param>
    public void SetSubState(StateBaseBattle _state)
    {
        if (SubState != null)
            SubState.OnExit(this);

        Debug.Log("--- Sub State ---");
        SubState = _state;

        if (_state == null)
        {
            Debug.Log("--- Sub State End ---");
            return;
        }

        SubState.OnEnter(this);
    }

    #endregion


    /// <summary>
    /// Runs the battle.
    /// </summary>
    /// <param name="_player1"></param>
    /// <param name="_player2"></param>
    public void Run(Player _player1, Player _player2)
    {
        //PhaseBattleView.Instance.SetSpeedButton(true);

        SetState(new InitializeState(Process.DurationInit));
    }

    /// <summary>
    /// Set boolean IsRunning.
    /// </summary>
    public void SetRunning()
    {
        IsStopped = !IsStopped;
        IsRunning = IsStopped ? 0f : 1f;
    }

    /// <summary>
    /// Hides the description of units on team slots while transporting.
    /// </summary>
    public void HideDescriptionByTransport()
    {
        foreach (var slot in slots1)
        {
            slot.HideDescription();
        }
        foreach (var slot in slots2)
        {
            slot.HideDescription();
        }
    }

    /// <summary>
    /// Enqueue the ability that will be triggered, and this queue will be executed later.
    /// </summary>
    /// <param name="ability"></param>
    /// <param name="isDestroyingUnit"></param>
    private void EnqueueAbility(AbilityBase ability, bool isDestroyingUnit)
    {
        UnitAbilities.Enqueue(ability);

        if (isDestroyingUnit)
        {
            EventManager.Instance.OnShutdown?.Invoke(ability.Controller);
        }
        Debug.Log($"{ability.ToString()} enqueue");
        Debug.Log($"{unitAbilities.Count} UnitAbilities");
    }

    /// <summary>
    /// Enqueue the unit that will be shutdown, and this queue will be executed later.
    /// </summary>
    /// <param name="unit"></param>
    private void EnqueueShutdown(UnitController unit)
    {
        ShutdownUnits.Enqueue(unit);
        Debug.Log($"{unit.gameObject.name} enqueue");
        Debug.Log($"{shutdownUnits.Count} ShutdownUnits");
    }

    /// <summary>
    /// Set the detect click active or not.
    /// </summary>
    /// <param name="_value"></param>
    public void SetDetectClickActive(bool _value)
    {
        //detectClickEnviroment.SetActive(_value);
    }

    /// <summary>
    /// Returns only active slots.
    /// </summary>
    /// <returns></returns>
    public Slot[] Slots1()
    {
        List<Slot> activeSlots = new List<Slot>();
        foreach (var slot in slots1)
        {
            if (slot.gameObject.activeSelf)
                activeSlots.Add(slot);
        }
        return activeSlots.ToArray();
    }

    /// <summary>
    /// Returns only active slots.
    /// </summary>
    /// <returns></returns>
    public Slot[] Slots2()
    {
        List<Slot> activeSlots = new List<Slot>();
        foreach (var slot in slots2)
        {
            if (slot.gameObject.activeSelf)
                activeSlots.Add(slot);
        }
        return activeSlots.ToArray();
    }

    /// <summary>
    /// Delays show hint to click.
    /// </summary>
    /// <param name="_duration"></param>
    public void DelayHintClick()
    {
        StartCoroutine(PhaseBattleView.Instance.ShowClick(Process.WaitForClickShow));
    }

}
