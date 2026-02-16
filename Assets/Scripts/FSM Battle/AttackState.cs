using System.Collections;
using UnityEngine;

public class AttackState : StateBase
{
    private bool setSubState = false;

    private bool setRefresh = false;
    private float timeCount = 0f;

    /// <summary>
    /// Constructor of AttackState.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public AttackState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log("--- AttackState");
        PhaseBattleController.Instance.StartCoroutine(AttackEachOther());
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        timeCount += _speed;

        if (setRefresh)
        {
            _ctx.SetState(new CheckOutcomeState(PhaseBattleController.Instance.Process.DurationCheckOutcome));
            setRefresh = false;
        }

        if (setSubState)
        {
            _ctx.SetSubState(new HandleAbilityState(0));
            setSubState = false;
        }

        if (IsDone)
        {
            _ctx.SetState(new HandleAbilityState(0));
        }
    }

    public override void OnExit(IFiniteStateMachine _ctx)
    {
        if (PhaseBattleController.Instance.SubState != null)
        {
            _ctx.SetSubState(null);
        }
    }

    /// <summary>
    /// Lets the 2 units attacking each other.
    /// </summary>
    private IEnumerator AttackEachOther()
    {
        UnitController unit1 = PhaseBattleController.Instance.AttackingUnit1;
        UnitController unit2 = PhaseBattleController.Instance.AttackingUnit2;

        yield return new WaitUntil(() =>
        {
            // If the time count was exceeded, check the outcome.
            if (timeCount > PhaseBattleController.Instance.Process.RefreshRate)
            {
                setRefresh = true;
            }

            return unit1 != null && unit2 != null;
        });
        // --------------------------------------------------------------------
        timeCount = 0f;

        // Triggers before attack
        int countTrigger = TriggerAbilityManager.Instance.TriggerBeforeAttack(unit1, unit2);

        if (countTrigger > 0)
        {
            setSubState = true;

            yield return null;

            yield return new WaitUntil(() =>
            {
                // If the time count was exceeded, check the outcome.
                if (timeCount > PhaseBattleController.Instance.Process.RefreshRate)
                {
                    setRefresh = true;
                }

                return PhaseBattleController.Instance.SubState == null;
            });
        }
        // ---------------------------------------------------------------------
        timeCount = 0f;

        unit1.MoveWhileAttacking();
        float animDelay = unit2.MoveWhileAttacking();

        Debug.Log($"Attack each other: {unit1} & {unit2} ");

        unit1.TakeDamage(unit2.TriggerAttack());
        unit2.TakeDamage(unit1.TriggerAttack());

        PhaseBattleController.Instance.StartCoroutine(ShowCollider(animDelay));

        yield return new WaitForSeconds(MaxTimeCount);

        IsDone = true;
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