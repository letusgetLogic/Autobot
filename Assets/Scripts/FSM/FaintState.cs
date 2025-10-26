using System.Collections;
using UnityEngine;

public class FaintState : StateBase
{
    bool isDone = false;
    public FaintState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Debug.Log("--- FaintState");

        isDone = PhaseBattleController.Instance.DestroyUnit();
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (isDone)
        {
            if (PhaseBattleController.Instance.UnitAbilities.Count > 0)
                ctx.SetState(new HandleAbilityState(0));
            else
           if (PhaseBattleController.Instance.SummonUnits.Count > 0)
                ctx.SetState(new SummonState(0));
            else
                ctx.SetState(new CheckOutcomeState(PhaseBattleController.Instance.DurationShowOutcome, false));
        }
    }

   
}