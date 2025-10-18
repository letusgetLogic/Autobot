public class AfterAttack : StateBase
{
    public AfterAttack(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        ctx.SetState(new FaintState(0));
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        
    }
}