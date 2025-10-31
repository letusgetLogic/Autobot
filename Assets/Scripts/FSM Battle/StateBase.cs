public abstract class StateBase
{
    protected float TimeCount {  get; set; }
    protected float MaxTimeCount {  get; set; }

    public StateBase(float maxTimeCount)
    {
        TimeCount = 0;
        MaxTimeCount = maxTimeCount;
    }

    /// <summary>
    /// OnEnter is called when the state is entered.
    /// </summary>
    public abstract void OnEnter(IFiniteStateMachine ctx);

    /// <summary>
    /// OnUpdate is called every frame while the state is active.
    /// </summary>
    public abstract void OnUpdate(IFiniteStateMachine ctx, float speed);

    /// <summary>
    /// OnExit is called when the state is exited.
    /// </summary>
    public virtual void OnExit(IFiniteStateMachine ctx)
    { }
}

