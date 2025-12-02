using UnityEngine;
public class HandleAbilityState : StateBase
{
    private bool isDone = false;

    /// <summary>
    /// Consturctor of HandleAbilityState.
    /// </summary>
    /// <param name="maxTimeCount"></param>
    public HandleAbilityState(float maxTimeCount) : base(maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine ctx)
    {
        Debug.Log("--- HandleAbilityState");
        HandleAbility();
    }

    public override void OnUpdate(IFiniteStateMachine ctx, float speed)
    {
        if (TimeCount < PhaseBattleController.Instance.Process.DurationEachAbility)
        {
            TimeCount += speed;
        }
        else
        {
            TimeCount = 0f;
            HandleAbility();
        }

        if (isDone)
        {
            if (PhaseBattleController.Instance.FaintUnits.Count > 0)
                ctx.SetState(new FaintState(
                     PhaseBattleController.Instance.Process.DurationFaint));
            else
                ctx.SetState(new CheckOutcomeState(
                    PhaseBattleController.Instance.Process.DurationCheckOutcome, false));
        }
    }

    /// <summary>
    /// Handles all registered abilities.
    /// </summary>
    private void HandleAbility()
    {
        if (PhaseBattleController.Instance.UnitAbilities.Count > 0)
        {
            var ability = PhaseBattleController.Instance.UnitAbilities.Dequeue();
            Debug.Log($"{ability.ToString()} dequeue/activate");
            Debug.Log($"{PhaseBattleController.Instance.UnitAbilities.Count} UnitAbilities left");
            PhaseBattleController.Instance.StartCoroutine(
                ability.Handle(
                    PhaseBattleController.Instance.Process.DurationHideAbilityDescription));
        }
        else
            isDone = true;
    }
}
