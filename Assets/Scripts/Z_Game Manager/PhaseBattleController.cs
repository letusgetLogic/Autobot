using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhaseBattleController : MonoBehaviour, IFiniteStateMachine
{
    public static PhaseBattleController Instance { get; private set; }

    public UnityAction StartBattle { get; private set; }

    [Header("Duration of each state")]
    [SerializeField]
    private SoBattleProcess process;
    public SoBattleProcess Process => process;

    [Header("Slots")]
    [SerializeField]
    private Slot[] slots1;
    [SerializeField]
    private Slot[] slots2;

    [Header("Detect Click Enviroment")]
    [SerializeField] private GameObject detectClickEnviroment;

    private StateBase state { get; set; }
    public StateBase SubState { get; set; }

    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }

    public UnitController AttackingUnit1 => slots1[0].UnitController();
    public UnitController AttackingUnit2 => slots2[0].UnitController();

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

    private UnityAction<AbilityBase, bool> onEnqueueAbility => (ability, isDestroyingUnit) =>
    {
        UnitAbilities.Enqueue(ability);

        if (isDestroyingUnit)
        {
            EventManager.Instance.OnShutdown?.Invoke(ability.Controller);
        }
        Debug.Log($"{ability.ToString()} enqueue");
        Debug.Log($"{unitAbilities.Count} UnitAbilities");
    };

    private UnityAction<UnitController> onEnqueueShutdown => unit =>
    {
        ShutdownUnits.Enqueue(unit);
        Debug.Log($"{unit.gameObject.name} enqueue");
        Debug.Log($"{shutdownUnits.Count} ShutdownUnits");
    };

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        Time.timeScale = 1f;
        detectClickEnviroment.SetActive(false);
        GameManager.Instance.Switch(GameState.StartOfBattle);
    }

    private void Start()
    {
        PhaseBattleView.Instance.SetRunningButton();
        SetIndex(slots1);
        SetIndex(slots2);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnTriggerAbility += onEnqueueAbility;
        EventManager.Instance.OnShutdown += onEnqueueShutdown;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnTriggerAbility -= onEnqueueAbility;
        EventManager.Instance.OnShutdown -= onEnqueueShutdown;
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
        if (state == null)
            return;

        float speed = IsRunning * Time.deltaTime/* * GameManager.Instance.CurrentSpeedMultiplier*/;
        state.OnUpdate(this, speed);

        if (SubState != null)
            SubState.OnUpdate(this, speed);
    }

    public void SetState(StateBase _state)
    {
        if (state != null)
            state.OnExit(this);

        state = _state;

        if (_state == null)
            return;

        state.OnEnter(this);
    }

    public void SetSubState(StateBase _state)
    {
        if (SubState != null)
            SubState.OnExit(this);

        SubState = _state;

        if (_state == null)
            return;

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
        Player1 = _player1;
        Player2 = _player2;

        StartBattle?.Invoke();
        //PhaseBattleView.Instance.SetSpeedButton(true);

        Player1.StartBattle();
        Player2.StartBattle();
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
    /// Set the detect click active or not.
    /// </summary>
    /// <param name="_value"></param>
    public void SetDetectClickActive(bool _value)
    {
        detectClickEnviroment.SetActive(_value);
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
}
