using System.Diagnostics;

public class StartOfBattleState : StateBase
{
    /// <summary>
    /// Constructor of StartOfBattleState.
    /// </summary>
    /// <param name="_maxCount"></param>
    public StartOfBattleState(float _maxCount) : base(_maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.WriteLine("--- StartOfBattleState");

        int trigger = 0;

        trigger += TriggerAbility(PhaseBattleController.Instance.Slots1());
        trigger += TriggerAbility(PhaseBattleController.Instance.Slots2());

        if (trigger > 0)
            _ctx.SetState(new HandleAbilityState(0));
        else
            _ctx.SetState(new AttackState(
                     PhaseBattleController.Instance.Process.DurationAttack));
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        
    }

    public override void OnExit(IFiniteStateMachine _ctx)
    {
        GameManager.Instance.CurrentGame.State = GameState.BattlePhase;
    }

    /// <summary>
    /// Triggers the ability before attacking.
    /// </summary>
    /// <param name="_slots"></param>
    private int TriggerAbility(Slot[] _slots)
    {
        int hasTrigger = 0;

        foreach (var slot in _slots)
        {
            var unit = slot.UnitController();
            if (unit != null)
            {
                hasTrigger += unit.TriggerStartOfBattle() ? 1 : 0;
            }
        }

        return hasTrigger;
    }
}