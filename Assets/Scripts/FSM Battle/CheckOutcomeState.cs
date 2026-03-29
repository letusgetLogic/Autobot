using UnityEngine;

public class CheckOutcomeState : StateBase
{
    private int outcome = -2;  // 0 = draw, 1 = right wins, -1 = left wins.
    private bool hasOutcomeOfBattle;
    private int amountOfActiveUnits1;
    private int amountOfActiveUnits2;

    /// <summary>
    /// Constructor of CheckOutcomeState
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    /// <param name="_startOfBattle"></param>
    public CheckOutcomeState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log("--- CheckOutcomeState");

        if (GameManager.Instance.Players.Count < 2)
        {
            Debug.LogWarning("Players.Count = " + GameManager.Instance.Players.Count);
            _ctx.SetState(null);
            return;
        }

        var player1 = GameManager.Instance.Players[0];
        var player2 = GameManager.Instance.Players[1];

        hasOutcomeOfBattle = CheckOutcome(player1, player2);

        // Necessary for jumping directly on next state, which isn't BattleOverState
        if (hasOutcomeOfBattle)
        {
            _ctx.SetState(new BattleOverState(
                  PhaseBattleController.Instance.Process.DurationBattleOver, outcome));
        }
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
        else
        {
            if (NeedInsert(PhaseBattleController.Instance.Slots1(), amountOfActiveUnits1) ||
                     NeedInsert(PhaseBattleController.Instance.Slots2(), amountOfActiveUnits2))
            {
                _ctx.SetState(new InsertState(
                    PhaseBattleController.Instance.Process.DurationInsert));
            }
            else
            {
                var gameManager = GameManager.Instance;
                if (gameManager.CurrentGame.State == GameState.StartOfBattle ||
                    gameManager.Replay != null && gameManager.Replay.State == GameState.StartOfBattle)
                {
                    _ctx.SetState(new StartOfBattleState(0));
                }
                else
                {
                    _ctx.SetState(new AttackState(
                         PhaseBattleController.Instance.Process.DurationAttack));
                }
            }
        }
    }

    /// <summary>
    /// Checks the outcome of battle.
    /// </summary>
    /// <returns></returns>
    private bool CheckOutcome(Player _player1, Player _player2)
    {
        var slots1 = PhaseBattleController.Instance.Slots1();
        var slots2 = PhaseBattleController.Instance.Slots2();

        amountOfActiveUnits1 = IsAnyoneIn(slots1, null);
        amountOfActiveUnits2 = IsAnyoneIn(slots2, null);

        if (amountOfActiveUnits1 > 0)
        {
            if (amountOfActiveUnits2 > 0)
            {
                return false; // Continue battle
            }
            else
            {
                outcome = -1; // Left wins
            }

            //PhaseBattleView.Instance.SetSpeedButton(false);
            return true;
        }
        else
        {
            if (amountOfActiveUnits2 > 0)
            {
                outcome = 1; // Right wins
            }
            else
            {
                outcome = 0;
            }

            //PhaseBattleView.Instance.SetSpeedButton(false);
            return true;
        }
    }

    /// <summary>
    /// Checks if any unit is in slots.
    /// </summary>
    /// <param name="_slots"></param>
    /// <param name="_exclusiveSlot"> The reference of Slot, which shouldn't be checked.</param>
    /// <returns></returns>
    public static int IsAnyoneIn(Slot[] _slots, Slot _exclusiveSlot)
    {
        int count = 0;

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].Unit() != null)
            {
                if (_exclusiveSlot != null && _exclusiveSlot == _slots[i])
                    continue;

                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// If all active units are at the front, then skip insert, otherwise insert unit.
    /// </summary>
    /// <param name="_slots"></param>
    /// <param name="_amountOfActive"></param>
    /// <returns></returns>
    private bool NeedInsert(Slot[] _slots, int _amountOfActive)
    {
        for (int i = 0; i < _amountOfActive; i++)
        {
            if (_slots[i].Unit() == null)
            {
                return true;
            }
        }
        return false;
    }
}
