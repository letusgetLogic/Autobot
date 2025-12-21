using UnityEngine;
public class HandleAbilityState : StateBase
{
    private bool isDone = false;

    /// <summary>
    /// Consturctor of HandleAbilityState.
    /// </summary>
    /// <param name="_maxTimeCount"></param>
    public HandleAbilityState(float _maxTimeCount) : base(_maxTimeCount)
    {
    }

    public override void OnEnter(IFiniteStateMachine _ctx)
    {
        Debug.Log("--- HandleAbilityState");
        HandleAbility();
    }

    public override void OnUpdate(IFiniteStateMachine _ctx, float _speed)
    {
        if (TimeCount < PhaseBattleController.Instance.Process.DurationHandleEachAbility)
        {
            TimeCount += _speed;
        }
        else
        {
            TimeCount = 0f;
            HandleAbility();
        }

        if (isDone)
        {
            if (PhaseBattleController.Instance.ShutdownUnits.Count > 0)
                _ctx.SetState(new ShutdownState(
                     PhaseBattleController.Instance.Process.DurationShutdown));
            else
                _ctx.SetState(new CheckOutcomeState(
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
                    PhaseBattleController.Instance.Process.DurationHideAbilityDescription,
                    false));
        }
        else
            isDone = true;
    }
}
