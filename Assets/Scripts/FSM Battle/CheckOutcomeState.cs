using UnityEngine;

public class CheckOutcomeState : StateBase
{
    private bool startOfBattle;
    private bool outcome;

    public CheckOutcomeState(float maxTimeCount, bool startOfBattle) : base(maxTimeCount)
    {
        this.startOfBattle = startOfBattle;
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Debug.Log("--- CheckOutcomeState");
        outcome = CheckOutcome();
        if (outcome)
        {
            MaxTimeCount += MaxTimeCount; // Extend time for showing winner
        }
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += speed;
        }
        else
        {
            if (outcome)
            {
                ctx.SetState(new BattleOverState(
                    PhaseBattleController.Instance.Process.DurationBattleOver));
            }
            else
            {
                if (startOfBattle)
                    ctx.SetState(new StartOfBattleState(0));
                else
                    ctx.SetState(new InsertState(
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
        if (IsAnyoneIn(PhaseBattleController.Instance.Slots1))
        {
            if (IsAnyoneIn(PhaseBattleController.Instance.Slots2))
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
            if (IsAnyoneIn(PhaseBattleController.Instance.Slots2))
            {
                GameManager.Instance.UpdatePlayerStats(1); // Right wins

                PhaseBattleView.Instance.ShowWinner(PhaseBattleController.Instance.Player2.Data.Name, false);
                PhaseBattleView.Instance.UpdateLives(
                  PhaseBattleController.Instance.Player1.Data,
                  PhaseBattleController.Instance.Player2.Data);
            }
            else
            {
                GameManager.Instance.UpdatePlayerStats(0); // Draw

                PhaseBattleView.Instance.ShowWinner("Nobody", false);
                PhaseBattleView.Instance.UpdateLives(
                  PhaseBattleController.Instance.Player1.Data,
                  PhaseBattleController.Instance.Player2.Data);
            }

            PhaseBattleView.Instance.SetSpeedButton(false);
            return true;
        }
    }

    /// <summary>
    /// Checks if any unit is in slots.
    /// </summary>
    /// <param name="slots"></param>
    /// <returns></returns>
    private bool IsAnyoneIn(Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Unit() != null)
            {
                return true;
            }
        }
        return false;
    }

}
