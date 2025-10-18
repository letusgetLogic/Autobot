public class BeforeAttackState : StateBase
{
    public BeforeAttackState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        ctx.SetState(new AttackState(0));
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        
    }
}