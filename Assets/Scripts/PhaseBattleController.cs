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
    public Slot[] Slot1 => slots1;
    public Slot[] Slot2 => slots2;

    private StateBase state { get;set; }

    private Template player1, player2;
    public Template Player1 => player1;
    public Template Player2 => player2;


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

        if (Initialize())
        {
            SetState(new CheckOutcomeState(0, true));
        }
    }

    /// <summary>
    /// Initializes the players.
    /// </summary>
    public bool Initialize()
    {
        PhaseBattleView.Instance.Initialize(player1, player2 );

        SetUnitsToPosition(player1, slots1, false);
        SetUnitsToPosition(player2, slots2, true);

        return true;
    }

    /// <summary>
    /// Instantiates the units.
    /// </summary>
    private void SetUnitsToPosition(Template player, Slot[] slots, bool isRight)
    {
        for (int i = 0; i < player.TeamSlots.Length; i++)
        {
            var unit = player.TeamSlots[i].Unit();
            if (unit != null)
            {
                var unitOnScene = Instantiate(unit);
                unitOnScene.GetComponent<UnitController>().SetModel(unit.GetComponent<UnitController>().Model);
                unitOnScene.transform.SetParent(slots[i].transform, false);

                if (isRight)
                    unitOnScene.GetComponent<UnitView>().SetRightSide();
            }
        }
    }
}
