using UnityEngine;

public class AttackState : StateBase
{
    public AttackState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        AttackEachOther();
    }

    public override void OnUpdate(IFiniteStateMachine ctx)
    {
        if (Count < 0.5f)
        {
            Count += Time.deltaTime;
        }
        else
        {
            ctx.SetState(new AfterAttack(0));
        }
    }

    private void AttackEachOther()
    {
        var unit1 = PhaseBattleController.Instance.AttackingUnit1;
        var unit2 = PhaseBattleController.Instance.AttackingUnit2;

        if (unit1 == null || unit2 == null)
            return;

        unit1.TakeDamage(unit2.Model.BattleAttack);
        unit2.TakeDamage(unit1.Model.BattleAttack);
    }
}