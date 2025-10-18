public abstract class StateBase
{
    protected float Count {  get; set; }
    protected float MaxCount {  get; set; }

    public StateBase(float maxCount)
    {
        Count = 0;
        MaxCount = maxCount;
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

