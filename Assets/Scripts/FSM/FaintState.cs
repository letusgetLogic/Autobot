public class FaintState : StateBase
{
    public FaintState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        PhaseBattleController.Instance.Player1.ManageFaintUnits(
            PhaseBattleController.Instance.Slots1, true);

        PhaseBattleController.Instance.Player2.ManageFaintUnits(
            PhaseBattleController.Instance.Slots2, true);

        ctx.SetState(new CheckOutcomeState(PhaseBattleController.Instance.DurationShowOutcome, false));
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        
    }
}