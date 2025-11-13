using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhaseBattleController : MonoBehaviour, IFiniteStateMachine
{
    public static PhaseBattleController Instance { get; private set; }

    public UnityAction StartBattle {  get; private set; }

    [Header("Duration of each state")]
    [SerializeField]
    private SoBattleProcess process;
    public SoBattleProcess Process => process;

    [Header("Slots")]
    [SerializeField]
    private Slot[] slots1;
    [SerializeField]
    private Slot[] slots2;
    public Slot[] Slots1 => slots1;
    public Slot[] Slots2 => slots2;

    private StateBase state { get;set; }

    public Player Player1 { get; private set; }
    public Player Player2 {  get; private set; }

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
        set
        {
            unitAbilities = value;
        }
    }
    private Queue<AbilityBase> unitAbilities;
    public Queue<GameObject> FaintUnits 
    {
        get
        {
            if (faintUnits == null)
                faintUnits = new Queue<GameObject>();
            return faintUnits;
        }
        set
        {
            faintUnits = value;
        }
    }
    private Queue<GameObject> faintUnits;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        SetIndex(slots1);
        SetIndex(slots2);
    }

    /// <summary>
    /// Set index depend on draw order.
    /// </summary>
    private void SetIndex(Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Index = i;
    }


    #region Finite State Machine

    public void Update()
    {
        if (state == null)
            return;

        float speed = Time.deltaTime * GameManager.Instance.CurrentSpeedMultiplier;

        state.OnUpdate(this, speed);
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
        PhaseBattleView.Instance.SetSpeedButton(true);

        Player1.StartBattle();
        Player2.StartBattle();
        SetState(new InitState(Process.DurationInit));
    }

}
