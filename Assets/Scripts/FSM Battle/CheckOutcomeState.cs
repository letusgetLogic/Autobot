using UnityEngine;

public class CheckOutcomeState : StateBase
{
    private bool startOfBattle;
    private bool hasOutcomeOfBattle;
    private int amountOfActiveUnits1;
    private int amountOfActiveUnits2;

    /// <summary>
    /// Constructor of CheckOutcomeState
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    /// <param name="_startOfBattle"></param>
    public CheckOutcomeState(float _maxTimeCount, bool _startOfBattle) : base(_maxTimeCount)
    {
        this.startOfBattle = _startOfBattle;
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log("--- CheckOutcomeState");
        hasOutcomeOfBattle = CheckOutcome();
        if (hasOutcomeOfBattle)
        {
            MaxTimeCount += MaxTimeCount; // Extend time for showing winner
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
            if (hasOutcomeOfBattle)
            {
                _ctx.SetState(new BattleOverState(
                    PhaseBattleController.Instance.Process.DurationBattleOver));
            }
            else
            {
                //if (startOfBattle)
                //    _ctx.SetState(new StartOfBattleState(0));
                //else

                if (NeedInsert(PhaseBattleController.Instance.Slots1(), amountOfActiveUnits1) ||
                    NeedInsert(PhaseBattleController.Instance.Slots2(), amountOfActiveUnits2))
                {
                    _ctx.SetState(new InsertState(
                        PhaseBattleController.Instance.Process.DurationInsert));
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
    private bool CheckOutcome()
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
                GameManager.Instance.UpdatePlayerStats(-1); // Left Wins

                PhaseBattleView.Instance.ShowWinner(PhaseBattleController.Instance.Player1.Data.Name, false);
                PhaseBattleView.Instance.UpdateLives(
                    PhaseBattleController.Instance.Player1.Data,
                    PhaseBattleController.Instance.Player2.Data);
            }

            //PhaseBattleView.Instance.SetSpeedButton(false);
            return true;
        }
        else
        {
            if (amountOfActiveUnits2 > 0)
            {
                GameManager.Instance.UpdatePlayerStats(1); // Right wins

                PhaseBattleView.Instance.ShowWinner(PhaseBattleController.Instance.Player2.Data.Name, false);
            }
            else
            {
                GameManager.Instance.UpdatePlayerStats(0); // Draw

                PhaseBattleView.Instance.ShowWinner("Nobody", false);
            }

            PhaseBattleView.Instance.UpdateLives(
              PhaseBattleController.Instance.Player1.Data,
              PhaseBattleController.Instance.Player2.Data);

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
