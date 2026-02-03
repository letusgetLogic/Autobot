using System.Collections;
using System.Diagnostics;
using UnityEngine;

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
        System.Diagnostics.Debug.WriteLine("--- AttackState");

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

        unit1.MoveWhileAttacking();
        float animDelay = unit2.MoveWhileAttacking();

        unit1.TakeDamage(unit2.TriggerAttack());
        unit2.TakeDamage(unit1.TriggerAttack());

        PhaseBattleController.Instance.StartCoroutine(ShowCollider(animDelay));
    }

    private IEnumerator ShowCollider(float _delay)
    {
        yield return new WaitForSeconds(_delay);

        PhaseBattleView.Instance.ShowCollideVisual();

        PhaseBattleController.Instance.StartCoroutine(
            PhaseBattleView.Instance.HideCollideVisual(
                PhaseBattleController.Instance.Process.DurationShowCollide));
    }
}