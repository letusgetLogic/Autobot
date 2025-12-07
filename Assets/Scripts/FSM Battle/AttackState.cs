using System.Diagnostics;

public class AttackState : StateBase
{
    /// <summary>
    /// Constructor of AttackState.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public AttackState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.WriteLine("--- AttackState");

        AttackEachOther();
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        if (TimeCount < MaxTimeCount)
        {
            TimeCount += _speed;
        }
        else
        {
            _ctx.SetState(new HandleAbilityState(0));
        }
    }

    /// <summary>
    /// Lets the 2 units attacking each other.
    /// </summary>
    private void AttackEachOther()
    {
        UnitController unit1 = PhaseBattleController.Instance.AttackingUnit1;
        UnitController unit2 = PhaseBattleController.Instance.AttackingUnit2;

        if (unit1 == null || unit2 == null)
            return;

        unit1.TakeDamage(unit2.TriggerAttack());
        unit2.TakeDamage(unit1.TriggerAttack());
    }
}