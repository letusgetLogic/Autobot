using UnityEngine;

public class InitState : StateBase
{
    public InitState(float maxTimeCount) : base(maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Debug.Log(PhaseBattleController.Instance.Player1.Data.Turns);
        Debug.Log("--- InitState");
        Initialize();
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += speed;
        }
        else
        {
            GameManager.Instance.CurrentGame.State = GameState.BattlePhase;
            ctx.SetState(new CheckOutcomeState(
                PhaseBattleController.Instance.Process.DurationCheckOutcome, true));
        }
    }

    /// <summary>
    /// Initializes the players.
    /// </summary>
    private void Initialize()
    {
        PhaseBattleView.Instance.Initialize(
            PhaseBattleController.Instance.Player1.Data,
            PhaseBattleController.Instance.Player2.Data);

        SpawnUnits(
            PhaseBattleController.Instance.Player1,
            PhaseBattleController.Instance.Slots1, false);

        SpawnUnits(
            PhaseBattleController.Instance.Player2,
            PhaseBattleController.Instance.Slots2, true);
    }

    /// <summary>
    /// Instantiates and initializes the units.
    /// </summary>
    private void SpawnUnits(Player _player, Slot[] _slots, bool _isRight)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            var unitData = _player.Data.TeamUnitDatas[i];
            if (unitData.HasReference && unitData.Cur.HP > 0)
            {
                Debug.Log(unitData.ID + " spawned - HP: " + unitData.Cur.HP);
                var unitController = SpawnManager.Instance.Spawn(
                    PackManager.Instance.Units[unitData.Index],
                    unitData.Index,
                    unitData,
                    UnitState.InPhaseBattle,
                    _slots[i].transform,
                    _isRight);

                _player.BattleUnits[i] = unitController;
            }
        }
    }
}