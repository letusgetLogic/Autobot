using System.Collections;
using UnityEngine;

public class InitializeState : StateBase
{
    /// <summary>
    /// Constructor of InitializeState.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public InitializeState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log(PhaseBattleController.Instance.Player1.Data.Turn);
        Debug.Log("--- InitState");
        PhaseBattleController.Instance.StartCoroutine(Initialize());
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
        else
        {
            _ctx.SetState(new CheckOutcomeState(
                PhaseBattleController.Instance.Process.DurationCheckOutcome));
        }
    }

    /// <summary>
    /// Initializes the unit team of players.
    /// </summary>
    private IEnumerator Initialize()
    {
        yield return new WaitUntil(() => PhaseBattleView.Instance != null);

        PhaseBattleView.Instance.Initialize(
            PhaseBattleController.Instance.Player1.Data,
            PhaseBattleController.Instance.Player2.Data);

        SpawnUnits(
            PhaseBattleController.Instance.Player1,
            PhaseBattleController.Instance.Slots1(), true);

        SpawnUnits(
            PhaseBattleController.Instance.Player2,
            PhaseBattleController.Instance.Slots2(), false);
    }

    /// <summary>
    /// Instantiates and initializes the units.
    /// </summary>
    private void SpawnUnits(Player _player, Slot[] _slots, bool _isLeft)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            var unitData = _player.Data.TeamUnitDatas[i];
            if (unitData.HasReference && unitData.Cur.HP > 0)
            {
                var unitController = SpawnManager.Instance.Spawn(
                    PackManager.Instance.GetSoUnit(unitData).soUnit,
                    PackManager.Instance.GetSoUnit(unitData).index,
                    unitData,
                    UnitState.InPhaseBattle,
                    _slots[i].transform,
                    _isLeft);

                _player.BattleUnits[i] = unitController;
            }
        }
    }
}