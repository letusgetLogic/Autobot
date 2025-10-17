public class FaintState : StateBase
{
    public FaintState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        PhaseBattleController.Instance.DestroyFaint();
        ctx.SetState(new CheckOutcomeState(0.5f, false));
    }

    public override void OnUpdate(IFiniteStateMachine ctx)
    {
        
    }
}