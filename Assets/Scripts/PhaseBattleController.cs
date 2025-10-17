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

    [Header("Slots")]
    [SerializeField]
    private Slot[] slots1;
    [SerializeField]
    private Slot[] slots2;

    public float DurationInsert => durationInsert;
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

    public Queue<UnitController> Triggers { get; set; }
 

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

        state.OnUpdate(this);
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

    /// <summary>
    /// Instantiate game object.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public GameObject Spawn(GameObject gameObject)
    {
        return Instantiate(gameObject);
    }

    public void DestroyFaint()
    {
        for (int i = 0; i < slots1.Length; i++)
        {
            var unit = slots1[i].UnitController();
            if (unit != null && unit.Model.IsFaint)
            {
                Destroy(unit.gameObject);
            }
        }
        for (int i = 0; i < slots2.Length; i++)
        {
            var unit = slots2[i].UnitController();
            if (unit != null && unit.Model.IsFaint)
            {
                Destroy(unit.gameObject);
            }
        }
    }
}
