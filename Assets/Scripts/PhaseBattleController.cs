using System.Collections.Generic;
using UnityEngine;

public class PhaseBattleController : MonoBehaviour, IFiniteStateMachine
{
    public static PhaseBattleController Instance { get; private set; }

    [Header("Setting")]
    [SerializeField] 
    private float durationInsert = 0.5f;
    [SerializeField]
    private float delayDeath = 0.5f;
    [SerializeField]
    private float durationShowOutcome = 1.0f;

    [Header("Slots")]
    [SerializeField]
    private Slot[] slots1;
    [SerializeField]
    private Slot[] slots2;

    public float DurationInsert => durationInsert;
    public float DurationShowOutcome => durationShowOutcome;
    public Slot[] Slots1 => slots1;
    public Slot[] Slots2 => slots2;

    private StateBase state { get;set; }

    private Template player1, player2;
    public Template Player1 => player1;
    public Template Player2 => player2;

    public UnitController AttackingUnit1
    {
        get
        {
            return slots1[0].UnitController();
        }
    }

    public UnitController AttackingUnit2
    {
        get
        {
            return slots2[0].UnitController();
        }
    }

    public Queue<AbilityBase> UnitAbilities { get; set; }
 

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }


    #region FSM

    public void Update()
    {
        if (state == null)
            return;

        state.OnUpdate(this, Time.deltaTime);
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
    public void Run(Template _player1, Template _player2)
    {
        player1 = _player1;
        player2 = _player2;

       SetState(new InitState(0.5f));
    }


}
