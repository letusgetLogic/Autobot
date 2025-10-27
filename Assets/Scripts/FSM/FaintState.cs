using System.Collections;
using UnityEngine;

public class FaintState : StateBase
{
    public FaintState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Debug.Log("--- FaintState");

        PhaseBattleController.Instance.StartCoroutine(DestroyUnit());

        if (PhaseBattleController.Instance.UnitAbilities.Count > 0)
            ctx.SetState(new HandleAbilityState(0));
        else
           if (PhaseBattleController.Instance.SummonUnits.Count > 0)
            ctx.SetState(new SummonState(0));
        else
            ctx.SetState(new CheckOutcomeState(PhaseBattleController.Instance.DurationShowOutcome, false));
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
    }

    private IEnumerator DestroyUnit()
    {
        while (PhaseBattleController.Instance.FaintUnits.Count > 0)
        {
            var unit = PhaseBattleController.Instance.FaintUnits.Dequeue();
            GameObject.Destroy(unit); 
            Debug.Log($"Fainted unit {unit.name} destroyed");

            yield return new WaitUntil(() => unit == null );
        }
    }
}