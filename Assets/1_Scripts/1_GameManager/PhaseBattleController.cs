using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhaseBattleController : MonoBehaviour, I_FSM_Battle
{
    public static PhaseBattleController Instance { get; private set; }

    [Header("Duration of each state")]
    [SerializeField]
    private SoBattleProcess process;
    public SoBattleProcess Process => process;

    [Header("Slots")]
    [SerializeField] private List<Slot> slots1;
    [SerializeField] private List<Slot> slots2;
    [SerializeField] private Transform[] winnerSlots;

    public List<Slot> Slots1 => slots1.Where(x => x.gameObject.activeSelf).ToList();
    public List<Slot> Slots2 => slots2.Where(x => x.gameObject.activeSelf).ToList();
    public Transform[] WinnerSlots => winnerSlots;

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

    public List<Coroutine> Coroutines
    {
        get
        {
            if (coroutines == null)
                coroutines = new();
            return coroutines;
        }
    }
    private List<Coroutine> coroutines;

    public float Speed { get; set; } = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.Replay != null)
            GameManager.Instance.Replay.Switch(GameState.StartOfBattle);
        else
            GameManager.Instance.Switch(GameState.StartOfBattle);
    }

    private void Start()
    {
        SetSpeed(GameManager.Instance.BattleSpeed);
        Slots1.ForEach(x => x.Index = Slots1.IndexOf(x));
        Slots2.ForEach(x => x.Index = Slots2.IndexOf(x));
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

    private void OnDestroy()
    {
        Instance = null;
    }

    #region Finite State Machine

    public void Update()
    {
        if (state == null)
            return;

        float speed = Speed * Time.deltaTime /** GameManager.Instance.CurrentSpeedMultiplier*/;
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
    public void SetRunning(bool _isRunning, bool _affectTimeScale)
    {
        GameManager.Instance.IsStopped = !_isRunning;
        Speed = _isRunning ? 1f : 0f;

        if (_affectTimeScale)
            GameManager.Instance.SetTime(_isRunning ? GameManager.Instance.BattleSpeed : 0f);

        PhaseBattleView.Instance.SetRunningButton();
    }

    public void SetSpeed(float _speed)
    {
        GameManager.Instance.BattleSpeed = _speed;

        if (!GameManager.Instance.IsStopped)
            GameManager.Instance.SetTime(GameManager.Instance.BattleSpeed);
        
        PhaseBattleView.Instance.SetRunningButton();
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
    /// Delays show hint to click.
    /// </summary>
    /// <param name="_duration"></param>
    public void DelayHintClick()
    {
        StartCoroutine(PhaseBattleView.Instance.ShowClick(Process.WaitForClickShow));
    }

    /// <summary>
    /// Checks the outcome of battle.
    /// </summary>
    /// <returns></returns>
    public bool HasOutcome()
    {
        int amountOfActiveUnits1 = Slots1.Count(n =>
        {
            var unit = n.UnitController();
            return unit != null && unit.Model.Data.Cur.HP > 0;
        }
        );
        int amountOfActiveUnits2 = Slots2.Count(n =>
        {
            var unit = n.UnitController();
            return unit != null && unit.Model.Data.Cur.HP > 0;
        }
        );

        if (amountOfActiveUnits1 > 0)
        {
            if (amountOfActiveUnits2 > 0)
            {
                return false; // Continue battle
            }
            else
            {
                return true; // Left wins
            }
        }
        else
        {
            if (amountOfActiveUnits2 > 0)
            {
                return true; // Right wins
            }
            else
            {
                return true; // Draw
            }
        }
    }

}
