using System.Collections;
using UnityEngine;

public class InitializeState : StateBase
{
    private Player player1;
    private Player player2;

    /// <summary>
    /// Constructor of InitializeState.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public InitializeState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log("--- InitState - Battle Phase " + GameManager.Instance.CurrentGame.SavedRounds.Count);


        if (GameManager.Instance.Players.Count < 2)
        {
            Debug.LogWarning("Players.Count = " + GameManager.Instance.Players.Count);
            _ctx.SetState(null);
            return;
        }

        player1 = GameManager.Instance.Players[0];
        player2 = GameManager.Instance.Players[1];

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

        PhaseBattleView.Instance.Initialize(player1.Data, player2.Data);

        SpawnUnits(player1, PhaseBattleController.Instance.Slots1(), true);
        SpawnUnits(player2, PhaseBattleController.Instance.Slots2(), false);
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