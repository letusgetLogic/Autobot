public abstract class StateBaseAI
{
    /// <summary>
    /// Base constructor of states of AI.
    /// </summary>
    public StateBaseAI()
    {
    }

    /// <summary>
    /// OnEnter is called when the state is entered.
    /// </summary>
    public abstract void OnEnter(I_FSM_AI _ctx);

    /// <summary>
    /// OnUpdate is called every frame while the state is active.
    /// </summary>
    public abstract void OnUpdate(I_FSM_AI _ctx, float _speed);

    /// <summary>
    /// OnExit is called when the state is exited.
    /// </summary>
    public virtual void OnExit(I_FSM_AI _ctx)
    { }
}

