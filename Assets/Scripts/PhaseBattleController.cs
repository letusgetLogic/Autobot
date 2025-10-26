using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhaseBattleController : MonoBehaviour, IFiniteStateMachine
{
    public static PhaseBattleController Instance { get; private set; }

    public UnityAction StartBattle {  get; private set; }

    [Header("Setting")]
    [SerializeField] 
    private float durationInsert = 0.5f;
    [SerializeField]
    private float durationShowOutcome = 1.0f;

    [Header("Slots")]
    [SerializeField]
    private Slot[] slots1;
    [SerializeField]
    private Slot[] slots2;
    public Slot[] Slots1 => slots1;
    public Slot[] Slots2 => slots2;

    [SerializeField]
    private GameObject unitPrefab;

    public float SpeedMultiplier { get; set; } = 1f;
    public float MaxMultiplier { get; set; } = 2f;


    public float DurationInsert => durationInsert;
    public float DurationShowOutcome => durationShowOutcome;

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
    
    public Queue<Summon> SummonUnits 
    {
        get
        {
            if (summonUnits == null)
                summonUnits = new Queue<Summon>();
            return summonUnits;
        }
        set
        {
            summonUnits = value;
        }
    }
    private Queue<Summon> summonUnits;

    public int IsAnyAbilityThere { get; set; }

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


    #region FSM

    public void Update()
    {
        if (state == null)
            return;

        state.OnUpdate(this, Time.deltaTime * SpeedMultiplier);
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
        SetState(new InitState(0.5f));
    }

    public bool DestroyUnit()
    {
        StartCoroutine(PleaseDie());
        return true;
    }

    private IEnumerator PleaseDie()
    {
        while (FaintUnits.Count > 0)
        {
            var unit = FaintUnits.Dequeue();
            GameObject.Destroy(unit); Debug.Log($"Fainted unit {unit.name} destroyed");
            yield return new WaitForEndOfFrame();
        }

    }
}
