public class CheckOutcomeState : StateBase
{
    private bool startOfBattle { get; set; }
    private bool outcome { get; set; }

    public CheckOutcomeState(float maxCount, bool startOfBattle) : base(maxCount)
    {
        this.startOfBattle = startOfBattle;
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        outcome = CheckOutcome();
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (Count < MaxCount)
        {
            Count += speed;
        }
        else
        {
            if (outcome)
                ctx.SetState(new BattleOverState(1f));
            else
            {
                if (startOfBattle)
                    ctx.SetState(new StartOfBattleState(0));
                else
                    ctx.SetState(new InsertState(PhaseBattleController.Instance.DurationInsert));
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
                PhaseBattleView.Instance.ShowWinner(PhaseBattleController.Instance.Player1.Data.Name, false);
                GameManager.Instance.UpdatePlayerStats(-1); // Left Wins
            }
            return true;
        }
        else
        {
            if (IsAnyoneIn(PhaseBattleController.Instance.Slots2))
            {
                PhaseBattleView.Instance.ShowWinner(PhaseBattleController.Instance.Player2.Data.Name, false);
                GameManager.Instance.UpdatePlayerStats(1); // Right wins
            }
            else
            {
                PhaseBattleView.Instance.ShowWinner("Nobody", false);
                GameManager.Instance.UpdatePlayerStats(0); // Draw
            }
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
