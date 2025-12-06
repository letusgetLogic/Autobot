using UnityEngine;

public class CheckOutcomeState : StateBase
{
    private bool startOfBattle;
    private bool hasOutcomeOfBattle;

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
                if (startOfBattle)
                    _ctx.SetState(new StartOfBattleState(0));
                else
                    _ctx.SetState(new InsertState(
                        PhaseBattleController.Instance.Process.DurationInsert));
            }
        }
    }

    /// <summary>
    /// Checks the outcome of battle.
    /// </summary>
    /// <returns></returns>
    private bool CheckOutcome()
    {
        if (IsAnyoneIn(PhaseBattleController.Instance.Slots1, null))
        {
            if (IsAnyoneIn(PhaseBattleController.Instance.Slots2, null))
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

            PhaseBattleView.Instance.SetSpeedButton(false);
            return true;
        }
        else
        {
            if (IsAnyoneIn(PhaseBattleController.Instance.Slots2, null))
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

            PhaseBattleView.Instance.SetSpeedButton(false);
            return true;
        }
    }

    /// <summary>
    /// Checks if any unit is in slots.
    /// </summary>
    /// <param name="slots"></param>
    /// <param name="_exclusiveSlot"> The reference of Slot, which shouldn't be checked.</param>
    /// <returns></returns>
    public static bool IsAnyoneIn(Slot[] slots, Slot _exclusiveSlot)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Unit() != null)
            {
                if (_exclusiveSlot != null && _exclusiveSlot == slots[i])
                    continue;

                return true;
            }
        }
        return false;
    }

}
