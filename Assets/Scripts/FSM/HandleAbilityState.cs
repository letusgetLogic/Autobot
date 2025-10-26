using UnityEngine;
public class HandleAbilityState : StateBase
{
    private bool isDone = false;

    public HandleAbilityState(float maxCount) : base(maxCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Debug.Log("--- HandleAbilityState");
        isDone = HandleAbilities();
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (isDone)
        {
            if (PhaseBattleController.Instance.FaintUnits.Count > 0)
                ctx.SetState(new FaintState(0));
            else 
                ctx.SetState(new CheckOutcomeState(PhaseBattleController.Instance.DurationShowOutcome, false));
        }
    }

    private bool HandleAbilities()
    {
        while (PhaseBattleController.Instance.UnitAbilities.Count > 0)
        {
            var ability = PhaseBattleController.Instance.UnitAbilities.Dequeue();
            Debug.Log($"{ability.ToString()} dequeue/activate");
            Debug.Log($"{PhaseBattleController.Instance.UnitAbilities.Count} UnitAbilities");
            ability.Activate();
        }

        return true;
    }
}
