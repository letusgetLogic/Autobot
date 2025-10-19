public class FaintState : StateBase
{
    public FaintState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        PhaseBattleController.Instance.Player1.HideFaintUnits(PhaseBattleController.Instance.Slots1);
        PhaseBattleController.Instance.Player2.HideFaintUnits(PhaseBattleController.Instance.Slots2);

        ctx.SetState(new CheckOutcomeState(PhaseBattleController.Instance.DurationShowOutcome, false));
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        
    }
}