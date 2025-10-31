public interface IFiniteStateMachine
{
    /// <summary>
    /// Update is called every frame to update the state machine.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// SetState is used to change the current state of the state machine.
    /// </summary>
    /// <param name="state"></param>
    public abstract void SetState(StateBase state);
}

