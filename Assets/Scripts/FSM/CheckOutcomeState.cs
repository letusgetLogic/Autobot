public class CheckOutcomeState : StateBase
{
    private bool startOfBattle { get; set; }
    public CheckOutcomeState(float maxCount, bool startOfBattle) : base(maxCount)
    {
        this.startOfBattle = startOfBattle;
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        if (CheckOutcome())
            ctx.SetState(null);
        else
        {
            if (startOfBattle)
                ctx.SetState(new StartOfBattleState(0));
            else
                ctx.SetState(new InsertState(PhaseBattleController.Instance.DurationInsert));
        }
    }

    public override void OnUpdate(IFiniteStateMachine ctx)
    {}


    private bool CheckOutcome()
    {
        if (IsAnyoneIn(PhaseBattleController.Instance.Slot1))
        {
            if (IsAnyoneIn(PhaseBattleController.Instance.Slot2))
            {
                return false; // Continue battle
            }
            else
            {
                GameManager.Instance.UpdatePlayerStats(-1); // Left Wins
            }
            return true;
        }
        else
        {
            if (IsAnyoneIn(PhaseBattleController.Instance.Slot2))
            {
                GameManager.Instance.UpdatePlayerStats(1); // Right wins
            }
            else
            {
                GameManager.Instance.UpdatePlayerStats(0); // Draw
            }
            return true;
        }
    }

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
