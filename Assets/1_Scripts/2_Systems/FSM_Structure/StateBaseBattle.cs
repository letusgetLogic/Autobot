public abstract class StateBaseBattle
{
    protected bool IsDone {  get; set; } = false;
    protected float TimeCount {  get; set; }
    protected float MaxTimeCount {  get; set; }

    /// <summary>
    /// Base constructor of states of the battle phase.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public StateBaseBattle(float _maxTimeCount)
    {
        TimeCount = 0;
        MaxTimeCount = _maxTimeCount;
    }

    /// <summary>
    /// OnEnter is called when the state is entered.
    /// </summary>
    public abstract void OnEnter(I_FSM_Battle _ctx);

    /// <summary>
    /// OnUpdate is called every frame while the state is active.
    /// </summary>
    public abstract void OnUpdate(I_FSM_Battle _ctx, float _speed);

    /// <summary>
    /// OnExit is called when the state is exited.
    /// </summary>
    public virtual void OnExit(I_FSM_Battle _ctx)
    { }
}

